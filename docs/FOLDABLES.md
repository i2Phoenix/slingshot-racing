# Foldable-screen compatibility

Recorded on **October 5, 2026**, for **Slingshot Racing Remastered 1.4.0.0**, Android versionCode 15. This describes the existing fix and its historical validation.

## Cause

On Samsung SM-F976B / Android 17, unfolding changed Unity's reported resolution from 2520×1080 to 2504×2256. Touch events still arrived and menus still worked. However, the resize branch in `TouchInputManager.Update` recreated the players' `TouchRegion` objects. Each `VehicleController.touch` still referenced the old object assigned when its car was created, so vehicle control stopped responding.

The owner confirmed that car control failed while menu buttons continued to work. Local baseline evidence is retained under ignored `work/qa/fold/`: `diagnostic-baseline.log`, `native-baseline.log`, and `baseline-open.png`.

## Changes

- Player setup creates touch regions once. Resizing updates their rectangles in place, preserving controller references, held/released state, and the upgrade button's excluded area.
- The GUI camera fits the complete original 4:3 area on near-square screens and retains height-based scaling on wide screens, using the safe area.
- `EZScreenPlacement` also tracks changes in the camera's orthographic scale.
- The light background plane covers both axes while respecting its original rotation; tiling preserves texture density. The credits background also covers the changed height.
- The build explicitly enables `PlayerSettings.Android.resizeableActivity` and `PlayerSettings.resetResolutionOnWindowResize`. The former disabled reset setting was not the observed input failure's cause: resolution already changed on the affected Samsung.

Related platform guidance: [Android's Unity large-screen recommendations](https://developer.android.com/games/engines/unity/unity-large-screen).

## Recorded checks

| Check | Result | Local evidence under `work/qa/fold/` |
|---|---|---|
| `FoldCompatibilityChecks` | 128 assertions; all 15 nonempty player masks at 2520×1080, 2504×2256, and 2256×2504; reference retention, new presses, releases during resize, and upgrade exclusion | `input-regression.json` |
| Live emulator race | Six hooks and six releases while resizing 1200×1080 → 2400×1080 → 1200×1080 | `emulator-input-cycle.json`, `emulator-input-cycle.log` |
| Final release UI | Title, career, and tour selection at 1200×1080; usable buttons, complete panels, and full background coverage | `release-inner-title.png`, `release-inner-career.png`, `release-inner-tours.png` |
| Release structure | Build succeeded with zero errors; signing, ARM64, SDK 26/36, no Development/diagnostic components, and 16 KB ELF/ZIP alignment checked | `scripts/verify_apk.py` output recorded locally |
| Physical installation | Installed `base.apk` matched the final release SHA-256 | `phone-release-install.json` |

The final release hash is `84cee58f0dd9bc770260427fdbc1030e55fff273f052c284b4fe2598efb15850`. Emulator window resizing exercises the resize path; it does not reproduce every physical hinge/device lifecycle condition.

## Physical-device confirmation

After the final release was installed, the owner repeated the check, including vehicle input across external/internal screen transitions, and confirmed that it worked. This is owner confirmation; no automated post-fix transition log was recorded.

The phone was locked and dozing during installation, so `Status: ok` from `am start` alone was not counted as visible-launch proof. The subsequent confirmation supports the physical fold/unfold fix, but does not imply a complete campaign retest. See [QA](QA.md) for the broader limits.
