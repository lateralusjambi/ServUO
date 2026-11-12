# Pre-Publish 16 ethereal mounting

The original ethereal horse, llama and ostard mount immediately from the
statuette in pre-P16 mode, without a casting delay or follower slot.
The ordinary item, living rider, mount-prevention and reward-use checks
still apply. Other ethereals and later-era mounting retain their existing
admission, accounting and casting paths.

## Publish boundary

- [Publish 12, July 24, 2001](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2001-2/2001-publish-12-24th-july/)
  lists the horse, llama and ostard as 36-month veteran rewards and
  describes double-clicking the statue to mount and dismounting to return
  it to the backpack.
- The [Publish 13 September 10 update](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2001-2/2001-publish-13-19th-august/)
  confirms the veteran reward program was active before the April 2002 cutoff.
- Publish 16 introduced the [one-slot ethereal charge](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/)
  and the [three-second summoning delay](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-2-23rd-july/).
  The checked-in LBR code previously used the shared cap and a two-second
  casting path.

## Reward timing and test scope

The shard's 30 calendar days per reward tier are intentional. The reward
catalog remains unchanged while we validate mounting: the horse is still
offered at tier one (30 days) and the llama and ostard at tier three (90 days)
with the default starting level. Moving the horse to tier three is deferred
until after the live test. Reward catalog and age-use reconciliation are
separate from this patch.

The follower charge is derived from the current era during world load; no
serialization changes are needed. Restart when switching the era setting,
rather than toggling it while a character is mounted.

Live acceptance pending: on a normal character with at least five occupied
follower slots, double-click the ethereal horse statuette in your backpack.
Verify that mounting is immediate, the follower total stays the same, and
dismounting returns the statuette without changing that total.
