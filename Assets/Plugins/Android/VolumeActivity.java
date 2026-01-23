package com.onionblossom.volumelistener;

import android.os.Bundle;
import android.view.KeyEvent;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class VolumeActivity extends UnityPlayerActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {

        if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
            UnityPlayer.UnitySendMessage("InputManager", "OnVolumeUp", "");
            return true; // block system volume change
        }

        if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
            UnityPlayer.UnitySendMessage("InputManager", "OnVolumeDown", "");
            return true;
        }

        return super.onKeyDown(keyCode, event);
    }
}
