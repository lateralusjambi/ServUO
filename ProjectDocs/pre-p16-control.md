# Pre-Publish 16 pet control: provisional implementation

Target: LBR, end of April 22, 2002. This patch concerns pet commands,
ownership, loyalty and calming. It does not change Animal Taming success or
the tested taming eligibility thresholds.

## What is established

The archived Stratics Animal Lore page, captured November 18, 2001 and
February 10, 2002, reproduces a table attributed to former OSI developer
Runesabre. At difficulty 100 it gives these control percentages:

| Taming | Lore | Percent |
|---:|---:|---:|
|0|0|0|
|0|60|1|
|0|100|2|
|50|0|14|
|50|60|30|
|90|0|58|
|90|20|63|
|100|0|70|
|100|20|77|
|100|40|83|
|100|60|87|
|100|80|95|
|100|100|98|

Source: https://web.archive.org/web/20011118101510/http://uo.stratics.com/content/professions/taming/taming-lore.shtml

The PRC Animal Lore page, updated April 16, 2001 and captured July 7, 2001,
contains 21 separate calculator-derived points for difficulty 93.9 and full
loyalty. The reconstructed Reena calculation reproduces those decimal values
when truncated, but conflicts with Runesabre at difficulty 100. This patch
chooses Runesabre as the acceptance target per the project decision. It does
not claim to match Reena/PRC or to recover OSI's original formula.

PRC source: https://web.archive.org/web/20010707030658/http://www.strangeworld.com:80/uo/prc/prc.php?Section=lore_lvo

Contemporary descriptions distinguish loyalty from orneriness. Transfers
contribute ownership history; release/feral events can raise orneriness;
successful Animal Lore calming during retaming reduces it by one level,
never below the creature's original difficulty. The feeding descriptions conflict with later change notes; see PET-FEED in uncertainties.md.

## Explicit reconstruction choices (YELLOW)

These choices are necessary to run the shard; they are NOT established by
matching thirteen table entries:

* Linear interpolation along Lore for each published Taming row, then along
  Taming. Sparse rows extend linearly outside their published Lore range.
  Values are bounded to 0-98%, including each row before Taming interpolation.
* Other creature difficulties normalize each skill by `100 / difficulty`.
  The evidence provides no uniquely determined general difficulty formula.
* Loyalty multiplies the full-loyalty chance by `Loyalty / MaxLoyalty`.
* Control difficulty is `MinTameSkill + 4.8 * ornerinessLevel^2`. This applies
  the documented retame-penalty family to separate control state; the exact
  original internal representation is unknown.
* First release after a new distinct-owner milestone adds that milestone's
  owner count in orneriness levels. Repeating release with no new owner adds
  nothing. Successful calming subtracts one level. Its chance is effective
  Animal Lore / 100, clamped to 0-100%, without adding a skill-gain roll.
* Newly tamed/retamed pets start at 10/100 loyalty. Existing command changes
  (+1 successful, -3 failed) are retained provisionally. Easy pets <=29.1 do
  not gain loyalty from commands; their existing guaranteed-control exemption
  is retained, as are summoned creatures and the GM control bypass.

Those exemptions apply to the original creature difficulty, so release
penalties on exempt easy pets do not remove their exemption in this version.
This exception itself remains a separate historical audit item.

## Integration and saves

Tested integration: ca907ece7ece3fee1addc658f1f153288ab24256.
Runtime results: [runtime-validation.md](runtime-validation.md).
Open decisions: [uncertainties.md](uncertainties.md).

Enable with `PrePublish16Control=true` in Config/Era.cfg. Both that setting
and the existing `Era.PrePublish16` gate must be active. With the new setting
false, the previous GetControlChance formula remains available.

Control history uses its own distinct-owner list and count. It never appends
transfer recipients to `Owners`, so it cannot alter the tested taming gates.
The independent count preserves departed/deleted owners in future penalties.
The SetControlMaster(null) transition covers explicit release and the usual
loyalty-driven release path; transfers change masters without a release penalty.

BaseCreature save version advances from 29 to 30. Older saves load; control
history starts from surviving known taming owners and the current master,
with zero inferred orneriness. Missing historical transfers/releases cannot
be reconstructed. New saves persist level, last penalized owner count,
surviving owner identities and total owner count. Later fields remain aligned.

**Before starting the patched server, retain a copy of the entire old Saves
directory while the server is stopped.** Code rollback after a version-30
save requires the matching older save as well. Disabling the setting changes
behavior but does not revert the save schema.

## Validation and runtime checks

The C# test program tests the actual model against all 13 published values,
then checks boundedness, monotonicity in both skills and difficulty, loyalty
scaling, repeat release, calming floor, and reconstructed state restoration.
Table matching validates calibration, not historical fidelity between points.

Compile tests/PrePublish16ControlTests.cs with
Scripts/Misc/PrePublish16ControlModel.cs as a standalone executable.

Validation performed for this patch: the actual C# model passed 614,945
assertions including all 13 Runesabre rows. Ultima, Server and all Scripts
compiled with Roslyn 4.8 against .NET Framework 4.8 reference assemblies.
Windows startup and the selected ownership/save/restart tests subsequently passed.
See runtime-validation.md for the exact inputs, screenshots and limits; normal-player
command rate trials and exhaustive save migration are not established.

In game, `[PreP16Control` asks for a creature then a player and prints the
normal-player calculation without changing skills or loyalty. It deliberately
bypasses the GM exemption for its read-only display; actual GM pet commands
still have the normal GM exemption. Test live command behavior on a player
account. Set a test creature's MinTameSkill to 100 and Loyalty to 100 to check
the published rows. Do not infer a precise percentage from a handful of commands.

Lifecycle check: a first owner's release gives level 1; 100 Lore retaming
reduces it to 0. Transfer to a second character does not raise the level;
their release gives level 2, and 100 Lore retaming reduces it to 1. Check the
level after save/restart. The taming Owners list must not grow on transfer.
Previously accumulated test pets may differ from freshly created pets because
older release events were not recorded.


## Source version and validation status

This specification is restored from the originally delivered patch. Its prior
lowercase docs path was excluded on Windows. The server still uses the original
two-target diagnostic. runtime-validation.md records the completed checks;
historical reconstruction choices remain YELLOW. PET-BOND, PET-LORE, PET-LIMITS and PET-FEED cover connected pet systems
that this control implementation did not reconcile.
