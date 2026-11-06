# Golem production quota: launch to Publish 16

Focused check: November 6, 2026 (UTC). Research only; no gameplay changes.
Project cutoff remains the end of April 22, 2002.

## Finding

A five-creations-per-approximately-24-hours rule is directly documented in
a contemporary Stratics reproduction of a developer statement dated
November 2, 2001. No later confirmation or explicit removal was recovered
in this pass. Carrying that announced rule forward to our cutoff is a
reasonable reconstruction, not a verified observation of April 2002 behavior.

Publish 16 explicitly introduces weighted pet control slots, including
three slots for a golem within a five-slot budget. The recovered notes do
not say that this replaced or removed the production quota. A production
restriction and an active-follower restriction can coexist.

## Positive launch evidence

[Stratics November 2001 news archive, captured March 31, 2013](https://web.archive.org/web/20130331055950/http://uo.stratics.com/news/arc10-2001.shtml)

Article: **Limit 5 Golems in 24 Hours**.
Posted November 2, 2001, 6:34 PM EST by Lady Malynn.
It attributes the statement to Jonathan “Calandryll” Hanna, Designer,
Ongoing Content, on the MyUO Discussion Boards.

Short extract: “The number of golems you can make per day is 5.”

The accompanying explanation says selling rather than retaining creations
does not avoid the quota, and describes a timer beginning with the first
creation and resetting approximately 24 hours later. This supports a
creation allowance rather than a maximum number owned.

This is a preserved contemporary quotation, not a recovered original
forum body or an independently measured server test. The original linked
thread is:

http://boards.uo.com/cgi-bin/showflat.pl?Cat=&Board=qscenarios&Number=816326&page=0&view=collapsed&sb=5&part=3&vc=1

An adjacent November 4 article links the same thread using part=all,
helping identify the original discussion; it supplies no later quota date.

Retrieved raw archive HTML SHA-256:
a925a6fb652b6843e5e57dc5fe590331cab04dc660ed46fb2c6a8aa80c257105

## Official publish-note coverage

The current official historical indexes were retrieved, and all 21 linked
publish pages across their 2001 and 2002 sections were downloaded and
searched for golem, clockwork, and pet-construction references. The
relevant pet/crafting sections were inspected. This is coverage of those
published pages, not a claim to possess every original FYI, hotfix,
client-patch announcement, or developer forum post.

[2001 official index](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2001-2/)
[2002 official index](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/)

| Documents checked | Result relevant to production quota |
|---|---|
| Publish 14, including its listed December updates | No explicit golem quota change recovered. |
| Publish 15, January 9, 2002 | Pet and crafting fixes, without a golem quota change. |
| Publish 15.5, January 30, 2002 | Sacrifice rules; no golem quota change. |
| Both April 23 Publish 16 entries | No explicit golem quota change. |
| July Publish 16 parts 2 through 2.5 and July 24 changes/bug fixes | Taming section specifies the follower budget; no explicit production quota removal. |
| September and December Publish 16 follow-ups and suffix fix | No explicit golem quota change recovered. |

[Publish 16 taming section](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/)

That section also excludes golems from veterinary healing. Neither
golem-specific passage addresses production frequency. Do not use the
three-slot rule as evidence that the five-per-day rule ended.

The historical archive's page labels are not sufficient to establish the
first deployment date of every feature: for example, its Publish 14 URL
and heading say November 14 while its body identifies November 30.
Our cutoff is unchanged by these labels.

## Other checks and their limits

* The April 8, 2002 Stratics Tinkering capture was retrieved. Its golem
  recipe table does not specify a production quota. This is a narrow
  omission, not contrary evidence.
  https://web.archive.org/web/20020408071427/http://uo.stratics.com/content/skills/tinkering.shtml
* A Usenet FAQ actually posted February 13, 2002 and internally updated
  January 23, 2002 discusses crafted golems without a quota. The thread
  title incorrectly says 2001. Its short golem description does not
  establish unrestricted production.
  https://groups.google.com/g/rec.games.computer.ultima.online/c/GCgb6cOF3VU
* The previously recovered March–April 2002 firsthand two-golem hunting
  report concerns active ownership, not how many were built in a day.
  It does not test the production quota.
  https://archives.jeuxonline.info/fils/76309.html
* Targeted English, French, and Japanese searches did not recover a
  dated report of a sixth creation succeeding or being refused within
  this period. Search failure is not evidence that no such report exists.
* December 2001–August 2002 monthly Stratics archive retrieval attempts
  encountered access failures, rate limits, or missing captures. Those
  months were NOT successfully reviewed in full. The June 2002 profession
  guide retry was rate limited. Original MyUO thread recovery failed;
  the CDX query timed out and one replay returned 404. Neither result
  proves the original thread was never archived.

## Later endpoint is different, but undated

The current official Mechanical Pets guide describes an approximately
ten-minute rest between constructions. It also covers much later pets,
including Sutek constructs and the Kotl Automaton.

https://uo.com/wiki/ultima-online-wiki/skills/tinkering/mechanical-pets/

This establishes that a later guide describes a different production
restriction. It does not date that restriction's introduction, prove the
old daily quota was removed, or connect a change to Publish 16.

## Reconstruction decision

For the April 22 cutoff, retain the five-per-approximately-24-hours quota
as the leading historical reconstruction, with moderate confidence in
continuation and strong confidence in the November announcement.
Do not claim “confirmed until P16” or “removed by P16.”

Implementation remains pending a separate decision. The statement does
not resolve character versus account scope, treatment of failed attempts,
server restart persistence, or exact timing tolerance. Its wording favors
a window starting with the first creation over a midnight reset; that is
not a recovered algorithm. These details must be identified as choices if
we implement the reconstruction.

The current code deliberately has no production quota; that interim
state is documented in pre-p16-golems.md. This research commit does not
change it.
