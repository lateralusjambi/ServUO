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

## Demo follow-up and confirmation standard

Checked November 8, 2026. Public decompiled demo mirror:
https://github.com/draxinar/uodemo
Pinned revision reported by the repository API: 77ca798a101a0eaa00ffe6703cae0a32a35907d3

Inspected scripts:
* rundir/scripts.wombat/bldsprts.m and bldsprtsbase.m
* rundir/scripts.wombat/nrgyvrtx.m and nrgyvrtxbase.m
* rundir/scripts.wombat/spelskil.m and sndfx.m
* summoncr.m and summoncrbase.m for a controlled-summon comparison
* vortexai.m as an additional inspected AI path

BS and EV target handlers perform visibility/map and shared spell checks,
then invoke their creation helpers. The shared Q5YC path checks reagents;
Q4LT/Q4LU handle casting requirements, skill and mana. Q5UL is a criminality/
guard helper, not a summon-cap test. In the inspected creation helpers,
createGlobalNPCAt creates the creature, copyControllerInfo records
responsibility, the AI and destcrea scripts are attached, and callbacks
schedule expiration (120 seconds for BS, 90 for EV in these demo helpers).
No per-caster count or follower-budget check was found in these inspected
script paths. This is a source-trace finding, not a demo runtime test or
proof that native creation has no internal limit.

The demo spell-cost switch uses 14 mana for circle five and 50 for circle
eight. Thus a 100-mana pool permits seven BS attempts before regeneration,
but only two EV attempts. That offers a practical explanation for the
reported BS count without establishing either spell's maximum. Failure,
regeneration, casting time and expiration further separate practical count
from a hard cap.

The demo dates to mid-1998, not the LBR cutoff. Reference:
https://uodemo.uo98.org/index.php?title=Main_Page

Confirmation should distinguish these tests:
1. Trace the native createGlobalNPCAt path and any relevant controller
   bookkeeping, or observe unmodified demo behavior above five active
   summons. Admin-only bypasses and changes to count checks invalidate
   that experiment; replenish mana separately if needed and record it.
2. Establish that the original summons remain active while one caster
   creates additional BS/EV. Success beyond five disproves a five-cap,
   but does not by itself prove there was no higher cap.
3. Trace original patch/publish announcements between the demo and April
   22, 2002 for any introduction or revision of summon limits.
4. Corroborate with pre-cutoff player/developer material, prioritizing
   explicit rule statements or one-caster reports beyond five. A group
   screenshot without caster attribution cannot establish a per-caster cap.

Working assessment: no hard cap is a credible hypothesis supported by
the inspected script structure and the January 2002 player lead. Neither
an arbitrary replacement cap nor unconditional historical certainty is
justified yet. No gameplay patch is included in this follow-up.


## Native demo follow-up: creation and controller bookkeeping

Static binary audit completed November 8, 2026, America/New_York.
This strengthens the no-hard-cap hypothesis; it is not a live demo
experiment or proof of the April 2002 production-server rule.

### Reproducible inputs and method

Public mirror, pinned commit:
https://github.com/draxinar/uodemo/tree/77ca798a101a0eaa00ffe6703cae0a32a35907d3

* Binary: uodemo/UoDemo.exe, 2,959,360 bytes.
* Binary SHA-256: bb691a1c80394657dd9c55395523869676b172859ce1bb3ef0527138e506f6c3
* PE32 x86 image base: 0x00400000.
* PE header timestamp: 1998-09-02 21:27:44 UTC. A header timestamp is
  metadata, not independent authentication of the mirror or production build.
* Template input: rundir/bank/templatestable.dat.
* Template SHA-256: f987c21f068049e36e39db63d7ebd6934682a1b4f964847f19df4479cdd5a7d1

Decoded the PE with pefile and disassembled x86 instructions with Capstone.
Located native entry points through the script dispatch table's name and
function pointers, then followed direct calls and conditional branches.
Manually resolved jump tables at 0x004af8d3 (4 entries), 0x004be1d8
(31 entries), 0x004ce9d8 and 0x004ce9ec (5 entries each).
Do not mistake stopping at an indirect switch jump for a complete function.

All addresses below are preferred-image virtual addresses. Function labels
other than the dispatch names describe this audit's interpretation, not
recovered original debug symbols. No binary was modified or executed.

### Findings

| Path | Native address | Observation |
|---|---|---|
| createGlobalNPCAt dispatch entry | 0x00609970 | Points to 0x0041e416; name pointer 0x0060d2bc |
| createGlobalNPCAt wrapper | 0x0041e416 | Copies location, checks map bounds, searches placement, invokes shared factory; no caster argument or per-caster count comparison |
| createGlobalNPCAtSpecificLoc wrapper | 0x0041e3c2 | Same shared factory, without the nearby-location search |
| Map-bounds helper | 0x0042f43f | XY bounds checks |
| Placement search | 0x004af695 | Tile/collision and position search, including direction switch; no caster argument |
| Shared template factory | 0x004bf8b5 | Bounds, template construction, placement and object initialization; no per-caster threshold in the inspected function |
| Template application | 0x004bd7c6 | Loads/applies template fields; allocation delegated to 0x004bd165 |
| Object allocation by template kind | 0x004bd165 | NORMAL selects constructor 0x00481127 |
| copyControllerInfo dispatch entry | 0x00609260 | Points to 0x0041f838; name pointer 0x0060c8f8 |
| copyControllerInfo wrapper | 0x0041f838 | Records controller identity/name and guild metadata; removes defensive objvar; no count/admission branch |
| Objvar setter wrapper | 0x004cdeea | Lazily creates the objvar store and invokes generic insertion 0x004ce6c7 |

The factory receives the template, location and creation options. On the
inspected createGlobalNPCAt route, it is called with options 0, 2, 0.
The script supplies the caster separately, after creation, through
copyControllerInfo. This data flow is stronger evidence than merely
failing to find a familiar refusal string.

The native controller wrapper writes controller, controllerName,
controllerGuild and controllerGuildType from the supplied controller and
its guildstoneId/guildType variables. It contains no summon-count increment,
list of that controller's summons, threshold test or deletion-on-limit path.
Its name getter is the virtual call at offset 0x34; this is not a hidden
admission callback in the wrapper.

Template 573 (0x023d, EV) and 574 (0x023e, BS) both select NORMAL.
Their template definitions contain creature properties rather than a
caster-count policy. NORMAL construction selects vtable 0x005ef1f8.
Resolving the factory's type-predicate calls against this table matters:
offsets 0xd0 and 0xe4 return true, while 0xe8 returns false. Therefore the
factory's special initialization/refusal block reached through 0xe8
(0x004bfb1e onward) is not the NORMAL BS/EV route. Its presence must not
be reported as a hidden summon cap.

The factory increments two global counters at 0x006e7674 and 0x006e7678.
Neither is compared against a limit there; the former increment is specific
to template 12, not BS/EV. A search for their literal absolute addresses in
the binary found only those increment reads/writes. They provide no
evidence of a per-caster maximum.

### Scope and conclusion

No hard per-caster cap was found in the inspected spell scripts, native
creation wrappers, factory bodies or controller-assignment wrapper.
The inspected mechanics are consistent with creating successive BS/EV
until practical casting costs, expiration and placement intervene.

This is a bounded static trace. Generic construction, world registration,
movement, notification, memory-management and scripting infrastructure
have further callees and virtual dispatch. They were not exhaustively
decompiled or exercised. Absence of a caster argument alone does not rule
out implicit script context or unrelated global mechanisms. Accordingly,
this audit does not claim mathematical proof that every possible demo
execution lacks a cap. No sixth-cast runtime result is being asserted.

The larger unresolved boundary is temporal: the demo executable predates
our LBR cutoff by years. This pass does not establish that no intervening
production-server change occurred. The January 2002 one-mage player lead
is compatible with this implementation, but is still not an explicit
unlimited-summon rule statement.

Recommendation: prefer no explicit per-caster BS/EV cap over inventing a
five-summon replacement. Treat that as an evidence-backed reconstruction.
For an implementation, exempt these two uncontrolled summons from both
pre-P16 admission blockers and assess their effect on other follower
admission checks; merely bypassing their own prechecks can leave an
indirect mixed-pet restriction. Retain SummonMaster, criminal attribution,
dispel and expiration behavior. Do not extend this finding automatically
to controlled elementals, daemons, golems or other summoned creatures.

Research documentation only. No gameplay code changed in this pass.


## Controlled-summon extension: Daemon and Summon Creature

Follow-up checked November 8, 2026. These spells require their own policy
decision rather than automatically inheriting the BS/EV conclusion.

At the same pinned demo revision, sumdaem.m calls Q4MG in sumdaembase.m;
summoncr.m calls Q4MF in summoncrbase.m. Both run the shared spell guards,
find a nearby location, call createGlobalNPCAt, attach destcrea and schedule
expiration. Daemon uses template 0x0255; Summon Creature chooses a random
animal template from its list.

Both then call Q558(creature, user, 100, 1), defined in spelskil.m.
This helper sets myBoss to a list containing the caster, myLoyalty to 100,
isPet to 1, configures movement/behaviors and attaches pet. It returns 1.
No existing-summon count, weighted follower total or admission comparison
appears in these inspected creation or ownership helpers.

The inspected pet.m creation trigger initializes pet sound state and
behaviors and, when myBoss exists, notoriety. It contains no summon count
admission test. This does not constitute an exhaustive audit of every
inherited pet/guard function or native script-attachment behavior.

Current ServUO spell prechecks:
* SummonDaemon.CheckCast: Followers + (Core.SE ? 4 : 5) > FollowersMax.
  Under LBR the added cost is five.
* SummonCreature.CheckCast: Followers + 2 > FollowersMax.
Both invoke SpellHelper.Summon, unlike the two BS/EV spell callers.
Their full completion/control-assignment admission paths still require
a separate implementation trace before editing.

The official P16 eighth-circle table gives Daemon five follower slots and
states that the spells were modified to work with the new system. It is
positive evidence that the current weighted model belongs to the later
system; it does not specify the exact former count allowance.
https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/

Working assessment: the inspected demo evidence also favors no explicit
count cap for these controlled summons. The 1998-to-April-2002 temporal
gap remains. Do not infer either a one-daemon restriction or a five-summon
allowance from modern costs. Extend the reconciliation to controlled
Magery summons, including elementals, while preserving their ownership,
commands, attribution and expiration. No gameplay code changed.
