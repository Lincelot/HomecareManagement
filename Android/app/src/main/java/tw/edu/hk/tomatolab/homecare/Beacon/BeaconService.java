package tw.edu.hk.tomatolab.homecare.Beacon;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.bluetooth.BluetoothAdapter;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Build;
import android.os.IBinder;
import android.preference.PreferenceManager;
import android.util.Log;
import android.widget.Toast;

import com.estimote.sdk.Beacon;
import com.estimote.sdk.BeaconManager;
import com.estimote.sdk.Nearable;
import com.estimote.sdk.Region;
import com.estimote.sdk.eddystone.Eddystone;
import com.estimote.sdk.repackaged.gson_v2_3_1.com.google.gson.Gson;
import com.loopj.android.http.JsonHttpResponseHandler;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.UUID;

import cz.msebera.android.httpclient.Header;
import cz.msebera.android.httpclient.entity.StringEntity;
import tw.edu.hk.tomatolab.homecare.Activity.MainActivity;
import tw.edu.hk.tomatolab.homecare.Helper.HttpHelper;
import tw.edu.hk.tomatolab.homecare.Helper.SQLiteHelper;
import tw.edu.hk.tomatolab.homecare.Model.WorkRecordModel;
import tw.edu.hk.tomatolab.homecare.R;

public class BeaconService extends Service {

    public static final String ACTION_RELAUNCH_SERVICE = "tw.edu.hk.tomatolab.homecare.action.RELAUNCH_SERVICE";
    public static final String ACTION_BEACON_CHANGE = "tw.edu.hk.tomatolab.homecare.action.BEACON_CHANGE";
    private static final String TAG = "BeaconService";
    private static final Region ALL_ESTIMOTE_BEACONS = new Region("rid", null, null, null);
    private static String NearableScanString;
    private static String EddystoneScanString;
    private static List<Beacon> listBeacon = new ArrayList<>();
    //Context
    private Context context;
    private SharedPreferences sharedPreferences;
    private BeaconManager beaconManager;
    private boolean isConnect = false;
    private boolean isDiscoveredBeacon = false;
    //sharedPreferences
    private int scanPeriodMillis = 5;
    private int waitTimeMillis = 60;
    //Record
    private boolean isCheckin = false;
    private SQLiteHelper db;
    private int didNotFindBeacon = 0;
    private List<String[]> dbBeacons = new ArrayList<>();
    private List<String[]> findBeacons = new ArrayList<>();
    private List<WorkRecordModel> workRecords = new ArrayList<>();

    public static List<Beacon> getListBeacon() {
        return listBeacon;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        Log.d(TAG, "onCreate()");
        context = getApplicationContext();

        sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        scanPeriodMillis = Integer.parseInt(sharedPreferences.getString("scanPeriodMillis", "5"));
        waitTimeMillis = Integer.parseInt(sharedPreferences.getString("waitTimeMillis", "60"));

        db = new SQLiteHelper(context);
        db.close();

        beaconManager = new BeaconManager(this);
        beaconManager.setRangingListener(new BeaconManager.RangingListener() {
            @Override
            public void onBeaconsDiscovered(Region region, List<Beacon> beacons) {
                workRecords.clear();
                dbBeacons.clear();
                findBeacons.clear();
                listBeacon.clear();
                db.open();
                if (beacons.size() > 0) {
                    listBeacon.addAll(beacons);
                    isDiscoveredBeacon = true;
                    for (Beacon beacon : beacons) {
                        Log.d(TAG, "onBeaconsDiscovered() called with: " + "beacon = [" + beacon + "]");
                    }
                } else {
                    isDiscoveredBeacon = false;
                }

                if (isDiscoveredBeacon) {
                    //檢查資料庫內Beacon與搜尋到的Beacon是否相同
                    String sql = "SELECT " + SQLiteHelper.Column_equipment_uid + ", " +
                            SQLiteHelper.Column_equipment_macaddress +
                            " FROM " + SQLiteHelper.Table_equipment +
                            " WHERE " + SQLiteHelper.Column_equipment_type + " != ?";
                    Cursor cursor = db.rawQuery(sql, new String[]{"1"});
                    if (cursor != null) {
                        while (cursor.moveToNext()) {
                            dbBeacons.add(new String[]{
                                    cursor.getString(0),
                                    cursor.getString(1)
                            });
                        }
                        cursor.close();
                    }
                    for (Beacon beacon : beacons) {
                        String MAC = beacon.getMacAddress().toString().replace("[", "").replace("]", "").replace(":", "");
                        for (String[] str : dbBeacons) {
                            if (MAC.equals(str[1])) {
                                findBeacons.add(new String[]{
                                        null,
                                        getDateNow(0),
                                        getDateNow(1),
                                        str[0],
                                        null,
                                        String.valueOf(0)
                                });
                            }
                        }
                    }
                }

                if (findBeacons.size() > 0) {
                    didNotFindBeacon = 0;
                    db.insertMulti(SQLiteHelper.Table_work_record, findBeacons);
                    Log.d(TAG, "findBeacons.size() > 0 & didNotFindBeacon = " + didNotFindBeacon);
                }

                if (!isCheckin && findBeacons.size() > 0) {
                    showNotification("簽到", "已於 " + getDateNow(0) + " 進行簽到！");
                    isCheckin = true;
                    Log.d(TAG, "isCheckin : " + true);
                }

                if (isCheckin && findBeacons.size() == 0) {
                    didNotFindBeacon++;
                    Log.d(TAG, "didNotFindBeaconDate：" + didNotFindBeacon);
                    if (didNotFindBeacon > 5) {
                        showNotification("簽退", "已於 " + getDateNow(0) + " 進行簽退！");
                        didNotFindBeacon = 0;
                        isCheckin = false;
                        Log.d(TAG, "isCheckin : " + false);
                    }
                }

                if (getNetworkStatus()) {
                    String sql = "SELECT *" +
                            " FROM " + SQLiteHelper.Table_work_record +
                            " WHERE " + SQLiteHelper.Column_work_record_update_status + " = ?" +
                            " LIMIT 0,30";
                    Cursor cursor = db.rawQuery(sql, new String[]{"0"});
                    if (cursor != null) {
                        while (cursor.moveToNext()) {
                            workRecords.add(new WorkRecordModel(
                                    cursor.getInt(0),
                                    cursor.getString(1),
                                    cursor.getString(2),
                                    cursor.getInt(3),
                                    cursor.getInt(4),
                                    cursor.getInt(5)
                            ));
                        }
                        cursor.close();
                    }
                    db.close();
                    if (workRecords.size() > 0) {
                        String url = "http://hcm.mobilehk.info/Attendant/setWorkRecord";
                        String WorkRecordList = new Gson().toJson(workRecords);
                        StringEntity stringEntity = null;
                        JSONObject obj = new JSONObject();
                        try {
                            obj.put("UUID", getDeviceUUID());
                            obj.put("WorkRecordList", new JSONArray(WorkRecordList));
                            stringEntity = new StringEntity(String.valueOf(obj), "UTF-8");
                        } catch (JSONException e) {
                            e.printStackTrace();
                        }

                        HttpHelper.asyncPost(context, url, stringEntity, new JsonHttpResponseHandler() {

                            //region onSuccess
                            @Override
                            public void onSuccess(int statusCode, Header[] headers, final JSONObject response) {
                                Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], response = [" + response + "]");
                                new Thread(new Runnable() {
                                    @Override
                                    public void run() {
                                        try {
                                            if (response.get("status").equals("ok")) {
                                                if (response.get("WorkRecordList") != null) {
                                                    db.open();
                                                    JSONArray array = response.getJSONArray("WorkRecordList");
                                                    String sql = "UPDATE " + SQLiteHelper.Table_work_record +
                                                            " SET " + SQLiteHelper.Column_work_record_update_status + " = 1" +
                                                            " WHERE " + SQLiteHelper.Column_work_record_uid + " = ?";
                                                    for (int i = 0; i < array.length(); i++) {
                                                        db.execSQL(sql, new String[]{String.valueOf(array.get(i))});
                                                    }
                                                    db.close();
                                                }
                                            }
                                        } catch (JSONException e) {
                                            e.printStackTrace();
                                        }
                                    }
                                }).start();
                            }

                            @Override
                            public void onSuccess(int statusCode, Header[] headers, JSONArray response) {
                                Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], response = [" + response + "]");
                            }

                            @Override
                            public void onSuccess(int statusCode, Header[] headers, String responseString) {
                                Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], responseString = [" + responseString + "]");
                            }
                            //endregion

                            //region onFailure
                            @Override
                            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                                Log.d(TAG, "onFailure() called with: " + "statusCode = [" + statusCode + "], throwable = [" + throwable + "], errorResponse = [" + errorResponse + "]");
                            }

                            @Override
                            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONArray errorResponse) {
                                Log.d(TAG, "onFailure() called with: " + "statusCode = [" + statusCode + "], throwable = [" + throwable + "], errorResponse = [" + errorResponse + "]");
                            }

                            @Override
                            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                                Log.d(TAG, "onFailure() called with: " + "statusCode = [" + statusCode + "], responseString = [" + responseString + "], throwable = [" + throwable + "]");
                            }
                            //endregion

                        });
                    }
                }

                //region BroadCast Beacons To Fragment
                Intent broadcastIntent = new Intent(ACTION_BEACON_CHANGE);
                broadcastIntent.putExtra("beacons", new ArrayList<>(beacons));
                sendBroadcast(broadcastIntent);
                //endregion

                //region 比較是否有修改間隔時間
                int tmp_scanPeriodMillis = Integer.parseInt(sharedPreferences.getString("scanPeriodMillis", "5"));
                int tmp_waitTimeMillis = Integer.parseInt(sharedPreferences.getString("waitTimeMillis", "60"));
                if (tmp_scanPeriodMillis != scanPeriodMillis
                        || tmp_waitTimeMillis != waitTimeMillis) {
                    Log.d(TAG, "[scanPeriodMillis] = " + scanPeriodMillis + " , " +
                            "[waitTimeMillis] = " + waitTimeMillis + " , " +
                            "[tmp_scanPeriodMillis] = " + tmp_scanPeriodMillis + " , " +
                            "[tmp_waitTimeMillis] = " + tmp_waitTimeMillis);
                    stopSelf();
                }
                //endregion

            }
        });
        beaconManager.setNearableListener(new BeaconManager.NearableListener() {
            @Override
            public void onNearablesDiscovered(List<Nearable> nearables) {
                if (nearables.size() > 0) {
                    for (Nearable nearable : nearables) {
                        Log.d(TAG, "onNearablesDiscovered() - " + nearable);
                    }
                }
            }
        });
        beaconManager.setEddystoneListener(new BeaconManager.EddystoneListener() {
            @Override
            public void onEddystonesFound(List<Eddystone> eddystones) {
                for (Eddystone eddystone : eddystones) {
                    Log.d(TAG, "onEddystonesFound() - " + eddystone);
                }
            }
        });
        beaconManager.setErrorListener(new BeaconManager.ErrorListener() {
            @Override
            public void onError(Integer integer) {
                Log.d(TAG, "onError() called with: " + "integer = [" + integer + "]");
            }
        });
        beaconManager.setForegroundScanPeriod(scanPeriodMillis * 1000, waitTimeMillis * 1000);
    }

    @Override
    public void onDestroy() {
        Log.d(TAG, "onDestroy()");
        beaconManager.stopRanging(ALL_ESTIMOTE_BEACONS);
        beaconManager.stopNearableDiscovery(NearableScanString);
        beaconManager.stopEddystoneScanning(EddystoneScanString);
        beaconManager.disconnect();
        isConnect = false;
        isDiscoveredBeacon = false;
        if (canStartService()) {
            sendBroadcast(new Intent(ACTION_RELAUNCH_SERVICE));
        }
        super.onDestroy();
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        boolean b = canStartService();
        if (b) {
            connectToService();
        } else {
            stopSelf();
        }
        return super.onStartCommand(intent, flags, startId);
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    private boolean canStartService() {
        boolean isBluetoothEnabled;
        BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
        if (mBluetoothAdapter == null) {
            // 如果裝置不支援藍芽
            Toast.makeText(this, "Device doesn't support bluetooth", Toast.LENGTH_SHORT).show();
            isBluetoothEnabled = false;
        } else {
            isBluetoothEnabled = true;
        }
        return isBluetoothEnabled;
    }

    private void connectToService() {
        if (!isConnect) {
            isConnect = true;
            beaconManager.connect(new BeaconManager.ServiceReadyCallback() {
                @Override
                public void onServiceReady() {
                    try {
                        // Beacons ranging.
                        beaconManager.startRanging(ALL_ESTIMOTE_BEACONS);

                        // Nearable discovery.
                        NearableScanString = beaconManager.startNearableDiscovery();

                        // Eddystone scanning.
                        EddystoneScanString = beaconManager.startEddystoneScanning();
                    } catch (Exception e) {
                        stopSelf();
                    }
                }
            });
        }
    }

    private boolean getNetworkStatus() {
        boolean isAllowMobileNetwork = sharedPreferences.getBoolean("switchNetwork", false);
        boolean isActive = false;
        ConnectivityManager conManager = (ConnectivityManager) getSystemService(CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = conManager.getActiveNetworkInfo();
        try {
            String status = networkInfo.getTypeName();

            //有網路
            if (networkInfo.isAvailable()) {
                isActive = true;
            }

            //有網路但使用者不允許使用3G
            if (!isAllowMobileNetwork && status.equals("MOBILE")) {
                isActive = false;
            }
        } catch (Exception ex) {
            isActive = false;
        }
        return isActive;
    }

    private void showNotification(String title, String message) {
        Intent notifyIntent = new Intent(this, MainActivity.class);
        notifyIntent.setFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP);
        PendingIntent pendingIntent = PendingIntent.getActivities(this, 0,
                new Intent[]{notifyIntent}, PendingIntent.FLAG_UPDATE_CURRENT);
        Notification notification = new Notification.Builder(this)
                .setSmallIcon(R.drawable.icon_notification_24)
                .setContentTitle(title)
                .setContentText(message)
                .setAutoCancel(true)
                .setContentIntent(pendingIntent)
                .build();
        notification.defaults |= Notification.DEFAULT_SOUND;
        NotificationManager notificationManager =
                (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
        notificationManager.notify(1, notification);
    }

    //取得目前時間
    private String getDateNow(int mins) {
        Calendar calendar = Calendar.getInstance();
        calendar.add(Calendar.MINUTE, mins);
        return new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(calendar.getTime());
    }

    //取得該裝置UUID
    public static String getDeviceUUID() {
        String devID = "" +
                Build.BOARD.length() % 10 +
                Build.DEVICE.length() % 10 +
                Build.DISPLAY.length() % 10 +
                Build.HOST.length() % 10 +
                Build.ID.length() % 10 +
                Build.MANUFACTURER.length() % 10 +
                Build.MODEL.length() % 10 +
                Build.PRODUCT.length() % 10 +
                Build.TAGS.length() % 10 +
                Build.TYPE.length() % 10 +
                Build.USER.length() % 10;
        String SERIAL = Build.SERIAL;
        return new UUID(devID.hashCode(), SERIAL.hashCode()).toString().toUpperCase();
    }
}
