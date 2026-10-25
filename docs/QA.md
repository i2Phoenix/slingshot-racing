# Validation record

This document translates and consolidates the port's **October 5, 2026** validation record. It distinguishes individual builds and the limits of each check. These historical results are not a claim that the entire matrix was rerun when this repository was created.

Runtime checks used real Android execution through ADB, diagnostic builds, and ordinary release builds. Compilation and reference audits do not replace runtime testing. Raw logs, device captures, and reports named below remain local under ignored `work/`; selected screenshots are included in [images](images/README.md).

## Current recorded release: Remastered 1.4.0.0

The visible name is **Slingshot Racing Remastered**, version **1.4.0.0**, Android versionCode **15**. The foldable-screen fix preserves the connection between a vehicle controller and its touch region during resolution changes. Menus fit the original 4:3 panels on near-square displays, screen anchors update, and the light background covers both axes.

Recorded artifact: `builds/SlingshotRacing-Remastered-1.4.0.0-arm64.apk`, **44,554,646 bytes**.

```text
SHA-256: 84cee58f0dd9bc770260427fdbc1030e55fff273f052c284b4fe2598efb15850
```

The build completed with zero errors. Verification covered signing, ARM64, SDK 26/36, absence of Development components, 16 KB ELF/ZIP alignment, and application name/version metadata. Its hash matched the installed `base.apk` on Samsung **SM-F976B**.

The input regression passed **128 assertions** across all 15 nonempty player masks, wide/inner/portrait dimensions, retained references, and held-touch release. During a live emulator race, resizing **1200×1080 → 2400×1080 → 1200×1080** produced **six hooks and six releases** for the local player. The final release's title, career, and tour-selection screens were checked at 1200×1080.

After installation, the owner repeated the physical fold/unfold check and confirmed that it worked. No automated post-fix physical-transition log was captured. The phone was locked during installation, so the successful `am start` result alone was not treated as visible-launch evidence. See [Foldable screens](FOLDABLES.md).

## Recorded test environments

| Environment | Android | Page size | Graphics | Main use |
|---|---|---|---|---|
| `Slingshot_QA_Android16` / `emulator-5556` | Android 16 / API 36 | 4,096 bytes | Host GPU | Track and mode catalogue |
| `Slingshot_QA_16KB` / `emulator-5554` | Android 17 / API 37 | 16,384 bytes | SwiftShader | Alignment, UI, input, lifecycle |
| Samsung SM-F976B | Android 17 | Not independently recorded here | Physical device | Installation and owner-confirmed fold/unfold fix |

Both emulators were ARM64; the recorded images reported `codename=REL` and `preview_sdk=0`. Screen sizes included 1440×1080 (4:3), 1920×1080 (16:9), 2400×1080 (20:9), and 2640×1080 (22:9). Pixel 7 cutout safe areas were exercised on both sides by rotating 180 degrees.

## Coverage across the porting builds

Paths in the last column are relative to local `work/qa/` unless stated otherwise.

| Area | Recorded result | Local evidence |
|---|---|---|
| Asset integrity | 101 scenes and 101 prefabs; no missing GUIDs, duplicate GUIDs, or unresolved local file IDs | `reference-audit.json`, `source-differences.json` |
| Native arithmetic | 3,400 comparisons against original ARMv7 execution; maximum difference about 1.91e-6 | `work/native-parity.json` |
| Suspended vehicle carousel | All nine models scrolled; rope bones follow attachments, position error at most 2e-6 | `carousel23-summary.json`, `carousel23-all.log` |
| Menus and wide screens | Title, settings, and credits at 4:3, 16:9, 20:9, and 22:9; full-width overlays and proportional buttons | `title23-default.png`, `options-4x3.png`, `options22x9.png`, `credits24.png` |
| Career and tutorial | Tour/race selection, locks, scrolling, information cards, and records; Android DOWN/UP tutorial inputs completed eight laps in 43.523 seconds with two awards | `drive-25b.log`, `results-25b.png`, `race-info25-22x9.png` |
| Saves | Record 44.085 / best lap 5.222, awards, and unlocks survived updates and cold starts | `progress25-cold.log`, `race-info25-22x9.png` |
| Settings | Toggles, volume, and five original languages persisted through replacement installation and cold start | `settings-slider24.png`, `options-language24-*.png`, `playerprefs25.xml` |
| Local multiplayer | Two/four simultaneous Android contacts generated two/four hooks in the same frame | `multiplayer23.log`, `four-player23.log` |
| Pause and lifecycle | Pause at 4:3 and 22:9, opposite-side cutout, resume; returning from Home leaves an active race paused | `rotate22x9-23.png`, `lifecycle25.log` |
| Tournament | Five laps, participants/AI, time gaps, best laps, and points 3/2/1/0; correctly colored rows fit 22:9 | `tournament26.log`, `tournament26-points22x9.png` |
| Track selection | ALL, SHUFFLE, CLEAR, and a single selected track behaved correctly | `tracks26-all.png`, `tracks26-shuffle.png`, `tracks26-single.png` |
| Replay and credits | Recorded racer movements replayed; credits displayed without the former `_MainTex` error | `replay25.log`, `credits24.png`, `settings24.log` |
| Audio | Active AudioFlinger track without underruns; 44.1 kHz stereo PCM was silent at zero volume and nonzero without clipping after unmuting | `stable-audioflinger21.txt`, `muted-audio25.json`, `restored-audio25.json` |
| Fullscreen | Cold launch and menu/race/results transitions were not covered by system bars | `cold26.png`, `tournament26-times.png` |
| Chomper shadow | Shader semantic mismatch fixed; pink squares absent on two targeted tracks | `shader-interface-audit.json`, `target26/track-00.png`, `target26/track-01.png` |
| Finish placement | Reactivating intermediate UI parents replaced a stale “1st” with the already correctly calculated 4th place | `grade-diag.log`, `grade-fixed.log`, `grade-fixed-result.png` |
| Result-screen particles | Last expired particle removal and GUI particle camera fixed whitening; stable for 60 seconds after a loss and 30 seconds after a tutorial win | `particle-compatibility.json`, `particle-runtime.json`, `background-fixed-hold60.png`, `particles-win-hold30.png` |

## Track catalogue: combined coverage, not one final 39/39 run

`ModernCatalogAudit` is restricted to Development/Editor. It reads `persistentDataPath/qa-command.txt` and exercises 39 selected scene/direction/objective combinations with the original loader, tracks, AI, and rules. Ordinary diagnostic races use a cloned event shortened to one lap/18 seconds; survival events retain their duration. The audit does not write campaign progress.

The initial `catalog23` run completed 31/39 scenarios. Several chases exceeded an overly short 55-second limit; two reverse routes exposed a real TrackHub direction error. The limit was raised to 210 seconds, ordinary local controls were retained for chases, and MAH5/DJS3 connections were recalculated after reversal. Both targeted reruns completed successfully.

The subsequent full `catalog25` run completed **38/39** without application errors. In the remaining `SP_MAH5_RevDef_Flee_Medium` case, the local racer had already finished and results were shown, but the harness waited for optional AI replay recording until its 210-second timeout. `NoRaceReplay` was added only to the audit; normal gameplay replay remains available. Build 26's targeted MAH5/DJS3 rerun completed **2/2** in 23.98/24.25 seconds without errors or timeouts.

Together these runs cover all 39 selected cases. They are **not one uninterrupted successful 39/39 run on the final APK**, nor all possible campaign, upgrade, or difficulty combinations. Successful cases require race completion, loader return, and no Error/Exception/Assert or timeout. All 39 starting frames were inspected; the shadow fix was subsequently checked in two targeted reruns. Evidence: `catalog25/summary.json`, `catalog25/runtime.log`, and `target26/runtime.log`.

## Earlier release checkpoints

These artifacts are historical. Reusing versionCode 15 does not make their hashes or test coverage interchangeable.

| Checkpoint | Bytes | SHA-256 |
|---|---:|---|
| Initial “Remaster” 1.4.0.0 name, before final fold fix | 44,553,602 | `fdd2cff9c35fd81745c93de222da61296c20a222de20a5569b8e9480df98d096` |
| Russian localization before 1.4.0.0 | 44,552,186 | `177aea744d256fa78e8c71e8516ec58f7a6c4a7cb54695ae1a15e4ccda130349` |
| Centered countdown | 44,503,474 | `c62c272e4c7cf3dccddb3738b90bb5755e338874bfb3a832f4f9985b31752ccf` |
| Particle/result stability before countdown relocation | 44,503,666 | `4ed08f639851652e241fbdb52fc9c28548dd0f08a9d04b3a80c6944e1f424cb8` |

The initial Remaster build installed and cold-launched on SM-F976B; that was not a full physical gameplay test. Its name/version metadata, signing, ARM64, SDK levels, and alignment were checked. Local report: `remaster-1.4.0.0/phone-install.json`.

The Russian build passed the 268-key/placeholder/glyph audit and preserved 133 original glyphs. Android 16 runtime checks covered title, career/tour/race selection, race help, loading text, and a one-line pause label at all four aspect ratios. The saved language survived replacement installation. Fresh-install Russian selection was established from the existing language-selection code and RU resources, not a separate reset-and-system-locale experiment. Android 17 and a physical phone were not retested at that intermediate checkpoint. See [Localization](LOCALIZATION.md).

The countdown's parent moved from `(0, 6.5, 15)` to `(0, 0, 15)` without changing animations or start events. Ordinary starts and quick restarts were checked at 16:9, 20:9, and 22:9; the countdown was centered and did not overlap pause/restart controls. Installed hashes matched, with no Unity/AndroidRuntime errors or diagnostic `QA_` messages. The catalogue and 16 KB runtime matrix were not repeated for this positional change. Local captures: `countdown-20x9/`, `countdown-22x9/`, and `countdown-16x9/`.

The earlier particle release passed managed/native compatibility checks and was installed with matching hashes on both emulators. Android 17 / 16 KB replayed Slingshot 101 / Race 2: 4th place, 49.074 total, 7.912 best lap, then 60 seconds on the results screen without background corruption or runtime errors. Broad catalogue/UI/input checks used diagnostic builds; tournament and Home/resume were also checked in intermediate release builds. Evidence: `release-runtime-pre-countdown.json`, `release-final-long.log`.

## Repeating checks

Follow [Building and verification](BUILDING.md) for source, localization, native, and APK checks. Runtime diagnostics require a Development build and a dedicated test installation. The recorded local `work/qa/run_catalog.py` runner is not distributed in this repository; its historical commands are not a fresh-clone dependency. Inspect `ModernCatalogAudit` before driving its command-file interface, as it temporarily controls the active game state.

## Remaining limits

- A full physical-device campaign, audio-listening assessment, and thermal/performance run have not been completed.
- The original APK was not launched on an old Android device. Comparison used recovered code/assets, ARMv7 numerical execution, and an owner-supplied menu reference. Old/new PhysX equivalence is not established.
- Captured PCM proves audio output, not speaker quality on physical hardware.
- Publisher services, purchases, cloud achievements/leaderboards, and Kamcord are not connected or validated as live services.
- Gamepads, every upgrade, and every race combination were not independently checked. Touch input is the primary validated control method.
