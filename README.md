# Block Strike 3.7.0 — восстановление Unity-проекта

## Главное: проект готов в `client/`

Папка `client/` — полный Unity-проект, извлечённый из рабочего APK 6.0.8.
**Открывать в Unity 5.6.7f1** (целевой редактор; оригинальный 4.7.2f1 апгрейдится автоматически, но лицензия у нас живая только на 5.6.7f1 — зафиксировано 2026-09-28).

Всё известное уже применено прямо к файлам проекта, ничего ставить не нужно:

- **35 шейдеров** восстановлены (плейсхолдеров `DummyShaderTextExporter` — ноль): NGUI UI, Mobile/VertexLit игровые, MADFINGER god-rays, ProBuilder, эра-совместимые built-in (см. `tools/unity-editor/ShaderRecovery`, источники в `tools/shader-extract`).
- **Все 56 сцен переименованы** в читаемые имена (`Assets/Levels/.../*.unity`), пути в `ProjectSettings/EditorBuildSettings.asset` обновлены, `LevelManager.cs` больше не шифрует/дешифрует имена.
- **54 папки лайтмапов** нормализованы (GUID/.meta сохранены).
- **7 компиляторных патчей для Unity 5.6** применены к скриптам (`tools/unity56-fixes.json`).
- **Универсальный инструмент починки геометрии** уже лежит в `client/Assets/Editor/BlockStrikeRecovery/` (56 манифестов карт).

## Остался один шаг в редакторе

Открой `client/` в Unity **5.6.7f1**, дождись компиляции и импорта, затем:

**Tools → Block Strike Recovery → Repair ALL scene geometry**

Это восстанавливает статичную геометрию всех 56 сцен (включая Menu) по манифестам из репозитория.

## Если нужна установка в другую копию экспорта

Один скрипт делает всё сразу (unity56-фиксы → имена сцен → лайтмапы → шейдеры → гео-инструмент):

```powershell
.\tools\Install-AllRecovery.ps1 -ProjectPath "ПУТЬ\К\UnityProject"
```

## Структура репозитория

| Путь | Содержимое |
|---|---|
| `client/` | готовый Unity-проект (открывать в 5.6.7f1) |
| `original/apk/` | исходный рабочий APK 3.7.0 |
| `tools/` | весь pipeline: экспорт, декрипт имён, шейдеры, геометрия, установщики |
| `docs/` | отчёты по восстановлению (см. `export-status.md`, `shader-lightmap-recovery.md`, `scene-names.md`, `unity56-fixes.md`, `map-geometry-repair.md`) |

Бэкапы изменений, если ставишь в другую копию, скрипты складывают в `RecoveryBackups/` рядом с `Assets` (папка вне Unity-импорта и вне git).
