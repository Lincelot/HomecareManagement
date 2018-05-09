package tw.edu.hk.tomatolab.homecare.Model;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by Minsheng on 15/12/16.
 */
public class FormQuestionModel {
    private int titleUID;
    private String title;
    private List<Integer> contentUID = new ArrayList<>();
    private List<String> content = new ArrayList<>();
    private String summary;

    public FormQuestionModel(int titleUID, String title, List<Integer> contentUID, List<String> content, String summary) {
        this.titleUID = titleUID;
        this.title = title;
        this.contentUID = contentUID;
        this.content = content;
        this.summary = summary;
    }

    public int getTitleUID() {
        return titleUID;
    }

    public void setTitleUID(int titleUID) {
        this.titleUID = titleUID;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public List<Integer> getContentUID() {
        return contentUID;
    }

    public void setContentUID(List<Integer> contentUID) {
        this.contentUID = contentUID;
    }

    public Integer getContentUID(int position) {
        return contentUID.get(position);
    }

    public List<String> getContent() {
        return content;
    }

    public void setContent(List<String> content) {
        this.content = content;
    }

    public String getContent(int position) {
        return content.get(position);
    }

    public String getSummary() {
        return summary;
    }

    public void setSummary(String summary) {
        this.summary = summary;
    }
}
