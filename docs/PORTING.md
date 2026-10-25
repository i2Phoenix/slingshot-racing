# Porting Slingshot Racing

Porting record: October 5, 2026. Documentation prepared in English on October 25, 2026. The pinned editor is **Unity 6.3 LTS 6000.3.25f1**. Later physical-device and foldable-screen validation is recorded in [QA](QA.md).

## Baseline and project structure

The source is Slingshot Racing **1.3.3.3**, versionCode **13**, built with Unity **3.5.7f6 / Mono** for ARMv7, min API 8 / target API 19. The original package is `com.crescentmoongames.slingshotracing`. The separately supplied APK's identity and SHA-256 are in [CREDITS.md](../CREDITS.md).

`recovered/ExportedProject` is the comparison baseline; changes live in `unity`. The export contains 101 scenes, 101 prefabs, 644 C# files, 59 shaders, 113 materials, and 35 animation clips. Of those 644 C# files, 642 are retained; only two exported system-assembly `AssemblyInfo` files are excluded. Twelve Prime31 files were moved into the first-pass plugin hierarchy to retain assembly dependencies.

`scripts/audit_resources.py` checks source coverage, GUIDs, and local file IDs. It writes `work/qa/reference-audit.json`, `source-differences.json`, and `shader-interface-audit.json`. These are local outputs, not versioned evidence.

## Engine and platform changes

- IL2CPP / ARM64, min API 26 / target API 36, OpenGL ES 3, Gamma. The modern package is `com.crescentmoongames.slingshotracing.modern`; the current app version is **1.4.0.0 / versionCode 15**. It installs alongside the original and does not use the publisher's signing key.
- Removed Unity APIs were replaced with current equivalents. Thirty-seven decompiled explicit interface implementations were repaired. Script GUIDs and scene connections were preserved.
- Thirty-four `GrappleRacerNative` computational exports were reconstructed in C++. Original ARMv7 instructions were run in Unicorn for 3,400 comparisons, with a maximum absolute error of `1.91e-6` within the test tolerances. This verifies numerical routines, not whole-game PhysX equivalence.
- The plugin builds for Android ARM64 and the macOS ARM64 editor. Android ELF output uses 16 KB alignment. Direct GLES drawing was replaced with Unity Mesh rendering, preserving vertex order and triangle-strip winding.
- Ninety legacy ParticleEmitter/ParticleAnimator/ParticleRenderer components were replaced with compatibility components using the game's particle system and preserved parameters.
- The particle adapter removes the final expired particle after the native call: the original scalar function leaves it in the buffer, while the old Unity emitter ended the effect. Without this correction, finish effects never completed, color extrapolated beyond lifetime, and the background turned white. GUI particles choose their camera from the original layer. Unity 3 enum values were verified: Stretch=3, HorizontalBillboard=4, VerticalBillboard=5. The numerical C++ implementation was not changed for this compatibility fix.
- Android dialogs use `com.snowbolt.SystemAlert`. Saves/settings use application storage. Both landscape orientations are allowed; portrait play is disabled.

## Graphics, menus, and suspended vehicles

Fifty-two GLES shaders were translated into HLSL from original APK programs; seven retain their fixed-function ShaderLab expressions. Matrix conventions, vertex semantics, UVs, half-pixel offsets, and material variants were restored. `LegacyLighting` compensates for the automatic intensity doubling applied when old lights are imported; spherical-harmonic lighting is matched to Gamma rendering.

The Chomper's pink shadow was traced to `BakedBlobShadow2`: the vertex output used `TEXCOORD1`, but the fragment input expected `TEXCOORD0`. Both the shader and its generator were corrected to align semantics by variable name. All 59 shader interfaces passed audit, followed by two live chase reruns.

Ten original RGBA4 textures were recovered from APK bytes because the old format identifier was being decoded incorrectly by modern tooling. Color channels and transparency were repaired for snow, trails, paint, arrows, lighting effects, and related textures. Tool: `scripts/restore_texture_pixels.py`; local report: `work/qa/restored-texture-pixels.json`.

Original lightmap textures and 170 renderer/UV bindings were restored for 20 scenes. `LegacyLightmaps` applies them on scene load; `scripts/restore_lightmaps.py` reconstructs the data.

Suspended vehicles and platforms are enabled with the restored prefabs. For constrained dynamic rigidbodies, `MovePosition` caused disagreement between physical and rendered position in modern Unity. `FrontendBrace` now moves the Transform and synchronizes physics, retaining the original CharacterJoint setup and swinging motion. All nine carousel models and their rope-bone attachments were checked.

Global `Shader.WarmupAllShaders` was removed from loading after a Unity 6 SIGSEGV while warming unsupported internal stereo variants on an emulator. Used variants compile when drawn; materials and included shaders remain. The original crash trace is local at `work/qa/target24/crash-warmup.log`.

## Physics and reverse tracks

The old WheelCollider values `extremumValue=1000` / `asymptoteValue=500` caused extreme acceleration and vehicles leaving the track under newer PhysX. They were normalized to `1 / 0.5`; wheel mass was made positive, while original spring, radius, slip, and gameplay-control parameters were preserved. An isolated Unity probe and live races separated this issue from track geometry.

Wrong-way recovery assigns Rigidbody position and rotation directly. The original condition—more than 2.5 seconds against track direction—remains. No general teleport-on-fall behavior was added.

Reverse races on MAH5 and DJS3 lost direction near the final connection because TrackHub sectors still described the forward route. After `ReverseTrack`, all node sectors and spline parameterization are rebuilt after node reversal. Both affected races finished in a targeted rerun (`work/qa/target25/runtime.log`).

`DYNAMIC_TRACK_SEGMENTS` was already present in the original file. The hypothesis that it was lost was rejected; no additional build-symbol change was made.

## Wide screens and UI

- The UI camera preserves vertical scale on wide displays and fits the full original 4:3 panel width on narrower displays. It uses the `Screen.safeArea` center. EZScreenPlacement recalculates anchors when screen size, cutout, or camera scale changes.
- Resize/fold transitions update touch regions in place. `VehicleController` keeps its `TouchRegion` reference and pressed/released state. Android `resizeableActivity` and `resetResolutionOnWindowResize` are enabled. See [Foldable screens](FOLDABLES.md).
- Main menu buttons anchor to the safe right edge. Closed panels remain hidden after initialization, including at 22:9.
- Steam, credits backgrounds, the loading overlay, and input blockers fill the camera width. Steam UV scaling preserves texture density.
- Race buttons anchor to safe corners. TouchRegion stores normalized bounds and recalculates them; use of `xMax/yMax` in place of width/height was corrected.
- The shared `DynamicCamera.prefab` countdown board moved from Y=6.5 to Y=0, centering it on the gameplay camera. Original animations and race-start timing remain. Normal starts and quick restarts were checked at 16:9, 20:9, and 22:9.
- Track-card and TransitionStore initialization order was restored; recursive panel deactivation was removed.
- Rewards and result rows activate their complete parent chain. In modern Unity, an active child under an inactive parent is invisible. Player colors, finish positions, and time icons are applied after the panel is shown so legacy EZ GUI does not skip `PlayAnim` while parents are disabled.
- SpriteMesh/SpriteRoot no longer request textures from materials without `_MainTex`, fixing the credits-screen error.
- Android 11+ uses `WindowInsetsController` to hide system bars; Android 8–10 retain the compatible path. Immersive mode is restored after the intro.

## Offline scope

Flurry, platform achievements/leaderboards, old social SDKs, cloud services, Kamcord, and in-app purchases are not connected. Unavailable login/restore-purchase controls are hidden. Some original APK functions were already stubs; [Online services](ONLINE_SERVICES.md) distinguishes them from active original integrations. Race replay remains independent of Kamcord. The FAQ remains an original external link.

Five unused legacy network-test classes are guarded by `SLINGSHOT_LEGACY_NETWORK_TEST`; assets do not reference them. Same-device multiplayer tournaments remain supported.

## Building and evidence

See [Building](BUILDING.md). The first enabled scene is `_Boot`; scenes are listed in `unity/ProjectSettings/EditorBuildSettings.asset`. Development builds contain observations and automated-race diagnostics. Release builds use `BuildOptions.None` and exclude those components at compile time.

[QA](QA.md) records the actual runtime coverage and its limits. The original APK was not run on an old Android device for a side-by-side comparison; code recovery and numerical parity do not establish complete physical equivalence.

## Technical references

- [Unity 3.5 to 4.0 object activation changes](https://docs.unity3d.com/2020.1/Documentation/Manual/UpgradeGuide3540.html).
- [Unity 5 shader/lighting migration](https://docs.unity3d.com/2020.2/Documentation/Manual/UpgradeGuide5-Shaders.html) and [Unity 5.3 SH/Gamma changes](https://docs.unity3d.com/2018.3/Documentation/Manual/UpgradeGuide53.html).
- [Unity PhysX 3 migration](https://docs.unity3d.com/2019.4/Documentation/Manual/UpgradeGuide5-Physics.html).
- [Android 16 KB page sizes](https://developer.android.com/guide/practices/page-sizes).
- [Android immersive mode](https://developer.android.com/develop/ui/views/layout/immersive).
