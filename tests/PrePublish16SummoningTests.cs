using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Misc;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Eighth;

// Compiled spell admission and creature lifecycle tests; no saves or clients.
public static class PrePublish16SummoningTests
{
    private static int assertions, serial = 29000;
    private static void Assert(bool value, string message)
    {
        ++assertions;
        if (!value) throw new Exception(message);
    }
    private static BaseCreature Creature(int slots)
    {
        var pet = new BaseCreature((Serial)(serial++)); pet.DefaultMobileInit();
        typeof(BaseCreature).GetField("m_Owners", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(pet, new List<Mobile>());
        pet.ControlSlots = slots; pet.MinTameSkill = 120; return pet;
    }
    private static Spell[] Spells(Mobile owner)
    {
        return new Spell[] { new BladeSpiritsSpell(owner, null), new EnergyVortexSpell(owner, null),
            new SummonCreatureSpell(owner, null), new AirElementalSpell(owner, null),
            new EarthElementalSpell(owner, null), new FireElementalSpell(owner, null),
            new WaterElementalSpell(owner, null), new SummonDaemonSpell(owner, null) };
    }
    private static bool Summon(BaseCreature pet, Mobile owner, bool controlled, bool magery)
    {
        return BaseCreature.Summon(pet, controlled, owner, Point3D.Zero, -1, TimeSpan.FromMinutes(5), magery);
    }
    public static void Main()
    {
        typeof(World).GetField("m_Mobiles", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new Dictionary<Serial, Item>());
        var data = typeof(World).GetField("_Data", BindingFlags.Static | BindingFlags.NonPublic);
        data.SetValue(null, Activator.CreateInstance(data.FieldType));
        MapDefinitions.Configure(); Core.Expansion = Expansion.LBR; Config.Load();
        var entries = (Dictionary<string, Config.Entry>)typeof(Config).GetField("_Entries", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        entries["Era.PrePublish16"] = new Config.Entry("Era.cfg", 0, "Era", "", "PrePublish16", "true", true, false);
        entries["Era.PrePublish16Control"] = new Config.Entry("Era.cfg", 1, "Era", "", "PrePublish16Control", "true", true, false);
        foreach (Expansion era in new[] { Expansion.LBR, Expansion.SE })
        foreach (bool pre16 in new[] { true, false })
        {
            Core.Expansion = era; Config.Set("Era.PrePublish16", pre16);
            var owner = new PlayerMobile();
            Assert(owner.AccessLevel == AccessLevel.Player, "normal character");
            int[] costs = { Core.SE ? 2 : 1, Core.SE ? 2 : 1, 2, 2, 2, 4, 3, Core.SE ? 4 : 5 };
            for (int followers = 0; followers <= 12; ++followers)
            {
                owner.Followers = followers;
                var spells = Spells(owner);
                for (int i = 0; i < spells.Length; ++i)
                    Assert(spells[i].CheckCast() == (pre16 || followers + costs[i] <= owner.FollowersMax),
                        spells[i].GetType().Name + " admission at " + followers + " in " + era + "/" + pre16);
            }
            foreach (bool controlled in new[] { true, false })
            foreach (bool magery in new[] { true, false })
            foreach (int followers in new[] { 0, 4, 5, 12 })
            {
                owner.Followers = followers;
                var pet = Creature(2);
                bool expected = (pre16 && magery) || followers + 2 <= owner.FollowersMax;
                Assert(Summon(pet, owner, controlled, magery) == expected, "factory rechecks live budget");
                Assert(pet.Deleted == !expected, "refused factory deletes unused creature");
                if (expected)
                {
                    Assert(pet.Summoned && pet.SummonMaster == owner, "summon ownership retained");
                    Assert(pet.ControlMaster == (controlled ? owner : null), "controlled and autonomous summon distinction");
                    Assert(owner.Followers == followers + 2 && owner.AllFollowers.Contains(pet), "account exactly once");
                    Assert(pet.GetControlChance(owner) == 1.0, "summon obeys without taming or lore");
                    pet.Delete();
                    Assert(owner.Followers == followers && !owner.AllFollowers.Contains(pet), "deletion removes accounting exactly once");
                }
                else Assert(owner.Followers == followers, "refusal does not change budget");
            }
        }
        Core.Expansion = Expansion.LBR; Config.Set("Era.PrePublish16", true);
        var mixed = new PlayerMobile();
        var animal = Creature(1); animal.Tamable = true; animal.SetControlMaster(mixed);
        var golem = new Golem((Serial)(serial++)); golem.DefaultMobileInit();
        typeof(BaseCreature).GetField("m_Owners", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(golem, new List<Mobile>());
        golem.ControlSlots = 3; golem.SetControlMaster(mixed);
        var summons = new List<BaseCreature>();
        for (int i = 0; i < 8; ++i)
        {
            var pet = Creature(i < 3 ? 5 : 1);
            Assert(Summon(pet, mixed, i < 3, true), "multiple daemons and more than five total summons");
            summons.Add(pet);
        }
        Assert(mixed.Followers == 24 && animal.ControlMaster == mixed && golem.ControlMaster == mixed, "mixed pets and summons retain weighted accounting and ownership");
        var timerType = typeof(Spell).Assembly.GetType("Server.Spells.UnsummonTimer");
        foreach (var pet in summons)
        {
            int before = mixed.Followers, slots = pet.ControlSlots;
            var timer = Activator.CreateInstance(timerType, new object[] { mixed, pet, TimeSpan.Zero });
            var tick = timerType.GetMethod("OnTick", BindingFlags.Instance | BindingFlags.NonPublic);
            tick.Invoke(timer, null); tick.Invoke(timer, null);
            Assert(pet.Deleted && mixed.Followers == before - slots && !mixed.AllFollowers.Contains(pet), "actual expiry callback is idempotent");
        }
        Assert(mixed.Followers == 4 && animal.ControlMaster == mixed && golem.ControlMaster == mixed, "expiry leaves ordinary pets untouched");
        var unrelated = Creature(1);
        Assert(BaseCreature.Summon(unrelated, mixed, Point3D.Zero, -1, TimeSpan.FromMinutes(1)), "legacy overload still admits within budget");
        unrelated.Delete(); mixed.Followers = 5; unrelated = Creature(1);
        Assert(!BaseCreature.Summon(unrelated, mixed, Point3D.Zero, -1, TimeSpan.FromMinutes(1)) && unrelated.Deleted, "legacy callers retain cap in pre16 mode");
        Config.Set("Era.PrePublish16", false);
        var changing = new PlayerMobile();
        Assert(new SummonDaemonSpell(changing, null).CheckCast(), "daemon initially fits budget");
        changing.Followers = 1;
        var changed = Creature(5);
        Assert(!Summon(changed, changing, true, true) && changed.Deleted, "creation refuses budget consumed after cast precheck");
        Config.Set("Era.PrePublish16", true);
        var start = DateTime.UtcNow; changed = Creature(5);
        Assert(Summon(changed, changing, true, true), "same completion admitted in pre16 mode");
        var end = (DateTime)typeof(BaseCreature).GetField("m_SummonEnd", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(changed);
        Assert(end >= start.AddMinutes(5) && end <= DateTime.UtcNow.AddMinutes(5), "requested duration retained");
        changed.Delete();
        Console.WriteLine("PASS: " + assertions + " summoning assertions");
    }
}
