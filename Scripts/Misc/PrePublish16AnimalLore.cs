using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Misc
{
    // Publish 14 descriptive reports, before Publish 16's numeric lore display.
    // Exact bands, skill aggregation and trait weights remain reconstruction:
    // see ProjectDocs/pre-p16-animal-lore.md. IDs refer to client localization.
    public static class PrePublish16AnimalLore
    {
        public static int[] CreateReport(BaseCreature creature)
        {
            var traits = new List<int[]>();
            traits.Add(new[] { MagicMessage(creature.Skills[SkillName.Magery].Base) });

            if (creature.Tamable)
            {
                double training = (creature.Skills[SkillName.Anatomy].Base +
                    creature.Skills[SkillName.Wrestling].Base + creature.Skills[SkillName.Tactics].Base +
                    creature.Skills[SkillName.MagicResist].Base + creature.Skills[SkillName.Meditation].Base +
                    creature.Skills[SkillName.Magery].Base + creature.Skills[SkillName.EvalInt].Base) / 7.0;
                traits.Add(new[] { 1042925 + SkillBand(training) });
                traits.Add(new[] { OwnerMessage(creature.Pre16DistinctOwnerCount) });
            }

            traits.Add(DietMessages(creature));
            traits.Add(ResourceMessages(creature));

            // Sample categories without replacement; alternatives within a category
            // do not give that category extra weight. Always produce two facts.
            int first = Utility.Random(traits.Count);
            int second = Utility.Random(traits.Count - 1);
            if (second >= first)
                second++;

            int[] report = new int[creature.Controlled ? 3 : 2];
            report[0] = Pick(traits[first]);
            report[1] = Pick(traits[second]);
            if (creature.Controlled)
                report[2] = LoyaltyMessage(creature.Loyalty);
            return report;
        }

        private static int Pick(int[] choices)
        {
            return choices[Utility.Random(choices.Length)];
        }

        public static int SkillBand(double value)
        {
            if (value >= 100.0) return 7;
            if (value >= 86.0) return 6;
            if (value >= 75.0) return 5;
            if (value >= 61.0) return 4;
            if (value >= 41.0) return 3;
            if (value >= 26.0) return 2;
            if (value >= 11.0) return 1;
            return 0;
        }

        public static int MagicMessage(double magery)
        {
            int band = SkillBand(magery);
            return band == 0 ? 1042947 : 1042932 + band;
        }

        public static int OwnerMessage(int owners)
        {
            if (owners <= 0) return 1042946;
            return 1042939 + Math.Min(owners, 6);
        }

        public static int LoyaltyMessage(int loyalty)
        {
            return 1043271 + Math.Max(0, Math.Min(BaseCreature.MaxLoyalty, loyalty)) / 10;
        }

        private static int[] DietMessages(BaseCreature creature)
        {
            var choices = new List<int>();
            FoodType food = creature.FavoriteFood;
            if ((food & FoodType.FruitsAndVegies) != 0)
            {
                choices.Add(1042904);
                choices.Add(1042948);
            }
            if ((food & FoodType.GrainsAndHay) != 0)
            {
                choices.Add(1042903);
                choices.Add(1042905);
                choices.Add(1042906);
            }
            if ((food & FoodType.Meat) != 0) choices.Add(1042907);
            if ((food & FoodType.Fish) != 0) choices.Add(1042908);
            // The period list has no separate egg descriptor. Retain a truthful
            // client food label for egg-eaters rather than invent period wording.
            if ((food & FoodType.Eggs) != 0) choices.Add(1044477);
            if (choices.Count == 0) choices.Add(1042949);
            return choices.ToArray();
        }

        private static int[] ResourceMessages(BaseCreature creature)
        {
            var choices = new List<int>();
            if (creature is Sheep) choices.Add(1042912);
            if (creature is PackHorse || creature is PackLlama || creature is Beetle)
                choices.Add(1042911);
            if (creature.Feathers > 0) choices.Add(1048138);
            if (creature.Hides > 0) choices.Add(1042910);
            if (creature.Meat > 0) choices.Add(1042909);
            if (creature is Reaper || creature is Bogling || creature is BogThing || creature is Corpser)
                choices.Add(1042914);
            if (choices.Count == 0) choices.Add(1042915);
            return choices.ToArray();
        }
    }
}
