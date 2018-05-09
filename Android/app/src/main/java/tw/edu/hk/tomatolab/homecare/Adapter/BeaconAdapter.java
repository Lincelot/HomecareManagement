package tw.edu.hk.tomatolab.homecare.Adapter;

import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.estimote.sdk.Beacon;
import com.estimote.sdk.Utils;

import java.util.Collection;
import java.util.List;

import tw.edu.hk.tomatolab.homecare.R;

/**
 * Created by Minsheng on 2015/12/15.
 */
public class BeaconAdapter extends RecyclerView.Adapter<BeaconAdapter.ViewHolder> {

    private static final String TAG = "BeaconAdapter";
    private List<Beacon> beacons;

    public BeaconAdapter(List<Beacon> beacons) {
        this.beacons = beacons;
    }


    public class ViewHolder extends RecyclerView.ViewHolder {
        TextView txtTitle;
        TextView txtRSSI;
        TextView txtmPower;
        TextView txtUUID;

        public ViewHolder(View v) {
            super(v);
            txtTitle = (TextView) v.findViewById(R.id.txtTitle);
            txtRSSI = (TextView) v.findViewById(R.id.txtRSSI);
            txtmPower = (TextView) v.findViewById(R.id.txtmPower);
            txtUUID = (TextView) v.findViewById(R.id.txtUUID);
        }
    }

    @Override
    public BeaconAdapter.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View v = LayoutInflater.from(parent.getContext()).inflate(R.layout.beacon_item, parent, false);
        return new ViewHolder(v);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, int position) {

        Beacon beacon = beacons.get(position);
        holder.txtTitle.setText(String.format(
                "MAC: %s (%.2fm)",
                beacon.getMacAddress().toStandardString(),
                Utils.computeAccuracy(beacon)
        ));
        holder.txtRSSI.setText("RSSI: " + beacon.getRssi());
        holder.txtmPower.setText("MPower: " + beacon.getMeasuredPower());
        holder.txtUUID.setText("UUID: " + beacon.getProximityUUID());

    }

    @Override
    public int getItemCount() {
        return beacons.size();
    }

    public void replaceWith(Collection<Beacon> newBeacons) {
        this.beacons.clear();
        this.beacons.addAll(newBeacons);
        notifyDataSetChanged();
    }
}

