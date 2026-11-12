# Pre-Publish 16 veteran reward catalog

The pre-P16 menu and claim confirmation offer only the 38 choices in
the [Publish 12 reward list](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2001-2/2001-publish-12-24th-july/).
The [September 10, 2001 update](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2001-2/2001-publish-13-19th-august/)
confirms the program was active by the cutoff. Eligibility is cumulative:
at tier three, an account can still choose from tiers one and two.

| Tier | Shard account age | New choices | Catalog inventory |
|---|---:|---:|---|
| 1 | 30 days | 21 | Black, furniture and special dye tubs; bronze and copper cloaks and robes; crocodile, daemon, dragon, earth elemental, ettin, gargoyle, gorilla, lich, lizardman, ogre, orc, ratman, skeleton and troll statuettes. |
| 2 | 60 days | 7 | Leather dye tub; agapite and gold cloaks and robes; banner and flaming skull deeds. |
| 3 | 90 days | 10 | Verite and valorite cloaks and robes; cow, zombie and llama statuettes; ethereal horse, llama and ostard. |

The first three ServUO tiers already contain all of these choices. Untagged
additions in those same tiers include the Flesh Renderer, Crystal Elemental
and Dark Father statuettes. An age cap or a check of the expansion setting
alone would not remove them. Later tiers also have entries without an
expansion requirement, including the runebook dye tub and the ethereal
ki-rin, unicorn, ridgeback, beetle and swamp dragon.

The original list is positive evidence for the choices above. The indexed
pre-cutoff notes do not establish additional choices; the checked 2003
publish notes did not resolve the original rollout date of the fourth- and
fifth-tier catalogs. RunUO Beta 12 and Beta 16 archives contain the original
three mount classes, but do not establish the OSI reward release dates.
The [current official catalog](https://uo.com/wiki/ultima-online-wiki/items/veteran-rewards/)
confirms many of the additions exist today, not when each was introduced.
The pre-P16 filter therefore follows documented choices rather than assigning
an unverified introduction date to every excluded entry.

## Scope and verification

`RewardSystem.HasAccess(Mobile, RewardEntry)` is shared by the choice menu
and final claim confirmation. It restricts pre-P16 claims to the original
reward-choice labels. The full table is retained for later-era mode and
the existing use checks on previously issued items. No existing item,
claim count, reward-point formula, or 30-day interval is changed.
Item behavior is separate from catalog availability.

The compiled test checks the exact choices at 29, 30, 59, 60, 89, 90 and
750 days, later-era access to excluded entries, existing expansion
requirements, and unchanged use checks on a previously issued reward.
Live acceptance pending: use a normal account with unspent rewards to
inspect the menu without spending a point. At tier three or higher it
should offer only the catalog above, including exactly the three original
ethereals.
