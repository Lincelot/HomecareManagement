package tw.edu.hk.tomatolab.homecare.Fragment;

import android.app.Activity;
import android.app.Fragment;
import android.content.Context;
import android.database.Cursor;
import android.os.Bundle;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import tw.edu.hk.tomatolab.homecare.Activity.MainActivity;
import tw.edu.hk.tomatolab.homecare.Adapter.WorkListAdapter;
import tw.edu.hk.tomatolab.homecare.Helper.SQLiteHelper;
import tw.edu.hk.tomatolab.homecare.Model.WorkListModel;
import tw.edu.hk.tomatolab.homecare.R;


/**
 * A simple {@link Fragment} subclass.
 */
public class FormFragment extends Fragment {

    private static final String TAG = "FormFragment";
    private Context context;
    private WorkListAdapter adapter;
    private boolean isPause = false;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_form, container, false);
        context = view.getContext();

        RecyclerView recyclerView = (RecyclerView) view.findViewById(R.id.form_worklist_recyclerbiew);
        LinearLayoutManager layoutManager = new LinearLayoutManager(context);
        layoutManager.setOrientation(LinearLayoutManager.VERTICAL);
        adapter = new WorkListAdapter();
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.setAdapter(adapter);

        new Thread(new Runnable() {
            @Override
            public void run() {
                ((Activity) context).runOnUiThread(new Runnable() {
                    public void run() {
                        //這邊是呼叫main thread handler幫我們處理UI部分
                        adapter.replaceWith(getWorkLists());
                    }
                });
            }
        }).start();

        return view;
    }

    @Override
    public void onPause() {
        super.onPause();
        isPause = true;
    }

    @Override
    public void onResume() {
        super.onResume();
        if (isPause) {
            new Thread(new Runnable() {
                @Override
                public void run() {
                    ((Activity) context).runOnUiThread(new Runnable() {
                        public void run() {
                            //這邊是呼叫main thread handler幫我們處理UI部分
                            adapter.replaceWith(getWorkLists());
                        }
                    });
                }
            }).start();
            isPause = false;
        }
    }

    private List<WorkListModel> getWorkLists() {
        List<WorkListModel> result = new ArrayList<>();
        try {
            SQLiteHelper db = new SQLiteHelper(context);
            String sql = "SELECT a." + SQLiteHelper.Column_schedule_uid + ", " +
                    " a." + SQLiteHelper.Column_schedule_start + ", " +
                    " a." + SQLiteHelper.Column_schedule_end + ", " +
                    " (SELECT z." + SQLiteHelper.Column_employer_info_employer_name +
                    "  FROM " + SQLiteHelper.Table_employer_info + " AS z" +
                    "  WHERE z." + SQLiteHelper.Column_employer_info_account_uid_1 + " = " +
                    SQLiteHelper.Column_schedule_account_uid_2 + ")," +
                    " ((SELECT COUNT(" + SQLiteHelper.Column_work_service_uid + ") " +
                    "   FROM " + SQLiteHelper.Table_work_service +
                    "   WHERE " + SQLiteHelper.Column_work_service_update_status + " != 99 " +
                    "   AND " + SQLiteHelper.Column_work_service_schedule_uid + " = " +
                    "   a." + SQLiteHelper.Column_schedule_uid + ") + " +
                    " (SELECT COUNT(" + SQLiteHelper.Column_work_case_record_uid + ") " +
                    "  FROM " + SQLiteHelper.Table_work_case_record +
                    "  WHERE " + SQLiteHelper.Column_work_case_record_update_status + " != 99 " +
                    "  AND " + SQLiteHelper.Column_work_case_record_schedule_uid + " = " +
                    "  a." + SQLiteHelper.Column_schedule_uid + "))" +
                    " FROM " + SQLiteHelper.Table_schedule + " AS a" +
                    " WHERE a." + SQLiteHelper.Column_schedule_start + " LIKE ?" +
                    " AND a." + SQLiteHelper.Column_schedule_account_uid_1 + " = " +
                    " ( SELECT z." + SQLiteHelper.Column_equipment_account_uid +
                    "   FROM " + SQLiteHelper.Table_equipment + " AS z" +
                    "   WHERE z." + SQLiteHelper.Column_equipment_macaddress + " = ? )" +
                    " ORDER BY a." + SQLiteHelper.Column_schedule_start;
            String[] strWhere = new String[]{
                    MainActivity.getDateNow(0, "yyyy/MM/dd") + " %",
                    MainActivity.getDeviceUUID()
            };
            Cursor cursor = db.rawQuery(sql, strWhere);
            if (cursor != null) {
                while (cursor.moveToNext()) {
                    int uid = cursor.getInt(0);
                    String start = new SimpleDateFormat("HH:mm").format(new Date(cursor.getString(1)));
                    String end = new SimpleDateFormat("HH:mm").format(new Date(cursor.getString(2)));
                    String name = cursor.getString(3);
                    int status = cursor.getInt(4);
                    result.add(new WorkListModel(uid, name, start, end, status));
                }
                cursor.close();
            }
            db.close();
        } catch (Exception ex) {
            Log.d(TAG, "getWorkLists() called with: Exception - " + ex.getMessage());
            result.clear();
        }
        return result;
    }

}
