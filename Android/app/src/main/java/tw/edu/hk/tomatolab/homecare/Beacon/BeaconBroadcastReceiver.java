package tw.edu.hk.tomatolab.homecare.Beacon;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

public class BeaconBroadcastReceiver extends BroadcastReceiver {

    private static final String TAG = "BeaconBroadcastReceiver";
    private static final String ServiceName = "tw.edu.hk.tomatolab.homecare.Beacon.BeaconService";

    @Override
    public void onReceive(Context context, Intent intent) {
        context.startService(new Intent(context, BeaconService.class));
        Log.d(TAG, "onReceive()");
    }

}
