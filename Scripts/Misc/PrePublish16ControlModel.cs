using System;

namespace Server.Misc
{
    // A provisional reconstruction calibrated to the 13 published Runesabre rows.
    // This is NOT a recovered OSI formula. See docs/pre-p16-control.md.
    public static class PrePublish16ControlModel
    {
        private static readonly double[] TamingKnots = { 0, 50, 90, 100 };
        private static readonly double[][] LoreKnots =
        {
            new double[] { 0, 60, 100 },
            new double[] { 0, 60 },
            new double[] { 0, 20 },
            new double[] { 0, 20, 40, 60, 80, 100 }
        };
        private static readonly double[][] Percentages =
        {
            new double[] { 0, 1, 2 },
            new double[] { 14, 30 },
            new double[] { 58, 63 },
            new double[] { 70, 77, 83, 87, 95, 98 }
        };

        // effectiveDifficulty includes orneriness, independently of taming eligibility.
        // Ratio normalization outside D=100 is a documented provisional choice.
        public static double Chance(double taming, double lore, double effectiveDifficulty,
            int loyalty, int maxLoyalty)
        {
            if (Double.IsNaN(taming) || Double.IsInfinity(taming) ||
                Double.IsNaN(lore) || Double.IsInfinity(lore) ||
                Double.IsNaN(effectiveDifficulty) || Double.IsInfinity(effectiveDifficulty))
                throw new ArgumentOutOfRangeException("Skills and difficulty must be finite.");
            if (maxLoyalty <= 0)
                throw new ArgumentOutOfRangeException("maxLoyalty");
            if (effectiveDifficulty <= 0)
                throw new ArgumentOutOfRangeException("effectiveDifficulty");

            double t = Math.Max(0, taming) * 100.0 / effectiveDifficulty;
            double l = Math.Max(0, lore) * 100.0 / effectiveDifficulty;
            // Beyond the table, cap each row before interpolation so more Taming
            // never reduces control, even after extrapolating a sparse Lore row.
            double[] rows = new double[TamingKnots.Length];
            for (int i = 0; i < rows.Length; i++)
                rows[i] = Clamp(Interpolate(l, LoreKnots[i], Percentages[i]), 0, 98);
            double percentage = Clamp(Interpolate(t, TamingKnots, rows), 0, 98);
            double loyaltyFactor = Clamp((double)loyalty / maxLoyalty, 0, 1);
            return percentage * 0.01 * loyaltyFactor;
        }

        public static double Difficulty(double minimum, int orneriness)
        {
            double level = Math.Max(0, orneriness);
            return minimum + 4.8 * level * level;
        }

        private static double Interpolate(double value, double[] x, double[] y)
        {
            int upper = 1;
            while (upper < x.Length - 1 && value > x[upper])
                upper++;
            int lower = upper - 1;
            return y[lower] + (value - x[lower]) *
                (y[upper] - y[lower]) / (x[upper] - x[lower]);
        }

        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    // Separate from Owners.Count: transfers count for control history, not tame eligibility.
    public sealed class PrePublish16Orneriness
    {
        public int Level { get; private set; }
        public int PenalizedOwners { get; private set; }

        public PrePublish16Orneriness() : this(0, 0) { }

        public PrePublish16Orneriness(int level, int penalizedOwners)
        {
            Level = Math.Max(0, level);
            PenalizedOwners = Math.Max(0, penalizedOwners);
        }

        public void Release(int distinctOwners)
        {
            if (distinctOwners <= PenalizedOwners)
                return;
            // One release penalty per new ownership milestone; repeat releases by
            // the same ownership set do not keep increasing the difficulty.
            Level = (int)Math.Min(Int32.MaxValue, (long)Level + distinctOwners);
            PenalizedOwners = distinctOwners;
        }

        public bool Calm()
        {
            if (Level == 0)
                return false;
            Level--;
            return true;
        }
    }
}
