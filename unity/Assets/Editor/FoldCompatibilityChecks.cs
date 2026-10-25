using System;
using System.IO;
using System.Reflection;
using UnityEngine;

// Regression: a resize used to replace the TouchRegion held by each car.
public static class FoldCompatibilityChecks
{
    public static void Run()
    {
        var refresh = typeof(TouchInputManager).GetMethod("RefreshTouchLayout", BindingFlags.Instance | BindingFlags.NonPublic);
        var bounds = typeof(TouchRegion).GetField("m_hitRect", BindingFlags.Instance | BindingFlags.NonPublic);
        int checks = 0;
        for (int mask = 1; mask < 16; mask++)
        {
            var go = new GameObject("Fold input regression");
            try
            {
                var input = go.AddComponent<TouchInputManager>();
                input.Event_ConfigureForHumanPlayers(mask);
                var heldByControllers = new TouchRegion[input.NumHumans];
                for (int i = 0; i < heldByControllers.Length; i++) heldByControllers[i] = input.GetTouchRegion(i);
                foreach (var size in new[] { new Vector2Int(2520, 1080), new Vector2Int(2504, 2256), new Vector2Int(2520, 1080), new Vector2Int(2256, 2504) })
                {
                    refresh.Invoke(input, new object[] { size.x, size.y });
                    for (int i = 0; i < heldByControllers.Length; i++)
                    {
                        refresh.Invoke(input, new object[] { size.x, size.y });
                        var region = heldByControllers[i];
                        Require(ReferenceEquals(region, input.GetTouchRegion(i)), "Controller lost its region on resize");
                        var rect = (Rect)bounds.GetValue(region);
                        var point = new Vector3(rect.center.x * Screen.width, rect.center.y * Screen.height);
                        region.PreUpdate();
                        region.Touch(point);
                        Require(region.GetTouchDown(), "Press was lost after resize");
                        // Reflow while a finger is down must still deliver its eventual release.
                        refresh.Invoke(input, new object[] { size.y, size.x });
                        Require(ReferenceEquals(region, input.GetTouchRegion(i)), "Controller lost a held touch during resize");
                        region.PreUpdate();
                        Require(region.GetTouchUp(), "Release was lost during resize");
                        region.PreUpdate();
                        Require(!region.GetTouchUp(), "Release repeated");
                        checks++;
                    }
                    if (input.NumHumans == 1)
                    {
                        input.Event_ConfigureCutout(TouchInputManager.Cutout.BottomLeft);
                        var region = heldByControllers[0];
                        region.Touch(Vector3.zero);
                        Require(!region.GetTouchDown(), "Upgrade button leaked into steering");
                        input.Event_ConfigureCutout(TouchInputManager.Cutout.Disabled);
                    }
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }
        string root = Directory.GetParent(Application.dataPath).Parent.FullName;
        Directory.CreateDirectory(Path.Combine(root, "work/qa/fold"));
        File.WriteAllText(Path.Combine(root, "work/qa/fold/input-regression.json"),
            "{\"passed\":" + checks + ",\"failed\":0,\"playerMasks\":15,\"sizes\":[[2520,1080],[2504,2256],[2520,1080],[2256,2504]],\"checks\":[\"controller reference survives resize\",\"press after resize\",\"release during resize\",\"upgrade cutout\"]}\n");
        Debug.Log("FOLD_INPUT_CHECKS passed=" + checks + " failed=0");
    }

    static void Require(bool condition, string reason)
    {
        if (!condition) throw new InvalidOperationException(reason);
    }
}
