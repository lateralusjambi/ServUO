# Pre-P16 bonding boundary

Target: LBR through April 22, 2002. Bonding, bonded-pet ghosts and pet
resurrection are excluded. The official July 23, 2002 Publish 16 taming notes
place these features after the cutoff:
https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/

## Implementation

`BaseCreature.BondingEnabled` follows `!SkillCheck.PrePublish16Rules`, using
`Era.PrePublish16`. It does not depend on the provisional control-model switch.
With pre-P16 rules enabled:

* Ordinary creatures are not bondable. Accepted feeding still consumes food
  and runs the existing stamina/loyalty logic, but cannot start or complete
  bonding, including when a derived creature overrides `IsBondable`.
* `IsBonded` reads false and rejects attempts to assign true. `BondingBegin`
  reads `DateTime.MinValue` and rejects attempts to start a timer. Commands,
  later-content scripts and raw flags loaded from a save cannot opt a living
  pet into the bonded death branch or display a bonded label.
* Existing `OnBeforeDeath`/`OnDeath` branches therefore use ordinary unbonded
  behavior; they cannot create a ghost through bonding.
* `ResurrectPet` returns before resurrection hooks, stats or effects.

With `Era.PrePublish16=false`, the existing bonding timers, feeding completion,
bonded death and resurrection paths remain enabled. There is no separate
bonding configuration switch, save-version increment or altered save layout.
Feeding probability, loyalty increments, taming, control, stable limits and
Animal Lore output are outside this patch.

## Existing worlds

Chris confirmed there are no existing bonded pets or pet ghosts in the test
world before applying this change. No bulk migration is performed. Private
serialized bonding fields retain their existing format and are not rewritten
just by reading their gated public properties; dormant stored values can become
visible again if the era switch is disabled. Explicit assignments still update
those fields through the gated setters.

This patch does not convert, revive or delete previously saved pet ghosts.
A world importing existing ghosts must resolve them before enabling pre-P16
rules: the pet-resurrection path is intentionally unavailable in that mode.
`IsDeadPet` remains intact because it also serves special later-content NPCs;
this is not an audit or removal of those world-content spawners.

## Validation

The full Ultima, Server and Scripts assemblies compile. All 18 automated
assertions passed. The standalone
`tests/PrePublish16BondingTests.cs` runs against the compiled server assemblies,
with empty in-memory world indexes and internal-map fixtures. It exercises
actual feeding, death and resurrection methods in both era modes, direct
bonding assignments, raw saved-field simulation and an `IsBondable` override.
The saved-field simulation is not a complete disk save/load test.

Run the test as a separate executable, never from server startup. For example,
after a Windows Debug build, from the repository root in PowerShell:

```powershell
$fxc = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
& $fxc /nologo /out:BondingTests.exe /reference:ServUO.exe /reference:Ultima.dll /reference:Scripts.dll tests\PrePublish16BondingTests.cs
if ($LASTEXITCODE -eq 0) { .\BondingTests.exe }
```

Compilation assumes these three build outputs are in the repository root.
Remove the generated test executable afterward; do not commit it.

Client acceptance remains pending:

1. With the existing pre-P16 configuration, feed a freshly tamed disposable
   bull its accepted food. It should consume it; GM properties should show
   `IsBonded=false` and `BondingBegin` at its minimum value.
2. Try setting `IsBonded=true` and an old `BondingBegin` with GM properties.
   They should remain disabled. Feed again and confirm no bonded message/tag.
3. Kill that disposable bull. Expect ordinary death/corpse behavior, with no
   following pet ghost. Save and restart; confirm the world loads normally.

The automated later-era negative-path tests do not require changing the live
world's era configuration. In-client feeding/death and a real restart still
need validation before marking runtime acceptance complete.
