package tw.edu.hk.tomatolab.homecare.Adapter;

import android.support.v7.widget.RecyclerView;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import tw.edu.hk.tomatolab.homecare.Activity.MainActivity;
import tw.edu.hk.tomatolab.homecare.Model.FormQuestionModel;
import tw.edu.hk.tomatolab.homecare.R;

/**
 * Created by Minsheng on 2015/12/18.
 */
public class FormCaseAdapter extends RecyclerView.Adapter<FormCaseAdapter.ViewHolder> {

    private static final String TAG = "FormServiceAdapter";
    private static List<FormQuestionModel> formQuestions;
    private FormQuestionModel formQuestion;
    private static List<String[]> formAnswers;

    public FormCaseAdapter() {
        formQuestions = new ArrayList<>();
        formAnswers = new ArrayList<>();
    }

    @Override
    public FormCaseAdapter.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View v = LayoutInflater.from(parent.getContext()).inflate(R.layout.form_record_case_item, parent, false);
        return new ViewHolder(v);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, final int position) {
        formQuestion = formQuestions.get(position);
        holder.txtCaseName.setText(formQuestion.getTitle() + "：");
        switch (formQuestion.getTitle()) {
            default: {
                formQuestion = formQuestions.get(position);
                ArrayAdapter<String> adapter = new ArrayAdapter<>(
                        holder.itemView.getContext(),
                        android.R.layout.simple_spinner_dropdown_item,
                        formQuestion.getContent()
                );
                holder.spinner.setAdapter(adapter);
                holder.spinner.setTag(position);

                //region 取得暫存選項並顯示
                for (int i = 0; i < formQuestion.getContentUID().size(); i++) {
                    if (Integer.parseInt(formAnswers.get(position)[0]) == formQuestion.getContentUID().get(i)) {
                        holder.spinner.setSelection(i);
                        break;
                    }
                }
                //endregion

                //region 將使用者選擇的項目暫存至變數
                holder.spinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
                    @Override
                    public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                        int tag = (int) parent.getTag();
                        formQuestion = formQuestions.get(tag);
                        formAnswers.set(tag, new String[]{
                                String.valueOf(formQuestion.getContentUID(position)),
                                MainActivity.getDateNow(0, "yyyy/MM/dd HH:mm")
                        });
                    }

                    @Override
                    public void onNothingSelected(AdapterView<?> parent) {
                        //Nothing
                    }
                });
                //endregion

                break;
            }

            case "備註": {
                holder.editText.setText(formAnswers.get(position)[1]);

                //region 將使用者輸入的內容暫存至變數
                holder.editText.addTextChangedListener(new TextWatcher() {
                    @Override
                    public void beforeTextChanged(CharSequence s, int start, int count, int after) {

                    }

                    @Override
                    public void onTextChanged(CharSequence s, int start, int before, int count) {
                        formQuestion = formQuestions.get(position);
                        formAnswers.set(position, new String[]{
                                String.valueOf(formQuestion.getContentUID(0)),
                                String.valueOf(s).replaceAll("\r\n", " ")
                        });
                    }

                    @Override
                    public void afterTextChanged(Editable s) {

                    }
                });
                //endregion

                holder.spinner.setVisibility(View.GONE);
                holder.editText.setVisibility(View.VISIBLE);
                break;
            }
        }
    }

    @Override
    public int getItemCount() {
        return formQuestions.size();
    }

    public static List<String[]> getAnswer() {
        return formAnswers;
    }

    public static List<FormQuestionModel> getQuestion() {
        return formQuestions;
    }

    public void replaceWith(Collection<FormQuestionModel> formQuestions, Collection<String[]> formAnswer) {
        FormCaseAdapter.formQuestions.clear();
        formAnswers.clear();
        FormCaseAdapter.formQuestions.addAll(formQuestions);
        formAnswers.addAll(formAnswer);
        notifyDataSetChanged();
    }

    public class ViewHolder extends RecyclerView.ViewHolder {
        TextView txtCaseName;
        Spinner spinner;
        EditText editText;

        public ViewHolder(View v) {
            super(v);
            txtCaseName = (TextView) v.findViewById(R.id.txtCaseName);
            spinner = (Spinner) v.findViewById(R.id.spinner);
            editText = (EditText) v.findViewById(R.id.editText);
        }
    }


}
