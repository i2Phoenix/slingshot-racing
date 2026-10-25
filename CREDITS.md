# Credits and provenance

## Original Slingshot Racing

The following credits come from the original game's English language table and credits-screen implementation, preserved in `recovered/ExportedProject/Assets/`. The modern game's credits retain this attribution.

| Role | Credit |
|---|---|
| Original developer and in-game copyright notice | **Snowbolt Interactive Ltd. — Copyright © 2012. All rights reserved.** |
| Original publisher | **[Crescent Moon Games](https://www.crescentmoongames.com/)** |
| Development | Matthew Halpin, Peter Halpin, Tim Hughes, Dean Searle, Matthew Simper |
| Sound design | Jerry Leeper |
| Game music | **© 2012 Andrew J Goose**; the original credits identify `www.studio96.co.uk` |
| Theme music | **[Sean Beeson](https://www.seanbeeson.com/)** |

Special thanks, as recorded in the game: Adhoc Actors, Nikki Braine, Steven Burgess, Andrew Chappell, Oscar Cooper, Matthew Dickinson, Ben Dowie, Phil Duncan, Craig Edwards, Richard Hole, Philip Hughes, Colm Mac Donnchadha, Michael Maidment, Owen McCarthy, Natalie Miller, Matthew Preece, Gary Richards, Ian Shaw, Andrew Scott, Rob Shoebottom.

Snowbolt's historical domain, `snowbolt.com`, currently leads to a domain-sale page rather than an active studio website. The [Snowbolt company record](https://www.mobygames.com/company/31070/snowbolt-interactive-ltd/) and [contemporary company profile](https://www.pocketgamer.biz/industry/snowbolt-interactive/) provide historical references. The Studio96 address is preserved as an in-game credit, not asserted to be a currently available contact.

These are the original credits and copyright statements, not a determination of subsequent ownership transfers.

## Independent restoration

This repository is maintained by [i2Phoenix](https://github.com/i2Phoenix). The port aims to preserve gaming heritage and make Slingshot Racing playable on modern phones. Its work includes the Unity upgrade, native plugin reconstruction, compatibility fixes, Russian localization, build tools, and validation. It does not claim authorship of the original game or an affiliation with its developer or publisher.

## Source baseline

| Item | Recorded value |
|---|---|
| Game | Slingshot Racing 1.3.3.3 |
| Android versionCode | 13 |
| Original package | `com.crescentmoongames.slingshotracing` |
| Original engine | Unity 3.5.7f6, Mono |
| Original architecture | ARMv7 |
| Original SDK range | min API 8 / target API 19 |
| Local APK filename | `SlingshotRacing_android_1416506021.apk.zip` |
| SHA-256 | `36337be2a945154f3fde558e206fb7fdb7771934e682d1a3ea692038ffcfee73` |

Despite the filename, that file is an APK-compatible ZIP with the game files directly inside it. It is excluded from Git, along with extracted original assemblies. The recovery baseline and modern editable assets remain separate so changes can be audited.

## Historical references

- [Official launch trailer, published by Crescent Moon Games](https://www.youtube.com/watch?v=T2YFg4pHar0): identifies Snowbolt Interactive as developer and Crescent Moon Games as publisher.
- [Release history](https://www.mobygames.com/game/100208/slingshot-racing/releases/): iOS on May 24, 2012; Android on February 26, 2013.
- [Game record and original description](https://www.mobygames.com/game/100208/slingshot-racing/).
- Original credit implementation: `recovered/ExportedProject/Assets/Scripts/Assembly-CSharp/PipeGUI_CreditsScreen.cs`.
- Original copyright text: `recovered/ExportedProject/Assets/Resources/languages/EN_Sheet1.txt`, key `ID_CreditsCopyright`.

External attribution references were reviewed on October 25, 2026. No original APK download is provided here.
