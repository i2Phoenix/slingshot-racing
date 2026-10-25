# Working on this repository

Use [Building and verification](docs/BUILDING.md) to set up the pinned Unity editor and native toolchain. Keep the recovery baseline unchanged; make game changes in `unity/` and native implementation changes in `native/`.

- Preserve Unity `.meta` files, GUIDs, original credits, and third-party notices.
- Keep original APKs, signing material, local tools, caches, generated libraries, builds, saves, and raw QA logs out of commits.
- Keep documentation, commit messages, and repository metadata in English. Game translations belong in their language-specific resources.
- Run the resource and localization checks for affected assets. For native changes, rebuild the plugin and run numerical parity using the separately supplied original library.
- Use a development APK for diagnostics. Validate release APK architecture, signing, absence of diagnostics, and 16 KB alignment before describing it as verified.
- State which APK, device, and scenarios were tested. Do not turn an old report or a compilation result into a new gameplay claim.
- Review [LICENSE.md](LICENSE.md) before sharing source or binaries outside this private repository.

Changes to services, purchases, game rules, and physics should be explicit and documented. The current build is an offline restoration with same-device multiplayer.
