# Block Strike 608 — восстановление Unity-проекта

## Открыть в Unity 4.7.2f1

Готовый **первичный экспорт** находится в:

`exports/BlockStrike-608-Unity-4.7.2f1.zip`

Архив содержит папку `UnityProject` с `Assets`, `ProjectSettings` и результатами
экспорта AssetRipper. Это не `.unitypackage`: не импортируйте архив в пустой проект.
Распакуйте его и откройте **папку UnityProject целиком** через Open Project.

После обновления репозитория в PowerShell:

```powershell
cd "C:\Users\vlal\blockstrikedecompiled"
git pull --ff-only origin arena/01a0dde4-blockstrikedecompiled
Expand-Archive -LiteralPath "exports\BlockStrike-608-Unity-4.7.2f1.zip" -DestinationPath "."
```

В Unity откройте:

`C:\Users\vlal\blockstrikedecompiled\UnityProject`

Не перезаписывайте уже изменённую копию проекта при повторной распаковке.
Не открывайте первую рабочую копию более новой Unity. Дождитесь импорта,
откройте Console и сохраните первые ошибки компиляции. Пока не удаляйте скрипты,
компоненты и библиотеки для устранения ошибок.

## Что внутри

- 881 файл C# с декомпилированными методами;
- 56 сцен, включая Surf/Jet;
- 86 префабов, модели, текстуры, материалы, анимации, аудио;
- настройки проекта с версией 4.7.2f1;
- `.meta`-файлы экспортера.

## Текущий статус восстановления в Unity 5.6.7f1

- Проект пользователя компилируется; отдельное исправление статической
  геометрии **Bust** проверено и визуально подтверждено.
- Универсальный инструмент восстанавливает статическую геометрию всех 56 сцен,
  включая Menu: `docs/map-geometry-repair.md`. Logo и AwakeScene такой геометрии
  не содержат.
- Инструменты исправлений Unity 5.6 и читаемых имён сцен: `tools/`.

Остаются нерешёнными:

- шейдеры-заглушки `DummyShaderTextExporter`, качество lightmap/occlusion;
- запуск Android/Google/Firebase/рекламных сервисов в Windows Editor;
- Account/Photon и проверка всех сетевых режимов;
- настройки Android-плагинов и сборка APK.

Архив проверен на CRC; количества файлов записаны в `exports/manifest.json`.

## Воспроизводимость

`tools/prepare_export.py` расшифровывает Assembly-CSharp.dll (TEA, ключевые слова
1,2,3,4, маркер `<J3Tech>`) и объединяет `.splitN` в отдельной копии данных.
Оригинальный APK не меняется.

```sh
python3 tools/prepare_export.py samples/com.rexetstudio.blockstrike-608.apk
```

Затем загрузите `recovered/export-input/assets/bin/Data` в AssetRipper 2.0.0 и
выполните Export Unity Project. Использованные параметры: стандартные,
ScriptContentLevel=Level2, ShaderExportMode=Dummy. Инструмент включён в
`tools-binaries/AssetRipper_linux_x64.tar.xz` (Linux, не запускать в Windows).

Проект хранится ZIP-архивом, чтобы не добавлять тысячи генерируемых файлов в Git.
Распакованная `UnityProject/`, временные данные и кэш исключены из Git.
