package tw.edu.hk.tomatolab.homecare.Fragment;


import android.app.Fragment;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.estimote.sdk.Beacon;

import java.util.ArrayList;
import java.util.List;

import tw.edu.hk.tomatolab.homecare.Adapter.BeaconAdapter;
import tw.edu.hk.tomatolab.homecare.Beacon.BeaconService;
import tw.edu.hk.tomatolab.homecare.R;


/**
 * A simple {@link Fragment} subclass.
 */
public class BeaconFragment extends Fragment {

    private static final String TAG = "BeaconFragment";
    private BeaconChangeReceiver beaconChangeReceiver;
    private List<Beacon> beacons = new ArrayList<>();
    private BeaconAdapter adapter;
    private View view;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        view = inflater.inflate(R.layout.fragment_beacon, container, false);
        Context context = view.getContext();

        beaconChangeReceiver = new BeaconChangeReceiver();
        IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction(BeaconService.ACTION_BEACON_CHANGE);
        getActivity().registerReceiver(beaconChangeReceiver, intentFilter);

        RecyclerView mList = (RecyclerView) view.findViewById(R.id.beacon_recyclerbiew);
        LinearLayoutManager layoutManager = new LinearLayoutManager(context);
        layoutManager.setOrientation(LinearLayoutManager.VERTICAL);
        mList.setLayoutManager(layoutManager);

        beacons.addAll(BeaconService.getListBeacon());
        adapter = new BeaconAdapter(beacons);
        mList.setAdapter(adapter);

        return view;
    }

    @Override
    public void onDestroy() {
        getActivity().unregisterReceiver(beaconChangeReceiver);
        super.onDestroy();
    }

    private class BeaconChangeReceiver extends BroadcastReceiver {
        @Override
        public void onReceive(Context context, Intent intent) {
            beacons.clear();
            beacons = intent.getParcelableArrayListExtra("beacons");
            adapter.replaceWith(beacons);
        }
    }

}
