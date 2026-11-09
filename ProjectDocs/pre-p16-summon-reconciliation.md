# First summon reconciliation: Blade Spirits and Energy Vortex

Focused audit: November 8, 2026, America/New_York.
Gameplay baseline: quota implementation d8fb686 plus wording f1fc292.
Research/code trace only; no summon behavior changed.

## Concrete defect to reconcile

Under the LBR configuration, both spells compare the caster's weighted
Followers + 1 with FollowersMax. This includes ordinary controlled pets
and golems, not just existing spell summons. A character at or above five
weighted followers therefore cannot start either spell, regardless of how
many spell summons that character actually has.

There is a second admission check in BaseCreature.Summon. It compares
Followers + the actual creature's ControlSlots with FollowersMax, deletes
the proposed creature and returns false when over budget. Removing only
the spell precheck would leave this blocker after the sequence check.

This is a confirmed code path. Its historical replacement remains an
evidence question; we have not established an unlimited summon allowance
or a separate five-summon cap.

## Paths

| Path | Blade Spirits | Energy Vortex |
|---|---|---|
| Spell source | Scripts/Spells/Fifth/BladeSpirits.cs | Scripts/Spells/Eighth/EnergyVortex.cs |
| CheckCast admission | Followers + (Core.SE ? 2 : 1) | Followers + (Core.SE ? 2 : 1) |
| Target placement | Surface, CanSpawnMobile, town and CheckSequence guards | Same guard sequence |
| Creation call | BaseCreature.Summon(new BladeSpirits(true), false, caster, ...) | BaseCreature.Summon(new EnergyVortex(true), false, caster, ...) |
| LBR creature accounting | ControlSlots = 1 | ControlSlots = 1 |
| LBR duration | Random integer 80–119 seconds | Random integer 80–119 seconds |

The false argument denotes uncontrolled summoning. BaseCreature.Summon
still sets Summoned and SummonMaster; follower accounting uses the latter
when there is no ControlMaster. The spells call this shared creation method
directly rather than going through SpellHelper.Summon.

The sixth creature is currently refused, not a reason to overwrite
FollowersMax or stop accounting for every summoned creature. The shared
method has many other callers whose later-era behavior must be retained.

## Primary historical boundary

The official Publish 16 taming notes describe the new five-slot budget and
the eighth-circle elemental/daemon weights. They establish a later shared
budget, but do not spell out the old BS/EV cap or mixed summon/pet rules.
https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/

Official Publish 31 notes, March 14, 2005, explicitly introduce two slots
for BS/EV and a combined six-per-5x5-area crowding limit. These are distinct
later changes; neither is a basis for our pre-cutoff replacement.
The local Core.SE branches already exclude the crowding and doubled-slot
behavior under the checked-in LBR expansion.
https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2005-2/publish-31-14th-march/

## New pre-cutoff player lead

Usenet thread: My Parrying is stuck.....HELP!!
Buddy Lee, January 3, 2002:
https://groups.google.com/g/rec.games.computer.ultima.online/c/Exj92TNtxXk

The advice describes arranging one mage and several healers, having the
mage cast successive Blade Spirits, and using multiple spirits to train
Parrying. It first discusses four or five and later mentions five to seven.
This is contemporary player advice suggesting more than five may have
been possible in the described setup. It is not an observed sixth-cast
transcript, explicit limit statement, or proof of an unlimited allowance.
Replies quoting the advice are not independent corroboration.

The thread also contains a January 23 report of hiring five to seven
fighters. Preserve that as a later hireling lead, not proof of a summon cap.

## Other period material checked

The February 2, 2002 Stratics Magical Strategy capture and April 7, 2002
Magery capture were retrieved directly. They do not resolve the summon cap.
The former's Energy Vortex description also differs from the later live
page on poison behavior, illustrating why the live page cannot substitute
for a dated capture. No poison or combat-stat change is included here.

https://web.archive.org/web/20020202061520/http://uo.stratics.com/content/guides/magic.shtml
https://web.archive.org/web/20020407221930/http://uo.stratics.com/content/skills/magery.shtml

The February 2002 reputation page and April 2002 spell-table capture
attempts were rate limited. The live reputation page concerns responsibility
and flags, and supplies no summon count. Failed retrieval is not contrary evidence.

## Next reconciliation decision

Establish whether BS/EV were counted with controlled Magery summons, and
what per-caster cap (if any) applied immediately before April 23, 2002.
Do not infer five merely from the modern FollowersMax value. Trace older
implementation evidence and dated player/developer material against this
specific question.

Then introduce a shared pre-P16 summon-admission policy used by both spell
prechecks and completion. Preserve town, placement, skill, mana/reagent and
ordinary expiration checks. Test pets/golems above five weighted followers,
BS/EV mixtures, controlled-summon interaction, expiration/deletion freeing
capacity, admission changing between cast and completion, and the later-era
negative path. Scope duration, targeting, damage and criminality as separate
questions unless evidence requires them for the admission fix.
