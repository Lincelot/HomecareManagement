package tw.edu.hk.tomatolab.homecare.Model;

/**
 * Created by Minsheng on 2015/12/16.
 */
public class ServiceRecordModel {
    private int uid;
    private int schedule_uid;
    private int service_uid;
    private int minutes;
    private String summary;

    public ServiceRecordModel(int uid, int schedule_uid, int service_uid, int minutes, String summary) {
        this.uid = uid;
        this.schedule_uid = schedule_uid;
        this.service_uid = service_uid;
        this.minutes = minutes;
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

    public int getService_uid() {
        return service_uid;
    }

    public void setService_uid(int service_uid) {
        this.service_uid = service_uid;
    }

    public int getMinutes() {
        return minutes;
    }

    public void setMinutes(int minutes) {
        this.minutes = minutes;
    }

    public String getSummary() {
        return summary;
    }

    public void setSummary(String summary) {
        this.summary = summary;
    }
}
