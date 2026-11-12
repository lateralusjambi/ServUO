# Pre-Publish 16 paid hirelings

In pre-P16 mode, hiring an available NPC by paying gold does not check the
shared follower budget. Hirelings still count toward the existing follower
total. Later-era paid hiring still charges two slots per hireling and rejects
over-budget hires before accepting payment. No separate headcount ceiling was
added; the original ceiling, if any, remains unknown.

## Historical basis

* Prima's 2002 *Ultima Online: Lord Blackthorn's Revenge Official Strategy
  Guide*, printed p. 191 (PDF p. 192), recommends “hiring one or two guards”
  and describes the risk of hirelings or pets blocking an escape route. This
  supports two concurrent hires and is advice, not a maximum.
* The same guide treats escort quests separately: p. 33 explicitly limits
  those to one at a time; p. 34 explains paid hiring and states no limit or
  pet-related hiring restriction. That silence does not prove unlimited hiring.
* Rez, signing as Blade, GM Warrior, Great Lakes, reported in the January 23,
  2002 Usenet reply to [My Parrying is stuck.....HELP!!](https://groups.google.com/g/rec.games.computer.ultima.online/c/Exj92TNtxXk)
  that he hired five to seven fighters to fight him in Jhelom's Fighting Pit.
  The user supplied the reply text. It supports one character hiring more than
  two and suggests overlap during training, but does not establish exactly
  how many were employed at the same instant or the server's maximum.
* The [July Publish 16 taming notes](https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/)
  explicitly put hirelings at two slots in the shared five-slot pool.
  A later UO Forever snapshot has a separate one-hireling guard; this is not
  evidence for the original LBR rule. Its use of the same escort refusal
  message as ServUO is also insufficient to establish the original cap.

The admission rule is a reconstruction. The one-escort rule is separate;
the current escort cooldown, hireling wages, and hireling transfer were not
reconciled in this pass.

## Verification

The compiled-handler fixture calls `BaseHire.OnDragDrop` with an ordinary
player who already controls a five-slot pet. It checks two distinct offers
with the era setting on and off, ownership and follower accounting, wages
and timer start, intact rejected gold, underpayment, and the refusal to
rehire an NPC already employed by someone else. The escort handler is tested
for first acceptance and rejection of a second escort in each era mode.

## Live acceptance: November 11, 2026

Chris reported that his normal character paid two different fighters while
controlling a tamed horse (one slot) and a golem (three slots). Both accepted
payment, and both fighters, the horse, and the golem followed together.

The normal character could not get a wage quote at the Jhelom pit. A GM
obtained the quotes without paying, and the normal character paid. This
isolated paid admission from speech delivery. On the current LBR
configuration, the context menu packet path is disabled with object property
lists. For an unhired NPC, ordinary player speech delivery through the
creature AI requires a human NPC in a guarded region within three tiles;
GM speech bypasses that restriction. The pit spawner center is at Z -21
and the Jhelom guarded region starts at Z -20. Chris confirmed that the
normal character received a wage quote at street level in Jhelom.

## Pit speech follow-up

Pre-P16 unhired hirelings now listen to living players within three tiles
even outside a guarded region. The existing hiring speech handler still
decides whether to quote a wage. General NPC speech, hired follower commands,
context menus and later-era behavior retain their existing rules. The
change restores access to the quote; it does not hire the NPC or change pay.

The fixture checks speech dispatch and handling in an unguarded location,
including the three-tile boundary, unrelated speech, employed hirelings,
and the era toggle. Live recheck pending: at the Jhelom pit, stand next to
a fresh fighter on a normal character and say “hire” without GM help.
