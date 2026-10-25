#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Development-only content coverage. Uses the original race loader and AI;
// shortened cloned events do not change the shipped campaign or its assets.
public sealed class ModernCatalogAudit : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        var go = new GameObject("Catalog audit");
        DontDestroyOnLoad(go);
        go.AddComponent<ModernCatalogAudit>();
    }

    IEnumerator Start()
    {
        string command = Path.Combine(Application.persistentDataPath, "qa-command.txt");
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            if (!File.Exists(command)) continue;
            string action = File.ReadAllText(command).Trim();
            File.Delete(command);
            if (action != "catalog" && action != "flee" && !action.StartsWith("events:")) continue;
            var title = FindFirstObjectByType<TitleScreenGUI>();
            var manager = FindFirstObjectByType<GameManager>();
            if (title == null || manager == null) { Debug.LogError("QA_AUDIT_REQUIRES_TITLE"); continue; }
            var all = manager.SaveManager.m_campaigns.Concat(new[] { title.m_freePlayCampaign })
                .Where(c => c != null).SelectMany(c => c.m_tours).SelectMany(t => t.m_races).ToArray();
            var events = all.Where(r => r.m_objective != RaceEvent.ObjectiveType.Tutorial)
                .GroupBy(r => r.m_layout.GetScene(0, r.m_variant) + "/" + (r.m_forceReverseTrack || r.m_layout.m_reverseTrack))
                .Select(g => g.First()).Concat(all.GroupBy(r => r.m_objective).Select(g => g.First()))
                .Distinct().ToArray();
            if (action == "flee") events = all.Where(r => r.m_objective == RaceEvent.ObjectiveType.LastStanding || r.m_objective == RaceEvent.ObjectiveType.Survival)
                .GroupBy(r => r.m_objective).Select(g => g.First()).ToArray();
            if (action.StartsWith("events:"))
            {
                var names = new HashSet<string>(action.Substring(7).Split(','));
                events = all.Where(r => names.Contains(r.name)).Distinct().ToArray();
            }
            yield return Run(manager, events);
        }
    }

    IEnumerator Run(GameManager manager, RaceEvent[] events)
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var run = typeof(GameManager).GetMethod("RunRaceEvent2", flags);
        var raceManager = manager.GetComponent<RaceManager>();
        // This is a disposable emulator audit: no campaign save is attached.
        manager.StopAllCoroutines();
        foreach (string field in new[] { "m_campaign", "m_tour", "m_saveData" })
            typeof(GameManager).GetField(field, flags).SetValue(manager, null);
        var state = PersistentState.Get();
        state.m_aiControlHumans = true;
        state.aiDifficulty = AIDifficulty.Hard;
        raceManager.AutoFinish = true;
        int failures = 0;
        Debug.Log($"QA_AUDIT_BEGIN count={events.Length}");
        for (int index = 0; index < events.Length; index++)
        {
            var race = Instantiate(events[index]);
            race.name = events[index].name;
            bool survival = race.m_objective == RaceEvent.ObjectiveType.LastStanding || race.m_objective == RaceEvent.ObjectiveType.Survival;
            if (!survival) { race.m_numLaps = 1; race.m_maxDuration = 18; }
            state.m_aiControlHumans = !survival;
            race.m_runFlags = RaceEvent.RunFlags.NoHelp | RaceEvent.RunFlags.NoWatchReplay | RaceEvent.RunFlags.NoRaceReplay |
                RaceEvent.RunFlags.DontAddRacePoints | RaceEvent.RunFlags.DontTrackTournamentPoints | RaceEvent.RunFlags.NoUpgrades;
            race.m_humanMask = 1 << Settings.PlayerColour;
            race.m_aiMask = 15 & ~race.m_humanMask;
            if (race.m_numRacers <= 1) race.m_aiMask = 0;
            bool done = false, finished = false;
            int errors = 0;
            EventHandler completed = (sender, args) => finished = true;
            Application.LogCallback logging = (message, stack, type) => {
                if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert) errors++;
            };
            raceManager.OnRaceCompleteEvent += completed;
            Application.logMessageReceived += logging;
            Debug.Log($"QA_AUDIT_CASE index={index} name={race.name} scene={race.m_layout.GetScene(0, race.m_variant)} reverse={race.m_forceReverseTrack || race.m_layout.m_reverseTrack} mode={race.m_objective}");
            StartCoroutine(WaitForRace(manager, (IEnumerator)run.Invoke(manager, new object[] { race, false }), () => done = true));
            float start = Time.realtimeSinceStartup;
            bool captured = false, timedOut = false;
            while (!done)
            {
                yield return new WaitForSecondsRealtime(0.25f);
                float elapsed = Time.realtimeSinceStartup - start;
                if (!captured && elapsed > 9f)
                {
                    captured = true;
                    Debug.Log($"QA_AUDIT_CAPTURE index={index} lightmaps={LightmapSettings.lightmaps.Length}");
                }
                float deadline = survival ? 210f : 55f;
                if (elapsed > deadline)
                {
                    timedOut = true;
                    raceManager.SendMessage("UIEvent_LeaveRace");
                }
                if (elapsed > deadline + 10f) { Debug.LogError("QA_AUDIT_ABORT loader did not return"); yield break; }
            }
            raceManager.OnRaceCompleteEvent -= completed;
            Application.logMessageReceived -= logging;
            bool passed = finished && !timedOut && errors == 0;
            if (!passed) failures++;
            Debug.Log($"QA_AUDIT_RESULT index={index} passed={passed} finished={finished} errors={errors} timeout={timedOut} seconds={Time.realtimeSinceStartup-start:F2}");
            Destroy(race);
        }
        state.m_aiControlHumans = false;
        raceManager.AutoFinish = false;
        Debug.Log($"QA_AUDIT_END count={events.Length} failures={failures}");
        manager.BootToTitleScreen();
    }

    static IEnumerator WaitForRace(GameManager manager, IEnumerator race, Action done)
    {
        yield return manager.StartCoroutine(race);
        done();
    }
}
#endif
