# Original online features

The original recovered source and `Assembly-CSharp.dll` read directly from the supplied APK were inspected on **October 5, 2026**. A class or button does not establish that its service was enabled in that build. Server availability and account sign-in were not tested.

The current port is offline. Local saves, same-device multiplayer, and the game's own race replay remain available.

| Feature | Original design | Observed state in the supplied APK and this port |
|---|---|---|
| Google Play Games | Sign-in, achievements, score submission, and per-race leaderboards | Original `IsGoogleGameServicesSupported` and `IsPlayerAuthenticated` always return `false`. No working integration is connected in the port. |
| Game Center / Amazon GameCircle | Alternative platform services, including asynchronous Game Center matches | Both support flags return `false` in the original Android assembly. Their abstractions do not provide working Android online play. |
| Cloud saves | Load, merge, and store progress | Original `CloudUtils.DocumentStoreAvailable` returns `false`; its cloud path is `null`. Local saves work; cloud transfer is not connected. |
| Google Play purchases | Unlock all stages, four vehicle bodies, and cog/time-trial guides; restore purchases | Product handlers exist, but the original Google Play manager does not implement store operations and inherited methods return `false`. The port does not connect a store. |
| Facebook / Twitter | Share text and images through old Prime31 plugins | Submission code exists, but original `SocialManager.Awake` performs no initialization and buttons depend on initialization flags. Sharing is not connected. |
| Kamcord | Record and publish gameplay video | Original `CanBeEnabled` returns `false` and startup disables the option. The game's separate movement replay still works. |
| Flurry | Session/device/menu/race/purchase analytics | Original `BootScene` calls `Analytics.Init`, which creates the Flurry Java class and starts a session. The port gates this integration behind a disabled compilation symbol. |

## Network play

`TurnBasedMatchManager` models asynchronous races and movement ghosts, but this APK's `FindMatch` creates local `Player1`/`Player2`, its server-wait method returns `false`, and the network-match path requires Game Center. This is not evidence of working matchmaking.

Legacy `NetworkManager`/`NetworkClient` classes also contain manual IP entry, a server button, and movement exchange through Unity Network. They are test code, gated in the port by `SLINGSHOT_LEGACY_NETWORK_TEST`, not a verified user-facing online mode. The confirmed multiplayer mode is two to four people sharing one screen.

FAQ and publisher-game links are external webpages, not game servers. The inspected leaderboard path uses platform service abstractions; it does not establish a separate active publisher leaderboard server.

## Evidence and preservation

The local, unversioned `work/qa/original-online-flags.json` records ten original methods with CIL `ldc.i4.0; ret`. The extracted assembly matched the file inside the supplied APK.

Relevant baseline files are in `recovered/ExportedProject/Assets/Scripts/Assembly-CSharp/`: `SocialManager.cs`, `CloudUtils.cs`, `IAPHandler.cs`, `InAppPurchaseManager_GooglePlay.cs`, `KamcordGameSettings.cs`, `PipeGUI_ShareResult.cs`, `TurnBasedMatchManager.cs`, `NetworkManager.cs`, `Analytics.cs`, and `BootScene.cs`.

The working `unity/Assets/Scripts/Assembly-CSharp/Analytics.cs` only loads Flurry with `SLINGSHOT_ENABLE_LEGACY_FLURRY`, which the current build does not define. The original embedded client analytics identifier is retained as source evidence; it is not a credential supplied for operating a new analytics service.

Restoring online features would require separately implementing and configuring the chosen services. Updating Unity alone does not supply store operations, authentication, cloud storage, or matchmaking.
