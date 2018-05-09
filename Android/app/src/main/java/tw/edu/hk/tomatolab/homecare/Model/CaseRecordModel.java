package tw.edu.hk.tomatolab.homecare.Model;

/**
 * Created by Minsheng on 2015/12/16.
 */
public class CaseRecordModel {
    private int uid;
    private int schedule_uid;
    private int case_record_answer_uid;
    private String summary;

    public CaseRecordModel(int uid, int schedule_uid, int case_record_answer_uid, String summary) {
        this.uid = uid;
        this.schedule_uid = schedule_uid;
        this.case_record_answer_uid = case_record_answer_uid;
        this.summary = summary;
    }

    public int getUid() {
        return uid;
    }

    public void setUid(int uid) {
        this.uid = uid;
    }

    public int getSchedule_uid() {
        return schedule_uid;
    }

    public void setSchedule_uid(int schedule_uid) {
        this.schedule_uid = schedule_uid;
    }

    public int getCase_record_answer_uid() {
        return case_record_answer_uid;
    }

    public void setCase_record_answer_uid(int case_record_answer_uid) {
        this.case_record_answer_uid = case_record_answer_uid;
    }

    public String getSummary() {
        return summary;
    }

    public void setSummary(String summary) {
        this.summary = summary;
    }
}
