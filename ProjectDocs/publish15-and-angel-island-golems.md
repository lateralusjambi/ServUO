# Publish Fifteen and Angel Island golem follow-up

Research checked November 1, 2026. No gameplay changes.

## Publish Fifteen: site recovered, evidence quality checked

Directory entry:
https://github.com/felladrin/uo-is-not-dead/blob/master/readme.md

Site:
https://publishfifteen.pages.dev/
https://publishfifteen.pages.dev/shard.html
https://publishfifteen.pages.dev/origins.html
https://publishfifteen.pages.dev/connect.html
https://publishfifteen.pages.dev/forum.html
https://publishfifteen.pages.dev/js/forum.js

The web retrieval service continued to fail on these pages. Direct HTTP
retrieval with a normal browser User-Agent successfully returned the root
and linked pages. This was public-page retrieval, without authentication.

The site claims a January 9–April 22, 2002 rules window, RunUO as its stock
baseline, no weighted pet control slots, no bonding, Enticement rather than
Discordance, and no power scrolls. It advertises an Era Accuracy Spec v2.1,
66 documented deltas, and 63 completed items. These are claims on the site,
not independently verified implementation or historical facts.

The pages examined do not link a downloadable specification, source
repository, or individual primary citations supporting the advertised
ledger. Their actual href links lead principally to the same small set
of site pages, fonts, local assets and official client downloads. Several
footers explicitly label the site an internal demo.

The Tavern is a demonstrable browser-only forum mockup. Its public
forum.js defines hard-coded SEED threads and replies; displayed relative
dates are calculated from Date.now(). User-added threads/replies are
stored through localStorage, using P15.store in site.js. No network fetch,
XMLHttpRequest, WebSocket or EventSource call appears in forum.js.
We did not post anything.

Consequently, the apparent forum users and their claims about historical
sources, such as a March 2002 Stratics dueling guide, are seeded display
content, not recovered independent testimony. They must not enter our
historical evidence ledger as player experiences.

There are also presentation inconsistencies: the root describes an LBR-era
client baseline while Connect directs users to current official client art;
different pages give hostname and numeric-IP connection addresses; the
site's slot/no-bonding policy is general and supplies no separate golem
quota or mixed-follower analysis. These do not prove no game server exists.
We did not connect to the advertised server, create an account, or test it.

**Assessment:** unusually close stated target, but no independently
auditable implementation or cited research recovered. Downgrade from
high-priority historical source to an unverified project lead. Obtain an
actual spec/source publication before relying on its numbers.

## Angel Island: pinned public source, not a live-server verification

Newer public repository:
https://github.com/Luke-Tomasello/Angel-Island-Server
Pinned commit: 7cbbf405aa4182b90a6fb47f48bbd93c75f06792

2016 release repository:
https://github.com/Luke-Tomasello/Angel-Island-2016-FW472
Pinned commit: 3a8bed05559d0e3ea7d1a2a5e8d2a1b92fc13fba

The newer repository README identifies Angel Island and Siege Perilous
configurations; it is not evidence of the exact deployment currently used
by 2026 UO Seasons or Upheaval.

### Creation, slots and accounting

In the newer core:
* scripts/Items/misc/ClockworkAssembly.cs requires 60.0 effective Tinkering,
  a backpack assembly, 1 PowerCrystal, 50 IronIngot, 50 BronzeIngot, 5 Gears.
* It rejects creation if FollowerCount + (SiegeII_CFG ? 3 : 4) exceeds
  FollowersMax, before consuming resources.
* scripts/Mobiles/Monsters/misc/Melee/Golem.cs sets ControlSlots to the same
  SiegeII_CFG ? 3 : 4 value.
* scripts/Engines/AI/Creature/BaseCreature.cs adds ControlSlots to the
  owner's FollowerCount. The latter is weighted accounting despite its name.
* SetControlMaster independently enforces the same owner budget unless
  explicitly forced. BaseAI's accepted secure-transfer validation also
  checks recipient FollowerCount + ControlSlots against FollowersMax.
* Mobile's constructor chooses FollowersMax by ruleset: non-Siege-style
  uses 5; Siege-style with Siege II uses 8; other Siege-style uses 16.
  Therefore, with the matching standard configuration and no slot modifiers:
  Angel Island proper permits one four-slot golem within five slots;
  Siege II permits two three-slot golems within eight slots; other
  Siege-style configurations permit four four-slot golems within sixteen.
  These are static-code deductions for new normal characters, not observed
  client tests or guarantees about saved/overridden limits or live shards.
  Core.RuleSets.SiegeStyleRules() includes Siege and Mortalis configurations.
* The assembly handler contains no daily-production timer/quota. This is
  bounded to the inspected creation path, not proof against all external
  overrides or deployment customizations.

The 2016 release independently confirms:
Scripts/Items/Misc/ClockworkAssembly.cs checks Followers + 4 > FollowersMax,
and Scripts/Mobiles/Monsters/Misc/Melee/Golem.cs sets ControlSlots = 4.
This weighted treatment predates the 2023 Siege II adjustments.

### Other golem behavior in the newer core

Golem is explicitly nonbondable, deletes on release, and overrides
HasLoyalty to false. Default low MinTameSkill reaches the automatic-control
branch of BaseCreature.GetControlChance; ordinary taming skill is not
the golem's control requirement. Construction's bool parameter named
summoned does not itself mark BaseCreature.Summoned true.

OnDamage: a controlled/summoned golem draws damage from a nearby player
master's mana, then health, while the master is on the same map and within
20 tiles. Siege II halves that drain. If the master fails the proximity/map
condition, the handler issues Stop. The golem itself still takes the full
damage. This is an implementation detail, not independently established
era evidence.

AnimalTrainer and BaseCreature explicitly provide special golem/chicken
stable accounting, separate from ordinary pets, with custom stable
allowances. Claim admission still checks the weighted follower budget.
Do not import these custom allowances into our corrected 2–5 era stables.

### Availability depends on shard rules

Golem.GenerateLoot supplies PowerCrystals and ClockworkAssemblies under
AngelIslandRules(). In the separate AllShards branch it date-gates those
drops to PublishDate >= June 3, 2002. Its 2023 comments call those resources
out-of-era in that context. This differs from the October/November 2001
craftable-golem evidence already recovered for our project. Do not adopt
that cutoff uncritically or conflate the Angel Island and Siege paths.

## Implication for our reconstruction

Angel Island offers traceable comparative source but does not resolve the
pre-P16 active golem maximum. Its weighted implementation is inconsistent
with our recovered March 2002 firsthand report of two active golems if
applied with its normal five-slot cap. Retain the distinction between a
shard's chosen implementation and a contemporary OSI source. No change
to our recently committed golem reconstruction follows from this pass.
