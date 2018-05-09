package tw.edu.hk.tomatolab.homecare.Helper;

import android.content.Context;
import android.util.Log;

import com.estimote.sdk.repackaged.gson_v2_3_1.com.google.gson.Gson;
import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.SyncHttpClient;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.List;

import cz.msebera.android.httpclient.Header;
import cz.msebera.android.httpclient.entity.StringEntity;
import tw.edu.hk.tomatolab.homecare.Model.CaseRecordModel;
import tw.edu.hk.tomatolab.homecare.Model.ServiceRecordModel;

/**
 * Created by Minsheng on 2015/12/15.
 */
public class HttpHelper {

    private static final String TAG = "HttpHelper";
    private static AsyncHttpClient asyncHttpClient = new AsyncHttpClient();

    private static SyncHttpClient syncHttpClient = new SyncHttpClient();

    public static void asyncGet(Context mContext, String url, StringEntity stringEntity, JsonHttpResponseHandler responseHandler) {
        asyncHttpClient.get(mContext, url, stringEntity, "application/json", responseHandler);
    }

    public static void asyncPost(Context mContext, String url, StringEntity stringEntity, JsonHttpResponseHandler responseHandler) {
        asyncHttpClient.post(mContext, url, stringEntity, "application/json", responseHandler);
    }

    public static void syncGet(Context mContext, String url, StringEntity stringEntity, JsonHttpResponseHandler responseHandler) {
        syncHttpClient.get(mContext, url, stringEntity, "application/json", responseHandler);
    }

    public static void syncPost(Context mContext, String url, StringEntity stringEntity, JsonHttpResponseHandler responseHandler) {
        syncHttpClient.post(mContext, url, stringEntity, "application/json", responseHandler);
    }

    public void sendRecord(final Context mContext, String UUID, List<ServiceRecordModel> listServiceRecord, List<CaseRecordModel> listCaseRecord) {
        String url = "http://hcm.mobilehk.info/Attendant/setRecordData";
        StringEntity stringEntity = null;
        JSONObject obj = new JSONObject();
        try {
            String ServiceRecordList = new Gson().toJson(listServiceRecord);
            String CaseRecordList = new Gson().toJson(listCaseRecord);
            obj.put("UUID", UUID);
            obj.put("ServiceRecordList", new JSONArray(ServiceRecordList));
            obj.put("CaseRecordList", new JSONArray(CaseRecordList));
            stringEntity = new StringEntity(String.valueOf(obj), "UTF-8");
        } catch (JSONException e) {
            e.printStackTrace();
        }
        asyncPost(mContext, url, stringEntity, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], response = [" + response + "]");
                SQLiteHelper db = new SQLiteHelper(mContext);
                try {
                    if (response.get("status").equals("ok")) {
                        db.open();
                        if (response.get("ServiceRecordList") != null) {
                            JSONArray array = response.getJSONArray("ServiceRecordList");
                            String sql = "UPDATE " + SQLiteHelper.Table_work_service +
                                    " SET " + SQLiteHelper.Column_work_service_update_status + " = 1" +
                                    " WHERE " + SQLiteHelper.Column_work_service_uid + " = ?";
                            for (int i = 0; i < array.length(); i++) {
                                db.execSQL(sql, new String[]{String.valueOf(array.get(i))});
                            }
                        }
                        if (response.get("CaseRecordList") != null) {
                            JSONArray array = response.getJSONArray("CaseRecordList");
                            String sql = "UPDATE " + SQLiteHelper.Table_work_case_record +
                                    " SET " + SQLiteHelper.Column_work_case_record_update_status + " = 1" +
                                    " WHERE " + SQLiteHelper.Column_work_case_record_uid + " = ?";
                            for (int i = 0; i < array.length(); i++) {
                                db.execSQL(sql, new String[]{String.valueOf(array.get(i))});
                            }
                        }
                        db.close();
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONArray response) {
                Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], response = [" + response + "]");
            }

            @Override
            public void onSuccess(int statusCode, Header[] headers, String responseString) {
                Log.d(TAG, "onSuccess() called with: " + "statusCode = [" + statusCode + "], responseString = [" + responseString + "]");
            }

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

        });
    }
}
