# Pre-P16 golem follower-budget reconstruction

Prepared October 29, 2026. Runtime acceptance is pending.

## Evidence and decision

The contemporary March 30–April 2, 2002 player discussion at
https://archives.jeuxonline.info/fils/76309.html includes a firsthand account
of hunting with two active golems. The November 8, 2001 report at
https://archives.jeuxonline.info/fils/14366.html describes five owned golems
kept in a town pen; it does not demonstrate five simultaneously commanded
combat pets. The separate November 2, 2001 Stratics news item quotes Calandryll on
five creations per approximately 24 hours; that production quota is distinct
from simultaneous ownership:
https://web.archive.org/web/20130331055950/http://uo.stratics.com/news/arc10-2001.shtml .

This patch removes the later weighted follower budget as an admission
restriction for pre-P16 golems. It does not add a replacement numerical cap.
That is an explicit reconstruction: multiple active golems are supported,
but an unlimited historical maximum and mixed-follower rules are unproven.
This supersedes the earlier golem exclusion in `pre-p16-pet-acquisition.md`.

## Implementation

* `PrePublish16PetLimits.CanConstructGolem` bypasses the assembly's existing
  four-free-slots check only while `SkillCheck.PrePublish16Rules` is true.
  Backpack, Tinkering requirement, ingredients, scalar and normal creation
  handling are unchanged.
* `IgnoresFollowerCap` now includes nonsummoned Golem instances, whether
  tamable or not. Existing secure-transfer and AnimalTrainer claim paths
  already use that helper. Ownership, control eligibility, range, combat
  and cancellation guards remain in place.
* `SetControlMaster` contains no additional follower-budget rejection.
  `Golem(true, scalar)` is the constructed creature variant; the constructor
  parameter does not set BaseCreature.Summoned. Creatures actually flagged
  Summoned remain excluded from the bypass, as do BaseHire instances.

ControlSlots remains 3 and FollowersMax remains 5. Existing follower
accounting is retained, including totals above five. No save format changes.
An existing pet plus golems can coexist under this reconstruction; other
admission paths, such as spell summons, still apply their existing checks.
This is not a complete mixed-follower reconciliation.

With pre-P16 off, the existing four-slot assembly check and three-slot
golem transfer/claim checks remain intact, including their pre-existing
mismatch. The provisional control-model switch does not choose this rule.

The historical daily production quota is **not implemented by this patch**.
It requires a separate decision about character/account scope and persisted
timer behavior. Removing the follower restriction makes repeat creation
possible without that quota; this interim behavior is explicit rather than
an assertion that OSI had no production restriction.

## Validation

Ultima, Server and all Scripts compiled using Roslyn 4.8 with .NET Framework
4.8 reference assemblies. `tests/PrePublish16GolemTests.cs` passed 156
assertions against the compiled handlers, including actual assembly use
at 0/1/2/3/5/6/12 followers in both era modes, material consumption/refusal,
skill and backpack requirements, actual secure-transfer completion,
reclaim eligibility, cancellation/wrong-owner/combat guards, summoned
exclusion, two sequential creations and coexistence with an ordinary pet.

The fixture uses an in-memory world and normal Player-access characters;
loaded ingredient metadata avoids dependence on client tiledata. Actual
constructed golems use the production constructor. No live trade UI,
combat command execution, shard save/reload or Windows SDK build was run.

Existing suites passed: acquisition 221, control 614,945, bonding 18,
Animal Lore 1,758, stabling/vendor 96, stable capacity 187. The diagnostic
pet-limit audit also completed successfully.

## Bounded client acceptance

After applying and building, launch the server and use a normal Player
character. An admin may supply ingredients or set skills beforehand;
perform the actual checks with Player access and FollowersMax left at 5.

1. At GM Tinkering, create a golem using a clockwork assembly in the
   backpack, a power crystal, 50 iron ingots, 50 bronze ingots and 5 gears.
2. Repeat with another complete set. Both golems should remain controlled;
   the weighted Followers value should be 6. Check each with follow/stay.
3. Have an ordinary horse or bull controlled as well; verify it remains
   controlled and both golems still obey. The implementation permits this;
   the test does not authenticate OSI's historical mixed-follower rule.
4. Transfer a golem from another normal character to a recipient already
   controlling two golems. Complete the actual secure trade, then check
   ownership and commands. Target world entities rather than status bars.
5. If stable space is available, stable a golem, then reclaim it while at
   or above five weighted followers. Stable capacity still applies.

Record observed results separately. A successful build is not runtime
acceptance. Save/restart persistence can be checked after these pass.
