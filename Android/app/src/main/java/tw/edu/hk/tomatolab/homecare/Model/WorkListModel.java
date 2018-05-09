package tw.edu.hk.tomatolab.homecare.Model;

/**
 * Created by Minsheng on 2015/12/15.
 */
public class WorkListModel {
    private int uid;
    private String name;
    private String start;
    private String end;
    private int status;

    public WorkListModel(int uid, String name, String start, String end, int status) {
        this.uid = uid;
        this.name = name;
        this.start = start;
        this.end = end;
        this.status = status;
    }

    public int getUid() {
        return uid;
    }

    public void setUid(int uid) {
        this.uid = uid;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
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

    public int getStatus() {
        return status;
    }

    public void setStatus(int status) {
        this.status = status;
    }
}
