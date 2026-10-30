using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

// Standalone compiled-handler tests. No shard saves or network clients are used.
public static class PrePublish16GolemTests
{
    private static int assertions, serial = 22000;
    private static void Assert(bool condition, string message)
    {
        assertions++;
        if (!condition) throw new Exception(message);
    }
    private static void Field(object target, Type type, string name, object value)
    {
        type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    }
    private sealed class Pack : Container
    {
        public override double DefaultWeight { get { return 0; } }
        public Pack() : base((Serial)(0x40000000 + serial++)) { Layer = Layer.Backpack; }
    }
    private static Mobile Owner(int followers)
    {
        var owner = new Mobile((Serial)(serial++)); owner.DefaultMobileInit();
        owner.AddItem(new Pack()); owner.Followers = followers;
        owner.Skills[SkillName.Tinkering].Base = 100;
        Assert(owner.AccessLevel == AccessLevel.Player, "normal player fixture");
        return owner;
    }
    private static Golem LoadedGolem(Mobile owner)
    {
        var pet = new Golem((Serial)(serial++)); pet.DefaultMobileInit();
        Field(pet, typeof(BaseCreature), "m_Owners", new List<Mobile>());
        pet.ControlSlots = 3; pet.SetControlMaster(owner); return pet;
    }
    private static Item Trade(BaseCreature pet)
    {
        var type = typeof(BaseAI).GetNestedType("TransferItem", BindingFlags.NonPublic);
        var item = (Item)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, new object[] { (Serial)(0x40000000 + serial++) }, null);
        Field(item, type, "m_Creature", pet); return item;
    }
    private static T Weightless<T>(T item, int amount = 1) where T : Item
    {
        // Seed loaded-item metadata directly: the public Weight setter first
        // looks up the old default in tiledata.mul, unavailable in this fixture.
        var info = typeof(Item).GetMethod("AcquireCompactInfo", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, null);
        info.GetType().GetField("m_Weight").SetValue(info, 0.0);
        if (amount > 1) item.Stackable = true;
        item.Amount = amount;
        return item;
    }
    private static ClockworkAssembly Materials(Mobile owner, bool complete)
    {
        var assembly = Weightless(new ClockworkAssembly((Serial)(0x40000000 + serial++)));
        owner.Backpack.AddItem(assembly);
        owner.Backpack.AddItem(Weightless(new PowerCrystal((Serial)(0x40000000 + serial++)), 1));
        owner.Backpack.AddItem(Weightless(new IronIngot((Serial)(0x40000000 + serial++)), 50));
        owner.Backpack.AddItem(Weightless(new BronzeIngot((Serial)(0x40000000 + serial++)), 50));
        if (complete) owner.Backpack.AddItem(Weightless(new Gears((Serial)(0x40000000 + serial++)), 5));
        return assembly;
    }
    private static int OwnedGolems(Mobile owner)
    {
        int count = 0;
        foreach (Mobile mobile in World.Mobiles.Values)
            if (mobile is Golem && ((Golem)mobile).ControlMaster == owner) count++;
        return count;
    }
    public static void Main()
    {
        typeof(World).GetField("m_Mobiles", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new Dictionary<Serial, Item>());
        var data = typeof(World).GetField("_Data", BindingFlags.Static | BindingFlags.NonPublic);
        data.SetValue(null, Activator.CreateInstance(data.FieldType));
        MapDefinitions.Configure(); PetTrainingHelper.Configure(); Core.Expansion = Expansion.LBR; Config.Load();
        var entries = (Dictionary<string, Config.Entry>)typeof(Config).GetField("_Entries", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        entries["Era.PrePublish16"] = new Config.Entry("Era.cfg", 0, "Era", "", "PrePublish16", "true", true, false);
        entries["Era.PrePublish16Control"] = new Config.Entry("Era.cfg", 1, "Era", "", "PrePublish16Control", "true", true, false);
        var trainer = new AnimalTrainer((Serial)(serial++)); trainer.DefaultMobileInit();
        foreach (bool pre16 in new[] { true, false })
        {
            Config.Set("Era.PrePublish16", pre16);
            foreach (int followers in new[] { 0, 1, 2, 3, 5, 6, 12 })
            {
                var maker = Owner(followers); var assembly = Materials(maker, true);
                assembly.OnDoubleClick(maker);
                bool created = pre16 || followers + 4 <= maker.FollowersMax;
                Assert(assembly.Deleted == created && OwnedGolems(maker) == (created ? 1 : 0), "actual assembly creation uses era gate");
                Assert(maker.Followers == followers + (created ? 3 : 0), "created golem retains three-slot accounting");
                Assert(maker.Backpack.GetAmount(typeof(IronIngot)) == (created ? 0 : 50), "refusal preserves materials");
                var sender = Owner(0); var recipient = Owner(followers); var pet = LoadedGolem(sender);
                var token = Trade(pet); bool accepted = token.AllowSecureTrade(sender, recipient, recipient, true);
                Assert(accepted == (pre16 || followers + 3 <= recipient.FollowersMax), "golem trade uses actual three-slot budget in later mode");
                if (accepted)
                {
                    token.OnSecureTrade(sender, recipient, recipient, true);
                    Assert(pet.ControlMaster == recipient && sender.Followers == 0 && recipient.Followers == followers + 3, "transfer ownership and accounting");
                }
                Assert(trainer.CanClaim(maker, pet) == (pre16 || maker.Followers + 3 <= maker.FollowersMax), "golem reclaim follows same era policy");
                pet.Summoned = true;
                Assert(!PrePublish16PetLimits.IgnoresFollowerCap(pet), "spell-summoned golems remain excluded");
            }
            foreach (bool skilled in new[] { true, false })
            {
                var maker = Owner(0); if (!skilled) maker.Skills[SkillName.Tinkering].Base = 59.9;
                var assembly = Materials(maker, !skilled); // missing gears OR insufficient skill
                assembly.OnDoubleClick(maker);
                Assert(!assembly.Deleted && OwnedGolems(maker) == 0 && maker.Followers == 0, "skill and materials still required");
                Assert(maker.Backpack.GetAmount(typeof(IronIngot)) == 50, "failed requirements consume no iron");
            }
        }
        Config.Set("Era.PrePublish16", true);
        var owner = Owner(0);
        Materials(owner, true).OnDoubleClick(owner);
        Materials(owner, true).OnDoubleClick(owner);
        Assert(OwnedGolems(owner) == 2 && owner.Followers == 6, "two actual creations on same normal character");
        var animal = new BaseCreature((Serial)(serial++)); animal.DefaultMobileInit();
        Field(animal, typeof(BaseCreature), "m_Owners", new List<Mobile>());
        animal.Tamable = true; animal.ControlSlots = 1; animal.SetControlMaster(owner);
        Materials(owner, true).OnDoubleClick(owner);
        Assert(OwnedGolems(owner) == 3 && animal.ControlMaster == owner && owner.Followers == 10, "mixed ordinary pet and constructed golems retain ownership");
        var outside = Materials(owner, true); outside.Internalize(); outside.OnDoubleClick(owner);
        Assert(!outside.Deleted && OwnedGolems(owner) == 3, "assembly must remain in backpack");
        var petForTrade = LoadedGolem(owner); var other = Owner(6); var cancelled = Trade(petForTrade);
        cancelled.OnSecureTrade(owner, other, other, false);
        Assert(petForTrade.ControlMaster == owner, "cancelled golem transfer preserves ownership");
        Assert(!Trade(petForTrade).AllowSecureTrade(other, owner, owner, true), "wrong owner rejected");
        petForTrade.Aggressors.Add(AggressorInfo.Create(owner, petForTrade, false));
        Assert(!Trade(petForTrade).AllowSecureTrade(owner, other, other, true), "combat transfer guard retained");
        Console.WriteLine("PASS: " + assertions + " golem assertions");
    }
}
