package tw.edu.hk.tomatolab.homecare.Model;

/**
 * Created by Minsheng on 2015/12/16.
 */
public class WorkRecordModel {
    private int uid;
    private String start;
    private String end;
    private int equipment_uid_1;
    private int equipment_uid_2;
    private int update_status;

    public WorkRecordModel(int uid, String start, String end, int equipment_uid_1, int equipment_uid_2, int update_status) {
        this.uid = uid;
        this.start = start;
        this.end = end;
        this.equipment_uid_1 = equipment_uid_1;
        this.equipment_uid_2 = equipment_uid_2;
        this.update_status = update_status;
    }

    public int getUid() {
        return uid;
    }

    public void setUid(int uid) {
        this.uid = uid;
    }

    public String getStart() {
        return start;
    }

    public void setStart(String start) {
        this.start = start;
    }

    public String getEnd() {
        return end;
    }

    public void setEnd(String end) {
        this.end = end;
    }

    public int getEquipment_uid_1() {
        return equipment_uid_1;
    }

    public void setEquipment_uid_1(int equipment_uid_1) {
        this.equipment_uid_1 = equipment_uid_1;
    }

    public int getEquipment_uid_2() {
        return equipment_uid_2;
    }

    public void setEquipment_uid_2(int equipment_uid_2) {
        this.equipment_uid_2 = equipment_uid_2;
    }

    public int getUpdate_status() {
        return update_status;
    }

    public void setUpdate_status(int update_status) {
        this.update_status = update_status;
    }
}
