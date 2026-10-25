# Russian localization

Russian uses the game's existing language system. The Settings button cycles through **English → Русский → Français → Italiano → Deutsch → Español**. The selection survives restarts and APK updates. A fresh installation chooses Russian for a Russian Android system locale; an existing saved selection takes priority.

## Coverage

- All 265 original dictionary entries: menus, tutorials, hints, tracks/tours, settings, results, opponent names, and messages.
- Three additional strings extracted from code/artwork: tap to continue, loading, and viewing a newly unlocked tour. Total: **268 keys**.
- The new-tour “VIEW TOUR” button uses the dictionary and displays “К ТУРУ”.
- The Russian loading screen displays “ЗАГРУЗКА”. The original logo texture remains; a shader hides its English loading text, while the bitmap translation follows the same fade.
- Russian finishing boards display “1-е”, “2-е”, etc. instead of the English text embedded in artwork.
- “ПРОДОЛЖИТЬ” auto-fits on one line in both pause scenes.

The Slingshot Racing name, logos, and original author credits remain. Translating old service messages does not restore those services; see [Online services](ONLINE_SERVICES.md).

## Fonts and editing

Edit `unity/Assets/Resources/languages/RU_Sheet1.txt`. Keys, substitutions such as `{0}`, and time formats must match the English table.

Cyrillic glyphs were added to the original bitmap atlas and all three font descriptions. The original 133 characters, metrics, and pixels remain unchanged. Seventy-two additional glyphs occupy the lower half of the expanded 512×512 atlas.

The new glyphs are rasterized from [Roboto Slab](https://github.com/googlefonts/robotoslab), weight 600. Source font, metadata, and Apache 2.0 license are in `third_party/roboto-slab/`. A modification notice and the full license are packaged through `unity/Assets/StreamingAssets/ThirdPartyNotices.txt`.

```sh
.venv/bin/python scripts/extend_cyrillic_font.py
.venv/bin/python scripts/verify_localization.py
scripts/build_android.sh
```

The generator uses the unchanged fonts in `recovered/ExportedProject/` and adjusts shadow offsets for the increased texture height. Verification checks keys, placeholders, complete character coverage, atlas bounds, and preservation of original glyphs. Both Python tools require Pillow; setup is described in [Building](BUILDING.md).

Local reports: `work/qa/cyrillic-font.json` and `work/qa/russian-localization.json`. Recorded emulator checks and their limits are in [QA](QA.md).
