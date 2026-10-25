# Third-party notices

## Original game

Slingshot Racing was developed by Snowbolt Interactive Ltd. and published by [Crescent Moon Games](https://www.crescentmoongames.com/). Original game code, assets, and music retain their owners' rights. See [CREDITS.md](CREDITS.md) for the in-game attribution and [LICENSE.md](LICENSE.md) for the repository's licensing boundaries.

## Roboto Slab

- Project: [googlefonts/robotoslab](https://github.com/googlefonts/robotoslab).
- Copyright: **2018 The Roboto Slab Project Authors**.
- Designer recorded in the supplied metadata: **Christian Robertson**.
- Source revision recorded in `METADATA.pb`: `67af3ce9c4ca574419e1295b6165a2eeee112e6e`.
- License: [Apache License 2.0](third_party/roboto-slab/LICENSE.txt).
- Included files: `third_party/roboto-slab/RobotoSlab-wght.ttf`, `METADATA.pb`, and `LICENSE.txt`.

The restoration rasterizes additional Cyrillic glyphs from Roboto Slab at weight 600 into the game's bitmap font atlas. Original glyph pixels and metrics are preserved. The modification notice and full license are also included in the application through `unity/Assets/StreamingAssets/ThirdPartyNotices.txt`.

## Recovered middleware

The recovery baseline includes legacy Unity integration code and middleware such as Prime31/P31RestKit, EZ GUI, and adapters for older platform and social services. Their presence is historical provenance, not a grant of a new license. No blanket permissive license is asserted for recovered middleware; preserve existing notices and review the applicable vendor terms before distribution.

The port does not connect the retired analytics, purchase, account, or video-sharing integrations. Their source-level status is described in [Online services](docs/ONLINE_SERVICES.md).

## Development tools and engine

[Unity](https://unity.com/), Android SDK/NDK, and Xcode Command Line Tools are installed separately and retain their own terms. Python tools depend on Pillow, NumPy, pyelftools, Unicorn, UnityPy, and PyYAML as listed in `requirements-dev.txt`; those installed packages are not vendored here and retain their upstream licenses.

Screenshots in `docs/images/` were captured from local port builds. They contain original game artwork and carry the same attribution; they are not separately licensed as original artwork by this project.
