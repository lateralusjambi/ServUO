# Prioritized boundary defects and historical uncertainties

Control runtime baseline: `ca907ece`. Bonding checkpoint: `d2765ca2`;
bounded client acceptance passed October 25, 2026. See [bonding boundary](pre-p16-bonding.md).
Priority is based on gameplay impact and evidence strength, not research age.

| Priority / ID | Classification | Finding and impact | Next bounded action / closure condition |
|---|---|---|---|
| P0 PET-BOND | CLOSED for bounded gate; automated and client checks passed | BondingEnabled now follows the era switch; feeding and direct assignments cannot start/complete bonding, stored bonded flags cannot select ghost death, and pet resurrection is blocked under pre-P16 rules. Full build and compiled-assembly checks cover both era modes. No existing bonded/ghost pets reported; no migration performed. | Feeding properties, rejected GM bonded assignment, corpse/no ghost and saved-world reload passed. Preserve the separate feeding-semantics HOLD, existing-ghost migration limit and later-world-content audit; see pre-p16-bonding.md. |
| P1 PET-LORE | CLOSED for bounded handler/display gate; YELLOW reconstruction details remain | P14 descriptive output and controlled-only loyalty replace numeric gumps and 100/110 access tiers under the era gate. P15 owner/feather corrections accounted for. Full build and 1,758 compiled-assembly assertions passed. Exact skill bands, training aggregation, success odds, trait weights and message transport remain qualified choices. | Wild/tamed bull output, eagle feathers and non-animal rejection passed screenshot review at 45996c09. AccessLevel is not shown; failed-roll/dead-viewer client cases were not separately reported. Keep formula/history questions open; see pre-p16-animal-lore.md. |
| P0 CONTROL-STABLE | Patched; automated and bounded client checks passed | EndStable clears the master before marking the pet stabled. The release-history hook consumes a new-owner milestone and raises a fresh bull from orneriness 0 / difficulty 71.1 to 1 / 75.9; claiming does not undo it. Ownership-sequence replay confirmed this. | Explicit temporary-stabling path now covers ten callers. Actual paid trainer/claim and genuine-release tests pass. Client stable/claim preserved 0 / 71.1; genuine release then produced 1 / 75.9. See pre-p16-stabling-vendor-fixes.md; existing saved levels/milestones are not rewritten. |
| P0 VENDOR-SLOTS | Patched; automated and bounded client checks passed | Above FollowersMax, BaseVendor computes a negative free-slot budget and its shared generic purchase helper rejects even ordinary goods costing zero slots. Six ownership assignments and the actual helper reproduced the failure. | Zero-slot goods now bypass slot reservation; pricing, stock and positive-slot rejection tests pass in both era modes. Client purchase above five weighted slots delivered the item and deducted gold; positive-slot rejection remains compiled-test coverage. |
| P1 PET-LIMITS | Audit complete; RED active P16 boundaries, HOLD for replacement summon/hireling rules | Taming has no slot cap, but transfer, claim, vendor-pet purchase, ethereal mounting and summoning still use weighted counters. The audited LBR 100/100/100 capacity of 8 is now corrected to 5 under the shared pre-P16 switch. Core.SE auto-stable is off at LBR. | See pre-p16-pet-limits-audit.md for paths and evidence. 2-5 stable admission is patched with 187 assertions including recovery above capacity; bounded client acceptance passed: sixth bull refused, no fee, control retained (pre-p16-stable-capacity.md). Reconcile acquisition/accounting against pre-cutoff evidence. |
| P1 PET-FEED | HOLD: source conflict plus reachable differing implementation | At LBR, BaseCreature.CheckFeed uses a 50% per-item chance to add 10 loyalty; the maximum-loyalty branch is Core.SE. The April 2001 PRC guide describes feeding as restoring maximum loyalty, but the July 2002 P16 notes explicitly promise maximum loyalty for any quantity. The distinction may involve quantity or an earlier change. | Resolve quantity/date semantics with pre-cutoff evidence. Test one accepted item and stacks, low loyalty, maximum clamp and feeding while bonding is disabled. Do not silently import the P16 any-quantity guarantee. |
| P1 CONTROL-CURVE | YELLOW / HOLD for original algorithm | The model matches the 13 Runesabre D=100 observations by construction. Reena/PRC provides a conflicting 21-point D=93.9 benchmark. Ratio normalization and interpolation outside measured points are not OSI proof. At GM/GM, the 98% cap can hide an orneriness change. | Seek an independent dated formula or additional points across difficulties. Compare moderate-skill characters at 71.1 / 75.9 / 90.3; distinguish validation of the chosen model from historical confirmation. Never describe table-fitting as algorithm recovery. |
| P1 CONTROL-LOYALTY | YELLOW | Multiplicative loyalty scaling and existing +1/-3 command changes are not historically established. Natural decay/ferality and feeding interact with them; stat bonuses affect diagnostic input. | Trace each change source, compare contemporary loyalty descriptions, and test normal-player speech/context commands, failed commands and natural release. Keep exact zero-effective-skill acceptance separate from base skill 0 with a stat bonus. |
| P1 TAME-SUCCESS | YELLOW / HOLD on precise probabilities | Quadratic eligibility and own-pet automatic retaming are established. Using the same quadratic requirement inside the completion window fixes an implementation contradiction but does not prove that success curve historically. | Use dated taming-success material or the separate Taming/Retame calculators. Runesabre and Reena CONTROL evidence cannot close this row. Preserve the accepted eligibility tests. |
| P2 CONTROL-HISTORY | YELLOW beyond tested scenarios | First/second-owner release, calming at 100 Lore, repeat release and restart passed. General accumulation, multiple owner sequences, deleted owners, natural ferality and non-100 Lore calming remain reconstruction. Old save migration cannot recover past transfers that were never stored. | Add targeted fixtures only for consequential untested transitions; compare pre-cutoff descriptions. Do not infer a unique original state machine from two-owner tests. |
| P2 BARD-HOLDS | YELLOW / HOLD retained from earlier ledger | Exact range/radius/reuse duration, general pursuit, criminality/witness behavior and damage/loot attribution remain distinct from the passed core smoke tests. | Close one consequential question at a time using dated evidence. Do not reopen the accepted Musicianship prerequisite or restore Discordance/P16 difficulty. |
| P2 ERA-WIDE | Unassessed / inventory only | Magery, combat, veterinary, crafting, notoriety, world content and other systems have not been made era-complete by a successful pet-control module. | Work module by module from the P16 boundary inventory; flag unrelated later features and unconditional paths as well as expansion conditionals. |

## Recommended next implementation

PET-BOND and PET-LORE bounded client acceptance is complete. The PET-LIMITS
audit found two concrete implementation bugs, now patched with 96 assertions:
temporary stabling consumed a release milestone, and negative free-slot budgets
blocked ordinary purchases. Complete their client checks, then restore the
documented 2-5 stable capacity. Broader
acquisition/summon limits still need an integrated specification. See the
ordered actions in pre-p16-pet-limits-audit.md. Do not change feeding probability
without resolving PET-FEED.

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

Bonding and the active Animal Lore skill have bounded era gates after the
recorded control baseline. Other unresolved rows and the explicit Animal Lore
reconstruction details remain open.
