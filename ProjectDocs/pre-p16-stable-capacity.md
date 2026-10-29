# Pre-P16 stable capacity

Implemented October 28, 2026, following client acceptance of the stabling-history
and ordinary-vendor fixes. Windows client acceptance for this capacity change
is pending.

## Rule and evidence

AnimalTrainer.GetMaxStabled now returns only the original skill-total tiers
when SkillCheck.PrePublish16Rules is enabled:

| Combined Animal Taming, Animal Lore and Veterinary | Pets |
|---|---|
| Below 160 | 2 |
| 160 through 199.9 | 3 |
| 200 through 239.9 | 4 |
| 240 or higher | 5 |

The official July 23, 2002 Publish 16 notes describe these as the continuing
existing tiers, then add per-skill bonuses at 100/110/120. Those additions fall
after the project's April 22 cutoff.

Source: https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/
Stable Slots section.

The pre-P16 return precedes reward, SA/TOL, GM-skill and Boarding mastery
bonuses. Stored reward/mastery values remain intact. With the pre-P16 switch
disabled, all existing bonus calculations remain in use. This gate uses the
shared era switch independently of the provisional control-model switch.

The existing Skill.Value calculation is retained. Whether historical OSI used
base or modified skills remains unresolved; this patch establishes the tiers
and excludes later bonuses without claiming to resolve that distinction.

## Existing pets and limits

Only the capacity calculation changes. No save layout or stored pet list is
rewritten. NPC stable admission already rejects a new pet when the stored
count is at or above capacity, before charging gold. Claim paths do not compare
stored count to capacity, so an over-cap stable remains recoverable. Existing
weighted follower checks still apply to claims; claim pets in batches if needed.
This patch does not reconcile those checks or change FollowersMax.

The shared calculation also supplies other callers that already use it. No
later-content storage system is enabled by this change. Lowered admission does
not delete stored pets or reset their ownership/control history.

## Validation

Ultima, Server and all Scripts compiled successfully. The standalone
`tests/PrePublish16StableCapacityTests.cs` passed 187 assertions against those
assemblies, covering:

* Exact tier thresholds and the tenth below each threshold.
* 100/110/120 skill totals, nonzero stored reward slots and an active three-slot
  Boarding fixture, under LBR/SA/TOL with pre-P16 rules enabled.
* Modified-skill calculation retained, and later-mode GM/reward/expansion/mastery
  bonuses retained with the era switch disabled.
* Actual paid NPC admission up to each tier, refusal of the next pet without a
  charge or ownership change, and preservation/recovery of eight existing pets
  under the five-pet capacity through actual individual trainer claims.

Existing suites passed: control 614,945 assertions; bonding 18; Animal Lore
1,758; stabling/vendor 96. The audit diagnostic now reports five at 100/100/100.
These are headless fixtures using real compiled handlers and gold stacks,
without a shard save/load, network client, bank-funded payment or every separate
storage UI. Recovery above capacity was tested using the individual claim path;
other claim paths were source-checked.

## Pending client acceptance

After a successful Windows Debug build, use a normal Player-access character
with Animal Taming, Animal Lore and Veterinary each at 100.0 and sufficient gold.
With five pets stabled, attempting to stable a sixth should be refused with no
fee, leaving that pet controlled. Existing weighted claim restrictions still
apply. Do not raise FollowersMax to perform this check.

No client result is recorded for this patch yet.
