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

Live acceptance pending: on a normal character with at least four weighted
followers, pay two different fighters the wages they quote. Confirm both
accept and respond to follow commands. No save changes are required.
