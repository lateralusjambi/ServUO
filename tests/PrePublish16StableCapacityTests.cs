using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

// Compile/run separately against the built assemblies; never during server startup.
public static class PrePublish16StableCapacityTests
{
    private static int assertions, serial = 12000;
    private static void Assert(bool value, string message)
    {
        assertions++;
        if (!value) throw new Exception(message);
    }
    private static void Field(object value, Type type, string name, object state)
    {
        type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, state);
    }
    private sealed class Pack : Container
    {
        public override double DefaultWeight { get { return 0; } }
        public Pack() : base((Serial)(0x40000000 + serial++)) { Layer = Layer.Backpack; }
    }
    private static Mobile Owner(int gold)
    {
        var owner = new Mobile((Serial)(serial++));
        owner.DefaultMobileInit();
        var pack = new Pack();
        owner.AddItem(pack);
        if (gold > 0)
            pack.AddItem(new Gold((Serial)(0x40000000 + serial++)) { Stackable = true, Amount = gold });
        return owner;
    }
    private static BaseCreature Pet(Mobile owner)
    {
        var pet = new BaseCreature((Serial)(serial++));
        pet.DefaultMobileInit();
        Field(pet, typeof(BaseCreature), "m_Owners", new List<Mobile>());
        pet.Body = 0xE8;
        pet.Tamable = true;
        pet.MinTameSkill = 71.1;
        pet.ControlSlots = 1;
        pet.SetControlMaster(owner);
        pet.Owners.Add(owner);
        return pet;
    }
    private static void Skills(Mobile m, double t, double l, double v)
    {
        m.Skills[SkillName.AnimalTaming].Cap = 120;
        m.Skills[SkillName.AnimalLore].Cap = 120;
        m.Skills[SkillName.Veterinary].Cap = 120;
        m.Skills[SkillName.AnimalTaming].Base = t;
        m.Skills[SkillName.AnimalLore].Base = l;
        m.Skills[SkillName.Veterinary].Base = v;
    }
    public static void Main()
    {
        typeof(World).GetField("m_Mobiles", BindingFlags.Static | BindingFlags.NonPublic)
            .SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", BindingFlags.Static | BindingFlags.NonPublic)
            .SetValue(null, new Dictionary<Serial, Item>());
        var data = typeof(World).GetField("_Data", BindingFlags.Static | BindingFlags.NonPublic);
        data.SetValue(null, Activator.CreateInstance(data.FieldType));
        MapDefinitions.Configure(); Core.Expansion = Expansion.LBR; Config.Load();
        var entries = (Dictionary<string, Config.Entry>)typeof(Config)
            .GetField("_Entries", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        entries["Era.PrePublish16"] = new Config.Entry("Era.cfg", 0, "Era", "", "PrePublish16", "true", true, false);
        entries["Era.PrePublish16Control"] = new Config.Entry("Era.cfg", 1, "Era", "", "PrePublish16Control", "false", true, false);
        var player = new PlayerMobile((Serial)(serial++)); player.DefaultMobileInit();
        Field(player, typeof(PlayerMobile), "m_AutoStabled", new List<Mobile>());
        Server.Spells.SkillMasteries.MasteryInfo.Infos = new List<Server.Spells.SkillMasteries.MasteryInfo> {
            new Server.Spells.SkillMasteries.MasteryInfo(null,744,SkillName.AnimalTaming,Server.Spells.SkillMasteries.PassiveSpell.Boarding) };
        player.Skills.CurrentMastery = SkillName.AnimalTaming;
        player.Skills[SkillName.AnimalTaming].VolumeLearned = 3;
        Assert(Server.Spells.SkillMasteries.MasteryInfo.BoardingSlotIncrease(player)==3, "nonzero stored boarding fixture");
        double[,] cases = { {0,0,0,2}, {60,50,49.9,2}, {60,50,50,3},
            {80,60,59.9,3}, {80,60,60,4}, {80,80,79.9,4}, {80,80,80,5},
            {100,100,100,5}, {110,110,110,5}, {120,120,120,5}, {100,0,0,2} };
        foreach (Expansion expansion in new[] { Expansion.LBR, Expansion.SA, Expansion.TOL })
        {
            Core.Expansion = expansion;
            for (int i = 0; i < cases.GetLength(0); i++)
            {
                Skills(player, cases[i,0], cases[i,1], cases[i,2]);
                player.Skills.CurrentMastery = SkillName.AnimalTaming;
                foreach (int rewards in new[] { 0, 7 })
                {
                    player.RewardStableSlots = rewards;
                    Assert(AnimalTrainer.GetMaxStabled(player) == (int)cases[i,3], "original tiers independent of later bonuses");
                    Assert(player.RewardStableSlots == rewards, "stored rewards preserved");
                }
            }
        }
        // Retain the existing modified-skill calculation, including skill mods.
        Core.Expansion = Expansion.LBR; Skills(player, 50,50,50);
        var mod = new DefaultSkillMod(SkillName.AnimalTaming, true, 10);
        player.AddSkillMod(mod);
        Assert(AnimalTrainer.GetMaxStabled(player) == 3, "existing modified-skill semantics retained");
        player.RemoveSkillMod(mod);
        Config.Set("Era.PrePublish16", false);
        foreach (Expansion expansion in new[] { Expansion.LBR, Expansion.SA, Expansion.TOL })
        {
            Core.Expansion = expansion;
            foreach (int skill in new[] { 100,110,120 })
            {
                Skills(player,skill,skill,skill);
                player.Skills.CurrentMastery = SkillName.AnimalTaming;
                foreach (int rewards in new[] { 0,7 })
                {
                    player.RewardStableSlots = rewards;
                    int expected = 5 + 3 * ((skill-90)/10) + rewards + 3 + (Core.SA ? 2 : 0) + (Core.TOL ? 2 : 0);
                    Assert(AnimalTrainer.GetMaxStabled(player) == expected, "later-mode bonuses preserved: " + expansion + " skill=" + skill + " expected=" + expected + " actual=" + AnimalTrainer.GetMaxStabled(player));
                }
            }
        }
        Core.Expansion = Expansion.LBR; Config.Set("Era.PrePublish16", true);
        var trainer = new AnimalTrainer((Serial)(serial++)); trainer.DefaultMobileInit();
        foreach (int limit in new[] { 2,3,4,5 })
        {
            var owner = Owner(300);
            Skills(owner, limit == 2 ? 0 : 80, limit == 2 ? 0 : 80, limit == 2 ? 0 : (limit-3)*40);
            Assert(AnimalTrainer.GetMaxStabled(owner) == limit, "fixture tier");
            for (int i = 0; i < limit; i++)
            {
                var pet = Pet(owner); trainer.EndStable(owner, pet);
                Assert(pet.IsStabled && owner.Stabled.Count == i+1, "paid admission up to tier limit");
            }
            var extra = Pet(owner); int gold = owner.Backpack.GetAmount(typeof(Gold));
            trainer.EndStable(owner,extra);
            Assert(extra.Controlled && !extra.IsStabled && owner.Stabled.Count == limit, "next pet rejected without changing ownership");
            Assert(owner.Backpack.GetAmount(typeof(Gold)) == gold, "full stable charges no fee");
        }
        // Simulate eight already-stabled pets from the old GM bonus calculation.
        var legacy = Owner(0); Skills(legacy,100,100,100);
        for (int i=0; i<8; i++)
        {
            var pet = Pet(legacy); pet.ClearControlMasterForStabling();
            pet.IsStabled=true; pet.StabledBy=legacy; legacy.Stabled.Add(pet);
        }
        Assert(AnimalTrainer.GetMaxStabled(legacy)==5 && legacy.Stabled.Count==8, "lower capacity preserves all existing pets");
        while (legacy.Stabled.Count>0)
        {
            int count=legacy.Stabled.Count; var pet=(BaseCreature)legacy.Stabled[0];
            trainer.EndClaimList(legacy,pet);
            Assert(legacy.Stabled.Count==count-1 && pet.ControlMaster==legacy && !pet.IsStabled, "over-cap pet individually recoverable");
            pet.ClearControlMasterForStabling(); // Free weighted follower budget for next claim.
        }
        Console.WriteLine("PASS: " + assertions + " stable-capacity assertions");
    }
}
