using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keep the original panel sizes and animations, anchoring their group to the
// safe right edge instead of the edge of the original 4:3 design canvas.
public sealed class ModernTitleLayout : MonoBehaviour
{
    readonly Dictionary<Transform, Vector3> anchors = new Dictionary<Transform, Vector3>();
    readonly Dictionary<EZTransition, Vector3> exits = new Dictionary<EZTransition, Vector3>();
    Camera uiCamera;
    Rect lastSafe;
    int lastWidth, lastHeight;

    IEnumerator Start()
    {
        // Let child controllers subscribe to game events before hiding title submenus.
        yield return null;
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        foreach (var name in new[] { "PipeGUI_TitleScreenButtons", "PipeGUI_CMGamesButton", "PipeGUI_LeaderboardsButton" })
        {
            var go = GameObject.Find(name);
            if (go != null) anchors.Add(go.transform, go.transform.position);
        }
        var gui = GetComponent<TitleScreenGUI>();
        foreach (var panel in new[] { gui.m_singlePlayerPanel, gui.m_freeplayPanel })
        {
            if (panel == null) continue;
            foreach (var transition in panel.Transitions.list)
                if (transition.name.StartsWith("Dismiss") && transition.animParams.Length > 0)
                    exits.Add(transition, transition.animParams[0].vec);
            if (panel.initialState == UIBistateInteractivePanel.STATE.HIDDEN && !panel.IsTransitioning)
                panel.gameObject.SetActive(false);
        }
        Apply();
    }

    void LateUpdate()
    {
        if (uiCamera != null && (lastSafe != Screen.safeArea || lastWidth != Screen.width || lastHeight != Screen.height)) Apply();
    }

    void Apply()
    {
        lastSafe = Screen.safeArea;
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        float right = uiCamera.ScreenToWorldPoint(new Vector3(lastSafe.xMax, lastSafe.center.y, 10)).x;
        float offset = right - 100f * (4f / 3f);
        foreach (var anchor in anchors) anchor.Key.position = anchor.Value + Vector3.right * offset;
        // Dismiss completely beyond the wider view before deactivating.
        foreach (var exit in exits)
            exit.Key.animParams[0].vec = exit.Value - Vector3.right * (2f * Mathf.Max(0, offset));
    }
}
