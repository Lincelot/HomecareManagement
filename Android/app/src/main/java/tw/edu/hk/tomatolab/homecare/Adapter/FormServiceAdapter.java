package tw.edu.hk.tomatolab.homecare.Adapter;

import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.NumberPicker;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import tw.edu.hk.tomatolab.homecare.Model.FormQuestionModel;
import tw.edu.hk.tomatolab.homecare.R;

/**
 * Created by Minsheng on 2015/12/15.
 */
public class FormServiceAdapter extends RecyclerView.Adapter<FormServiceAdapter.ViewHolder> {

    private static final String TAG = "FormServiceAdapter";
    private static List<FormQuestionModel> formQuestions;
    private static List<Integer> formAnswers;
    private FormQuestionModel formQuestion;

    public FormServiceAdapter() {
        formQuestions = new ArrayList<>();
        formAnswers = new ArrayList<>();
    }

    @Override
    public FormServiceAdapter.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View v = LayoutInflater.from(parent.getContext()).inflate(R.layout.form_record_service_item, parent, false);
        return new ViewHolder(v);
    }

    @Override
    public void onBindViewHolder(final ViewHolder holder, int position) {
        formQuestion = formQuestions.get(position);
        holder.txtServiceName.setText(formQuestion.getTitle().replaceAll("[0-99]", "").replace(".", ""));
        //最大600分鐘
        holder.numpickerTime.setMaxValue(120);
        holder.numpickerTime.setMinValue(0);
        ArrayList<String> numberStr = new ArrayList<>();
        //單位五分鐘
        for (int i = 0; i < 605; i += 5) {
            numberStr.add(String.valueOf(i) + "分");
        }
        holder.numpickerTime.setDisplayedValues(numberStr.toArray(new String[numberStr.size()]));
        //禁止鍵盤
        holder.numpickerTime.setDescendantFocusability(NumberPicker.FOCUS_BLOCK_DESCENDANTS);
        holder.numpickerTime.setTag(position);

        //region 將使用者選擇的項目暫存至資料庫
        holder.numpickerTime.setOnValueChangedListener(new NumberPicker.OnValueChangeListener() {
            @Override
            public void onValueChange(NumberPicker picker, int oldVal, final int newVal) {
                int tag = (int) picker.getTag();
                formQuestion = formQuestions.get(tag);
                formAnswers.set(tag, newVal * 5);
            }
        });
        //endregion

        //取得暫存選項並顯示
        holder.numpickerTime.setValue(formAnswers.get(position) / 5);

    }

    @Override
    public int getItemCount() {
        return formQuestions.size();
    }

    public static List<Integer> getAnswer() {
        return formAnswers;
    }

    public static List<FormQuestionModel> getQuestion() {
        return formQuestions;
    }

    public void replaceWith(Collection<FormQuestionModel> formQuestions, Collection<Integer> formAnswers) {
        FormServiceAdapter.formQuestions.clear();
        FormServiceAdapter.formAnswers.clear();
        FormServiceAdapter.formQuestions.addAll(formQuestions);
        FormServiceAdapter.formAnswers.addAll(formAnswers);
        notifyDataSetChanged();
    }

    public class ViewHolder extends RecyclerView.ViewHolder {
        TextView txtServiceName;
        NumberPicker numpickerTime;

        public ViewHolder(View v) {
            super(v);
            txtServiceName = (TextView) v.findViewById(R.id.txtServiceName);
            numpickerTime = (NumberPicker) v.findViewById(R.id.numpickerTime);
        }
    }
}

