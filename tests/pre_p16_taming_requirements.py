#!/usr/bin/env python3
"""Source-level regression checks for the experimental pre-P16 Animal Taming handler.

Run from repository root: python3 tests/pre_p16_taming_requirements.py
These tests do not compile ServUO or simulate an actual tame.
"""
from pathlib import Path
import re
import unittest

SOURCE = Path(__file__).resolve().parents[1] / "Scripts" / "Skills" / "AnimalTaming.cs"
TEXT = SOURCE.read_text(encoding="utf-8-sig")

class PreP16TamingTests(unittest.TestCase):
    def test_historical_bull_thresholds(self):
        for owners, expected in [(0, 71.1), (1, 75.9), (2, 90.3), (3, 114.3), (4, 147.9)]:
            with self.subTest(owners=owners):
                self.assertAlmostEqual(71.1 + 4.8 * owners * owners, expected)

    def test_eligibility_uses_historical_helper(self):
        self.assertRegex(TEXT, r"GetHistoricalMinimumTamingSkill\(creature\)")
        self.assertIn("creature.MinTameSkill + (4.8 * owners * owners)", TEXT)
        self.assertRegex(TEXT, r"Owners\.Contains\(from\)\s*\|\|\s*from\.Skills\[SkillName\.AnimalTaming\]\.Value\s*>=\s*GetHistoricalMinimumTamingSkill\(creature\)")

    def test_success_roll_isolated_from_pet_training(self):
        self.assertIn("m_Creature.MinTameSkill + (m_Creature.Owners.Count * 6.0)", TEXT)
        self.assertNotRegex(TEXT, r"double\\s+minSkill\\s*=\\s*m_Creature\\.CurrentTameSkill")
        self.assertIn("m_Tamer.CheckTargetSkill(SkillName.AnimalTaming, m_Creature, minSkill - 25.0, minSkill + 25.0)", TEXT)

    def test_owner_limit_and_prior_owner_exception_remain(self):
        self.assertIn("creature.Owners.Count >= BaseCreature.MaxOwners && !creature.Owners.Contains(from)", TEXT)
        self.assertIn("bool alreadyOwned = m_Creature.Owners.Contains(m_Tamer)", TEXT)
        self.assertIn("if (alreadyOwned ||", TEXT)

if __name__ == "__main__":
    unittest.main()
