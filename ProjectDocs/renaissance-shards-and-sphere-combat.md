# Renaissance shard golems and recovered Sphere combat sources

Research checked October 31, 2026. Research only; no ServUO gameplay changes.

## UO Renaissance shard

The previous emulator research included RunUO, Angel Island and other era
implementations, but had not completed a focused audit of uorenn.com.
The supplied forum currently exposes a newer, relatively small board.
Searches, its Tinkering wiki, bestiary, and its consolidated 2012–2020 patch
archive did not recover a rule for constructing or concurrently controlling
the OSI-style pet golem. Do not infer nonexistence from these negative searches.

Sources checked:

* https://uorenn.com/forum/index.php
* https://uorenn.com/wiki/index.php/Tinkering
* https://uorenn.com/wiki/index.php/Bestiary
* https://uorenn.com/wiki/index.php/Patch_Notes_Archive
* https://www.uorenaissance.com/info/Tinkering
* https://www.uorenaissance.com/info/OreGolem

The positive golem hits concern custom ore/gem/event monsters. The 2014
patch notes describe newly introduced Ore Golems and Ore Golem Lords as
mining enemies. Those are not crafted pets and supply no pet-control cap.
The old forum host uorforum.com returned 503 during direct retrieval.
Some web-tool forum links were normalized incorrectly and resolved to the
forum homepage; those responses were not treated as actual board coverage.

This shard also deliberately customizes follower rules: its July 2012
beta patch records increasing the follower limit from five to eight. Its
choice is a shard implementation, not a primary statement of 2001 OSI rules.

A separate Renaissance-style shard, UO Forever, has an explicit May 31,
2017 response by halygon in a proposed Tinker Golems thread: ordinary
golems were not then implemented apart from a few event creations.
https://www.uoforum.com/threads/tinker-golems.48197/
This supplies no missing historical concurrency limit. The thread contains
later-era recollections and proposed custom requirements, not a 2001 report.

## Khaos combat source recovery

In the supplied February 20, 2016 post Khaos says he intends to re-release
Ren/LBR combat code:
https://forum.spherecommunity.net/Thread-UO-Renaissance
In the March 2016 replies to SCP Break Down he identifies a released combat
system as his and Joe's work, used by Anarch Cassius:
https://forum.spherecommunity.net/Thread-SCP-Break-Down?pid=33964
That points to the Sphere Community Pack. Khaos's current SShare catalog
has nine uploads, but no standalone combat download was listed there.

Recovered the complete original Google Code source archive successfully:
https://storage.googleapis.com/google-code-archive-source/v2/code.google.com/sphere-community-pack/source-archive.zip
The archive is 7,203,921 bytes and its SHA-256 is
`4423ced6b4c8058603bbd331ad560cc93b8ffe07c1dc3571e393131bef6b6e63`.
Google Code project metadata identifies the source license as GPLv3:
https://storage.googleapis.com/google-code-archive/v2/code.google.com/sphere-community-pack/project.json
The rendered Google Code archive page misleadingly returned project-not-found;
the actual metadata and source download were both retrieved successfully.

Also cloned the public mirror:
https://github.com/JulianUO/sphere-community-pack
Pinned commit: `0ade636be2185a48d322a2ec94043b8e2cdcf353`.

Relevant paths:

* `scripts/add-on/combat/sphere_combat_functions.scp`
* `scripts/add-on/combat/sphere_combat_events.scp`
* `scripts/add-on/sphere_settings.scp`
* `scripts/spheretables.scp` (loads the combat directory)
* `documentation/revisions-scp.txt`

All three combat/settings files are byte-identical between the GitHub
mirror and original Google Code archive. The functions header credits
Khaos, Anarch Cassius, Jeem and ShiryuX. This establishes actual recovered
Khaos-credited code, not just a promise or a dead download. It does not
establish that this is the exact later private revision contemplated in 2016.

The source separates UO:R and AoS combat and contains functions for swing
speed, hit chance, shield blocking and damage bonuses. The settings select
`scp.Combat_FormulasUsed=2` for UO:R and `3` for AoS; the recovered pack
defaults to 3. Helpers also depend on Sphere events, item definitions,
settings, properties and other pack functions. It is not a drop-in ServUO
C# script, and has not been executed or certified as OSI-accurate.
Its Git history includes a December 9, 2009 beta combat addition and many
subsequent fixes. Treat it as a comparison implementation and trace formulas
against contemporary OSI/Stratics evidence before porting any behavior.

Combat functions SHA-256:
`5d9f2951632ecf79e42c55270d77b7f587c697af6c3a536ecd060aeb16064795`.
Combat events SHA-256:
`f0e3a8331a9f60f307324c210f2381424406f8523ce2c2b287385c1a958f1373`.

## Other recovered historical scripts

SphereWiki links to a working public script mirror:
https://mirror.ashkantra.de/scripts/Sphere/

* `51a_damage_system_eng.zip` is by **Silver Ghost**, not Khaos. Its header
  identifies v51PvP beta 0.9 and July 2002 dates. Do not misattribute it.
* `Combat_Damage-1.zip` and `combat_damage.zip` credit Jorge Pereira/Kell
  ap Aran (the latter also credits FredyC for adaptation). Not Khaos.
* `PODGOLEM.zip` contains OnlineUO's ADMIN Peter golem script v2.1, with a
  June 23, 2002 README. Its creation handler makes a new golem and owner
  memory without an explicit follower-count or daily-production check.
  That does not establish uncapped effective behavior: it depends on the
  absent OnlineUO base package and Sphere's own pet handling. Its skill,
  recipe, repair and command choices differ from the OSI evidence; the
  README and code even disagree on gear quantity. It is after our April
  cutoff and is a custom implementation, not a recovered OSI specification.

## Working Stratics collection found through the shard

https://uorenn.com/stratics/
This is an independent working collection of selected Stratics pages,
explicitly labeled as collected October 13, 1999. It contains weapons,
armor, combat, parry, animals and several other tables/guides. It credits
Tealmon for the original archive. This is useful for earlier combat
comparisons, but is not a complete forum backup and predates craftable
golems. Do not apply its 1999 weapon damage or exceptional-quality numbers
to April 2002 without checking intervening changes.

## Outcome

Recovered usable, attributable combat source and a genuinely early custom
golem script. No new evidence establishes the exact pre-P16 active golem
maximum or mixed-follower rules. The existing ServUO reconstruction remains
unchanged. Next useful technical work is a formula-by-formula comparison
of the UO:R branch with our existing pre-AoS combat code and dated sources.
