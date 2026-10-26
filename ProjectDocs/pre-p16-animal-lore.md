# Pre-P16 Animal Lore

Target: LBR through April 22, 2002. This patch replaces the numeric Animal Lore
skill result under `Era.PrePublish16` with the descriptive Publish 14 system.
It uses the shared era switch, independently of `Era.PrePublish16Control`.

## Historical boundary

* GREEN: Publish 14, deployed November 30, 2001, describes two random facts on
  an animal, with an additional loyalty report for tamed pets. Its categories
  cover magic, training, owners, diet and useful resources. Training and owner
  descriptions are specified for tamable animals.
* GREEN: Publish 15, January 9, 2002, corrects owner-count reporting and feather
  messages. In particular, the eagle must not produce the erroneous lock text.
* GREEN: Publish 16's July taming changes add numeric pet stats/skills, armor,
  pack instincts and the 100/110 Lore target-access tiers. Those results and
  tiers are excluded from this pre-P16 handler.

Primary sources:

* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2001-2/2001-publish-14-14th-november/
  The page title says November 14; its body explicitly dates deployment to
  November 30, 2001. Use the body date for this baseline.
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/2002-publish-15-9th-january/
  Pet/hireling changes. Later updates on that same page extend beyond our
  cutoff and are not blanket approval for later changes.
* https://uo.com/wiki/ultima-online-wiki/technical/previous-publishes/2002-2/publish-16-part-2-4-23rd-july/
  Animal Lore numeric output and access restrictions.

## Implementation and explicit reconstruction limits

`AnimalLore.OnUse` retains the normal cursor and one-second handler delay;
pre-P16 mode bypasses the later pet-training-gump throttle. After existing
alive/body/dead-pet validation, the handler makes one shared target skill check
and either sends failure message 500334 or the descriptive report. It returns
before any numeric gump, modern tier check or later pet-training callback.

`PrePublish16AnimalLore.CreateReport` chooses two different eligible categories
with equal category weights. It chooses one localized message from each, then
appends a loyalty message only if the target is controlled. A report therefore
has two messages for wild creatures and three for controlled creatures.
Training and owner categories are omitted for untamable creatures. Reading
Lore does not calm a pet, add an owner, raise loyalty or change control chance.

The following are YELLOW implementation choices, not recovered OSI formulas:

| Choice | Selected implementation / limit |
|---|---|
| Success probability | Shared CheckTargetSkill, 0.0 to 100.0. This follows the pre-P16 skill ceiling and similar descriptive skills; period notes do not establish exact odds. The shared gain/anti-macro implementation is reused. |
| Magic ranking | Base Magery with boundaries 11, 26, 41, 61, 75, 86 and 100. |
| Training ranking | Mean of base Anatomy, Wrestling, Tactics, Resist, Meditation, Magery and Eval Int, with the same boundaries. This is an older Angel Island reconstruction, not independent period proof. |
| Trait sampling | Two categories without replacement, uniform category weights; uniform alternatives within a category. Period notes establish random facts, not those exact weights. |
| Owner interpretation | Known distinct control masters, including transfers, using the existing separately serialized pre-P16 history. Taming's Owners list is untouched. Exact original owner-count semantics remain qualified by the control-history ledger. Old unrecorded transfers cannot be recovered. |
| Loyalty descriptions | Existing 0-100 loyalty is mapped in ten-point bands to localized report IDs 1043271-1043281. This does not establish the historical loyalty scale or change loyalty mechanics. |
| Food granularity | Existing food flags supply appropriate alternatives. GrainsAndHay cannot distinguish individual animals that prefer grass, hay or grain. Eggs use the truthful client food label rather than invented period phrasing. No new food acceptance is added. |
| Unsupported food/resource traits | Known localized unknown-diet/unknown-use messages fill otherwise empty categories. Leather eating is not added; ServUO has no corresponding food flag in this baseline. Later special diets remain outside this patch. |
| Resource interpretation | Existing meat/hides/feathers properties and sheep/pack/wood-creature types supply alternatives. Feather output uses 1048138, not an old erroneous message. Exact original resource-selection priorities are not established. |
| Message transport | Localized system/journal messages, consistent with existing Arms Lore. Exact original overhead-versus-journal presentation is not established by the publish notes. |

Implementation leads (source code is primary for what those emulators do;
it is not primary evidence of OSI behavior):

* Older Angel Island mapping, at pinned commit:
  https://github.com/Luke-Tomasello/Angel-Island-2016-FW472/blob/3a8bed05559d0e3ea7d1a2a5e8d2a1b92fc13fba/Scripts/Skills/AnimalLore.cs
  Provides the older Magery bands and seven-skill training reconstruction.
  Its retained modern access tiers are deliberately not imported.
* Later Angel Island localized IDs, at pinned commit:
  https://github.com/Luke-Tomasello/Angel-Island-Server/blob/7cbbf405aa4182b90a6fb47f48bbd93c75f06792/scripts/Skills/AnimalLore.cs
  Identifies client message IDs, including loyalty and feathers. Its changelog
  explicitly identifies a 2023 trait rewrite and later averaging changes;
  those revisions are not treated as period evidence.

## Compatibility and validation

No save layout/version or configuration default changes. The BaseCreature
addition exposes the already stored known-owner count through its existing
history initialization. There is no new save field and no change to taming
eligibility, control formulas, bonding, food acceptance or veterinary.
With `Era.PrePublish16=false`, the existing numeric gump and 100/110 access
paths remain. Other scripts that instantiate lore gumps directly are not
removed; this is the active Animal Lore skill boundary, not a later-content
inventory or an administrator-tool restriction.

Full Ultima, Server and Scripts compilation passed. The standalone test
`tests/PrePublish16AnimalLoreTests.cs` passed 1,758 assertions against the built
assemblies. It dispatches the actual target handler, captures localized message
IDs and gump packets, and checks success/failure, dead and invalid targets,
under-GM wild access, one pre-P16 skill roll, controlled-only loyalty, distinct
fact categories, chosen band boundaries, transfer-aware owner counts, preserved
taming history and later-era gump/access behavior. It also checks that the
later pet-training throttle does not block the pre-P16 cursor.

These tests use a controlled skill-roll delegate and a packet collector with
an unconnected socket. They do not measure historical probability, validate the
real gain/anti-macro algorithm, decode a client's localized text, or exercise
shared range/LOS validation. Those claims are not implied by passing tests.

After building on Windows with assemblies in the repository root, the test can
be compiled and run separately (never during server startup):

```powershell
$fxc = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
& $fxc /nologo /out:AnimalLoreTests.exe /reference:ServUO.exe /reference:Ultima.dll /reference:Scripts.dll tests\PrePublish16AnimalLoreTests.cs
if ($LASTEXITCODE -eq 0) { .\AnimalLoreTests.exe }
```

Remove the generated test executable afterward; do not commit it.

## Pending client acceptance

1. Use a normal character with effective Animal Lore below 100 on a wild bull.
   Successful attempts should give two descriptive messages, without a numeric
   gump or the P16 tamed-only rejection. A failed attempt should reveal no facts.
2. Lore a tamed bull. Expect two facts plus a loyalty description. Repeating
   successful attempts should reveal different categories over time.
3. Lore an eagle repeatedly until resources are reported. Expect its feather
   description, never a lock message. Check one non-animal target and, if useful,
   a dead character's attempt for the appropriate rejection.

Start with step 1; a few focused client checks are sufficient. Later-era client
configuration changes are not required for this acceptance. A successful
runtime check confirms the selected implementation, not the YELLOW formulas.
