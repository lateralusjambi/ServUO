# Pre-P16 golem production quota

Implemented November 6, 2026. Bounded client acceptance pending.

## Rule and evidence

When Era.PrePublish16 is enabled, each PlayerMobile may successfully
construct five golems within a fixed 24-hour window. The first successful
creation starts the window. Later creations, refusals, transfers, releases,
stabling and pet deaths do not move its deadline or refund creations.
At or after the deadline, the next successful creation starts a new window.
The allowance is independent of the number of golems currently controlled.

The November 2, 2001 Stratics reproduction of Calandryll's statement
supports five creations, keeping versus selling making no difference,
and an approximately 24-hour reset measured from the first creation.
See [the focused source trace](golem-production-quota-trace.md).
Continuation to the April 22, 2002 cutoff is an inference. Character scope,
counting successful creations only, exact 24-hour timing and persistence
across restarts are explicit reconstruction choices, not recovered OSI code.

## Player behavior

An exhausted character receives the wording preserved in the later Sphere script:

> You are too drained to make another golem. You must wait approximately one day to construct another.

This wording is taken from the March 27, 2016 Sphere script comment for
message 1071942, selected as a sourced reconstruction rather than newly
written dialogue. It has not been verified in unpatched LBR. The server sends
the literal text rather than the cliloc ID, preserving the selected wording
when using a modern client. Source:
https://forum.spherecommunity.net/Thread-tGolemAssembly?pid=23443

The approximately-one-day phrase describes the production window; a refusal
does not start a fresh day-long penalty. The refusal occurs before consuming any
ingredients or deleting the assembly. Missing ingredients, insufficient
skill and an assembly outside the backpack do not start or increment a window.
The current creation path has no random craft-failure roll; this patch adds none.

The quota applies to PlayerMobile, including staff characters; it has no
access-level exemption. NPC Mobile instances have no character quota.
When Era.PrePublish16 is off, construction neither checks nor increments
this quota. Existing saved counters remain stored, and their absolute
UTC deadlines continue to elapse.

## Persistence and compatibility

PrePublish16GolemQuota stores a creation count and an absolute UTC reset
in PlayerMobile save version 41. Version 40 and earlier players initialize
with an unused allowance. No retroactive creations are inferred from pets.
No item save format is changed.

**Before installation, save and shut down the server and preserve a full
Saves backup.** After saving with this code, reverting to version-40 player
code requires restoring the corresponding older world save as well.
Disabling Era.PrePublish16 is supported without reverting code or saves.

Read-only GM properties on the character are PrePublish16GolemsCreated
(the current, unexpired count) and PrePublish16GolemQuotaReset (UTC deadline;
a past value remains visible until the next successful creation).

## Compiled validation

Ultima, Server and all Scripts compiled with Roslyn 4.8 using Mono's .NET
Framework 4.8 API references. The expanded PrePublish16GolemTests suite
passed 197 assertions against the compiled handlers, now using actual
PlayerMobile makers. Coverage includes five successes, repeated sixth
refusal, preservation of all four ingredients and the assembly, transfer
without allowance refund, independent characters, failed prerequisites,
exact deadline boundaries, actual creation after expiration, staff refusal,
full PlayerMobile version-41 round trip, a version-40-shaped payload migration,
and the later-era negative path. The save fixture checks full byte consumption.

Related suites passed: pet acquisition 221, stable capacity 187, bonding 18,
and paid stabling/vendor behavior 96. Total across these suites: 719 assertions.
No live shard was launched and no user Saves were read or written. The tests
use an in-memory world without client tiledata. Windows SDK build, live
refusal display and actual shard save/restart remain bounded client checks.

## Bounded client check

Use a normal Player character, GM Tinkering, and FollowersMax left at 5.
Prepare six complete sets: each has one clockwork assembly, one power
crystal, 50 iron ingots, 50 bronze ingots and five gears. Keep stable capacity
separate from production allowance; do not try to stable all five golems.

1. Create five golems on the same character. All five should succeed.
2. Attempt the sixth. Check the refusal text, the intact assembly, all
   four resource totals, and unchanged ownership of the existing golems.
3. Repeat the refusal. It must not move the reset deadline.
4. Save, shut down normally and restart. Attempt the sixth again: refusal
   and unchanged resources should persist.
5. Transfer a golem away and retry: transfer must not restore allowance.
   A second character should still have its own allowance.

The compiled boundary tests cover expiration without waiting a real day.
The initial client check need not modify clocks or save properties. A later
normal login after the recorded deadline can independently check expiration.
