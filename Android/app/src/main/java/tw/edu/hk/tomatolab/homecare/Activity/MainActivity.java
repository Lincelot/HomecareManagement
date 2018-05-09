package tw.edu.hk.tomatolab.homecare.Activity;

import android.app.AlertDialog;
import android.app.Fragment;
import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.app.ProgressDialog;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.preference.PreferenceManager;
import android.support.design.widget.NavigationView;
import android.support.design.widget.Snackbar;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;

import com.estimote.sdk.SystemRequirementsChecker;
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
import tw.edu.hk.tomatolab.homecare.Beacon.BeaconService;
import tw.edu.hk.tomatolab.homecare.Fragment.BeaconFragment;
import tw.edu.hk.tomatolab.homecare.Fragment.FormFragment;
import tw.edu.hk.tomatolab.homecare.Fragment.MainFragment;
import tw.edu.hk.tomatolab.homecare.Helper.HttpHelper;
import tw.edu.hk.tomatolab.homecare.Helper.SQLiteHelper;
import tw.edu.hk.tomatolab.homecare.Model.CaseRecordModel;
import tw.edu.hk.tomatolab.homecare.Model.ServiceRecordModel;
import tw.edu.hk.tomatolab.homecare.R;

public class MainActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener {


    private static final String TAG = "MainActivity";
    private FragmentManager fragmentManager = getFragmentManager();
    private Context context;
    private ProgressDialog progressDialog;
    private int mProgress = 0;
    private boolean isBusy = false;
    private AlertDialog.Builder alertDialog;

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

    //取得目前時間
    public static String getDateNow(int mins, String format) {
        Calendar calendar = Calendar.getInstance();
        calendar.add(Calendar.MINUTE, mins);
        return new SimpleDateFormat(format).format(calendar.getTime());
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawer.setDrawerListener(toggle);
        toggle.syncState();

        NavigationView navigationView = (NavigationView) findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);

        context = getApplicationContext();

        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        boolean isChecked = sharedPreferences.getBoolean("switchUpdate", true);
        if (isChecked) {
            alertDialog = new AlertDialog.Builder(this);
            alertDialog.setTitle(R.string.alertdialog_update_title);
            alertDialog.setMessage(R.string.alertdialog_update_message);
            alertDialog.setNegativeButton(R.string.alertdialog_update_btnupdate, new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    getDataFromServer();
                }
            });
            alertDialog.setNeutralButton(R.string.alertdialog_close, new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    //Close
                }
            });
            alertDialog.show();
        }

        navigationView.getMenu().getItem(0).setChecked(true);
        fragmentManager.beginTransaction()
                .replace(R.id.content_frame, new MainFragment())
                .commit();
    }

    @Override
    protected void onResume() {
        super.onResume();
        SystemRequirementsChecker.checkWithDefaultDialogs(this);
        sendBroadcast(new Intent(BeaconService.ACTION_RELAUNCH_SERVICE));
    }

    @Override
    public void onBackPressed() {
        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        if (drawer.isDrawerOpen(GravityCompat.START)) {
            drawer.closeDrawer(GravityCompat.START);
        } else {
            drawer.openDrawer(GravityCompat.START);
        }
    }

    @SuppressWarnings("StatementWithEmptyBody")
    @Override
    public boolean onNavigationItemSelected(MenuItem item) {
        // Handle navigation view item clicks here.
        int id = item.getItemId();
        Fragment mFragment = null;
        switch (id) {
            case R.id.nav_menu_main: {
                mFragment = new MainFragment();
                break;
            }
            case R.id.nav_menu_beacon: {
                mFragment = new BeaconFragment();
                break;
            }
            case R.id.nav_menu_Form: {
                mFragment = new FormFragment();
                break;
            }
            case R.id.nav_menu_Preferences: {
                mFragment = new PreferenceFragment();
                break;
            }
            case R.id.nav_menu_refresh: {

                alertDialog = new AlertDialog.Builder(this);
                alertDialog.setTitle(R.string.alertdialog_update_title);
                alertDialog.setMessage(R.string.alertdialog_update_message);
                alertDialog.setNegativeButton(R.string.alertdialog_update_btnupdate, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        getDataFromServer();
                    }
                });
                alertDialog.setNeutralButton(R.string.alertdialog_close, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        //Close
                    }
                });
                alertDialog.show();

                break;
            }
            case R.id.nav_menu_showSerialID: {
                alertDialog = new AlertDialog.Builder(this);
                alertDialog.setTitle(R.string.alertdialog_showSerialID_title);
                alertDialog.setMessage(getDeviceUUID());
                alertDialog.setNeutralButton(R.string.alertdialog_showSerialID_copy, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        //Close
                        ClipboardManager manager = (ClipboardManager) getSystemService(CLIPBOARD_SERVICE);
                        ClipData clip = ClipData.newPlainText("", getDeviceUUID());
                        manager.setPrimaryClip(clip);
                        showSnackbar(R.string.snackbar_uuid_copy);
                    }
                });
                alertDialog.show();
                break;
            }
        }

        if (mFragment != null) {
            FragmentTransaction ft = fragmentManager.beginTransaction();
            ft.setCustomAnimations(R.anim.enter, R.anim.exit);
            ft.replace(R.id.content_frame, mFragment);
            ft.commit();
        }

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        drawer.closeDrawer(GravityCompat.START);
        return true;
    }

    private boolean getNetworkStatus() {
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
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

    private void getDataFromServer() {
        if (!isBusy && getNetworkStatus()) {
            isBusy = true;
            progressDialog = new ProgressDialog(this);
            progressDialog.setCancelable(false);
            progressDialog.setMax(100);
            progressDialog.setTitle(R.string.progressDialog_Download_Title);
            progressDialog.setMessage("Loading …");
            progressDialog.setProgressStyle(ProgressDialog.STYLE_HORIZONTAL);
            //消除右下角數字
            progressDialog.setProgressNumberFormat(" ");
            progressDialog.show();

            //region Read Server Data
            String url = "http://hcm.mobilehk.info/Attendant/getMobileData";
            StringEntity stringEntity = null;
            try {
                JSONObject jsonObject = new JSONObject();
                jsonObject.put("UUID", getDeviceUUID());
                stringEntity = new StringEntity(String.valueOf(jsonObject), "UTF-8");
            } catch (JSONException e) {
                e.printStackTrace();
            }

            HttpHelper.asyncPost(context, url, stringEntity, new JsonHttpResponseHandler() {
                @Override
                public void onStart() {
                    super.onStart();
                    new Thread(new Runnable() {
                        @Override
                        public void run() {
                            if (mProgress < (progressDialog.getMax() / 2)) {
                                mProgress += 3;
                                mHandler.sendMessage(mHandler.obtainMessage(mProgress));
                                try {
                                    Thread.sleep(500);
                                } catch (InterruptedException e) {
                                    e.printStackTrace();
                                }
                            }
                        }
                    }).start();
                }

                //region onFailure()
                @Override
                public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                    Log.d(TAG, "onFailure() called with: " + "statusCode = [" + statusCode + "], throwable = [" + throwable + "], errorResponse = [" + errorResponse + "]");
                    showSnackbar(R.string.snackbar_update_server_error);
                    progressDialog.dismiss();
                    mProgress = 0;
                    isBusy = false;
                }

                @Override
                public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONArray errorResponse) {
                    Log.d(TAG, "onFailure() called with: " + "statusCode = [" + statusCode + "], throwable = [" + throwable + "], errorResponse = [" + errorResponse + "]");
                    showSnackbar(R.string.snackbar_update_server_error);
                    progressDialog.dismiss();
                    mProgress = 0;
                    isBusy = false;
                }

                @Override
                public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                    Log.d(TAG, "onFailure() called with: " + "statusCode = [" + statusCode + "], responseString = [" + responseString + "], throwable = [" + throwable + "]");
                    showSnackbar(R.string.snackbar_update_server_error);
                    progressDialog.dismiss();
                    mProgress = 0;
                    isBusy = false;
                }
                //endregion

                //region onSuccess()
                @Override
                public void onSuccess(int statusCode, Header[] headers, JSONArray response) {
                    Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], response = [" + response + "]");
                }

                @Override
                public void onSuccess(int statusCode, Header[] headers, String responseString) {
                    Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], responseString = [" + responseString + "]");
                }

                @Override
                public void onSuccess(int statusCode, Header[] headers, final JSONObject response) {
                    Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], response = [" + response + "]");
                    new Thread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                mProgress = 50;
                                mHandler.sendMessage(mHandler.obtainMessage(mProgress));
                                SQLiteHelper db = new SQLiteHelper(context);
                                if (response.get("status").equals("ok")) {
                                    JSONArray Equipment = response.getJSONArray("Equipment");
                                    JSONArray Service = response.getJSONArray("Service");
                                    JSONArray Schedule = response.getJSONArray("Schedule");
                                    JSONArray Schedule_Service = response.getJSONArray("Schedule_Service");
                                    JSONArray Employer = response.getJSONArray("Employer");
                                    JSONArray CaseRecordItem = response.getJSONArray("CaseRecordItem");
                                    JSONArray CaseRecordAnswer = response.getJSONArray("CaseRecordAnswer");

                                    //region equipment
                                    if (Equipment != null) {
                                        db.clearTable(SQLiteHelper.Table_equipment);
                                        List<String[]> list = new ArrayList<>();
                                        for (int i = 0; i < Equipment.length(); i++) {
                                            JSONObject obj = Equipment.getJSONObject(i);
                                            list.add(new String[]{
                                                    obj.getString(SQLiteHelper.Column_equipment_uid),
                                                    obj.getString(SQLiteHelper.Column_equipment_account_uid),
                                                    obj.getString(SQLiteHelper.Column_equipment_macaddress),
                                                    obj.getString(SQLiteHelper.Column_equipment_type),
                                                    obj.getString(SQLiteHelper.Column_equipment_edit_time)
                                            });
                                        }
                                        db.insertMulti(SQLiteHelper.Table_equipment, list);
                                    }
                                    //endregion

                                    mProgress += 5;
                                    mHandler.sendMessage(mHandler.obtainMessage(mProgress));

                                    //region service
                                    if (Service != null) {
                                        db.clearTable(SQLiteHelper.Table_service);
                                        List<String[]> list = new ArrayList<>();
                                        for (int i = 0; i < Service.length(); i++) {
                                            JSONObject obj = Service.getJSONObject(i);
                                            list.add(new String[]{
                                                    obj.getString(SQLiteHelper.Column_service_uid),
                                                    obj.getString(SQLiteHelper.Column_service_service_item_uid),
                                                    obj.getString(SQLiteHelper.Column_service_name),
                                                    obj.getString(SQLiteHelper.Column_service_edit_time),
                                                    obj.getString(SQLiteHelper.Column_service_isdelete)
                                            });
                                        }
                                        db.insertMulti(SQLiteHelper.Table_service, list);
                                    }
                                    //endregion

                                    mProgress += 5;
                                    mHandler.sendMessage(mHandler.obtainMessage(mProgress));

                                    //region schedule
                                    if (Schedule != null) {
                                        db.clearTable(SQLiteHelper.Table_schedule);
                                        List<String[]> list = new ArrayList<>();
                                        for (int i = 0; i < Schedule.length(); i++) {
                                            JSONObject obj = Schedule.getJSONObject(i);
                                            list.add(new String[]{
                                                    obj.getString(SQLiteHelper.Column_schedule_uid),
                                                    obj.getString(SQLiteHelper.Column_schedule_account_uid_1),
                                                    obj.getString(SQLiteHelper.Column_schedule_account_uid_2),
                                                    obj.getString(SQLiteHelper.Column_schedule_start),
                                                    obj.getString(SQLiteHelper.Column_schedule_end),
                                                    obj.getString(SQLiteHelper.Column_schedule_edit_time),
                                                    obj.getString(SQLiteHelper.Column_schedule_summary)
                                            });
                                        }
                                        db.insertMulti(SQLiteHelper.Table_schedule, list);
                                    }
                                    //endregion

                                    mProgress += 5;
                                    mHandler.sendMessage(mHandler.obtainMessage(mProgress));

                                    //region schedule_service
                                    if (Schedule_Service != null) {
                                        db.clearTable(SQLiteHelper.Table_schedule_service);
                                        List<String[]> list = new ArrayList<>();
                                        for (int i = 0; i < Schedule_Service.length(); i++) {
                                            JSONObject obj = Schedule_Service.getJSONObject(i);
                                            list.add(new String[]{
                                                    obj.getString(SQLiteHelper.Column_schedule_service_uid),
                                                    obj.getString(SQLiteHelper.Column_schedule_service_schedule_uid),
                                                    obj.getString(SQLiteHelper.Column_schedule_service_service_uid)
                                            });
                                        }
                                        db.insertMulti(SQLiteHelper.Table_schedule_service, list);
                                    }
                                    //endregion

                                    mProgress += 5;
                                    mHandler.sendMessage(mHandler.obtainMessage(mProgress));


                                    //region employer_info
                                    if (Employer != null) {
                                        db.clearTable(SQLiteHelper.Table_employer_info);
                                        List<String[]> list = new ArrayList<>();
                                        for (int i = 0; i < Employer.length(); i++) {
                                            JSONObject obj = Employer.getJSONObject(i);
                                            list.add(new String[]{
                                                    obj.getString(SQLiteHelper.Column_schedule_service_uid),
                                                    obj.getString(SQLiteHelper.Column_employer_info_account_uid_1),
                                                    obj.getString(SQLiteHelper.Column_employer_info_birthday),
                                                    obj.getString(SQLiteHelper.Column_employer_info_sex),
                                                    obj.getString(SQLiteHelper.Column_employer_info_address),
                                                    obj.getString(SQLiteHelper.Column_employer_info_employer_name),
                                                    obj.getString(SQLiteHelper.Column_employer_info_employer_phone1),
                                                    obj.getString(SQLiteHelper.Column_employer_info_employer_phone2),
                                                    obj.getString(SQLiteHelper.Column_employer_info_employer_item1),
                                                    obj.getString(SQLiteHelper.Column_employer_info_employer_item2),
                                                    obj.getString(SQLiteHelper.Column_employer_info_employer_item3),
                                                    obj.getString(SQLiteHelper.Column_employer_info_emg1_name),
                                                    obj.getString(SQLiteHelper.Column_employer_info_emg1_phone1),
                                                    obj.getString(SQLiteHelper.Column_employer_info_emg1_phone2),
                                                    obj.getString(SQLiteHelper.Column_employer_info_emg2_name),
                                                    obj.getString(SQLiteHelper.Column_employer_info_emg2_phone1),
                                                    obj.getString(SQLiteHelper.Column_employer_info_emg2_phone2),
                                                    obj.getString(SQLiteHelper.Column_employer_info_summary)
                                            });
                                        }
                                        db.insertMulti(SQLiteHelper.Table_employer_info, list);
                                    }
                                    //endregion

                                    mProgress += 5;
                                    mHandler.sendMessage(mHandler.obtainMessage(mProgress));

                                    //region case_serivce_record_item
                                    if (CaseRecordItem != null) {
                                        db.clearTable(SQLiteHelper.Table_case_serivce_record_item);
                                        List<String[]> list = new ArrayList<>();
                                        for (int i = 0; i < CaseRecordItem.length(); i++) {
                                            JSONObject obj = CaseRecordItem.getJSONObject(i);
                                            list.add(new String[]{
                                                    obj.getString(SQLiteHelper.Column_case_serivce_record_item_uid),
                                                    obj.getString(SQLiteHelper.Column_case_serivce_record_item_name)
                                            });
                                        }
                                        db.insertMulti(SQLiteHelper.Table_case_serivce_record_item, list);
                                    }
                                    //endregion

                                    mProgress += 5;
                                    mHandler.sendMessage(mHandler.obtainMessage(mProgress));

                                    //region case_serivce_record_answer
                                    if (CaseRecordAnswer != null) {
                                        db.clearTable(SQLiteHelper.Table_case_serivce_record_answer);
                                        List<String[]> list = new ArrayList<>();
                                        for (int i = 0; i < CaseRecordAnswer.length(); i++) {
                                            JSONObject obj = CaseRecordAnswer.getJSONObject(i);
                                            list.add(new String[]{
                                                    obj.getString(SQLiteHelper.Column_case_serivce_record_answer_uid),
                                                    obj.getString(SQLiteHelper.Column_case_serivce_record_answer_case_serivce_record_item_uid),
                                                    obj.getString(SQLiteHelper.Column_case_serivce_record_answer_name)
                                            });
                                        }
                                        db.insertMulti(SQLiteHelper.Table_case_serivce_record_answer, list);
                                    }
                                    //endregion

                                    mProgress += 5;
                                    mHandler.sendMessage(mHandler.obtainMessage(mProgress));
                                } else {
                                    showSnackbar(R.string.snackbar_update_error);
                                    db.clearTable(SQLiteHelper.Table_equipment);
                                    db.clearTable(SQLiteHelper.Table_service);
                                    db.clearTable(SQLiteHelper.Table_schedule);
                                    db.clearTable(SQLiteHelper.Table_schedule_service);
                                    db.clearTable(SQLiteHelper.Table_employer_info);
                                    db.clearTable(SQLiteHelper.Table_case_serivce_record_item);
                                    db.clearTable(SQLiteHelper.Table_case_serivce_record_answer);
                                }
                                db.close();

                                while (mProgress < progressDialog.getMax()) {
                                    mProgress++;
                                    mHandler.sendMessage(mHandler.obtainMessage(mProgress));
                                    Thread.sleep(50);
                                }
                                if (mProgress == progressDialog.getMax()) {
                                    progressDialog.dismiss();
                                    mProgress = 0;
                                    isBusy = false;
                                }
                            } catch (JSONException | InterruptedException e) {
                                e.printStackTrace();
                            }


                        }
                    }).start();
                }

                //endregion

            });
            //endregion

            //region Send Data to Server
            new Thread(new Runnable() {
                @Override
                public void run() {
                    final List<ServiceRecordModel> listServiceRecord = new ArrayList<>();
                    final List<CaseRecordModel> listCaseRecord = new ArrayList<>();
                    String sql1 = "SELECT *" +
                            " FROM " + SQLiteHelper.Table_work_service +
                            " WHERE " + SQLiteHelper.Column_work_case_record_update_status + " = ?";
                    String sql2 = "SELECT *" +
                            " FROM " + SQLiteHelper.Table_work_case_record +
                            " WHERE " + SQLiteHelper.Column_work_case_record_update_status + " = ?";
                    SQLiteHelper db = new SQLiteHelper(context);
                    Cursor cursor = db.rawQuery(sql1, new String[]{"0"});
                    if (cursor != null) {
                        while (cursor.moveToNext()) {
                            listServiceRecord.add(new ServiceRecordModel(
                                    cursor.getInt(0),
                                    cursor.getInt(1),
                                    cursor.getInt(2),
                                    cursor.getInt(3),
                                    cursor.getString(4)
                            ));
                        }
                        cursor.close();
                    }
                    cursor = db.rawQuery(sql2, new String[]{"0"});
                    if (cursor != null) {
                        while (cursor.moveToNext()) {
                            listCaseRecord.add(new CaseRecordModel(
                                    cursor.getInt(0),
                                    cursor.getInt(1),
                                    cursor.getInt(2),
                                    cursor.getString(3)
                            ));
                        }
                        cursor.close();
                    }
                    db.close();
                    if (listServiceRecord.size() > 0 || listCaseRecord.size() > 0) {
                        runOnUiThread(new Runnable() {
                            public void run() {
                                new HttpHelper().sendRecord(context, getDeviceUUID(), listServiceRecord, listCaseRecord);
                            }
                        });
                    }
                }
            }).start();
            //endregion

        } else {
            alertDialog = new AlertDialog.Builder(this);
            alertDialog.setTitle(R.string.alertdialog_networkstatus_title);
            alertDialog.setMessage(R.string.alertdialog_networkstatus_message);
            alertDialog.setNeutralButton(R.string.alertdialog_close, new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    //Close
                }
            });
            alertDialog.show();
        }
    }

    private void showSnackbar(int str) {
        View parentLayout = findViewById(R.id.content_frame);
        Snackbar.make(parentLayout, str, Snackbar.LENGTH_LONG).show();
    }

    private Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            progressDialog.setProgress(msg.what);
        }
    };

    public static class PreferenceFragment extends android.preference.PreferenceFragment {
        @Override
        public void onCreate(Bundle savedInstanceState) {
            super.onCreate(savedInstanceState);
            addPreferencesFromResource(R.xml.item_preferences);
        }


//        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(getActivity());
//        boolean isChecked = sharedPreferences.getBoolean("switchNetwork", false);
//        Log.d(TAG, "onCreateView()" + isChecked);

    }
}
