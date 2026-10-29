# Runtime validation: October 21, 2026

Code checkpoint: `ca907ece7ece3fee1addc658f1f153288ab24256`, branch
`pre-p16-integration`. Results below were reported by Chris and, where noted,
confirmed from the screenshots supplied in the project conversation.

The Windows diagnostic is the original TWO-TARGET command:
`[PreP16Control`, target the creature, then the character. Target their world
entities rather than stat bars. The proposed single-target replacement was
not installed and is not part of this tested baseline.

## Build and installation

* The first attempt ran without the new command source. File presence was
  checked, the patch was applied, and the diagnostic subsequently registered.
* A successful server start by itself does not prove a new build succeeded:
  `_windebug.bat` continues to launch the existing executable after a failed
  `dotnet build`. Inspect the build result when making source changes.
* Independently, Ultima, Server and all Scripts compiled with Roslyn 4.8 and
  .NET Framework 4.8 reference assemblies in the development environment.
* The actual C# model passed 13/13 published Runesabre rows and 614,945 total
  assertions covering bounds, skill/difficulty monotonicity, loyalty scaling,
  repeat release, calming floor and reconstruction of its small state object.

## Diagnostic and lifecycle results

| Check | Observed inputs / transition | Result | Evidence |
|---|---|---|---|
| Full-loyalty high-skill result | Taming 100.0, Lore 100.0, difficulty 100.0, orneriness 0, loyalty 100 | 98.00% | image(4).png |
| Low-skill input with stat bonus | Taming 9.3, Lore 100.0, difficulty 100.0, loyalty 100 | 9.19%, matching the selected interpolation | image(5).png |
| Half loyalty | Taming/Lore 100/100, difficulty 100, loyalty 50 | 49.00% | image(6).png |
| First owner's release | Bull minimum 71.1; tamed then released by first owner | Orneriness 1; difficulty 75.9; loyalty 11; chance 10.78% | image(7).png |
| First owner's calming retame | Same bull, Lore 100 | Calming message; orneriness 0; difficulty 71.1; loyalty 10; chance 9.80% | image(8).png |
| Direct transfer to second character | Transferred without an intervening release; loyalty restored to 100 | Orneriness 0; difficulty 71.1; chance 98.00% | image(9).png |
| Second owner's release | Same transferred bull released by second owner | Orneriness 2; difficulty 90.3 | image(10).png |
| Persistence | Saved in released state, shut down and restarted via `_windebug.bat` | Orneriness 2 and difficulty 90.3 preserved | image(20261022-023444).png |
| Calming after restart | Second owner retamed with Lore 100 | Orneriness 1; difficulty 75.9; loyalty 10; chance 9.80% | image(20261022-023712).png |
| Repeat release without another owner | Same second owner released again | Orneriness stayed 1; difficulty stayed 75.9; loyalty 5; chance 4.90% | image(20261022-023836).png |

Screenshots are evidence references to the supplied session artifacts; no
personal screenshots or world-save bytes are embedded in this repository.

## Scope limits

* The 0 Taming / 100 Lore = 2% row passed the C# model test, but was NOT
  exercised at exactly zero effective skill in game: the character displayed
  9.3 because of a stat bonus. Do not record that screenshot as a zero-skill pass.
* The GM diagnostic deliberately bypasses the administrator control exemption
  when reporting the normal-player calculation. Displayed odds are not an
  empirical measurement of success rates from normal-player command trials.
* The general probability curve, difficulty normalization, loyalty multiplier,
  intermediate Lore calming chance and general release accumulation rule are
  still YELLOW. These tests validate the selected reconstruction.
* The restart check exercised a saved nonzero state on the same bull. It is
  not an exhaustive migration test for every old BaseCreature save version,
  deleted owners, exceptional creature overrides or long histories.
* Transfer did not raise the displayed orneriness. The unchanged taming-owner
  list follows from the code separation; its count was not separately printed
  by this diagnostic.
* Bonding, pet death/resurrection, feeding, natural loyalty-driven ferality,
  stabling, mount checks and the entire command matrix were not validated by
  these screenshots.

## Save compatibility

This code writes BaseCreature version 30. Keep the pre-patch full Saves backup.
Reverting to version-29 code after saving requires the corresponding earlier
world save. Switching the control configuration off does not downgrade saves.

After these checks, Chris committed the five tracked implementation/test files
and pushed `pre-p16-integration`. The intended lowercase documentation file
was absent from that commit because `/Docs` was ignored on Windows; these
ProjectDocs records repair that documentation gap without changing gameplay.

## Bonding acceptance: October 25, 2026

The subsequent bonding gate at `d2765ca2` passed its bounded client checks:
Windows Debug build succeeded in 8.5 seconds; after feeding, a fresh bull showed
`IsBonded=False` and `BondingBegin=1/1/0001`; attempting to assign
`IsBonded=True` left it false; killing the bull left a corpse and no pet ghost.
After the requested save/shutdown/relaunch sequence, Chris confirmed the world
loaded. These are user-reported results, without a captured startup log.

See [bonding boundary and validation limits](pre-p16-bonding.md) for the 18
automated assertions, both-era coverage and the checks not separately repeated
in the client. The earlier control-model uncertainties remain unchanged.

## Animal Lore acceptance: October 25, 2026

Code checkpoint `45996c09`; Windows Debug build succeeded in 8.1 seconds.
The four inspected journal screenshots confirm the bounded lore output:

* `image(20261026-015301).png`: wild bull at client-reported Lore 90.0
  returned never-tamed and combat-training facts, without a P16 access rejection
  or numeric lore result shown.
* `image(20261026-015426).png`: tamed bull returned one-master and meat-resource
  facts, plus extremely unhappy loyalty.
* `image(20261026-015635).png`: eagle returned the corrected feather message.
* `image(20261026-015723).png`: non-animal target was rejected without facts.

Client acceptance for this bounded handler/display change passed. AccessLevel
is not visible in the images; the eagle image includes an administrative spawn
log. Failed-roll/dead-viewer paths and later-era behavior passed compiled tests
but were not separately shown in this client session. See
[Animal Lore evidence and limits](pre-p16-animal-lore.md). Exact bands, odds,
aggregation, selection weights and original message transport remain YELLOW.

## Stabling and ordinary vendor purchases: October 28, 2026

Gameplay checkpoint `778bc69a`; Chris reported a Windows Debug build succeeding
in 8.3 seconds. A fresh bull remained at orneriness 0 / difficulty 71.1 across
paid NPC stable/claim, then reached 1 / 75.9 on genuine release. Following the
normal-player test instructions with six one-slot bulls and FollowersMax 5,
an ordinary vendor purchase delivered the item and deducted gold.

Both bounded client checks passed. These are user-reported results; character
privilege/follower properties and payment amount were not separately captured.
Positive-slot purchase rejection has compiled-test coverage but was not
separately reported in the client. No stabled-pet save/restart was performed
for this patch. See [implementation and validation limits](pre-p16-stabling-vendor-fixes.md).
