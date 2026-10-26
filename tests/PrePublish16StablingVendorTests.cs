using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

// Compile/run separately against the built assemblies; never during server startup.
public static class PrePublish16StablingVendorTests
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
    private static void Stable(AnimalTrainer trainer, Mobile owner, BaseCreature pet)
    {
        int gold = owner.Backpack.GetAmount(typeof(Gold));
        int level = pet.Pre16OrnerinessLevel, owners = pet.Pre16DistinctOwnerCount;
        double difficulty = pet.Pre16ControlDifficulty;
        trainer.EndStable(owner, pet);
        Assert(owner.Backpack.GetAmount(typeof(Gold)) == gold - 30, "actual trainer charges 30 gold");
        Assert(pet.IsStabled && pet.StabledBy == owner && owner.Stabled.Contains(pet), "actual trainer stores pet");
        Assert(!pet.Controlled && pet.ControlMaster == null && owner.Followers == 0, "stabling detaches follower");
        Assert(pet.Pre16OrnerinessLevel == level && pet.Pre16ControlDifficulty == difficulty, "stabling preserves control difficulty");
        Assert(pet.Pre16DistinctOwnerCount == owners, "stabling preserves owner history");
    }
    private static void Claim(AnimalTrainer trainer, Mobile owner, BaseCreature pet)
    {
        int level = pet.Pre16OrnerinessLevel;
        trainer.EndClaimList(owner, pet);
        Assert(pet.Controlled && pet.ControlMaster == owner && owner.Followers == 1, "actual trainer claim returns control");
        Assert(!pet.IsStabled && pet.StabledBy == null && !owner.Stabled.Contains(pet), "claim removes stable entry");
        Assert(pet.Pre16OrnerinessLevel == level, "claim preserves existing level");
    }
    private sealed class BuyInfo : GenericBuyInfo
    {
        private readonly int slots;
        public override int ControlSlots { get { return slots; } }
        public BuyInfo(int slots, int price, int stock) : base((Type)null, price, stock, 0, 0) { this.slots = slots; }
    }
    private static object[] Buy(AnimalTrainer vendor, int budget, int slots, int quantity, int stock, bool full)
    {
        object[] args = { new BuyItemResponse(Serial.Zero, quantity), new BuyInfo(slots, 7, stock),
            new List<BuyItemResponse>(), budget, full, 0.0 };
        typeof(BaseVendor).GetMethod("ProcessSinglePurchase", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(vendor, args);
        return args;
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
        entries["Era.PrePublish16Control"] = new Config.Entry("Era.cfg", 1, "Era", "", "PrePublish16Control", "true", true, false);
        var trainer = new AnimalTrainer((Serial)(serial++)); trainer.DefaultMobileInit();
        var owner = Owner(120); var pet = Pet(owner);
        Assert(pet.Pre16OrnerinessLevel == 0 && pet.Pre16ControlDifficulty == 71.1, "fresh bull baseline");
        Stable(trainer, owner, pet);
        owner.Followers = owner.FollowersMax;
        trainer.EndClaimList(owner, pet);
        Assert(pet.IsStabled && owner.Stabled.Contains(pet) && !pet.Controlled, "existing weighted claim restriction preserved");
        owner.Followers = 0; Claim(trainer, owner, pet);
        Stable(trainer, owner, pet); Claim(trainer, owner, pet);
        Assert(pet.Owners.Count == 1 && pet.Owners[0] == owner, "paid cycles do not change taming eligibility history");

        // Use the real release command after stabling, so storage cannot silently
        // consume the milestone needed by the next genuine release.
        Assert(new AnimalAI(pet).DoOrderRelease(), "actual first release command succeeds");
        Assert(pet.Pre16OrnerinessLevel == 1 && Math.Abs(pet.Pre16ControlDifficulty - 75.9) < 1e-9, "first release milestone still available");
        pet.SetControlMaster(owner); owner.Skills[SkillName.AnimalLore].Base = 100;
        pet.OnAfterTame(owner);
        Assert(pet.Pre16OrnerinessLevel == 0, "actual GM-Lore retame still calms");
        Stable(trainer, owner, pet); Claim(trainer, owner, pet);
        new AnimalAI(pet).DoOrderRelease();
        Assert(pet.Pre16OrnerinessLevel == 0, "repeat release remains protected after storage");

        var second = Owner(30); pet.SetControlMaster(second);
        Assert(pet.Pre16DistinctOwnerCount == 2 && pet.Owners.Count == 1, "transfer remains separate from taming owners");
        Stable(trainer, second, pet); Claim(trainer, second, pet);
        new AnimalAI(pet).DoOrderRelease();
        Assert(pet.Pre16OrnerinessLevel == 2 && Math.Abs(pet.Pre16ControlDifficulty - 90.3) < 1e-9, "second-owner release milestone still available");
        pet.SetControlMaster(second);
        pet.ClearControlMasterForStabling(); pet.SetControlMaster(second);
        Assert(pet.Pre16OrnerinessLevel == 2, "storage helper does not erase existing penalty");
        trainer.EndStable(second, pet); // All 30 gold were already spent.
        Assert(pet.Controlled && !pet.IsStabled && pet.Pre16OrnerinessLevel == 2, "failed unpaid stable leaves pet and history intact");

        // Auto-stable is off at LBR. Exercise its caller in the supported later
        // expansion + independent pre-P16 switch combination as well.
        var player = new PlayerMobile((Serial)(serial++)); player.DefaultMobileInit();
        Field(player, typeof(PlayerMobile), "m_AutoStabled", new List<Mobile>());
        var autoPet = Pet(player);
        player.AutoStablePets();
        Assert(autoPet.Controlled && !autoPet.IsStabled, "LBR auto-stable remains disabled");
        Core.Expansion = Expansion.SE; player.AutoStablePets();
        Assert(autoPet.IsStabled && player.AutoStabled.Contains(autoPet) && !autoPet.Controlled, "actual auto-stable caller stores pet");
        Assert(autoPet.Pre16OrnerinessLevel == 0, "auto-stable does not record release");
        trainer.EndClaimList(player, autoPet);
        Assert(autoPet.Controlled && autoPet.ControlMaster == player && autoPet.Pre16OrnerinessLevel == 0, "auto-stabled pet claim preserves history");
        Core.Expansion = Expansion.LBR;

        foreach (bool pre16 in new[] { true, false })
        {
            Config.Set("Era.PrePublish16", pre16);
            foreach (int budget in new[] { -5, -1, 0, 1, 5 })
            {
                object[] args = Buy(trainer, budget, 0, 2, 10, true);
                Assert(((List<BuyItemResponse>)args[2]).Count == 1 && (double)args[5] == 14, "ordinary goods admitted with normal price");
                Assert((int)args[3] == budget && (bool)args[4], "ordinary goods preserve slot budget and completion state");
                args = Buy(trainer, budget, 1, 2, 10, true);
                bool expected = budget >= 2;
                Assert((((List<BuyItemResponse>)args[2]).Count == 1) == expected, "positive-slot pet admission retained");
                Assert((int)args[3] == (expected ? budget - 2 : budget) && (bool)args[4] == expected, "pet reservation/rejection retained");
            }
            object[] clamped = Buy(trainer, -1, 0, 5, 2, true);
            Assert(((List<BuyItemResponse>)clamped[2]).Count == 1 && (double)clamped[5] == 14, "ordinary stock clamp and price retained");
            object[] empty = Buy(trainer, -1, 0, 1, 0, true);
            Assert(((List<BuyItemResponse>)empty[2]).Count == 0 && (double)empty[5] == 0, "empty stock rejected");
            object[] zero = Buy(trainer, -1, 0, 0, 10, true);
            Assert(((List<BuyItemResponse>)zero[2]).Count == 0 && (double)zero[5] == 0, "zero quantity rejected");
            object[] partial = Buy(trainer, -1, 0, 1, 10, false);
            Assert(((List<BuyItemResponse>)partial[2]).Count == 1 && !(bool)partial[4], "ordinary line does not erase previous partial-purchase state");
        }
        Config.Set("Era.PrePublish16", false);
        var laterPet = Pet(owner);
        laterPet.ClearControlMasterForStabling(); laterPet.SetControlMaster(owner); laterPet.SetControlMaster(null);
        Assert(!laterPet.Controlled && laterPet.Pre16OrnerinessLevel == 0, "later-era ownership behavior retained");
        Console.WriteLine("PASS: " + assertions + " paid-stabling, release, auto-stabling and vendor assertions.");
    }
}
