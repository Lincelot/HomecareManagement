package tw.edu.hk.tomatolab.homecare.Activity;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.design.widget.TabLayout;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;

import tw.edu.hk.tomatolab.homecare.Adapter.FormCaseAdapter;
import tw.edu.hk.tomatolab.homecare.Adapter.FormServiceAdapter;
import tw.edu.hk.tomatolab.homecare.Helper.HttpHelper;
import tw.edu.hk.tomatolab.homecare.Helper.SQLiteHelper;
import tw.edu.hk.tomatolab.homecare.Model.CaseRecordModel;
import tw.edu.hk.tomatolab.homecare.Model.FormQuestionModel;
import tw.edu.hk.tomatolab.homecare.Model.ServiceRecordModel;
import tw.edu.hk.tomatolab.homecare.R;

public class FromRecordActivity extends AppCompatActivity {

    private static final String TAG = "FromRecordActivity";
    private static int uid = 0;
    private AlertDialog.Builder alertDialog;
    private SQLiteHelper db;
    private List<ServiceRecordModel> listServiceRecord;
    private List<CaseRecordModel> listCaseRecord;
    List<FormQuestionModel> formServiceQuestion;
    List<Integer> formServiceAnswer;
    List<FormQuestionModel> formCaseQuestion;
    List<String[]> formCaseAnswer;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_from_record);

        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        //Override左上角返回事件，否則會重啟MainActivity
        toolbar.setNavigationOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });
        try {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        } catch (Exception e) {
            Log.d(TAG, "getSupportActionBar().setDisplayHomeAsUpEnabled(true) - NullPointerException:" + e.getMessage());
        }
        // Create the adapter that will return a fragment for each of the three
        // primary sections of the activity.
        /*
      The {@link android.support.v4.view.PagerAdapter} that will provide
      fragments for each of the sections. We use a
      {@link FragmentPagerAdapter} derivative, which will keep every
      loaded fragment in memory. If this becomes too memory intensive, it
      may be best to switch to a
      {@link android.support.v4.app.FragmentStatePagerAdapter}.
     */
        SectionsPagerAdapter mSectionsPagerAdapter = new SectionsPagerAdapter(getSupportFragmentManager());

        // Set up the ViewPager with the sections adapter.
        /*
      The {@link ViewPager} that will host the section contents.
     */
        ViewPager mViewPager = (ViewPager) findViewById(R.id.container);
        mViewPager.setAdapter(mSectionsPagerAdapter);

        TabLayout tabLayout = (TabLayout) findViewById(R.id.tabs);
        tabLayout.setupWithViewPager(mViewPager);

        //接收來自上一個頁面傳送的值
        uid = getIntent().getIntExtra("uid", 0);

        listServiceRecord = new ArrayList<>();
        listCaseRecord = new ArrayList<>();
        formServiceQuestion = new ArrayList<>();
        formServiceAnswer = new ArrayList<>();
        formCaseQuestion = new ArrayList<>();
        formCaseAnswer = new ArrayList<>();

        FloatingActionButton fabSendRecord = (FloatingActionButton) findViewById(R.id.fabSendRecord);
        fabSendRecord.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(final View view) {
                formServiceQuestion.clear();
                formServiceAnswer.clear();
                formCaseQuestion.clear();
                formCaseAnswer.clear();
                formServiceQuestion.addAll(FormServiceAdapter.getQuestion());
                formServiceAnswer.addAll(FormServiceAdapter.getAnswer());
                formCaseQuestion.addAll(FormCaseAdapter.getQuestion());
                formCaseAnswer.addAll(FormCaseAdapter.getAnswer());
                int totalMinutes = 0;
                for (int i = 0; i < formServiceAnswer.size(); i++) {
                    totalMinutes += formServiceAnswer.get(i);
                }
                alertDialog = new AlertDialog.Builder(view.getContext());
                alertDialog.setTitle(R.string.alertdialog_formrecord_send_title);
                alertDialog.setMessage("填寫時數：" + totalMinutes + "分");
                alertDialog.setNeutralButton(R.string.alertdialog_formrecord_send_close, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        //Close
                    }
                });
                alertDialog.setNegativeButton(R.string.alertdialog_formrecord_send, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        new Thread(new Runnable() {
                            @Override
                            public void run() {
                                listServiceRecord.clear();
                                listCaseRecord.clear();
                                String sql1 = "UPDATE " + SQLiteHelper.Table_work_service +
                                        " SET " + SQLiteHelper.Column_work_service_minutes + " = ?" +
                                        " WHERE " + SQLiteHelper.Column_work_service_schedule_uid + " = ?" +
                                        " AND " + SQLiteHelper.Column_work_service_service_uid + " = ?" +
                                        " AND " + SQLiteHelper.Column_work_service_update_status + " = ?";
                                String sql2 = "UPDATE " + SQLiteHelper.Table_work_case_record +
                                        " SET " + SQLiteHelper.Column_work_case_record_case_record_answer_uid + " = ?, " +
                                        SQLiteHelper.Column_work_case_record_summary + " = ?" +
                                        " WHERE " + SQLiteHelper.Column_work_case_record_schedule_uid + " = ?" +
                                        " AND " + SQLiteHelper.Column_work_case_record_case_record_answer_uid + " = ?" +
                                        " AND " + SQLiteHelper.Column_work_case_record_update_status + " = ?";
                                String sql3 = "UPDATE " + SQLiteHelper.Table_work_service +
                                        " SET " + SQLiteHelper.Column_work_service_update_status + " = ?" +
                                        " WHERE " + SQLiteHelper.Column_work_service_schedule_uid + " = ?";
                                String sql4 = "UPDATE " + SQLiteHelper.Table_work_case_record +
                                        " SET " + SQLiteHelper.Column_work_case_record_update_status + " = ?" +
                                        " WHERE " + SQLiteHelper.Column_work_case_record_schedule_uid + " = ?";
                                String sql5 = "SELECT *" +
                                        " FROM " + SQLiteHelper.Table_work_service +
                                        " WHERE " + SQLiteHelper.Column_work_case_record_update_status + " = ?" +
                                        " AND " + SQLiteHelper.Column_work_case_record_schedule_uid + " = ?";
                                String sql6 = "SELECT *" +
                                        " FROM " + SQLiteHelper.Table_work_case_record +
                                        " WHERE " + SQLiteHelper.Column_work_case_record_update_status + " = ?" +
                                        " AND " + SQLiteHelper.Column_work_case_record_schedule_uid + " = ?";
                                db = new SQLiteHelper(view.getContext());
                                for (int i = 0; i < formServiceQuestion.size(); i++) {
                                    db.execSQL(sql1, new String[]{
                                            String.valueOf(formServiceAnswer.get(i)),
                                            String.valueOf(uid),
                                            String.valueOf(formServiceQuestion.get(i).getTitleUID()),
                                            String.valueOf(99)
                                    });
                                }
                                for (int i = 0; i < formCaseQuestion.size(); i++) {
                                    for (int j = 0; j < formCaseQuestion.get(i).getContentUID().size(); j++) {
                                        db.execSQL(sql2, new String[]{
                                                formCaseAnswer.get(i)[0],
                                                formCaseAnswer.get(i)[1],
                                                String.valueOf(uid),
                                                String.valueOf(formCaseQuestion.get(i).getContentUID(j)),
                                                String.valueOf(99)
                                        });
                                    }
                                }
                                db.execSQL(sql3, new Integer[]{0, uid});
                                db.execSQL(sql4, new Integer[]{0, uid});
                                Cursor cursor = db.rawQuery(sql5, new String[]{"0", String.valueOf(uid)});
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
                                cursor = db.rawQuery(sql6, new String[]{"0", String.valueOf(uid)});
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
                                // region PostDataToServer
                                runOnUiThread(new Runnable() {
                                    public void run() {
                                        //這邊是呼叫main thread handler幫我們處理UI部分
                                        if (getNetworkStatus()) {
                                            new HttpHelper().sendRecord(view.getContext(),
                                                    MainActivity.getDeviceUUID(),
                                                    listServiceRecord,
                                                    listCaseRecord
                                            );
                                        } else {
                                            Snackbar.make(view, R.string.alertdialog_networkstatus_message, Snackbar.LENGTH_SHORT).show();
                                        }
                                        finish();
                                    }
                                });
                                //endregion
                            }
                        }).start();

                        showToast("已送出！");


                    }
                });
                alertDialog.show();
            }
        });
    }

    private void showToast(String msg) {
        Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
    }

    private boolean getNetworkStatus() {
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(this);
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

    /**
     * A placeholder fragment containing a simple view.
     */
    public static class PlaceholderFragment extends Fragment {
        /**
         * The fragment argument representing the section number for this
         * fragment.
         */
        private static final String ARG_SECTION_NUMBER = "section_number";
        private FormServiceAdapter formServiceAdapter;
        private FormCaseAdapter formCaseAdapter;
        private Context context;
        private int page = 0;
        private SQLiteHelper db;

        private List<FormQuestionModel> formServiceQuestion;
        private List<Integer> formServiceAnswer;
        private List<FormQuestionModel> formCaseQuestion;
        private List<String[]> formCaseAnswer;

        public PlaceholderFragment() {
        }

        /**
         * Returns a new instance of this fragment for the given section
         * number.
         */
        public static PlaceholderFragment newInstance(int sectionNumber) {
            PlaceholderFragment fragment = new PlaceholderFragment();
            Bundle args = new Bundle();
            args.putInt(ARG_SECTION_NUMBER, sectionNumber);
            fragment.setArguments(args);
            return fragment;
        }

        @Override
        public View onCreateView(LayoutInflater inflater, ViewGroup container,
                                 Bundle savedInstanceState) {
            View rootView = inflater.inflate(R.layout.fragment_from_record, container, false);
            context = rootView.getContext();

            RecyclerView recyclerView = (RecyclerView) rootView.findViewById(R.id.form_record_recyclerbiew);
            LinearLayoutManager layoutManager = new LinearLayoutManager(context);
            layoutManager.setOrientation(LinearLayoutManager.VERTICAL);

            page = getArguments().getInt(ARG_SECTION_NUMBER);
            db = new SQLiteHelper(context);

            switch (page) {
                case 1: {
                    formServiceAdapter = new FormServiceAdapter();
                    recyclerView.setLayoutManager(layoutManager);
                    recyclerView.setAdapter(formServiceAdapter);

                    //region Initialize DATA
                    new Thread(new Runnable() {
                        @Override
                        public void run() {
                            final List<FormQuestionModel> formQuestions = new ArrayList<>();
                            String sql = "SELECT " + SQLiteHelper.Column_service_uid + ", " +
                                    SQLiteHelper.Column_service_name +
                                    " FROM " + SQLiteHelper.Table_service +
                                    " WHERE " + SQLiteHelper.Column_service_isdelete + " = ?" +
                                    " ORDER BY " + SQLiteHelper.Column_service_name;
                            Cursor cursor = db.rawQuery(sql, new String[]{"0"});
                            if (cursor != null) {
                                while (cursor.moveToNext()) {
                                    int titleUID = cursor.getInt(0);
                                    String title = cursor.getString(1);
                                    formQuestions.add(new FormQuestionModel(titleUID, title, null, null, null));
                                }
                                cursor.close();
                            }
                            sql = "SELECT * " +
                                    " FROM " + SQLiteHelper.Table_work_service +
                                    " WHERE " + SQLiteHelper.Column_work_service_schedule_uid + " = ?";
                            cursor = db.rawQuery(sql, new String[]{String.valueOf(uid)});
                            if (cursor != null) {
                                boolean isWrite = false;
                                if (cursor.getCount() > 0) {
                                    isWrite = true;
                                }
                                if (!isWrite) {
                                    List<String[]> list = new ArrayList<>();
                                    for (int i = 0; i < formQuestions.size(); i++) {
                                        list.add(new String[]{
                                                null,
                                                String.valueOf(uid),
                                                String.valueOf(formQuestions.get(i).getTitleUID()),
                                                String.valueOf(0),
                                                null,
                                                String.valueOf(99)
                                        });
                                    }
                                    db.insertMulti(SQLiteHelper.Table_work_service, list);
                                }
                                cursor.close();
                            }

                            final List<Integer> intAnswer = new ArrayList<>();
                            sql = "SELECT " + SQLiteHelper.Column_work_service_minutes +
                                    " FROM " + SQLiteHelper.Table_work_service +
                                    " WHERE " + SQLiteHelper.Column_work_service_schedule_uid + " = ?";
                            cursor = db.rawQuery(sql, new String[]{
                                    String.valueOf(uid)
                            });
                            if (cursor != null) {
                                while (cursor.moveToNext()) {
                                    intAnswer.add(cursor.getInt(0));
                                }
                                cursor.close();
                            }
                            db.close();
                            ((Activity) context).runOnUiThread(new Runnable() {
                                public void run() {
                                    //這邊是呼叫main thread handler幫我們處理UI部分
                                    formServiceAdapter.replaceWith(formQuestions, intAnswer);
                                }
                            });
                        }
                    }).start();
                    //endregion

                    break;
                }
                case 2: {
                    formCaseAdapter = new FormCaseAdapter();
                    recyclerView.setLayoutManager(layoutManager);
                    recyclerView.setAdapter(formCaseAdapter);

                    //region Initialize DATA
                    new Thread(new Runnable() {
                        @Override
                        public void run() {
                            final List<FormQuestionModel> formQuestions = new ArrayList<>();
                            String sql = "SELECT * " +
                                    "FROM " + SQLiteHelper.Table_case_serivce_record_item;
                            Cursor cursor = db.rawQuery(sql, null);
                            if (cursor != null) {
                                while (cursor.moveToNext()) {
                                    int titleUID = cursor.getInt(0);
                                    String title = cursor.getString(1);
                                    formQuestions.add(new FormQuestionModel(titleUID, title, null, null, null));
                                }
                                cursor.close();
                            }

                            sql = "SELECT * " +
                                    "FROM " + SQLiteHelper.Table_case_serivce_record_answer;
                            cursor = db.rawQuery(sql, null);
                            if (cursor != null) {
                                while (cursor.moveToNext()) {
                                    int titleUID = cursor.getInt(1);
                                    int contentUID = cursor.getInt(0);
                                    String content = cursor.getString(2);
                                    for (FormQuestionModel item : formQuestions) {
                                        if (item.getTitleUID() == titleUID) {
                                            List<Integer> tmp_contentUID = item.getContentUID();
                                            List<String> tmp_content = item.getContent();
                                            if (tmp_contentUID == null) {
                                                tmp_contentUID = new ArrayList<>();
                                            }
                                            if (tmp_content == null) {
                                                tmp_content = new ArrayList<>();
                                            }
                                            tmp_contentUID.add(contentUID);
                                            tmp_content.add(content);
                                            item.setContentUID(tmp_contentUID);
                                            item.setContent(tmp_content);
                                            break;
                                        }
                                    }
                                }
                                cursor.close();
                            }
                            sql = "SELECT * " +
                                    " FROM " + SQLiteHelper.Table_work_case_record +
                                    " WHERE " + SQLiteHelper.Column_work_case_record_schedule_uid + " = ?";
                            cursor = db.rawQuery(sql, new String[]{String.valueOf(uid)});
                            if (cursor != null) {
                                boolean isWrite = false;
                                if (cursor.getCount() > 0) {
                                    isWrite = true;
                                }
                                if (!isWrite) {
                                    List<String[]> list = new ArrayList<>();
                                    for (int i = 0; i < formQuestions.size(); i++) {
                                        int tmp = formQuestions.get(i).getContentUID().get(0);
                                        list.add(new String[]{
                                                null,
                                                String.valueOf(uid),
                                                String.valueOf(tmp),
                                                null,
                                                String.valueOf(99)
                                        });
                                    }
                                    db.insertMulti(SQLiteHelper.Table_work_case_record, list);
                                }
                                cursor.close();
                            }

                            final List<String[]> strAnswer = new ArrayList<>();
                            sql = "SELECT " + SQLiteHelper.Column_work_case_record_case_record_answer_uid + ", " +
                                    SQLiteHelper.Column_work_case_record_summary +
                                    " FROM " + SQLiteHelper.Table_work_case_record +
                                    " WHERE " + SQLiteHelper.Column_work_case_record_schedule_uid + " = ?";
                            cursor = db.rawQuery(sql, new String[]{
                                    String.valueOf(uid)
                            });
                            if (cursor != null) {
                                while (cursor.moveToNext()) {
                                    strAnswer.add(new String[]{cursor.getString(0), cursor.getString(1)});
                                }
                                cursor.close();
                            }

                            db.close();
                            ((Activity) context).runOnUiThread(new Runnable() {
                                public void run() {
                                    //這邊是呼叫main thread handler幫我們處理UI部分
                                    formCaseAdapter.replaceWith(formQuestions, strAnswer);
                                }
                            });
                        }
                    }).start();
                    //endregion

                    break;
                }
            }

            return rootView;
        }

        @Override
        public void onPause() {

            //save
            new Thread(new Runnable() {
                @Override
                public void run() {
                    saveRecord(page);
                }
            }).start();

            super.onPause();
        }

        private void saveRecord(int page) {
            switch (page) {
                case 1: {
                    db.open();
                    formServiceQuestion = FormServiceAdapter.getQuestion();
                    formServiceAnswer = FormServiceAdapter.getAnswer();
                    String sql = "UPDATE " + SQLiteHelper.Table_work_service +
                            " SET " + SQLiteHelper.Column_work_service_minutes + " = ?" +
                            " WHERE " + SQLiteHelper.Column_work_service_schedule_uid + " = ?" +
                            " AND " + SQLiteHelper.Column_work_service_service_uid + " = ?" +
                            " AND " + SQLiteHelper.Column_work_service_update_status + " = ?";
                    for (int i = 0; i < formServiceQuestion.size(); i++) {
                        db.execSQL(sql, new String[]{
                                String.valueOf(formServiceAnswer.get(i)),
                                String.valueOf(uid),
                                String.valueOf(formServiceQuestion.get(i).getTitleUID()),
                                String.valueOf(99)
                        });
                    }
                    db.close();

                    break;
                }
                case 2: {
                    db.open();
                    formCaseQuestion = FormCaseAdapter.getQuestion();
                    formCaseAnswer = FormCaseAdapter.getAnswer();
                    String sql = "UPDATE " + SQLiteHelper.Table_work_case_record +
                            " SET " + SQLiteHelper.Column_work_case_record_case_record_answer_uid + " = ?, " +
                            SQLiteHelper.Column_work_case_record_summary + " = ?" +
                            " WHERE " + SQLiteHelper.Column_work_case_record_schedule_uid + " = ?" +
                            " AND " + SQLiteHelper.Column_work_case_record_case_record_answer_uid + " = ?" +
                            " AND " + SQLiteHelper.Column_work_case_record_update_status + " = ?";
                    for (int i = 0; i < formCaseQuestion.size(); i++) {
                        for (int j = 0; j < formCaseQuestion.get(i).getContentUID().size(); j++) {
                            db.execSQL(sql, new String[]{
                                    formCaseAnswer.get(i)[0],
                                    formCaseAnswer.get(i)[1],
                                    String.valueOf(uid),
                                    String.valueOf(formCaseQuestion.get(i).getContentUID(j)),
                                    String.valueOf(99)
                            });
                        }
                    }
                    db.close();
                    break;
                }
            }
        }

    }

    /**
     * A {@link FragmentPagerAdapter} that returns a fragment corresponding to
     * one of the sections/tabs/pages.
     */
    public class SectionsPagerAdapter extends FragmentPagerAdapter {

        public SectionsPagerAdapter(FragmentManager fm) {
            super(fm);
        }

        @Override
        public Fragment getItem(int position) {
            // getItem is called to instantiate the fragment for the given page.
            // Return a PlaceholderFragment (defined as a static inner class below).
            return PlaceholderFragment.newInstance(position + 1);
        }

        @Override
        public int getCount() {
            // Show 2 total pages.
            return 2;
        }

        @Override
        public CharSequence getPageTitle(int position) {
            switch (position) {
                case 0:
                    return "工作時數紀錄";
                case 1:
                    return "個案服務紀錄";
            }
            return null;
        }

    }
}
