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

## Reward timing

The shard's 30 calendar days per reward tier are intentional. After live
mounting validation, the pre-P16 catalog puts all three original mounts at
tier three (90 calendar days with the default starting level). With pre-P16
rules off, the horse stays at tier one and the llama and ostard at tier three.
The existing reward-use check follows the catalog tier, so already-claimed
reward horses stay in the world but cannot be used below tier three in
pre-P16 mode. Other rewards, reward-point counts and the clock are unchanged.

## Verification

On November 11, 2026, Chris used a normal character with a horse, a golem
and two hirelings (eight weighted slots). He claimed the tier-one statuette
for the test, then mounted and dismounted the ethereal horse. The follower
count stayed at eight throughout. This verified the mounting change before
the catalog change.

Compiled fixtures exercise immediate mount and dismount at and above the
budget for all three originals, the later-era timed cast and refusal,
and claim and reward-statuette use checks around the age thresholds.
The revised reward catalog has not been checked in a live client.

The follower charge is derived from the current era during world load; no
serialization changes are needed. Restart when switching the era setting,
rather than toggling it while a character is mounted.
