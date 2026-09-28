# Block Strike 3.7.0 — восстановление Unity-проекта

## Проект находится в `client/`

`client/` — восстановленный Unity-проект Block Strike 3.7.0. Открывать в Unity **5.6.7f1** (оригинальный проект был 4.7.2f1; лицензия на целевой редактор подтверждена 2026-09-28).

В закоммиченном проекте уже сделано:

- **35 шейдеров** восстановлены (плейсхолдеров `DummyShaderTextExporter` — ноль): NGUI UI, Mobile/VertexLit игровые, MADFINGER god-rays, ProBuilder, эра-совместимые built-in (см. `tools/unity-editor/ShaderRecovery`, источники в `tools/shader-extract`).
- **Все 56 сцен** переименованы в читаемые имена (`Assets/Levels/.../*.unity`), пути в `ProjectSettings/EditorBuildSettings.asset` обновлены, `LevelManager.cs` больше не шифрует/дешифрует имена.
- **Геометрический ремонт уже применён** к 54 сценам с картами: 3 891 восстановленный меш используется сценами, ссылки на старые `Combined Mesh` убраны. Инструмент и 56 манифестов остаются в проекте для аудита/повторного запуска.
- **54 папки лайтмапов** и GUID-файлы сохранены/нормализованы. Но привязки в сценах отсутствуют: Unity 5.6 `LightingData.asset` не восстановлен. Текстуры на месте, но для работы света нужно выполнить необязательный пункт ниже (см. `docs/lightmap-binding.md`).
- **7 компиляторных патчей Unity 5.6** применены к скриптам (`tools/unity56-fixes.json`).
- **Редакторный биндер лайтмапов** добавлен, но пока не применён к сценам: он проверяет данные, делает резервные копии и имеет полный Revert.

## Проверка и включение лайтмапов

В Unity 5.6.7f1 после компиляции:

1. `Tools → Block Strike Recovery → Validate ALL legacy lightmaps` — проверка без изменений файлов.
2. `Tools → Block Strike Recovery → Bind ALL legacy lightmaps` — после подтверждения резервирует сцены и `.meta`, добавляет рантайм-биндер в 54 карты, помечает их PNG как Lightmap и исправляет `MeshAtlas`-ссылки, которые подменяют восстановленные меши временными клонами.

Если визуальный результат не понравится: `Tools → Block Strike Recovery → Revert last legacy lightmap binding`. В текущем окружении Unity нет, поэтому биндер ещё не компилировался/не проверялся визуально — см. ограничения в `docs/lightmap-binding.md`.

## Установка в другую копию экспорта

Один скрипт ставит Unity 5.6 фиксы, имена сцен, нормализацию путей лайтмапов, шейдеры, геометрический инструмент и биндер:

```powershell
.\tools\Install-AllRecovery.ps1 -ProjectPath "ПУТЬ\К\UnityProject"
```

В свежей копии экспорта сначала запусти `Tools → Block Strike Recovery → Repair ALL scene geometry`, затем проверь и привяжи лайтмапы командами выше. В `client/` геометрия уже восстановлена.

## Структура репозитория

| Путь | Содержимое |
|---|---|
| `client/` | Unity-проект для 5.6.7f1 |
| `original/apk/` | исходный рабочий APK 3.7.0 |
| `tools/` | pipeline: экспорт, декрипт имён, шейдеры, геометрия, лайтмапы и установщики |
| `docs/` | отчёты по восстановлению (`export-status.md`, `shader-lightmap-recovery.md`, `lightmap-binding.md`, `scene-names.md`, `unity56-fixes.md`, `map-geometry-repair.md`) |

Бэкапы установщиков и инструментов лежат в `RecoveryBackups/` рядом с `Assets` (вне Unity-импорта и вне git).
