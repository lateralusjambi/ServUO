using System;
using Server.Misc;

public static class PrePublish16ControlTests
{
    private static int assertions;
    private static void Assert(bool condition, string message)
    {
        assertions++;
        if (!condition) throw new Exception(message);
    }
    private static void Equal(double actual, double expected, string message)
    {
        Assert(Math.Abs(actual - expected) < 1e-10, message + ": " + actual + " != " + expected);
    }

    public static void Main()
    {
        // Published acceptance data, not generated from the interpolation knots.
        double[,] runesabre = {
            {0,0,0}, {0,60,1}, {0,100,2}, {50,0,14}, {50,60,30},
            {90,0,58}, {90,20,63}, {100,0,70}, {100,20,77},
            {100,40,83}, {100,60,87}, {100,80,95}, {100,100,98}
        };
        for (int i = 0; i < runesabre.GetLength(0); i++)
            Equal(PrePublish16ControlModel.Chance(runesabre[i,0], runesabre[i,1], 100, 100, 100),
                runesabre[i,2] / 100.0, "Runesabre row " + i);

        // Full domain checks detect discontinuities and non-monotone sparse-row extensions.
        foreach (double difficulty in new double[] { 29.2, 35.1, 71.1, 93.9, 100, 119.2, 250 })
        {
            for (int t = 0; t <= 120; t++)
            for (int l = 0; l <= 120; l++)
            {
                double p = PrePublish16ControlModel.Chance(t, l, difficulty, 100, 100);
                Assert(p >= 0 && p <= .98000000001, "bounds");
                Assert(PrePublish16ControlModel.Chance(t+1, l, difficulty, 100, 100) >= p-1e-12, "Taming monotonic");
                Assert(PrePublish16ControlModel.Chance(t, l+1, difficulty, 100, 100) >= p-1e-12, "Lore monotonic");
                Assert(PrePublish16ControlModel.Chance(t, l, difficulty+1, 100, 100) <= p+1e-12, "difficulty monotonic");
                Equal(PrePublish16ControlModel.Chance(t, l, difficulty, 50, 100), p / 2, "loyalty scaling");
                Equal(PrePublish16ControlModel.Chance(t, l, difficulty, 0, 100), 0, "zero loyalty");
            }
        }

        var state = new PrePublish16Orneriness();
        state.Release(1);
        Assert(state.Level == 1 && state.PenalizedOwners == 1, "first release");
        state.Release(1);
        Assert(state.Level == 1, "repeat release must not accumulate");
        Assert(state.Calm() && state.Level == 0 && !state.Calm(), "calming floor");
        state.Release(1);
        Assert(state.Level == 0, "old ownership milestone stays consumed after calming");
        state.Release(2);
        Assert(state.Level == 2, "second owner release");
        Assert(state.Calm() && state.Level == 1, "second owner calming");
        Equal(PrePublish16ControlModel.Difficulty(71.1, state.Level), 75.9, "control difficulty");
        var restored = new PrePublish16Orneriness(state.Level, state.PenalizedOwners);
        restored.Release(2);
        Assert(restored.Level == 1, "restored milestone");
        Equal(PrePublish16ControlModel.Difficulty(71.1, 0), 71.1, "no penalty");
        Equal(PrePublish16ControlModel.Difficulty(71.1, 2), 90.3, "quadratic penalty");
        Console.WriteLine("PASS: 13/13 Runesabre rows; " + assertions + " total assertions.");
    }
}
