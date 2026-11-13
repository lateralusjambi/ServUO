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

Live acceptance pending: repeat the single-apple test on a dismounted
horse at 30 loyalty. The normal owner should see the apple consumed and
the GM inspection should show 100. Avoid commands between measurements.
