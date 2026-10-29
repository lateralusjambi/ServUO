# Pre-P16 ordinary pet acquisition

Implemented October 28, 2026, after stable-capacity client acceptance at
`2b29d568`. Full build, compiled tests and bounded client acceptance passed.

## Historical boundary and scope

The official UO Team July 23, 2002 Publish 16 taming notes introduce the weighted
five-slot pet budget and distinguish it from the existing 2-5 stable capacity:
https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/
Pet Limit and Stable Slots sections.

The bounded inference implemented here is that the new weighted budget should
not prevent ordinary nonsummoned pets being transferred, reclaimed or bought
under the pre-P16 setting. This does not establish every earlier creature limit
or recover an OSI implementation. Summon, hireling and golem rules require their
own evidence and replacement specification; they are not made unlimited here.

## Implementation

`PrePublish16PetLimits.IgnoresFollowerCap` requires the shared
`SkillCheck.PrePublish16Rules` switch and a tamable, nonsummoned creature. It
explicitly excludes BaseHire and Golem, even if customized as tamable.
The provisional control switch does not select this acquisition rule.

* BaseAI.TransferItem.AllowSecureTrade skips only its weighted recipient-budget
  rejection for those pets. Ownership, alive/range, control-eligibility, combat
  and other existing trade handling remain in place. Completion still uses
  SetControlMaster and the existing distinct-owner history.
* AnimalTrainer.CanClaim applies the same exception. Individual claim-list,
  named claim and claim-all paths already share this check. Stable admission
  still uses the separately corrected 2-5 skill-total capacity.
* BaseVendor.ProcessSinglePurchase skips slot reservation for AnimalBuyInfo
  catalogs under the shared pre-P16 switch. The repository's AnimalBuyInfo
  entries sell ordinary animals through trainer, ranger, rancher and veterinary
  catalogs. Other positive-slot IBuyItemInfo categories retain their budget
  check. Prices, stock, quantity limits, validation, payment and delivery remain
  in the original transaction. The generic zero-slot-goods fix remains active
  in both era modes.

No ControlSlots, FollowersMax, AddFollowers/RemoveFollowers, save version or
stored history is changed. Followers may exceed five; the retained accounting
still affects unresolved summon/hireling and other admission paths. This is an
intentional intermediate boundary, not a complete pet-limit reconciliation.
With the shared era switch off, the existing weighted restrictions remain.

No changes are made to ethereal mounting, summon checks, hireling hiring, golem
creation, later-content pet brokers or SE auto-stable claiming. Those require
separate review. Normal LBR configuration keeps SE auto-stabling disabled.
The unchanged trade guards were not independently proven historically accurate
by this patch.

## Compiled validation

Ultima, Server and all Scripts compiled successfully. Standalone
`tests/PrePublish16PetAcquisitionTests.cs` passed 221 assertions against actual
compiled handlers:

* Transfer acceptance at 0/4/5/6/12 follower totals in both era modes; accepted
  transfer completion updates ownership and accounting without changing the
  taming-owner list or consuming release penalties.
* One- and three-slot pet admission; later-mode weighted refusal retained.
* Actual paid stabling and individual claims, plus named/all claims above five
  followers; summons and non-tamable creatures retain their existing checks.
* Animal purchase-line admission, normal pricing, stock clamps and zero
  quantities in both modes with negative, zero and positive budgets; unknown
  positive-slot categories remain restricted.
* Full BaseVendor.OnBuyItems transactions with real gold and fixture animal
  stock at 0/5/6 followers in both modes, checking payment, stock, actual
  delivered creatures, control master and follower accounting. Insufficient
  funds reject delivery without charging gold or decrementing stock.
* Transfer cancellation, wrong owner, rejected control eligibility, combat,
  summoned/non-tamable exclusions, and a genuine second-owner release after
  an over-cap transfer.

The suite uses in-memory world indexes, internal-map entities, lightweight
backpacks and fixture pets/buy info. It invokes the actual private trade-token
methods, without a live secure-trade window. It does not load/save a shard,
exercise every vendor catalog through the client, or test all remaining trade
or storage restrictions. Full payment/delivery is tested through the vendor
handler, without a network shop UI.

Existing suites passed: control 614,945; bonding 18; Animal Lore 1,758;
stabling/vendor 96; stable capacity 187. The earlier stabling test now selects
later-era mode for its expected weighted claim rejection. Its history and
payment checks remain intact. The audit diagnostic reports CanClaim=True at
five followers plus an ordinary one-slot pet.

## Client acceptance: October 28, 2026

Gameplay checkpoint `c0e45c71`. Chris reported the updated server running and
followed the normal Player-access instructions with FollowersMax left at 5:

* With the sixth bull still controlled, saying claim returned all five stabled
  bulls, leaving six controlled bulls.
* Buying one horse from the trainer delivered the horse to that character and
  deducted gold. Both delivery and payment were explicitly confirmed.
* A second character transferred a bull to the already-over-cap character;
  Chris confirmed the bull became the recipient's pet after accepting the trade.

All three bounded client checks passed. These are user-reported results;
AccessLevel, exact follower totals after later purchases/transfers and gold
amounts were not independently captured in screenshots or a transaction log.
Named/individual claims, three-slot pets, later-mode behavior, rejection cases
and release-history regressions retain compiled-test coverage; they were not
all repeated through the client. No save/restart check was performed for this
patch. Summon/hireling/golem/ethereal and other special paths remain open.
