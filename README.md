# Block Strike 3.7.0

An unofficial reconstruction of the **Block Strike 3.7.0** Unity project from the Android release. The goal is to make this version's scenes, scripts, and assets accessible for study, preservation, and further repair. This repository is a working Unity project and a set of recovery tools, **not an official game release**.

## Open the project

1. Clone or download this repository.
2. Open the **`client/`** directory as a project in **Unity 5.6.7f1**. The original game used Unity 4.7.2f1; the recovered project is adapted for 5.6.7f1. Opening it in a newer editor may upgrade or rewrite project files.
3. Let Unity import the assets, then browse the scenes under `client/Assets/Levels/`. The Menu scene is at `client/Assets/Levels/Menu.unity`.

The recovered project in `client/` already contains the available repairs. You do not need to run the installation scripts just to open it.

## Repository layout

| Path | Contents |
| --- | --- |
| `client/` | Recovered Unity project: scenes, scripts, assets, and editor tooling. |
| `original/apk/` | Original Android APK kept as reference material; it is not a build of this project. |
| `tools/` | Scripts and Unity editor tools for reproducing parts of the recovery process. |
| `docs/` | Technical notes on the export, scene names, geometry, shaders, lightmaps, and compatibility. |

If you are working from another export rather than the included `client/` project, see the recovery notes in [`docs/`](docs/) and the installer at [`tools/Install-AllRecovery.ps1`](tools/Install-AllRecovery.ps1).

## Bugs and contributions

This is a reconstruction, so bugs, missing functionality, and differences from the original game are possible. In particular, opening a scene in the editor is not a guarantee that an Android build or online gameplay will work: some platform integrations and services depend on components outside the recovered Unity project. We plan to investigate reported problems and fix what we can over time.

If you find an issue, please include the scene or feature involved, steps to reproduce it, your Unity version, and any relevant logs or screenshots. Contributions and fixes are welcome. The technical background and current limitations are documented in [`docs/`](docs/).

## Rights

Block Strike and its original content belong to their respective rights holders. The presence of a [`LICENSE`](LICENSE) file does not by itself grant permission to redistribute the original game, APK, or third-party assets; please respect their applicable rights when using this repository.
