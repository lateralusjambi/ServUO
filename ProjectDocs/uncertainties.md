# Prioritized boundary defects and historical uncertainties

Assessment against tested code checkpoint `ca907ece`.
Priority is based on gameplay impact and evidence strength, not research age.

| Priority / ID | Classification | Finding and impact | Next bounded action / closure condition |
|---|---|---|---|
| P0 PET-BOND | RED boundary defect; reachable code, not observed bonding | BaseCreature.BondingEnabled is true; IsBondable has no pre-P16 gate; accepted feeding can set BondingBegin and eventually IsBonded. Bonding/ghost pets/resurrection are explicitly in the July 23, 2002 P16 changes. Resetting IsBonded only at tame time does not stop later feeding from bonding. | Audit all entry points and serialized existing bonded state, then gate bonding, ghost behavior and resurrection for the April 22 ruleset. Ordinary feeding and ordinary pet death must still work; non-pre-P16 behavior must remain intact. |
| P1 PET-LORE | RED boundary defect candidate confirmed at handler level | AnimalLore.cs still contains numeric lore gumps and 100/110 skill-tier access rules. The P16 note describes those displays/restrictions as changes. | Specify the old loyalty/food output and access behavior, then replace the pre-P16 handler path. Test success/failure and modern negative path. |
| P1 PET-LIMITS | HOLD: boundary audit required | Official P16 introduces five weighted control slots and adds stable slots beyond the earlier 2-5 system. ControlSlots and follower machinery still exist; existence alone does not prove each path is reachable under current settings. | Trace tame, transfer, summon, mount, stable/claim and login auto-stable paths. Record which paths execute before removing/gating P16 limits. |
| P1 PET-FEED | HOLD: source conflict plus reachable differing implementation | At LBR, BaseCreature.CheckFeed uses a 50% per-item chance to add 10 loyalty; the maximum-loyalty branch is Core.SE. The April 2001 PRC guide describes feeding as restoring maximum loyalty, but the July 2002 P16 notes explicitly promise maximum loyalty for any quantity. The distinction may involve quantity or an earlier change. | Resolve quantity/date semantics with pre-cutoff evidence. Test one accepted item and stacks, low loyalty, maximum clamp and feeding while bonding is disabled. Do not silently import the P16 any-quantity guarantee. |
| P1 CONTROL-CURVE | YELLOW / HOLD for original algorithm | The model matches the 13 Runesabre D=100 observations by construction. Reena/PRC provides a conflicting 21-point D=93.9 benchmark. Ratio normalization and interpolation outside measured points are not OSI proof. At GM/GM, the 98% cap can hide an orneriness change. | Seek an independent dated formula or additional points across difficulties. Compare moderate-skill characters at 71.1 / 75.9 / 90.3; distinguish validation of the chosen model from historical confirmation. Never describe table-fitting as algorithm recovery. |
| P1 CONTROL-LOYALTY | YELLOW | Multiplicative loyalty scaling and existing +1/-3 command changes are not historically established. Natural decay/ferality and feeding interact with them; stat bonuses affect diagnostic input. | Trace each change source, compare contemporary loyalty descriptions, and test normal-player speech/context commands, failed commands and natural release. Keep exact zero-effective-skill acceptance separate from base skill 0 with a stat bonus. |
| P1 TAME-SUCCESS | YELLOW / HOLD on precise probabilities | Quadratic eligibility and own-pet automatic retaming are established. Using the same quadratic requirement inside the completion window fixes an implementation contradiction but does not prove that success curve historically. | Use dated taming-success material or the separate Taming/Retame calculators. Runesabre and Reena CONTROL evidence cannot close this row. Preserve the accepted eligibility tests. |
| P2 CONTROL-HISTORY | YELLOW beyond tested scenarios | First/second-owner release, calming at 100 Lore, repeat release and restart passed. General accumulation, multiple owner sequences, deleted owners, natural ferality and non-100 Lore calming remain reconstruction. Old save migration cannot recover past transfers that were never stored. | Add targeted fixtures only for consequential untested transitions; compare pre-cutoff descriptions. Do not infer a unique original state machine from two-owner tests. |
| P2 BARD-HOLDS | YELLOW / HOLD retained from earlier ledger | Exact range/radius/reuse duration, general pursuit, criminality/witness behavior and damage/loot attribution remain distinct from the passed core smoke tests. | Close one consequential question at a time using dated evidence. Do not reopen the accepted Musicianship prerequisite or restore Discordance/P16 difficulty. |
| P2 ERA-WIDE | Unassessed / inventory only | Magery, combat, veterinary, crafting, notoriety, world content and other systems have not been made era-complete by a successful pet-control module. | Work module by module from the P16 boundary inventory; flag unrelated later features and unconditional paths as well as expansion conditionals. |

## Recommended next implementation

Start with PET-BOND. It is a concrete post-cutoff feature with a reachable
entry point and stronger evidence than a new attempt to infer a probability
curve. Audit feeding and pet-death interactions as dependencies, but keep the
first patch bounded and regression-test behavior with the era gate both on
and off. Do not change feeding probability during that patch without first
resolving PET-FEED.

## Sources and implementation anchors

* Official UO Team, July 23, 2002, Publish 16 changes to the taming profession:
  https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/
  Animal Lore display/access changes; old and added stable capacity; weighted
  control slots; tighter command checks; removal of previous-owner effects;
  bonding/ghosts/resurrection; feeding and other pet changes. This is a change
  boundary, not blanket evidence that every listed behavior was new.
* PRC Animal Lore, updated April 16, 2001, captured July 7, 2001:
  https://web.archive.org/web/20010707030658/http://www.strangeworld.com:80/uo/prc/prc.php?Section=lore_lvo
  Distinguishes loyalty/owner-related orneriness; describes feeding and calming;
  includes the Reena-derived D=93.9 table.
* Stratics Animal Lore, captured November 18, 2001:
  https://web.archive.org/web/20011118101510/http://uo.stratics.com/content/professions/taming/taming-lore.shtml
  Runesabre D=100 table and contemporary ownership/calming descriptions.
* Code: Scripts/Mobiles/Normal/BaseCreature.cs (IsBondable, CheckFeed,
  SetControlMaster, OnAfterTame, GetControlChance); Scripts/Skills/AnimalLore.cs;
  Scripts/Skills/AnimalTaming.cs; Scripts/Mobiles/AI/BaseAI.cs;
  Scripts/Mobiles/NPCs/AnimalTrainer.cs; Scripts/Mobiles/PlayerMobile.cs.

No gameplay fix is included in this documentation checkpoint.
