# Stabling history and ordinary vendor-purchase fixes

This bounded patch fixes the two implementation defects reproduced in the
[pet-limit audit](pre-p16-pet-limits-audit.md). It does not change stable
capacity, weighted transfer/claim/pet-purchase limits, summons, loyalty or the
provisional control formula. The 2-5 stable-capacity correction remains next.

## Temporary storage versus release

`BaseCreature.ClearControlMasterForStabling()` clears the current control
master through the existing ownership-management implementation, explicitly
suppressing release-milestone consumption. It still maintains known-owner
history, removes followers and performs the existing control cleanup.
The public `SetControlMaster(Mobile)` entry point retains genuine-release
behavior, including the AI's release command and repeat-release protection.

The ten storage call sites now use that explicit method:

* NPC AnimalTrainer
* PlayerMobile auto-stabling
* ordinary and dungeon hitching posts
* chicken coops
* Blackthorn dungeon pet storage
* pet-broker stable delivery and broker storage (two sites)
* warehouse return to stables
* broker-entry internal storage

The extra later-content callers share the same provisional-history hook if
those systems are invoked. Updating them does not enable their content under
LBR. Their fees, flags, owner lists, claim rules, storage destination and loyalty
handling are unchanged. Mastery expiry, escort completion, hireling dismissal,
actual release and deletion do not use the temporary-stabling method.

No save-layout/version change or bulk migration is made. Existing saved levels
and consumed release milestones are preserved. The old bug's contribution
cannot be distinguished from a genuine release in stored state, so this patch
does not automatically lower orneriness or restore milestones on existing pets.
Use a fresh bull for acceptance.

With pre-P16 control disabled, the wrapper delegates the same control cleanup;
the pre-P16 history hook remains disabled as before.

## Ordinary purchases above the follower cap

`BaseVendor.ProcessSinglePurchase` now reserves/checks a follower budget only
when a purchase line's computed slot cost is nonzero. A normal item requiring
zero slots is accepted even if FollowersMax minus Followers is negative.
Positive-slot pet purchases still require sufficient slots. Stock clamping,
quantity validation, price calculation and partial-purchase reporting retain
their existing behavior. Payment and item delivery continue through the
existing vendor transaction code; this is not a free-item bypass.

This is a general programming fix in both era modes, not a historical rule
choice. Weighted pet limits are still present and remain a separate boundary
reconciliation task.

## Validation

Ultima, Server and all Scripts compile successfully. The standalone compiled-
assembly test `tests/PrePublish16StablingVendorTests.cs` passed 96 assertions:

* actual AnimalTrainer.EndStable calls charge 30 gold, store the pet and leave
  difficulty/history unchanged; actual EndClaimList returns it unchanged;
* repeated paid cycles preserve the taming-owner list and follower accounting;
* a blocked claim leaves the pet stabled; an unpaid stable attempt leaves the
  controlled pet and its history intact;
* the actual AI release command after storage still consumes the first release
  milestone, taking a fresh bull from 71.1 to 75.9;
* actual retame calming at 100 Lore and repeat-release protection still work;
* a second owner's later genuine release still reaches level 2 / difficulty 90.3;
* temporary clearing preserves an existing nonzero penalty;
* LBR auto-stabling remains off; the actual auto-stable caller under SE plus
  the independent pre-P16 switch stores/claims the pet without a false release;
* ordinary goods work with negative, zero and positive budgets in both era
  modes, with normal pricing and stock clamps;
* positive-slot admission/rejection and previously incomplete purchase state
  are preserved; empty stock and zero quantities do not create purchases;
* later-era ownership cleanup retains its existing behavior.

Existing control-model (614,945 assertions), bonding (18) and Animal Lore
(1,758) regression tests also passed against the newly built assemblies. The
audit probe now keeps difficulty at 71.1 through temporary storage/claim and
accepts the zero-slot purchase with a negative budget. Its still-open stable
capacity and weighted-claim checks remain visible.

Fixtures use empty in-memory indexes, internal-map entities, a lightweight
backpack and real gold stacks. They do not load or save a shard. The vendor
assertions invoke the actual private purchase-line helper; they do not emulate
a full shop transaction or verify payment/delivery through a client. Shared
storage callers beyond the trainer and auto-stable path were source-checked
and compiled, not all exercised through their separate interfaces.

The standalone audit diagnostic was updated to replay the corrected temporary
storage call. Its original printed output in the audit document is retained as
historical evidence against its pinned baseline.

For Windows, compile/run the test separately after the server build (assemblies
in the repository root), never as part of server startup:

```powershell
$fxc = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
& $fxc /nologo /out:StablingVendorTests.exe /reference:ServUO.exe /reference:Ultima.dll /reference:Scripts.dll tests\PrePublish16StablingVendorTests.cs
if ($LASTEXITCODE -eq 0) { .\StablingVendorTests.exe }
```

Remove the generated test executable afterward; do not commit it.

## Pending client acceptance

1. Fresh tamed bull: record its `Pre16OrnerinessLevel` and
   `Pre16ControlDifficulty`; pay an NPC trainer to stable it, then claim it.
   Both values should remain unchanged. Claim restrictions still apply, so
   have sufficient current follower slots for this first check.
2. Release that bull normally. Its first genuine release should still increase
   the level from 0 to 1 and difficulty from 71.1 to 75.9. Do not expect a GM/GM
   98% control result to reveal the difficulty change; inspect the actual fields.
3. On a normal test character above five weighted follower slots, buy an ordinary
   item. Confirm gold is charged and the item arrives. A positive-slot animal
   purchase without sufficient slots should still fail. Do not raise FollowersMax
   to hide the original condition.

Start with the fresh-bull stable/claim check after a successful Windows build.
Save/restart persistence of an actually stabled pet is an additional useful check
but has not been performed by these automated fixtures. This record must not be
marked client-complete until the relevant results are reported.
