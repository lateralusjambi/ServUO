# Pre-Publish 16 reconstruction: current project baseline

Target: Lord Blackthorn's Revenge, end of April 22, 2002. Use the modern
client; enforce the historical rules on the server. All parts of Publish 16
after the cutoff remain excluded, including its July taming changes.

Working branch: `pre-p16-integration`.
Control runtime checkpoint: `ca907ece7ece3fee1addc658f1f153288ab24256`
(October 21, 2026). Bonding runtime checkpoint:
`d2765ca2f7524ba782851d0014990417a944352c` (October 25, 2026).
Dates use America/New_York.

Start with:

* [Runtime validation and limits](runtime-validation.md)
* [Provisional pet-control specification](pre-p16-control.md)
* [Bonding boundary and completed client checks](pre-p16-bonding.md)
* [Prioritized open questions and boundary defects](uncertainties.md)

## Module register

| Module | Current status | Historical confidence / remaining scope |
|---|---|---|
| Barding: Provocation, Peacemaking, Enticement | Earlier tested handlers integrated; core smoke tests passed after merge | Invocation/check architecture selected; exact range/radius/delay and criminality/damage attribution remain qualified by the earlier research ledger. Passing smoke tests does not settle them. |
| Animal Taming eligibility | Wild/one-owner/two-owner bull thresholds verified at 71.1 / 75.9 / 90.3; own former pet retamed at 0.0 | Quadratic eligibility supported; transfers must not alter this taming-owner list. |
| Animal Taming completion roll | Wrong helper name corrected; fresh-bull taming succeeded after integration | Success probabilities above eligibility remain provisional; never use the control tables as taming-success evidence. |
| Pet control probability | Provisional Runesabre-calibrated model; C# acceptance tests pass; runtime diagnostic exercised | Lookup/interpolation and difficulty/loyalty scaling are reconstruction choices, not a recovered OSI algorithm. |
| Pet ownership / orneriness / calming | Selected first/second-owner transitions and repeat-release behavior passed; nonzero state survived restart | General release/feral semantics and intermediate Lore calming odds remain provisional. |
| Bonding / bonded death / pet resurrection | Pre-P16 gate implemented; 18 compiled-assembly assertions passed; bounded client acceptance passed October 25 | No existing pet ghosts reported. No save migration; see the bonding boundary record. |
| Feeding / Animal Lore UI / follower and stable limits | Not reconciled as a complete module | Feeding semantics and remaining post-cutoff paths still require reconciliation. See the prioritized ledger. |
| Magery, combat, healing, crafting, notoriety, housing and world content | No completion established by this checkpoint | Repository-era inventories are search aids, not proof that gameplay branches are correctly gated. |

## Evidence and workflow

Keep three claims separate: a period source describes a behavior; a selected
implementation models it; a test confirms that implementation executes.
Runtime tests cannot establish historical truth by themselves.

GREEN = sufficient historical evidence; YELLOW = documented reconstruction;
RED = known post-cutoff behavior to gate out; HOLD = consequential uncertainty.
Do not silently promote a YELLOW mechanic because its tests pass.

Use dated primary sources first, then contemporary player documentation.
Later emulator code and recollections supply leads or corroboration. Do not
count related emulator descendants as independent sources.

For each change: record the chosen rule and evidence, isolate it behind the
era gate, test it and its modern negative path, then commit a bounded patch.
Preserve the tested working tree and never replace the current integration
branch with one of its older constituent branches.

`ProjectDocs` is used deliberately: the upstream `/Docs` ignore entry also
matches lowercase `docs` on Windows. These records must remain versioned.
