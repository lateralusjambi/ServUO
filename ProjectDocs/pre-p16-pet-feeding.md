# Pre-Publish 16 feeding and loyalty

One accepted piece of food restores a controlled pet's loyalty to 100 in
pre-P16 mode. A stack does the same. The change applies only to loyalty
within the existing feeding handler; food preferences, owner/friend checks,
food consumption, stamina, bonding, and natural decay retain their existing
behavior. The stamina formula and command/decay rules are separate era audits.

## Evidence

- Reena Dae's [Pet Loyalty](https://web.archive.org/web/20020423192836/http://www.tamingarchive.com/pets/loyalty.php)
  explicitly says one piece restores 100% loyalty regardless of the starting
  level. The page says it was last modified April 9, 2002; the Wayback capture
  is April 23. The footer date is self-reported. This is contemporary player
  documentation, and the page separates the feeding advice from the 1999
  Test Center measurements at the bottom.
- Prima's LBR strategy guide, printed p. 37 (PDF p. 38), also describes
  maximum loyalty on feeding and favors feeding periodically over a large
  meal. The [April 2001 PRC guide](https://web.archive.org/web/20010707030658/http://www.strangeworld.com/uo/prc/prc.php?Section=lore_lvo)
  describes a reset to maximum, without specifying quantity.
- [Publish 16, July 23, 2002](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/)
  states that any quantity gives maximum loyalty. This does not establish
  that one piece first became sufficient in July: Reena's April text gives
  an explicit earlier account. No inference about the date of the stamina
  rule is needed for this change.

## Verification

Chris's November 12, 2026 baseline: a normal owner fed one apple to a
tamed horse set to 30 loyalty; the result was 40. That matches the old
ServUO roll and is not historical evidence.

The feeding/bonding fixture checks single and stacked food, rejection of
non-owner/non-friend feeding, the maximum clamp, continued absence of
bonding, and preserved behavior with the pre-P16 switch off.

On November 12, 2026, Chris repeated the single-apple horse test on
`a367e6f` and reported loyalty rising from 30 to 100. This verifies the
loyalty result in the live client. Food consumption is covered by the
compiled fixture; it was not separately reported in the client recheck.

## Command loyalty: ordinary horse

On November 12, 2026, Chris tested a tamed horse at 50 loyalty with the
normal owner. The successful “all follow me” check left loyalty at 50.
This verifies the existing pre-P16 control exception for this horse.
[Publish 3, February 23, 2000](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2000-2/2000-publish-03-23rd-february/)
excluded automatically controlled pets from loyalty gains on successful
commands. This check does not establish command deltas for higher-skill
pets or the natural decay rate.

## Command loyalty: bull

Chris confirmed that a tamed bull obeyed the normal owner's follow command
and gained one loyalty point (95 to 96). This verifies the success path
for a pet that requires taming skill under the current model. The refused
command penalty was not measured in this trial.

In a second trial starting at 30 loyalty, the bull obeyed once (30 to 31)
and then refused the next order (31 to 28). This confirms the live +1 and
-3 deltas under the current control model. Natural decay was not tested.

## Natural decay: nearby owner behind an obstruction

Reena Dae's [Pet Loyalty page](https://web.archive.org/web/20020423192836/http://www.tamingarchive.com/pets/loyalty.php),
marked April 9, 2002, describes one loyalty level per hour normally and
two levels at the server's five-minute checks when the owner is out of
sight. It describes a shared cadence, not a timer started by crossing a wall.

The old timer woke every five minutes but ran pet decay only hourly.
A forced baseline tick dropped both a nearby pet and one with an owner on
another facet from 100 to 90, with no change on the next non-hourly tick.

The new pre-P16 branch applies a 20-point loss to a living, controlled,
tamable pet whose nearby, logged-in owner is on the same map but blocked
by line of sight. It excludes stabled pets, ridden mounts and summons.
Logout, cross-facet and long-distance cases retain the existing hourly
behavior. If the five-minute and hourly checks coincide, an obstructed pet
loses 20 rather than 30. These boundaries are deliberate implementation
choices where the period account does not specify the details. Existing
release processing still applies at zero; the patch does not alter
command deltas or feeding.

The compiled timer fixture uses an actual map line-of-sight obstruction
and forces the scheduler deadline without changing system time. Client
acceptance is pending. For a live check, start at 100 with the owner logged
in behind a nearby solid wall. The first obstructed timer check should show
80; no full hour is required.
