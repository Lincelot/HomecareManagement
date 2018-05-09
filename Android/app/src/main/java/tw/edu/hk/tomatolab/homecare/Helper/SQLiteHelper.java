package tw.edu.hk.tomatolab.homecare.Helper;

import android.content.Context;
import android.database.Cursor;
import android.database.DatabaseUtils;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;

import java.util.Arrays;
import java.util.List;

/**
 * Created by Minsheng on 2015/12/14.
 */
public class SQLiteHelper extends SQLiteOpenHelper {

    public static final String TAG = "SQLite";
    //資料庫名稱
    private static final String DATABASE_NAME = "homecare.db";
    //資料庫版本
    private static final int DATABASE_VERSION = 57;
    private SQLiteDatabase db;
    private Cursor cursor;


    //region Table_service
    public static final String Table_service = "service";
    public static final String Column_service_uid = "uid";
    public static final String Column_service_service_item_uid = "service_item_uid";
    public static final String Column_service_name = "name";
    public static final String Column_service_edit_time = "edit_time";
    public static final String Column_service_isdelete = "isdelete";
    //endregion

    //region Table_equipment
    public static final String Table_equipment = "equipment";
    public static final String Column_equipment_uid = "uid";
    public static final String Column_equipment_account_uid = "account_uid";
    public static final String Column_equipment_macaddress = "macaddress";
    public static final String Column_equipment_type = "type";
    public static final String Column_equipment_edit_time = "edit_time";
    //endregion

    //region Table_schedule
    public static final String Table_schedule = "schedule";
    public static final String Column_schedule_uid = "uid";
    //照服員編號
    public static final String Column_schedule_account_uid_1 = "account_uid_1";
    //案主編號
    public static final String Column_schedule_account_uid_2 = "account_uid_2";
    public static final String Column_schedule_start = "start";
    public static final String Column_schedule_end = "end";
    public static final String Column_schedule_edit_time = "edit_time";
    public static final String Column_schedule_summary = "summary";
    //endregion

    //region Table_schedule_service
    public static final String Table_schedule_service = "schedule_service";
    public static final String Column_schedule_service_uid = "uid";
    public static final String Column_schedule_service_schedule_uid = "schedule_uid";
    public static final String Column_schedule_service_service_uid = "service_uid";
    //endregion

    //region Table_employer_info
    public static final String Table_employer_info = "employer_info";
    public static final String Column_employer_info_uid = "uid";
    public static final String Column_employer_info_account_uid_1 = "account_uid_1";
    public static final String Column_employer_info_birthday = "birthday";
    public static final String Column_employer_info_sex = "sex";
    public static final String Column_employer_info_address = "address";
    public static final String Column_employer_info_employer_name = "employer_name";
    public static final String Column_employer_info_employer_phone1 = "employer_phone1";
    public static final String Column_employer_info_employer_phone2 = "employer_phone2";
    public static final String Column_employer_info_employer_item1 = "employer_item1";
    public static final String Column_employer_info_employer_item2 = "employer_item2";
    public static final String Column_employer_info_employer_item3 = "employer_item3";
    public static final String Column_employer_info_emg1_name = "emg1_displayname";
    public static final String Column_employer_info_emg1_phone1 = "emg1_phone1";
    public static final String Column_employer_info_emg1_phone2 = "emg1_phone2";
    public static final String Column_employer_info_emg2_name = "emg2_displayname";
    public static final String Column_employer_info_emg2_phone1 = "emg2_phone1";
    public static final String Column_employer_info_emg2_phone2 = "emg2_phone2";
    public static final String Column_employer_info_summary = "summary";
    //endregion

    //region Table_case_serivce_record_item
    public static final String Table_case_serivce_record_item = "case_serivce_record_item";
    public static final String Column_case_serivce_record_item_uid = "uid";
    public static final String Column_case_serivce_record_item_name = "name";
    //endregion

    //region Table_case_serivce_record_answer
    public static final String Table_case_serivce_record_answer = "case_serivce_record_answer";
    public static final String Column_case_serivce_record_answer_uid = "uid";
    public static final String Column_case_serivce_record_answer_case_serivce_record_item_uid = "case_serivce_record_item_uid";
    public static final String Column_case_serivce_record_answer_name = "name";
    //endregion

    //region Table_work_service
    public static final String Table_work_service = "work_service";
    public static final String Column_work_service_uid = "uid";
    public static final String Column_work_service_schedule_uid = "schedule_uid";
    public static final String Column_work_service_service_uid = "service_uid";
    public static final String Column_work_service_minutes = "minutes";
    public static final String Column_work_service_summary = "summary";
    public static final String Column_work_service_update_status = "update_status";
    //endregion

    //region Table_work_case_record
    public static final String Table_work_case_record = "work_case_record";
    public static final String Column_work_case_record_uid = "uid";
    public static final String Column_work_case_record_schedule_uid = "schedule_uid";
    public static final String Column_work_case_record_case_record_answer_uid = "case_record_answer_uid";
    public static final String Column_work_case_record_summary = "summary";
    public static final String Column_work_case_record_update_status = "update_status";
    //endregion\

    //region Table_work_record
    public static final String Table_work_record = "work_record";
    public static final String Column_work_record_uid = "uid";
    public static final String Column_work_record_start = "start";
    public static final String Column_work_record_end = "end";
    //Beacon UID
    public static final String Column_work_record_equipment_uid_1 = "equipment_uid_1";
    //Mobile UID
    public static final String Column_work_record_equipment_uid_2 = "equipment_uid_2";
    public static final String Column_work_record_update_status = "update_status";
    //endregion

    public SQLiteHelper(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
        db = this.getWritableDatabase();
    }

    public void open() {
        if (!db.isOpen()) {
            db = this.getWritableDatabase();
        }
    }

    @Override
    public synchronized void close() {
        super.close();
        if (cursor != null && !cursor.isClosed()) {
            cursor.close();
        }
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        Log.d(TAG, "onCreate()");
        String Create_service = "CREATE TABLE IF NOT EXISTS " + Table_service + " ( " +
                Column_service_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_service_service_item_uid + " INTEGER, " +
                Column_service_name + " VARCHAR, " +
                Column_service_edit_time + " DATETIME, " +
                Column_service_isdelete + " TINYINT" +
                ");";
        String Create_equipment = "CREATE TABLE IF NOT EXISTS " + Table_equipment + " ( " +
                Column_equipment_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_equipment_account_uid + " INTEGER, " +
                Column_equipment_macaddress + " VARCHAR, " +
                Column_equipment_type + " TINYINT, " +
                Column_equipment_edit_time + " DATETIME" +
                ");";
        String Create_schedule = "CREATE TABLE IF NOT EXISTS " + Table_schedule + " ( " +
                Column_schedule_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_schedule_account_uid_1 + " INTEGER, " +
                Column_schedule_account_uid_2 + " INTEGER, " +
                Column_schedule_start + " DATETIME, " +
                Column_schedule_end + " DATETIME, " +
                Column_schedule_edit_time + " DATETIME, " +
                Column_schedule_summary + " VARCHAR " +
                ");";
        String Create_schedule_service = "CREATE TABLE IF NOT EXISTS " + Table_schedule_service + " ( " +
                Column_schedule_service_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_schedule_service_schedule_uid + " INTEGER, " +
                Column_schedule_service_service_uid + " INTEGER" +
                ");";
        String Create_employer_info = "CREATE TABLE IF NOT EXISTS " + Table_employer_info + " ( " +
                Column_employer_info_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_employer_info_account_uid_1 + " INTEGER, " +
                Column_employer_info_birthday + " DATETIME, " +
                Column_employer_info_sex + " TINYINT, " +
                Column_employer_info_address + " VARCHAR, " +
                Column_employer_info_employer_name + " VARCHAR, " +
                Column_employer_info_employer_phone1 + " VARCHAR, " +
                Column_employer_info_employer_phone2 + " VARCHAR, " +
                Column_employer_info_employer_item1 + " VARCHAR, " +
                Column_employer_info_employer_item2 + " VARCHAR, " +
                Column_employer_info_employer_item3 + " VARCHAR, " +
                Column_employer_info_emg1_name + " VARCHAR, " +
                Column_employer_info_emg1_phone1 + " VARCHAR, " +
                Column_employer_info_emg1_phone2 + " VARCHAR, " +
                Column_employer_info_emg2_name + " VARCHAR, " +
                Column_employer_info_emg2_phone1 + " VARCHAR, " +
                Column_employer_info_emg2_phone2 + " VARCHAR, " +
                Column_employer_info_summary + " VARCHAR " +
                ");";
        String Create_case_serivce_record_item = "CREATE TABLE IF NOT EXISTS " + Table_case_serivce_record_item + " ( " +
                Column_case_serivce_record_item_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_case_serivce_record_item_name + " VARCHAR " +
                ");";
        String Create_case_serivce_record_answer = "CREATE TABLE IF NOT EXISTS " + Table_case_serivce_record_answer + " ( " +
                Column_case_serivce_record_answer_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_case_serivce_record_answer_case_serivce_record_item_uid + " INTEGER, " +
                Column_case_serivce_record_answer_name + " VARCHAR " +
                ");";
        String Create_work_service = "CREATE TABLE IF NOT EXISTS " + Table_work_service + " ( " +
                Column_work_service_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_work_service_schedule_uid + " INTEGER, " +
                Column_work_service_service_uid + " INTEGER, " +
                Column_work_service_minutes + " INTEGER, " +
                Column_work_service_summary + " VARCHAR, " +
                Column_work_service_update_status + " INTEGER " +
                ");";
        String Create_work_case_record = "CREATE TABLE IF NOT EXISTS " + Table_work_case_record + " ( " +
                Column_work_case_record_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_work_case_record_schedule_uid + " INTEGER, " +
                Column_work_case_record_case_record_answer_uid + " INTEGER, " +
                Column_work_case_record_summary + " VARCHAR, " +
                Column_work_case_record_update_status + " INTEGER " +
                ");";
        String Create_work_record = "CREATE TABLE IF NOT EXISTS " + Table_work_record + " ( " +
                Column_work_record_uid + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                Column_work_record_start + " DATETIME, " +
                Column_work_record_end + " DATETIME, " +
                Column_work_record_equipment_uid_1 + " INTEGER, " +
                Column_work_record_equipment_uid_2 + " INTEGER, " +
                Column_work_record_update_status + " INTEGER " +
                ");";
        db.execSQL(Create_service);
        db.execSQL(Create_equipment);
        db.execSQL(Create_schedule);
        db.execSQL(Create_schedule_service);
        db.execSQL(Create_employer_info);
        db.execSQL(Create_case_serivce_record_item);
        db.execSQL(Create_case_serivce_record_answer);
        db.execSQL(Create_work_service);
        db.execSQL(Create_work_case_record);
        db.execSQL(Create_work_record);
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        String Drop_service = "DROP TABLE IF EXISTS " + Table_service + ";";
        String Drop_equipment = "DROP TABLE IF EXISTS " + Table_equipment + ";";
        String Drop_schedule = "DROP TABLE IF EXISTS " + Table_schedule + ";";
        String Drop_schedule_service = "DROP TABLE IF EXISTS " + Table_schedule_service + ";";
        String Drop_employer_info = "DROP TABLE IF EXISTS " + Table_employer_info + ";";
        String Drop_case_serivce_record_item = "DROP TABLE IF EXISTS " + Table_case_serivce_record_item + ";";
        String Drop_case_serivce_record_answer = "DROP TABLE IF EXISTS " + Table_case_serivce_record_answer + ";";
        String Drop_work_service = "DROP TABLE IF EXISTS " + Table_work_service + ";";
        String Drop_work_case_record = "DROP TABLE IF EXISTS " + Table_work_case_record + ";";
        String Drop_work_record = "DROP TABLE IF EXISTS " + Table_work_record + ";";
        db.execSQL(Drop_service);
        db.execSQL(Drop_equipment);
        db.execSQL(Drop_schedule);
        db.execSQL(Drop_schedule_service);
        db.execSQL(Drop_employer_info);
        db.execSQL(Drop_case_serivce_record_item);
        db.execSQL(Drop_case_serivce_record_answer);
        db.execSQL(Drop_work_service);
        db.execSQL(Drop_work_case_record);
        db.execSQL(Drop_work_record);
        onCreate(db);
    }

    public void execSQL(String sql) {
        db.beginTransaction();
        try {
            db.execSQL(sql);
            db.setTransactionSuccessful();
        } catch (Exception ex) {
            Log.d(TAG, "execSQL() called with: " + "sql = [" + sql + "]");
            Log.d(TAG, "execSQL() called with: " + "error = [" + ex.getMessage() + "]");
        } finally {
            db.endTransaction();
        }
    }

    public void execSQL(String sql, Object[] objects) {
        db.beginTransaction();
        try {
            db.execSQL(sql, objects);
            db.setTransactionSuccessful();
        } catch (Exception ex) {
            Log.d(TAG, "execSQL() called with: " + "sql = [" + sql + "], objects = [" + Arrays.toString(objects) + "]");
            Log.d(TAG, "execSQL() called with: " + "error = [" + ex.getMessage() + "]");
        } finally {
            db.endTransaction();
        }
    }

    public void insertMulti(String tableName, List<String[]> data) {
        String sql = "INSERT INTO " + tableName + " VALUES";
        for (String[] strings : data) {
            sql += " ( ";
            for (String str : strings) {
                if (str == null) {
                    sql += "null,";
                } else {
                    sql += "'" + str + "',";
                }
            }
            sql = sql.substring(0, sql.length() - 1);
            sql += " ),";
        }
        sql = sql.substring(0, sql.length() - 1);
        db.beginTransaction();
        try {
            db.execSQL(sql);
            db.setTransactionSuccessful();
        } catch (Exception ex) {
            Log.d(TAG, "insertMulti() called with: " + "sql = [" + sql + "]");
            Log.d(TAG, "insertMulti() called with: " + "error = [" + ex.getMessage() + "]");
        } finally {
            db.endTransaction();
        }
    }

    public Cursor rawQuery(String sql, String[] strings) {
        db.beginTransaction();
        try {
            cursor = db.rawQuery(sql, strings);
            db.setTransactionSuccessful();
        } catch (Exception ex) {
            Log.d(TAG, "rawQuery() called with: " + "sql = [" + sql + "], objects = [" + Arrays.toString(strings) + "]");
            Log.d(TAG, "rawQuery() called with: " + "error = [" + ex.getMessage() + "]");
        } finally {
            db.endTransaction();
        }
        return cursor;
    }

    public void getAll(String tableName) {
        String sql = "SELECT * FROM " + tableName;
        rawQuery(sql, null);
        Log.d(TAG, "getAll: " + DatabaseUtils.dumpCursorToString(cursor));
    }

    public void clearTable(String tableName) {
        String sql = "DELETE FROM " + tableName;
        execSQL(sql);
    }

}
