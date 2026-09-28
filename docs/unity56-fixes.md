# First Unity 5.6 compiler fixes

> NOTE (2026-09-28): all 7 patches below are ALREADY APPLIED inside `client/`.
> This document now only documents the standalone installer for other project copies.


Apply with Unity closed, on the migrated working copy, not by overwriting it
with the original ZIP:

```powershell
.\tools\Apply-Unity56Fixes.ps1 -ProjectPath "$env:USERPROFILE\Desktop\BlockStrike-Unity56\UnityProject"
```

The script checks all four replacement targets before writing, accepts already
applied fixes, and backs up all changed files outside Assets. It preserves other
changes made by Unity API Updater. The original export ZIP is unchanged.

- InAppManager: explicitly uses FreeJSON.JsonObject. Verified against the
  original recovered DLL: SendFirebase's second newobj refers to its constructor.
- vp_EventDump: reconstructs Get/Set/default switch. Original IL maps Get to 0,
  Set to 1, and appends Unsupported listener for unmatched input. The exporter
  had lost the Set branch and emitted an unassigned local variable.
- vp_Interactable/vp_Switch: express the existing enumerator search as a while
  loop. Exhaustion returns; a matching tag exits the search and continues the
  existing interaction. Enumerator disposal remains in using. No artificial
  initial values or removed interactions.

Validation: all replacement targets occur exactly once in original exported
sources. Constructor owner and event-switch behavior inspected in recovered IL.
No Unity/C# compiler is available in this environment: user editor compilation
is still required. These fixes do not address map rendering or other errors
missing from the supplied log excerpt.

CEF JavaScript errors refer to the embedded Asset Store browser. Update-check
HTTP 404 is separate from script compilation. Neither justifies deleting game
scripts. The supplied excerpt starts after a GameManager.cs:436 diagnostic;
the preceding actual error message is still needed.

## Second pass: ByteReader and GameManager

The same patch command now handles six files and skips the four already fixed.

- ByteReader.ReadLine(bool): restructure the do/continue condition into an
  explicit loop with a locally assigned byte. Preserve CR/LF handling and the
  existing EOF increment, including the final unterminated line. Exhaustive
  Python models of old/new loop offsets agreed for short buffers over seven
  byte values (including CR, LF, NUL and non-ASCII).
- GameManager.StartAutoBalance: replace the un-compilable delegate-pointer
  construction with a parameterless wrapper calling BalanceTeam(false).
  vp_Timer.Callback is void(), while BalanceTeam takes an optional bool defaulting
  to false. Preserve delay=30, iterations=-1 and interval=30. This is an explicit
  compatibility adaptation: identical behavior of the original mismatched
  delegate under the old runtime is NOT proven. Auto-balance must be tested.

User logs contain historical compiler diagnostics. The latest entries supplied
contain only ByteReader and GameManager, suggesting the initial fixes applied.
Unity compilation of this second pass is still pending on the user's machine.
