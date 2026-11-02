# Other Renaissance shards: golem source and documentation audit

Checked November 2, 2026 UTC (November 1 America/New_York). Research only.
Project target remains LBR at the end of April 22, 2002.

## Result

Recovered and inspected six public source snapshots across Forever, Divinity,
Defiance and Rebirth. Also retrieved the current main-namespace wiki text for
UO Renaissance, Lost Lands and Phoenix: 504 pages in total. Checked forum
indexes, relevant complete discussions, and current published patch material.
Coverage and access failures are recorded below; this is not a claim to have
read every forum post or verified any live shard deployment.

The important correction is **UO Forever did have craftable golems by May
2017**. Our earlier report stopped at a statement that the author retracted on
the next page. The public code also exposes a repeated four-slot creation
check paired with a three-slot creature. These are later emulator rules;
they do not resolve the original pre-P16 concurrency maximum.

## Source snapshots and effective default limits

All numbers below describe inspected code, with the default maximum of five
weighted followers, no other followers, and no saved/staff overrides. They
are static deductions, not runtime tests. Creation, transfer and stable
claim are separate paths.

| Snapshot | Creation admission | Golem accounting | Default concurrent golems | Historical value |
|---|---|---|---|---|
| Forever public archive | Existing followers + 4 must fit | 3 slots | One | Later shard implementation; archive not authenticated as a live release |
| Divinity Community Edition | Existing followers + 4 must fit | 4 slots | One | Pre-UOR shard family with retained later scripts; not Hybrid |
| Defiance pre-1.10 | Existing followers + 4 must fit | 4 slots | One | Older implementation, not an OSI specification |
| Defiance 1.10 | Existing followers + 4 must fit | 3 slots | One | Same creation source as Forever and Divinity |
| Defiance ModernUO port | Existing followers + 4 must fit | 3 slots | One | Modern port of an emulator implementation |
| Rebirth v3 release | No working construction branch | No ordinary Golem class recovered | Not derivable/applicable | Explicitly targets 1997–1998; retained assembly script is incomplete |

Pins, repositories, and scope:

* [Forever archive](https://github.com/zerodownedArchives/UltimaOnlineForever/tree/b2fe1a13c96f52d79aa84da5ceb09214c79b1ad5)
  — `b2fe1a13c96f52d79aa84da5ceb09214c79b1ad5`, commit dated May 16, 2015.
  Source root: `UOForeverFULL/uofcodeNEW - Copy/`.
* [Divinity](https://github.com/SakuraUO/UOGamers_Divinity/tree/9dacbd1f563e2e11244f30352c41a5550359f3da)
  — `9dacbd1f563e2e11244f30352c41a5550359f3da`, March 28, 2014.
* [Defiance pre-1.10](https://github.com/greeduomacro/DefianceUO-Pre1.10/tree/dc760925ff916077b84d202e6d2fedb6d47b3d65)
  — `dc760925ff916077b84d202e6d2fedb6d47b3d65`, mirror commit February 15, 2016.
* [Defiance 1.10](https://github.com/greeduomacro/Defiance-UOR-1.10/tree/655381e72343b53fe981d98734fa40c468aac1a3)
  — `655381e72343b53fe981d98734fa40c468aac1a3`, February 15, 2016.
* [Defiance ModernUO port](https://github.com/3HMonkey/DefianceUOR/tree/9fa01e791740825a704f2fc8e81570fc3e430263)
  — `9fa01e791740825a704f2fc8e81570fc3e430263`, December 21, 2021.
* [Rebirth v3](https://github.com/Delphi79/RunUO-UOGamers-Rebirth-v3/tree/fc547dfde2156bbb87d0b28f75ae3ab757de072b)
  — `fc547dfde2156bbb87d0b28f75ae3ab757de072b`.
  The README identifies an authorized release representing September 20,
  2005, later adapted for newer cores/clients. That is the README's provenance
  claim; the public mirror is not itself a contemporary OSI source.

Repository dates are not dates when mechanics first became active. Source
presence also does not establish that components were obtainable or creatures
spawned on a particular live shard.

## UO Forever: correction, player evidence, source chain

Read both pages of [Tinker Golems](https://www.uoforum.com/threads/tinker-golems.48197/),
including [page two](https://www.uoforum.com/threads/tinker-golems.48197/page-2).

On May 31, 2017, post 20 says golems are absent except event creations. That
statement is explicitly corrected in post 24: a component had become a drop,
making construction possible. Post 22 supplies a firsthand report of making
one, transferring it to a character without Tinkering, and stabling it. Posts
22 and 26 identify three control slots. Later posts discuss a custom bonding
deed and inability to resurrect the result; those are shard-specific details.

A [December 22, 2015 sale](https://www.uoforum.com/threads/golem-400k.71878/)
already advertises a three-slot golem requiring no Taming. That establishes
an earlier owned/tradable example, not general craft availability in 2015.
Do not equate an event pet sale with normal production.

Inspected these files under the archive source root:

* `Shard/Scripts/Items/Misc/ClockworkAssembly.cs`
* `Shard/Scripts/Mobiles/Monsters/Misc/Melee/Golem.cs`
* `Core/Mobile.cs`
* `Shard/Scripts/Mobiles/BaseCreature.cs`
* `Shard/Scripts/Mobiles/AI/BaseAI.cs`
* `Shard/Scripts/Mobiles/Vendors/NPC/AnimalTrainer.cs`
* `Shard/Scripts/Engines/Craft/Core/Repair.cs`
* `Shard/Scripts/New/Mobiles/Hirables/BaseHire.cs`

The assembly requires 60 effective Tinkering, an assembly in the backpack,
one crystal, 50 iron, 50 bronze and five gears. Tinkering selects a stat scalar
from 0.6 to 1.0. Construction checks four free weighted slots before consuming
resources. It creates `Golem(true, scalar)` and assigns the control master.
The creature itself uses three slots; `Mobile` initializes the maximum to five.

`BaseCreature.SetControlMaster`, accepted trade in `BaseAI`, and stable claim
each enforce the actual creature slots against that same follower budget.
Control and summon masters contribute to the shared counter. Hirelings also
use master assignment, with an additional one-hireling restriction in this
snapshot. This is a weighted shared pool, not a separate historical golem cap.

Consequently an existing golem blocks another creation/transfer/claim at the
default maximum. With two one-slot pets, a player could fit a three-slot
transferred or claimed golem, but construction's four-slot precheck rejects
the equivalent creation. This asymmetry is an implementation finding.

The constructor's `summoned` argument changes appearance/loot behavior;
it does not set `BaseCreature.Summoned`. Thus the ordinary stable prohibition
on summoned creatures does not itself prohibit constructed golems. Owned,
living golems can pass the inspected trainer checks; claims still need space.
They occupy ordinary stable entries. Capacity uses the later 2–5 base formula
plus GM-and-higher skill bonuses, giving eight at three GM pet skills.
That is not our project's pre-P16 stable formula.

The golem prohibits normal bonding, deletes on release, and forwards received
damage into a nearby master's mana, then health. Repair has a 12-second action
cooldown. Neither the inspected construction handler nor its serialization
contains the historical five-per-day production counter. The repair timer is
not a construction quota. Command control returns certainty for the golem's
default minimum taming difficulty in this archive; construction Tinkering is
not imposed on a recipient through that command check.

Current [Tinkering documentation](https://uoforever.com/wiki/index.php/Tinkering)
does not specify a golem concurrency rule. Current search hits also include
seasonal enemy golems and the [January 2026 Mud Golem polymorph form](https://uoforever.com/2026/01/19/patch-notes-version-9-1-0-0/).
Neither should be mistaken for the crafted pet. Direct wiki/API and site
search requests returned 406, although rendered pages and selected forum
threads were retrievable. We have not certified the live 2026 crafting rule.

## Defiance: older and newer snapshots

Inspected assembly, golem, core `Mobile`, `BaseCreature`, trainer, and accepted
transfer handling across the Defiance snapshots. For pre-1.10, the core is
`SunUO/Mobile.cs` and creature base is
`Scripts/Mobiles/AI/Creature/BaseCreature.cs`. Version 1.10 uses
`Server/Mobile.cs`, `Scripts/Mobiles/BaseCreature.cs`, and
`Scripts/Mobiles/AI/BaseAI.cs`. The ModernUO port uses
`Projects/Server/Mobiles/Mobile.cs` and corresponding files beneath
`Projects/UOContent/`.

The pre-1.10 golem uses four slots. Version 1.10 and the ModernUO port use
three. All three inspected creation handlers still require four free slots;
core default maxima are five. Master assignment, transfer and claim enforce
weighted capacity. No separate pre-P16 golem allowance or daily-production
counter appears in these creation handlers. Their constructors likewise do
not mark the crafted variant as a timed summon. Later snapshots explicitly
prohibit ordinary bonding and delete released golems.

These multiple versions explain how a shard can retain a four-slot creation
restriction after changing the creature's slot charge. They do not document
why OSI chose any rule, or establish when an OSI rule changed. The 2021
ModernUO repository should not be presented as recovered original Defiance
server code from 2001–2002.

## Divinity and Rebirth: do not substitute them for Hybrid

Divinity's README calls it the Community Edition. It retains the four-slot
assembly/golem implementation, a core maximum of five, and a player load path
setting that maximum to five. Master assignment, accepted trade, and claim
check the shared weighted budget. The trainer accepts qualifying owned,
living, nonsummoned golems. The creature disables ordinary bonding and deletes
on release. Construction contains no five-per-day quota.

Its command probability is customized rather than simply the stock early
minimum-Taming shortcut: loyalty affects the result, and nearby owned pets
with high minimum Taming can reduce command probability. That is another
reason not to transplant this shard's broader pet system as an era authority.

Rebirth's release README explicitly targets late 1997/early 1998 and warns
that unauthentic standard scripts remain in the package. Its assembly file
has an empty construction-success branch and mismatched resource type/count
arrays. The ordinary `Golem` creature class was not recovered. This is not
evidence that a functional, uncapped golem system existed there. In particular,
removal of a follower precheck from an incomplete handler proves no such rule.

Hybrid remains a separate case. Searched public repository names/descriptions
for UOGamers/Hybrid and followed the current site's published material.
The [current patch index](https://www.uogamers.com/patch-notes/) linked 23 patch
pages; all 23 were retrieved and searched for golem, clockwork, follower and
Tinkering references, with no matches. No attributable public Hybrid server
source was located in this pass. The Rebirth/Divinity releases cannot fill
that gap merely because their names share UOGamers.

## Shared code is one implementation family

The **entire** ClockworkAssembly files in Forever, Divinity and Defiance 1.10
are identical after normalizing line endings. Their normalized UTF-8 SHA-256:

`dc1de02ecbcdb26aa4e2cd8394f9530a41c494655806756f43e81d72d6fb2185`

The publicly fetched `runuo/runuo` version has the same file content after
line-ending normalization as well. This establishes a shared implementation,
not three independently researched confirmations of the original OSI rule.
Pinning and comparing these sources was more useful than counting shard names.

## UO Renaissance proper

Retrieved all **218 main-namespace pages** returned by the current wiki's
allpages API, then retrieved their revision text. No failed batches. Search
terms were golem, clockwork and power crystal, followed by contextual review.

* [Tinkering](https://uorenn.com/wiki/index.php/Tinkering) gives tools/training,
  not a crafted-golem rule.
* `Carpentry & Tinkering` generically mentions clockwork assemblies among
  components. That is not proof the OSI pet can be constructed or commanded.
* [Bestiary](https://uorenn.com/wiki/index.php/Bestiary), the consolidated
  [patch archive](https://uorenn.com/wiki/index.php/Patch_Notes_Archive),
  Ancient MIB guides and Halloween guides describe ore/gem/laborer/event
  enemies. These do not establish a pet cap.
* The Earth Elemental spell's prose uses the word golem descriptively;
  it is not the Tinkering construct.

The previous July 2012 five-to-eight follower change remains evidence of a
deliberate shard customization, not the OSI rule at our cutoff. The old
`uorforum.com` host still returned 503. Current forum indexes were retrieved
using raw XenForo query URLs, avoiding the web renderer's malformed query
normalization. Traversed 187 current forum index URLs, exhausting discovered
index pagination without retrieval errors. Deduplicating the thread URLs by
XenForo thread ID yielded 1,005 threads. Their URL title slugs contained no
golem or Tinkering thread; selected pet/hireling/bonding discussions supplied
no crafted-golem limit. This is title screening, not a full-body search of
all 1,005 threads. The indexed 2012 bonding patch confirms deliberately
custom pet rules, including special food and a one-bond-per-character rule.
Public repository searches found client/tool/macro projects, but no verified
server implementation for this shard. Do not label a launcher as server code.

## Lost Lands

The site's [own era statement](https://www.uolostlands.com/pages/about/)
targets T2A/1999, with deliberate customizations. Its
[Animal Taming page](https://uolostlands.com/wiki/index.php?title=Animal_Taming)
describes unlimited ordinary pets. This is useful comparative evidence for
that shard's pet model, but predates the October 2001 golem feature.

Retrieved all **119 current main-namespace wiki pages**, with no failed
batches. No golem, clockwork or power-crystal references were found in their
revision text. The Tinkering craft list does not supply a golem rule.

Traversed the public MyBB archive's **88 forum index pages**, exhausting the
discovered index queue without errors, and collected **1,317 distinct thread
URLs/titles**. No title contained golem. Read all 11 titles matching the
targeted Tinkering/follower/hireling/summoning terms, then all seven pages of
[Tamer Update — Limit number of followers](https://uolostlands.com/forum/archive/index.php?thread-633.html).
This is title screening plus selected body reading, not a full-body search
of every archived thread.

That December 2015–January 2016 discussion reports very large dragon/wyrm
groups and proposes limits. On December 16, Jack discusses changes to guard
response and control/loyalty, rather than announcing an adopted numeric cap.
Later replies continue advocating a cap. A suggestion is not a shipped rule,
and nothing in the complete discussion supplies a golem maximum. No public
Lost Lands server source was located by the repository searches.

## Phoenix, New Renaissance and Revealed

**Phoenix:** Retrieved all **167 main-namespace wiki pages**, with no failed
batches. Positive golem references concern flesh-golem enemies/spawns, not a
clockwork pet. Its [Ruleset](https://uo-phoenix.com/wiki/index.php/Ruleset)
explicitly mixes UOR with later pets, scroll requirements and custom content;
its strongest pets consume five control slots. It also mentions 20 initial
"taming slots" without establishing a crafted-golem allowance. Do not silently
equate that number with the active weighted follower maximum. No attributable
server repository was found in this pass.

**New Renaissance:** Followed the [homepage](https://newrenaissanceuo.com/)
to its actual GitHub download. It is `xzCad/New-Renaissance-Launcher`, an
installer/patcher project, not recovered server logic. The linked wiki is
`uonewrenaissance.fandom.com`; direct retrieval returned HTTP 402, and the
web renderer could not open it. A separate similarly named Fandom wiki exists
in search results; do not merge their identities or claim complete coverage.
No exact golem cap or daily quota was recovered.

**Revealed:** The [homepage](https://uorevealed.com/) describes Renaissance
PvP with custom pets, powerscrolls, events and other changes. Followed its
actual wiki, forum and news links. The renderer failed with 500 responses;
direct requests returned 406. Search did not recover a golem rule or an
attributable server repository. This remains an access gap, not proof of
absence.

## Consequence for our reconstruction

No recovered shard source or documentation in this pass establishes the
exact OSI pre-P16 golem concurrency ceiling, mixed-follower rule, or the
scope/persistence details of its production quota.

The stronger existing period evidence remains:

* [March–April 2002 firsthand hunting account](https://archives.jeuxonline.info/fils/76309.html):
  at least two active golems, not an exact maximum.
* [November 2001 five-owned account](https://archives.jeuxonline.info/fils/14366.html):
  ownership/world storage, not five simultaneously commanded combat pets.
* Calandryll's November 2, 2001 five-creations-per-approximately-24-hours
  statement in the [archived Stratics news](https://web.archive.org/web/20130331055950/http://uo.stratics.com/news/arc10-2001.shtml):
  production, not simultaneous active control.

Retain the documented distinction between known period behavior and our
reconstruction choices. Do not restore a later one-golem weighted-slot limit
because multiple descendants share it, and do not infer unlimited original
golems because a modern shard omits them from its documentation. The existing
golem change and its explicitly unimplemented daily production quota remain
as recorded in [pre-p16-golems.md](pre-p16-golems.md). No gameplay changes or
runtime validation are included in this research pass.

## Retrieval method

Public Git clones were pinned before inspection. Used `rg` to locate handlers
and traced full creation/master-assignment/transfer/stable paths. Compared
normalized file hashes; no shard executables or client installers were run.
Wiki coverage used read-only `action=query&list=allpages&aplimit=500`, followed
by batched `prop=revisions&rvprop=content&rvslots=main` requests. Wiki counts
exclude other namespaces and deleted/private content. Forum crawling followed
public index/pagination links and selected thread pages, without signing in
or submitting posts. No staff contact was made.
