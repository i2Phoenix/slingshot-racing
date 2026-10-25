# Building and verification

## Supported development environment

The existing scripts target macOS on Apple Silicon. Use Unity **6000.3.25f1**, with Android Build Support, Android SDK & NDK Tools, and OpenJDK installed through Unity Hub. Activate Unity and install Xcode Command Line Tools. The Python tooling was used with Python 3.12; native compilation itself uses only Python's standard library.

The editor and NDK defaults point to `/Applications/Unity/Hub/Editor/6000.3.25f1/`. Override `SLINGSHOT_UNITY` with the editor executable and `SLINGSHOT_NDK` with the NDK directory when needed. The Android compiler path in `build_native.py` uses the NDK's macOS host toolchain; Windows/Linux build hosts are not implemented by that script.

For a separately installed Android SDK, select its NDK explicitly. For example, the clean-source native check used NDK 27.2.12479018:

```sh
export SLINGSHOT_NDK="$HOME/Library/Android/sdk/ndk/27.2.12479018"
```

## Normal build

```sh
python3 scripts/build_native.py
scripts/build_android.sh
```

The first command compiles `native/GrappleRacerNative.cpp` into:

- `unity/Assets/Plugins/macOS/libGrappleRacerNative.dylib` for the editor.
- `unity/Assets/Plugins/Android/arm64-v8a/libGrappleRacerNative.so` for Android.

These generated libraries are ignored; their stable importer `.meta` files are versioned. The Android build runs `ModernBuild.Android`, including the fold-input regression, and writes `builds/SlingshotRacing-modern-arm64.apk`. Its log is `work/logs/android-release.log`.

```sh
scripts/build_android.sh development
```

This invokes `ModernBuild.AndroidDevelopment` and writes `builds/SlingshotRacing-modern-arm64-development.apk`. Diagnostic components are compiled only for Development/Editor.

The recorded `SlingshotRacing-Remastered-1.4.0.0-arm64.apk` is a separately named local release artifact. The build script continues to use the `modern-arm64.apk` output filename even though the app's visible name is **Slingshot Racing Remastered**.

The normal build uses the included editable project. It does not need the original APK, the original DLLs, or a fresh recovery export. Git LFS is not required for this initial source tree.

## Python checks

```sh
python3.12 -m venv .venv
.venv/bin/python -m pip install -r requirements-dev.txt
.venv/bin/python scripts/audit_resources.py
.venv/bin/python scripts/verify_localization.py
```

The resource audit checks scene/prefab references, GUID collisions, local file IDs, recovered source coverage, and shader interfaces. The localization check compares keys, placeholders, all three font descriptions, glyph coverage, and unchanged original font pixels/metrics. Both write reports under ignored `work/qa/`.

To verify a release APK:

```sh
.venv/bin/python scripts/verify_apk.py builds/SlingshotRacing-modern-arm64.apk
```

The verifier defaults to Android Build Tools `36.0.0` in `~/Library/Android/sdk` and Unity's bundled OpenJDK. Use `--build-tools`, `--java-home`, and `--output` to select other paths. It checks ARM64, min/target SDK 26/36, signing, ELF and ZIP alignment for 16 KB pages, and absence of development diagnostics.

## Optional original-code parity test

This test needs an independently supplied copy of the original APK identified in [CREDITS.md](../CREDITS.md); it is not downloaded or stored by this repository. The filename used in the existing workspace is an APK-compatible ZIP, not a ZIP containing another APK.

```sh
mkdir -p work/apk
unzip -q SlingshotRacing_android_1416506021.apk.zip -d work/apk
python3 scripts/build_native.py
.venv/bin/python tests/native_parity.py
```

The test reads the original ARMv7 `work/apk/lib/armeabi-v7a/libGrappleRacerNative.so` and the tracked ABI descriptions in `tests/fixtures/`. It compares original ARM instructions executed in Unicorn with the reconstructed macOS library. Its output is `work/native-parity.json`. It does not launch Android or validate whole-game physics.

## Optional asset reconstruction

Restoration scripts can rewrite working assets; they are not prerequisites for a normal build.

| Tool | Inputs / additional requirements |
|---|---|
| `extend_cyrillic_font.py` | Included recovery fonts and Roboto Slab; Pillow |
| `restore_lightmaps.py` | Included recovery scenes |
| `convert_legacy_particles.py` | Recovery/working project data; inspect the script before rerunning |
| `restore_texture_pixels.py` | Original extracted `work/apk/assets/bin/Data`; UnityPy |
| `restore_shaders.py` | Original extracted game data, `recovered/OriginalShaders`, `glslangValidator`, and `spirv-cross` |
| `ExportNative.java` | Ghidra analysis of the original native library |

Keep `recovered/ExportedProject/` as the unchanged comparison baseline. Original assemblies under `recovered/AuxiliaryFiles/` are local analysis inputs and are excluded from Git.

## Install and validate on a device

```sh
adb install -r builds/SlingshotRacing-modern-arm64.apk
```

Installation requires a connected, authorized ARM64 Android device with API 26 or newer. The package is `com.crescentmoongames.slingshotracing.modern`, separate from the original. It uses local debug signing. Rebuilding on another machine may use a different signing certificate; preserve the intended signing setup and save data when updating an existing installation.

See [QA](QA.md) for the runtime matrix and recorded limits. Use a dedicated test installation for diagnostic catalogue runs because they take control of game state.
