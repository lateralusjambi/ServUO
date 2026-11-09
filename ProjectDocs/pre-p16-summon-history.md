# Pre-P16 summon limits: intervening history and emulator comparison

Research completed November 9, 2026 UTC (November 8 in America/New_York).
Target: LBR through April 22, 2002. Research only; no gameplay changes.

## Finding and decision

The intervening material inspected in this pass supplies no positive
pre-cutoff rule imposing a per-caster Magery summon cap. The available
implementation evidence favors no explicit count cap. This is a supported
reconstruction, not proof that every production build between 1998 and
2002 had identical behavior.

The strongest affirmative historical statement remains the P16 introduction
of the shared five-slot model and weighted eighth-circle summons. The
later freeshard source bank is divided between retaining that model,
changing its weights, and removing it. Counting those repositories as
independent historical votes would be misleading.

Recommendation: reconcile all ordinary Magery summons under the pre-P16
configuration, with no invented five-summon replacement. This includes
Summon Creature, the four elementals, Summon Daemon, BS and EV.
Keep controlled summons controllable and keep BS/EV autonomous. Preserve
caster attribution, spell costs, placement restrictions and expiration.
Resolve admission/accounting consistently, including mixed pets/golems/
summons and checks during completion. Other spell properties need their
own evidence review.

Terminology: the native artifact previously inspected was the 1998 UO
demo executable, not the 1997 public beta. RunUO beta release numbers are
also unrelated to OSI publish numbers.

## Official patch-history pass

Retrieved every distinct child-page URL linked from the current official
1998–2002 archive indexes: 60 pages, zero retrieval failures.
Counts: 1998 22; 1999 9; 2000 8; 2001 7; 2002 14.
This includes pre-demo material and later P16 parts as boundary checks.
Pages can consolidate several dated updates, so 60 pages is not 60
individual production patches.

Searched article text for summon, blade spirit, energy vort and follower;
reviewed the resulting passages and nearby limit/number language.
No explicit introduction of a pre-April-23-2002 per-caster count cap was
found. This is a negative finding within the linked archive, not a claim
that all original announcements, hotfixes or undocumented changes survive.

Archive indexes:
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/1998-2/
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/1999-2/
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2000-2/
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2001-2/
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/

| Actual update | Relevant result | Effect on cap question |
|---|---|---|
| February 12, 1998 | Longer creature-summoning ritual | Casting throughput, not a numeric allowance |
| June 11, 1998 | Dispel/Mass Dispel affect summons, including BS/EV | Removal mechanic, not a count rule |
| September 16, 1998 | NPC casters dispel combat summons | Counterplay, not admission |
| November 23, 1998 | BS tower-casting fix; BS/EV aggression change | Placement/AI, not a stated cap |
| January 19, 1999 | Blocked EV placement no longer consumes reagents | Placement/refund rule |
| February 26, 1999 | Summon and hireling obedience exempted from Taming skill | Positive controlled-summon rule to preserve |
| March 19, 1999 | BS/EV reputation consequences revised | Attribution, not allowance |
| March 28, 1999 | Elemental summoning in towns disabled | Location restriction |
| May 25, 1999 | Ownership transfer of summoned creatures disabled | Positive restriction; do not copy the demo's older transfer behavior |
| December 22, 1999 | BS resistance-training exploit fixed | Skill gain, not a count rule |
| January 24, 2000 | Banning summoned creatures to attack passersby exploit fixed | Housing/attribution |
| P5 update material | Summoned elementals/daemons brought closer to normal counterparts | Broad balance wording; does not specify count |
| July 24, 2001 | BS/EV made less processor-intensive | Possible implementation change, but no cap stated; cannot assume it introduced one |
| January 2002 P15/P15.5 | No count rule recovered in inspected article text | Negative evidence only |
| July 23, 2002 P16 taming changes | Shared five-slot model; elemental costs 2/2/3/4 and Daemon 5 | Explicit later boundary |

Primary pages for the important middle changes:
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/1999-2/1999-03-24th-february/
  (February 26 and March 19 are dated updates within this page.)
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/1999-2/1999-04-28th-march/
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/1999-2/1999-07-25th-may/
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2000-2/2000-publish-05-27th-april/
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2001-2/2001-publish-12-24th-july/
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/

Do not assign every item within a consolidated page to its heading date.
Our cutoff excludes all P16 features even though these particular follower
changes are documented in its July portion.

## Earlier emulator bank

These are emulator implementations, not OSI server source. Absence of a
check in a helper does not establish exhaustive absence in all callers,
script hooks and configurations.

| Source actually inspected | Inspected admission/ownership behavior | Evidence weight |
|---|---|---|
| UOX Build19 archive | magic.cpp SummonMonster, lines 328–536, allocates/initializes, sets owner for controlled summons and schedules expiration; no caster-count threshold in this routine | Supporting older emulator lineage; no independently authenticated release date established |
| WolfPack 11.3 archive | magic.cpp SummonMonster, lines 409–636, has closely related allocation/owner/expiration code and no caster-count threshold in this routine | Same UOX ancestry, not independent corroboration |
| UOX3 site-backup source | work/dev/magic.cpp summonmonster around line 711 onward uses the earlier pattern; no threshold found in the inspected helper | Another UOX-family comparison |
| Sphere 0.52 source snapshot | Spell_Summon in ccharskill.cpp creates a temporary summon; excludes BS/EV from NPC_SetOwner; inspected NPC_SetOwner adds ownership without a numeric allowance check | Separate emulator lineage; snapshot identity is known, exact release provenance is not |
| Sphere 55j snapshot | Spell_Summon creates timed summon and calls NPC_PetSetOwner; inspected owner setter has no numeric admission test | Same Sphere lineage |
| Sphere 0.55.402 snapshot | Creation and owner setter likewise lack a count check in the inspected routines | This directory identifies a Torfo/Linux-patched variant; not a pristine period OSI surrogate |
| RunUO Beta 12 | BaseCreature.Summon and SetControlMaster enforce weighted Followers + ControlSlots against FollowersMax | Source-file ZIP timestamp December 24, 2002: later than P16, not evidence of April's rule |
| RunUO Beta 16 | Same shared admission pattern | BaseCreature ZIP timestamp February 5, 2003: later than P16 |

In UOX/Wolfpack, setserial(creature, caster, 5) selects the ownership field.
The literal 5 there is not a maximum-five-summons comparison. Both routines
exclude BS/EV from ordinary ownership while allowing the other summon kinds
to be assigned an owner.

Wolfpack's update notes explicitly describe boosting EV, BS and Daemon.
That customization is another reason not to import its combat values as
OSI facts. The UOX helper includes a February 17, 2000 revision comment;
a comment alone does not date/authenticate the entire supplied archive.

Sphere source:
https://github.com/Sphereserver/Source-Archive/tree/cf4a7cc56df3afc18cdd41379fa704bbed323e82

Inspected paths:
* 0.52/GraySvr/ccharskill.cpp
* 0.52/GraySvr/CCharNPCPet.cpp
* 55j/ccharspell.cpp
* 55j/CCharNPCPet.cpp
* 0.55.402/graysvr/CCharSpell.cpp
* 0.55.402/graysvr/CCharNPCPet.cpp

The later Sphere community script pack's spell definitions delegate summon
behavior to native handling. Their lack of an explicit script check cannot
establish a native cap policy. That pack identifies a January 2013 spell
file; it is not period testimony.
https://github.com/JulianUO/sphere-community-pack/tree/0ade636be2185a48d322a2ec94043b8e2cdcf353

RunUO Beta 12 public archive:
https://github.com/Archivist-Taryn/runuo.net-archive/blob/a40a633bb71751fa03e2c5b7fc4c8ec2e7a092a4/Downloads.RunUO.net/Emulators/runuo/RunUO-Beta-12.zip

Also recovered Beta 02, but it contains an InstallShield self-extracting
installer rather than directly readable scripts. The outer CAB listing
was examined without executing it; no Beta 02 gameplay conclusion is
drawn. Its container timestamp is not accepted as its release date.

### Supplied archive identities

| Archive | SHA-256 |
|---|---|
| UOX (Build19 Source).zip | 7ac5c6f2c578ca16742f86f85aca73d836f5492030d45cc86ea2d983f7aaa9b3 |
| WolfPack (11.3 Source).zip | e7cff0801a1f78fe6c3754abc65c4e4848b612b81c7ce5b67fc9bc5e8e7fbda3 |
| UOX3 site backup with Source.zip | 55e9a7c340ea87c9814797fb9043588eba9f1e95afd33df4bfb838c81d16186a |
| RunUO-Beta-12.zip | d6408e6699c46b5718d95924052adb0b4ef1829a74abe1936a82da800d8f725c |
| RunUO-Beta-16.zip | e73a1ad3f013d5789e0aec9f516848f08d4f7b0c92f1bd929192ee90c13d82d7 |
| RunUO-Beta-02.zip | 90d6a0784d8899f54bb7dfacdcd2a942b06312994a55859fbc021b95f7bc1ad5 |

ZIP timestamps are contextual metadata, not independently verified release
dates. Archive contents were inspected statically, not run as live servers.

## Freeshard comparison

Costs below describe the inspected spell prechecks, not a claim that each
deployment uses a five-slot total or that completion always charges the
same amount. A complete gameplay audit must also inspect actual creature
ControlSlots and shared completion/control-assignment checks.

| Pinned implementation | BS / EV spell precheck | Daemon precheck | Summon Creature precheck | Interpretation |
|---|---|---|---|---|
| Defiance pre-1.10 | +1 / +1 | +5 | No spell-level follower precheck; shared BaseCreature limit exists | Retains later RunUO weighted model |
| Defiance 1.10 | +1 or +2 under SE | +5 or +4 under SE | +2; creature slots explicitly set to 2 | Later-era branches preserved |
| Defiance ModernUO port | Era-dependent checks | +5 or +4 under SE | +2 | Later port, not independent historical testimony |
| Forever archive | +1 or +2 by caster era | +4 | +2 | Customized later model |
| Divinity | BS era-dependent; EV +2 | +2 | +2; creature slots set to 2 | Customized weights, not period cap proof |
| Rebirth v3 | No follower admission check in inspected spell paths | No follower admission check in inspected spell path | No follower admission check in inspected spell path | Shared Summon/SetControlMaster checks commented out; ControlSlots getter returns 0 |
| Angel Island newer source | BS +1 | +4 | +2; includes custom Horde Minion branch | Preserves/modifies later systems |
| Grimoric RunUO.T2A mirror | +1 / +1 | +5 | +2 | T2A repository name does not make these later inherited checks period-correct |

Repositories and pins:

* Defiance pre-1.10: https://github.com/greeduomacro/DefianceUO-Pre1.10/tree/dc760925ff916077b84d202e6d2fedb6d47b3d65
* Defiance 1.10: https://github.com/greeduomacro/Defiance-UOR-1.10/tree/655381e72343b53fe981d98734fa40c468aac1a3
* Modern port: https://github.com/3HMonkey/DefianceUOR/tree/9fa01e791740825a704f2fc8e81570fc3e430263
* Forever: https://github.com/zerodownedArchives/UltimaOnlineForever/tree/b2fe1a13c96f52d79aa84da5ceb09214c79b1ad5
* Divinity: https://github.com/SakuraUO/UOGamers_Divinity/tree/9dacbd1f563e2e11244f30352c41a5550359f3da
* Rebirth: https://github.com/Delphi79/RunUO-UOGamers-Rebirth-v3/tree/fc547dfde2156bbb87d0b28f75ae3ab757de072b
* Angel Island: https://github.com/Luke-Tomasello/Angel-Island-Server/tree/7cbbf405aa4182b90a6fb47f48bbd93c75f06792
* T2A mirror: https://github.com/Grimoric/RunUO.T2A/tree/3e99cd87dfcf4e019c7c25e2fb4f20b379014702

Read spell files at Scripts/Spells/Fifth/{BladeSpirits,SummonCreature}.cs
and Scripts/Spells/Eighth/{EnergyVortex,SummonDaemon}.cs where present.
Angel Island uses lower-case scripts; the ModernUO port places these under
Projects/UOContent. Shared BaseCreature paths vary by release.

Rebirth is a useful explicit no-weighted-cap implementation, but it is
still a later shard decision, not OSI proof. Its ControlSlots getter
returning zero and removed admission checks matter more than an isolated
constructor assignment elsewhere.

UO Renaissance's own patch archive records a five-to-eight follower-limit
change under its July 2012 B9 notes. It even describes this as an era
deviation, but supplies no contemporary OSI citation for its claimed old
baseline. This positively establishes shard customization; it cannot
resolve the pre-April-2002 OSI summon allowance.
https://uorenn.com/wiki/index.php/Patch_Notes_Archive

The UOSA Magery page and indexed forum leads checked in this pass yielded
no explicit, dated OSI per-caster-cap statement. The Grimoric mirror above
is not being represented as the live UO Second Age server's official source.

## Period player evidence and remaining gap

The January 3, 2002 one-mage BS training advice remains the best pre-cutoff
count lead recovered:
https://groups.google.com/g/rec.games.computer.ultima.online/c/Exj92TNtxXk

It describes successive casts and mentions five to seven, but is not an
explicit limit rule or a recorded sixth-cast experiment. Repeated quoted
replies do not add independent corroboration. Mana provides a plausible
explanation for the practical count; it is not proof of a cap or no cap.

Searches for additional 1999–2002 summon/daemon/elemental limits did not
recover a stronger dated one-caster report in this pass. The already
retrieved February/April 2002 Stratics guide pages likewise do not state
the limit. Do not promote generic plural references, later shard play,
admin-spawned armies or unverified screenshots into count evidence.

What is now better established:
1. The linked official patch record does not supply an intervening cap.
2. The P16 weighted model is an affirmative later change.
3. At least two non-RunUO emulator lineages inspected lack a count check
   in relevant older creation/ownership helpers.
4. Much apparent freeshard agreement is inherited later RunUO behavior.
5. Controlled-summon obedience and non-transferability have positive
   period documentation and should survive admission changes.

What remains unproved: an explicit OSI statement or unambiguous ordinary
player test of the exact active-summon allowance immediately before the
cutoff. We have enough to make a documented reconstruction decision; we
do not have enough to label unlimited summons an incontrovertible fact.

See [the initial code and native-demo audit](pre-p16-summon-reconciliation.md).


## Wayback and preserved Stratics follow-up

Checked November 8, 2026, America/New_York. No gameplay changes.

### Recovered period Mage forum FAQ

The December 5, 2001 Mage board index exposes two pinned threads:
https://web.archive.org/web/20011205101944/http://boards.stratics.com:80/php-bin/uo/postlist.php?Cat=&Board=uomage

CDX lookup found successful September 1, 2001 captures for both. Retrieved
the HTML directly through Wayback's id_ replay; the ordinary web retrieval
service failed to open the normalized forum URL, but direct HTTP succeeded.

The substantive Mage FAQ is posted by moderator Kaltes Feuer with displayed
post date July 9, 2001, 10:44 PM; the inspected capture is September 1, 2001:
https://web.archive.org/web/20010901132450/http://boards.stratics.com:80/php-bin/uo/showflat.php?Cat=2&Board=uomage&Number=1994176&page=0&view=collapsed&sb=5&o=7&part=

Raw retrieved HTML SHA-256:
1836587486600def29cb10fa0804e935c63c18f904d221235be181d901827e05
Retrieved size: 73,091 bytes. Extracted text: 53,751 characters.

Its resistance-training essay is credited to MasterCraft. At skill 65–70,
it describes two GM mage friends summoning daemons while two warrior
friends heal the trainee, discussing approximately two or three daemons.
At skill 85–93, it discusses needing eight or more daemons simultaneously,
says the author tried the approach, and recommends wisps instead.

This is positive pre-cutoff player-guide evidence that groups of summoned
daemons were used/contemplated for training. It is stronger than an
undated modern shard guide. It is not an explicit unlimited-summon rule.
The text does not unambiguously assign every daemon to one caster or
specify how many mages participated in the larger setup. The trainee is
also advised earlier in the essay to have high Magery. Do not assume the
two listed mage friends were necessarily the only possible casters, or
claim the passage proves eight daemons per caster.

The FAQ's full text was searched for summon, daemon, limit, maximum and
control-slot language. No explicit per-caster maximum was found.
It also contains BS/EV target-selection material worth a separate future
audit; no targeting changes are made from it here.

The other captured pinned post is a June 12, 2001 moderator notice
directing users to read the FAQ; it provides no summon allowance:
https://web.archive.org/web/20010901142757/http://boards.stratics.com:80/php-bin/uo/showflat.php?Cat=2&Board=uomage&Number=1862174&page=0&view=collapsed&sb=5&o=7&part=

### Other recovered pages and archive scope

* April 5, 2002 Mage profession guide: direct retrieval succeeded.
  No explicit cap found. Some town-casting advice may reflect older
  contributed text, illustrating why capture date does not authenticate
  every statement as current for that day.
  https://web.archive.org/web/20020405164639/http://uo.stratics.com/content/professions/mage.shtml
* April 8, 2002 spell table: direct retrieval succeeded; no count rule found.
  https://web.archive.org/web/20020408154007/http://uo.stratics.com/content/basics/spellsn.shtml
* UO House of Commons developer chats for November 2, 2001 and March 7,
  2002 were retrieved. The inspected text supplied no summon-cap answer.
  https://web.archive.org/web/20011225225538/http://uohoc.stratics.com:80/logs/pub011102.shtml
  https://web.archive.org/web/20020317121319/http://uohoc.stratics.com:80/logs/pub020307.shtml
* October 2001 HoC and February 2002 pet-reputation replay attempts
  returned HTTP 429. Those are retrieval gaps, not negative content findings.
* The Mage board CDX query returned 14 capture records before the cutoff;
  not every listed board page or linked discussion was fetched in this pass.
* The HoC CDX query returned 100 rows at the requested limit, including
  assets and duplicate .htm/.shtml paths. That result is not a complete
  inventory of all HoC transcripts.

A genuine selected Stratics article mirror exists at:
https://uorenn.com/stratics/
Its navigation covers weapons/armor/combat, resist/Eval, animal tables and
buildings. The weapons page labels collection date October 13, 1999;
the resist/Eval page labels March 6, 2000. These are mirror provenance
claims, not independently authenticated capture timestamps. This is not
a recovered complete Stratics forum database or a complete Magery archive.
https://uorenn.com/stratics/resist_eval.html

Internet Archive catalog search for stratics returned eight items. The
similarly titled cached-website items resolve to 2019 DocumentCloud PDF
records, not a downloadable UO Stratics site/forum backup established by
this pass. Do not promote their titles into a backup discovery.

Updated assessment: we now have an actual pre-P16 Stratics Mage forum FAQ
with simultaneous-daemon discussion. The outstanding per-caster attribution
gap remains. A full Stratics forum backup has not been located.


## Focused Mage-thread pass and one-caster daemon evidence

Checked November 8, 2026, America/New_York. No gameplay changes.

### Batlin's essay: original dated capture recovered

A preserved Stratics parrying essay by Batlin/Devlin of BaH, Napa Valley,
describes reaching GM Parry by fighting summoned daemons in a tower.
The author identifies one female guildmate training Magery as the person
summoning, and says he usually fought two daemons at once. The surrounding
text repeatedly discusses simultaneous two-daemon combat and a summoning
friend healing the trainee.

Mirror:
https://uorenn.com/stratics/parry_essay.html

The mirror labels its material March 6, 2000, but this pass independently
recovered the original Stratics document from Wayback:
https://web.archive.org/web/20000129115406/http://uo.stratics.com/parryessay.htm

This establishes that the account existed by January 29, 2000, rather
than relying solely on the modern mirror's collection-date label.
The original publication date has not been established.

Provenance cross-check:
* The request for March 6, 2000 with the no-port original URL redirected
  to the January 29, 2000 id_ replay above.
* The returned HTML was 33,030 bytes.
* SHA-256: 123785bcceea1117678e216972362551eb94c2172ad4ae883e8ec4bc1fab6852
* Its base32 SHA-1 is DABDRMMWHUDET2FBHMY2SKESOOBG74OY.
* That exactly matches CDX's payload digest for the January 29 capture.
  The content is therefore matched to that capture, not merely assigned
  the date requested in the first replay URL.
* Initial attempts using the :80 URL returned HTTP 429; their failures
  did not establish that the article was unavailable.
* The separate ordinary web retrieval service could not open this replay;
  direct HTTP retrieved the content and supplied the actual redirect URL.

Interpretation: strong contemporary player-account evidence for one mage
supplying at least two simultaneously active summoned daemons. It supports
rejecting the later one-daemon result of charging five slots per daemon
as a universal rule for that earlier period. The narrative is not an
ownership-property dump or controlled experiment, and does not explicitly
test a third daemon, a sixth summon, a higher ceiling, or April 2002
continuity. Do not turn a two-daemon report into proof of unlimited summons.
It also supports summoning ordinary creatures inside a house at that time,
but does not establish BS/EV house-placement rules.

### Forum recovery coverage

The CDX query for boards.stratics.com/php-bin/uo/showflat.php* with Mage
board URLs, successful responses, and dates through April 22, 2002 returned
61 digest-collapsed records. The analogous showthreaded query returned no
records. These are results within those URL/query families, not proof that
every possible alias, archive collection or migrated thread was searched.

Excluded the two already read FAQ records and ten records of the large
babble thread. Attempted the remaining 49 records:
* 27 HTML captures retrieved, covering 25 distinct thread IDs.
* 22 attempts returned HTTP 429; these remain unread retrieval gaps.
* Searched retrieved text for summon, daemon, vortex and blade-spirit
  language, then inspected the matching contexts.
* No explicit per-caster ceiling or unambiguous one-caster count above
  five was recovered from those retrieved discussions.
* Repeated forum signatures mentioning daemons were excluded as evidence.

A December 2001 training discussion recommends moving around while
summoning different elementals and daemons, but gives no simultaneous
count. It supplies no cap conclusion:
https://web.archive.org/web/20011225224123/http://boards.stratics.com:80/php-bin/uo/showflat.php?Board=uomage&Cat=&Number=2570490&o=7&page=0&part=&sb=5&view=collapsed

Also followed five more Mage board index captures. August 7, October 30,
December 25, 2001 and January 23, 2002 were retrieved; June 30, 2001 returned
429. Relevant titles exposed by the successful indexes included:
* How do pure mages hunt? — Number 2126647
* BS's and EV's, Liches and Lich Lords — Number 2122413
* Need info on pvm. — Number 2113866
* Lord Edrick attempts to answer all Forum questions in one shot! — Number 2470841
* I was out working on resist..... — Number 2619774
* Questions Re: Resist — Number 2608594
* Mage PvP FAQ — Number 2675936

A targeted CDX query for these and two additional candidate thread IDs,
extending capture dates through 2003, returned no records. The titles are
leads from archived indexes, not recovered thread bodies or testimony.

Useful successful index replays:
https://web.archive.org/web/20010807123702/http://boards.stratics.com:80/php-bin/uo/postlist.php?Cat=&Board=uomage
https://web.archive.org/web/20011030035720/http://boards.stratics.com:80/php-bin/uo/postlist.php?Cat=&Board=uomage
https://web.archive.org/web/20020123054232/http://boards.stratics.com:80/php-bin/uo/postlist.php?Cat=&Board=uomage&page=0&view=collapsed&sb=5&o=7

### Updated conclusion

The one-caster/multiple-daemon question now has a dated affirmative player
account. The exact upper-limit question remains open. Combined with the
demo trace, older emulator routines, January 2002 BS advice and explicit
P16 introduction of weighted summon costs, this strengthens the
no-explicit-cap reconstruction without converting it to a fully proved
April 2002 production-server rule.
