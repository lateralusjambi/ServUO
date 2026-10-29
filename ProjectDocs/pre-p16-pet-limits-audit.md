# Pet-limit and stabling audit

Audited branch: `pre-p16-integration`, checkpoint `9d25577b79a6e15211d59eb58ac01809032343c3`.
Date: October 25, 2026, America/New_York. This checkpoint adds findings and a
standalone diagnostic; that audit commit made no gameplay changes.

Subsequent status: the false stabling-release and ordinary-purchase bugs are
patched; see [fixes and completed bounded client acceptance](pre-p16-stabling-vendor-fixes.md).
The findings and output below describe the original pinned baseline. Stable
capacity is now patched separately with automated and bounded client checks
passed; see [stable capacity](pre-p16-stable-capacity.md). The broader
weighted-limit reconciliation remains open. Ordinary pet transfer, trainer
claim and vendor-animal gates are now patched separately; see
[pet acquisition and completed bounded client checks](pre-p16-pet-acquisition.md).

## Confirmed era boundary

The official July 23, 2002 Publish 16 taming changes explicitly retain an
existing stable-capacity base of 2/3/4/5 pets at combined Taming/Lore/Veterinary
thresholds below 160 / 160 / 200 / 240. They add per-skill bonuses at 100, 110
and 120, raising the possible maximum to 14. The added bonuses are after our
April 22 cutoff. The same note introduces weighted five-slot pet control and
mount costs, with new weights for eighth-circle summons.

Source:
https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/
Sections: Stable Slots, Pet Limit, and 8th Circle Summoning Spells.

The note does not by itself establish every old summon, hireling or constructed
creature limit. Those replacements require pre-cutoff evidence; removing all
limits or simply raising FollowersMax would not be a resolved reconstruction.
The exact old stable calculation's use of base versus modified skill is also
not established by this note.

## Reachability under the checked-in configuration

`Config/Expansion.cfg` sets `CurrentExpansion=LBR`. `SkillCheck.PrePublish16Rules`
defaults true; `Config/Era.cfg` explicitly enables the provisional control model.
Thus Core.AOS/SE/SA/TOL branches are false, but unconditional slot checks remain.

| Path | Current code / gate | Assessment |
|---|---|---|
| Tame start and completion | AnimalTaming.InternalTarget / InternalTimer contain no FollowersMax check; completion calls SetControlMaster | No weighted cap here. Source-traced; a six-tame client trial was not performed. |
| Ownership accounting | BaseCreature.AddFollowers / RemoveFollowers add or subtract ControlSlots; SetControlMaster itself does not enforce a cap | Weighted counters still affect other paths. Six one-slot ownership assignments produce Followers=6 with FollowersMax=5. |
| Transfer | BaseAI.TransferItem.AllowSecureTrade rejects an accepted trade when recipient Followers + ControlSlots exceeds FollowersMax | Unconditional P16-style weighted restriction survives under LBR. Actual secure-trade UI not exercised in this audit. |
| NPC stable admission | AnimalTrainer.EndStable checks Stabled.Count against GetMaxStabled | Base 2-5 tiers are present, but the 100+ per-skill bonuses are unconditional. A zero-reward 100/100/100 character gets 8 spaces rather than the pre-P16 maximum of 5. |
| Other stable bonuses | RewardStableSlots is read unconditionally; SA/TOL flat bonuses have expansion gates; BoardingSlotIncrease is called unconditionally | SA/TOL flat bonuses are off at LBR. Nonzero stored reward/mastery state can require separate handling; no claim that a new LBR character normally acquires those rewards. |
| NPC stable claim, single/all/list | All successful claim paths use AnimalTrainer.CanClaim, which tests weighted Followers + ControlSlots against FollowersMax | A one-slot pet cannot be claimed at Followers=5. This does not match the uncapped tame path. |
| Stabling history | EndStable internalizes the pet, then calls SetControlMaster(null), and only afterward sets IsStabled | Confirmed false release classification in our provisional history hook; see reproduction below. |
| Login/logout auto-stable | PlayerMobile.PetAutoStable is Core.SE; AutoStablePets and ClaimAutoStabledPets guard on it | Off under current LBR configuration. Their internal weighted claim checks and ownership-removal ordering remain if a later expansion is selected with the separate pre-P16 switch. |
| Ordinary creature mounts | BaseMount.OnDoubleClick checks ownership and mount restrictions; no additional FollowersMax comparison found | Already-owned mounts can be ridden without another slot-cap check here. Their ControlSlots remain included in owner accounting. Other mount combat restrictions were outside this audit. |
| Ethereal mounts | EtherealMount.Validate checks Followers + FollowerSlots against FollowersMax; AddFollowers/RemoveFollowers count the mount | Unconditional P16-style weighted restriction survives. Reward availability and actual mount UI not runtime tested here. |
| Vendor animal purchases | BaseVendor.OnBuyItems starts a free-slot budget at FollowersMax - Followers; ProcessSinglePurchase deducts IBuyItemInfo.ControlSlots times quantity | Animal acquisition is capped even though taming is not. The shared purchase helper has an additional zero-slot-goods bug below. |
| Hirelings | BaseHire.OnDragDrop checks Followers + ControlSlots against FollowersMax; its default cost is 2 | Weighted restriction reachable when hiring. Original hireling limit remains an evidence question. |
| Golem creation | ClockworkAssembly.OnDoubleClick requires four available slots, while Golem.ControlSlots is 3 | Unconditional admission/accounting mismatch. Original golem rules and construct-item availability need a separate specification. |
| Spell admission | SummonCreature checks +2; Air/Earth +2; Water +3; Fire +4; Daemon +5 at LBR; Blade Spirits/EV +1 at LBR | P16-style weights remain in pre-cast paths. Original summon counts/interactions require independent evidence. |
| Spell completion | SpellHelper.Summon leads to BaseCreature.Summon; the latter rechecks actual creature ControlSlots against the same weighted counter | Fixing pre-cast checks alone would leave a second blocker. SummonCreature also has a disabled ControlSlots=2 assignment, so its precheck can differ from the randomly chosen animal's accounting cost. |
| Broader later-content summons | Necromancy, Ninjitsu, Spellweaving, Mysticism and mastery handlers also contain weighted checks | Inventory only. Reachability through item/spell registration was not fully audited; existence does not establish availability under LBR. |

## Compiled-assembly reproduction

`tests/PrePublish16PetLimitsAudit.cs` is a standalone diagnostic that prints the
current behavior. It was compiled against the full build at `45996c09`, which
has the same gameplay code as audited checkpoint `9d25577b` (the latter is a
documentation-only commit). The diagnostic uses empty in-memory world indexes,
internal-map mobiles and era/config entries. It does not load or save a shard.

Observed output:

```text
era=True, expansion=LBR, autoStable=False
stable skills=0/0/0 max=2
stable skills=53.3/53.3/53.3 max=2
stable skills=60/50/50 max=3
stable skills=80/60/60 max=4
stable skills=80/80/80 max=5
stable skills=100/100/100 max=8
CanClaim at 5 slots + 1=False
pre-stable level=0 D=71.1
replayed EndStable ownership sequence: level=1 D=75.9
replayed claim: level=1 D=75.9
six ownership assignments: Followers=6 max=5
ordinary goods, slots=0, free=-1, accepted lines=0, fullPurchase=False
```

### False release penalty

A new tamable bull fixture begins with one known owner, orneriness 0 and natural
difficulty 71.1. Replaying EndStable's internalize/clear-master/set-stabled order
calls the history hook while Controlled and the old master are still present.
That consumes the first release milestone and raises orneriness to 1, changing
control difficulty to 75.9. Reattaching the same owner, as on claim, leaves it
there. It does not invoke the calming logic in OnAfterTame.

This is a confirmed bug relative to our selected release-history model;
stabling should not be classified as releasing a pet into the wild. It is not
new historical evidence for the quadratic orneriness formula. The diagnostic
replays the ownership sequence; it does not execute a complete paid trainer
transaction, targeting, bank withdrawal or save/restart. The source trace ties
that exact sequence to EndStable. Previously completed release/retame tests did
not cover this case.

### Ordinary purchase blocked above the slot cap

Six one-slot ownership assignments are accepted, leaving Followers=6 and a
maximum of 5. BaseVendor then computes a free-slot budget of -1.
ProcessSinglePurchase compares that budget with 0 even for ordinary goods
whose IBuyItemInfo.ControlSlots is zero. Because -1 is less than 0, it rejects
the line and marks the purchase incomplete. This affects the shared generic
purchase helper; not every possible vendor sale path was exercised.

The diagnostic invokes that actual helper with reflection and an ordinary
zero-slot buy-info fixture. It does not perform a client shop purchase or charge
gold. This is a programming bug without needing an assumed historical pet cap.

## Ordered fixes

1. Fix false release accounting for temporary stabling. Use an explicit
   distinction between genuine release and temporary master removal, covering
   all relevant stable paths. Retain true first/new-owner release penalties,
   repeat-release protection and taming-owner independence. Do not silently
   reset existing saved orneriness: its cause cannot be recovered from the
   stored level alone.
2. Fix ordinary zero-slot purchase admission when the free-slot budget is
   negative. Keep paid-item and pet-purchase validation intact and test both
   over-cap and ordinary cases.
3. Restore pre-P16 stable capacity to its 2-5 base and gate out later bonuses.
   Preserve already stabled pets; limiting future admission must not delete
   existing pets or prevent recovery. Test thresholds, GM/GM/GM and both modes.
4. Reconcile transfer, claim, vendor-pet, ethereal-mount and summon rules as
   one acquisition/accounting design. Establish original summon/hireling/golem
   rules before choosing replacement counters. Keep admission and completion
   checks consistent. Do not use a large FollowersMax as a general workaround.

No gameplay fix or client validation is included in this audit commit.
