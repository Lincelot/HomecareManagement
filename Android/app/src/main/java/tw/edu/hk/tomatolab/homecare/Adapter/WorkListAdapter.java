package tw.edu.hk.tomatolab.homecare.Adapter;

import android.content.Intent;
import android.support.design.widget.Snackbar;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import tw.edu.hk.tomatolab.homecare.Activity.FromRecordActivity;
import tw.edu.hk.tomatolab.homecare.Model.WorkListModel;
import tw.edu.hk.tomatolab.homecare.R;

/**
 * Created by Minsheng on 2015/12/15.
 */
public class WorkListAdapter extends RecyclerView.Adapter<WorkListAdapter.ViewHolder> {

    private static final String TAG = "WorkListAdapter";
    private List<WorkListModel> workLists = new ArrayList<>();

    @Override
    public WorkListAdapter.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View v = LayoutInflater.from(parent.getContext()).inflate(R.layout.form_worklist_item, parent, false);
        return new ViewHolder(v);
    }

    @Override
    public void onBindViewHolder(final ViewHolder holder, final int position) {

        WorkListModel workList = workLists.get(position);
        holder.txtName.setText(workList.getName());
        holder.txtStart.setText(workList.getStart());
        holder.txtEnd.setText(workList.getEnd());

        View v = holder.itemView;
        v.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (workLists.get(position).getStatus() == 0) {
                    Intent intent = new Intent(v.getContext(), FromRecordActivity.class);
                    intent.putExtra("uid", workLists.get(position).getUid());
                    v.getContext().startActivity(intent);
                } else {
                    Snackbar.make(v, R.string.form_record_write, Snackbar.LENGTH_SHORT).show();
                }
            }
        });
    }

    @Override
    public int getItemCount() {
        return workLists.size();
    }

    public void replaceWith(Collection<WorkListModel> workLists) {
        this.workLists.clear();
        this.workLists.addAll(workLists);
        notifyDataSetChanged();
    }

    public class ViewHolder extends RecyclerView.ViewHolder {
        TextView txtName;
        TextView txtStart;
        TextView txtEnd;

        public ViewHolder(View v) {
            super(v);
            txtName = (TextView) v.findViewById(R.id.txtName);
            txtStart = (TextView) v.findViewById(R.id.txtStart);
            txtEnd = (TextView) v.findViewById(R.id.txtEnd);
        }
    }
}

