# Pre-P16 Magery summon admission

Implemented November 8, 2026 (America/New_York). Live client acceptance is pending.

## Selected rule and confidence

With `Era.PrePublish16=true`, the eight original Magery summon spells have
no explicit creature-count or weighted-follower admission cap. This is a
YELLOW reconstruction supported by the evidence in
[the history ledger](pre-p16-summon-history.md), not a recovered OSI rule
proving an unlimited April 2002 allowance.

Affected spells: Blade Spirits, Energy Vortex, Summon Creature, Air
Elemental, Earth Elemental, Fire Elemental, Water Elemental, Summon Daemon.
Ordinary pets and golems cannot prevent these spells through their follower
weight. Skill, mana, reagents, placement and region checks still run.

## Implementation boundary

`PrePublish16Summoning.CanSummon` applies the exemption only when the caller
explicitly selects `magerySummon` and the era option is enabled. All eight
spells select it for their precheck and creation call. `SpellHelper.Summon`
forwards it to `BaseCreature.Summon`, which performs the final admission check
against the live follower total. Optional parameters default to false, so
unmodified summoning systems retain their existing budget checks.

Disabling the era option retains each spell's existing cost, including the
SE-dependent BS/EV and daemon costs. This switch changes admission only;
it does not remove already-created creatures when toggled.

`Followers`, `ControlSlots`, `ControlMaster`, `SummonMaster`, `Summoned`,
`AllFollowers`, duration and deletion accounting are preserved. Controlled
summons still have a control master; BS/EV remain autonomous. Existing
summon obedience exemption and ordinary-command transfer refusal were
traced and left unchanged. Hireling limits and other summon systems are
outside this change. No serialization fields or save versions changed.

## Automated verification

All Ultima, Server and Scripts sources compiled with Roslyn 4.8 against
.NET Framework 4.8 reference assemblies, executed with Mono.

`tests/PrePublish16SummoningTests.cs`: 748 assertions passed:

* Actual `CheckCast` methods for all eight spells, totals 0 through 12,
  pre16 option on/off, LBR and SE expansion settings.
* Actual shared creation method for controlled/autonomous summons,
  explicit Magery and default callers, within and beyond the budget.
* Three five-slot summons plus five one-slot summons alongside an ordinary
  pet and golem, with ownership and weighted accounting intact.
* Zero base Taming/Lore player obedience; ordinary deletion and actual
  `UnsummonTimer.OnTick` cleanup, including repeated expiry callbacks.
* Final admission rejects a budget consumed after an initially successful
  spell precheck in later mode; pre16 completion succeeds.
* Requested expiry time is retained.

Existing regressions also passed: golems 197, pet acquisition 221, stable
capacity 187, paid stabling/vendor 96, bonding 18. Total: 1,467 assertions.

These fixtures do not run a network client, full spell sequence, map-data
placement, natural elapsed timer dispatch or server save/restart. Source
review confirms opt-in propagation through every creation branch, including
both AOS and older creature variants. Live tests below cover the remaining
player-facing path; they do not establish historical truth.

## Bounded live acceptance (pending)

Save and stop the server, pull/build, verify build success, then restart.
Use a normal-access character with sufficient Magery, mana and reagents;
an administrator can observe or provision the test character.

1. In an allowed open location, cast two Summon Daemons before the first
   expires. Both should remain owned, follow and obey the same character.
2. With an ordinary pet or golem already following, repeat. Its follower
   weight should not cause the summon refusal message.
3. In a clear safe test area, cast at least six BS/EV while earlier ones
   remain active. The sixth should not be refused for follower count.
   Record the simultaneous count; do not confuse expiration with refusal.
4. Exercise Summon Creature and each elemental with pets already following.
   Normal mana/reagent/placement failure remains possible.
5. Let summons expire and confirm ordinary pets remain owned and responsive.
   Confirm normal transfer commands continue to refuse summoned pets.
6. On a separate test configuration with `Era.PrePublish16=false`, restart
   and confirm the previous weighted admission limits return. Do not change
   the live era merely for this check.

Record results, date and deployed commit before marking client acceptance
complete. No live results are claimed by this implementation document.
