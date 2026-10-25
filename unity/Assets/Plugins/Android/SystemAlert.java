package com.snowbolt;

import android.app.AlertDialog;
import android.os.Build;
import android.view.View;
import android.view.WindowInsets;
import android.view.WindowInsetsController;
import android.widget.ProgressBar;
import com.unity3d.player.UnityPlayer;

/** Android framework dialogs; no dependency on the discontinued Prime31 plugin. */
public final class SystemAlert {
    private static final SystemAlert INSTANCE = new SystemAlert();
    private static AlertDialog progress;
    public static SystemAlert instance() { return INSTANCE; }
    public static void setImmersive(boolean enabled) {
        if (UnityPlayer.currentActivity == null) return;
        UnityPlayer.currentActivity.runOnUiThread(() -> {
            if (Build.VERSION.SDK_INT >= 30) {
                WindowInsetsController controller = UnityPlayer.currentActivity.getWindow().getInsetsController();
                if (controller == null) return;
                controller.setSystemBarsBehavior(WindowInsetsController.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE);
                if (enabled) controller.hide(WindowInsets.Type.systemBars());
                else controller.show(WindowInsets.Type.systemBars());
            } else {
                int flags = enabled ? View.SYSTEM_UI_FLAG_FULLSCREEN | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                    | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY | View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                    | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION : 0;
                UnityPlayer.currentActivity.getWindow().getDecorView().setSystemUiVisibility(flags);
            }
        });
    }
    public void showDialog(String title, String text, String cancel, String confirm) {
        UnityPlayer.currentActivity.runOnUiThread(() -> {
            AlertDialog.Builder builder = new AlertDialog.Builder(UnityPlayer.currentActivity)
                .setTitle(title).setMessage(text)
                .setOnCancelListener(dialog -> reply(false));
            if (!confirm.isEmpty()) builder.setPositiveButton(confirm, (dialog, button) -> reply(true));
            if (!cancel.isEmpty()) builder.setNegativeButton(cancel, (dialog, button) -> reply(false));
            builder.show();
        });
    }
    private static void reply(boolean confirmed) {
        UnityPlayer.UnitySendMessage("PlatformManager", confirmed ? "OnConfirm" : "OnCancel", "");
    }
    public static void showProgress(String message) {
        UnityPlayer.currentActivity.runOnUiThread(() -> {
            if (progress != null) progress.dismiss();
            progress = new AlertDialog.Builder(UnityPlayer.currentActivity).setMessage(message)
                .setView(new ProgressBar(UnityPlayer.currentActivity)).setCancelable(false).create();
            progress.show();
        });
    }
    public static void hideProgress() {
        UnityPlayer.currentActivity.runOnUiThread(() -> {
            if (progress != null) { progress.dismiss(); progress = null; }
        });
    }
}
