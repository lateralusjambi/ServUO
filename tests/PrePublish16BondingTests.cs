using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

// Compile against the built ServUO.exe, Ultima.dll and Scripts.dll; run separately
// from server startup. Fixtures live on Map.Internal and need no client data.
public static class PrePublish16BondingTests
{
    private static int assertions;
    private static int serial = 1000;
    private static void Assert(bool condition, string message)
    {
        assertions++;
        if (!condition) throw new Exception(message);
    }
    private static void Raw(BaseCreature pet, string field, object value)
    {
        typeof(BaseCreature).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(pet, value);
    }
    private sealed class Pet : BaseCreature
    {
        // Deliberately bypass IsBondable's base implementation: feeding must still
        // honor the global era gate even if a derived creature opts into bonding.
        public override bool IsBondable { get { return true; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public int Resurrections;
        public Pet() : base((Serial)(serial++))
        {
            DefaultMobileInit();
            Raw(this, "m_Owners", new List<Mobile>());
            RawStr = RawDex = RawInt = 100;
            NoKillAwards = true;
        }
        public override void OnAfterResurrect() { Resurrections++; }
        public override int GetDeathSound() { return -1; }
    }
    private static RawRibs Food()
    {
        var food = new RawRibs((Serial)(0x40000000 + serial++));
        // A loaded item needs no TileData lookup just to seed its stack amount.
        typeof(Item).GetField("m_Amount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(food, 1);
        return food;
    }
    private static Pet Owned(Mobile owner)
    {
        var pet = new Pet();
        Raw(pet, "m_bControlled", true);
        Raw(pet, "m_ControlMaster", owner);
        return pet;
    }
    public static void Main()
    {
        // Bootstrap empty in-memory world indexes without loading or writing Saves.
        typeof(World).GetField("m_Mobiles", BindingFlags.NonPublic | BindingFlags.Static)
            .SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", BindingFlags.NonPublic | BindingFlags.Static)
            .SetValue(null, new Dictionary<Serial, Item>());
        var data = typeof(World).GetField("_Data", BindingFlags.NonPublic | BindingFlags.Static);
        data.SetValue(null, Activator.CreateInstance(data.FieldType));
        MapDefinitions.Configure();
        Core.Expansion = Expansion.LBR;
        Config.Load();
        // Seed a scoped entry when no Era.cfg exists beside the test executable.
        // Config.Set can then update the same entry as it does in a configured server.
        var entries = (Dictionary<string, Config.Entry>)typeof(Config)
            .GetField("_Entries", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        entries["Era.PrePublish16"] = new Config.Entry("Era.cfg", 0, "Era", "", "PrePublish16", "true", true, false);
        entries["Era.PrePublish16Control"] = new Config.Entry("Era.cfg", 1, "Era", "", "PrePublish16Control", "false", false, false);
        Config.Set("Era.PrePublish16", true);
        Assert(!BaseCreature.BondingEnabled, "pre-P16 disables bonding independently of control model");
        var normal = new BaseCreature((Serial)(serial++));
        Assert(!normal.IsBondable, "ordinary creature not bondable");
        var pet = new Pet();
        pet.IsBonded = true;
        pet.BondingBegin = DateTime.UtcNow.AddDays(-8);
        Assert(!pet.IsBonded && pet.BondingBegin == DateTime.MinValue, "direct assignment cannot opt in");
        Raw(pet, "m_IsBonded", true);
        Raw(pet, "m_BondingBegin", DateTime.UtcNow.AddDays(-8));
        Assert(!pet.IsBonded && pet.BondingBegin == DateTime.MinValue, "loaded raw fields cannot opt in");
        pet.IsDeadPet = true;
        pet.ResurrectPet();
        Assert(pet.IsDeadPet && pet.Resurrections == 0, "pet resurrection blocked before effects/hooks");

        var owner = new Mobile();
        pet = Owned(owner);
        var food = Food();
        Assert(pet.CheckFeed(owner, food) && food.Deleted, "accepted food consumed under pre-P16");
        Assert(!pet.IsBonded && pet.BondingBegin == DateTime.MinValue, "feeding cannot start bonding through override");
        Raw(pet, "m_BondingBegin", DateTime.UtcNow.AddDays(-8));
        food = Food();
        Assert(pet.CheckFeed(owner, food) && food.Deleted, "second feeding still works");
        Assert(!pet.IsBonded, "feeding cannot complete old bonding timer");

        // Exercise the actual death handler with a dormant saved bonded flag.
        pet = new Pet();
        Raw(pet, "m_IsBonded", true);
        var corpse = new Container((Serial)(0x40000000 + serial++));
        pet.OnDeath(corpse);
        Assert(pet.Deleted && !pet.IsDeadPet, "pre-P16 death deletes creature rather than making pet ghost");

        Config.Set("Era.PrePublish16", false);
        Assert(BaseCreature.BondingEnabled && normal.IsBondable, "later-era base bonding retained");
        pet = Owned(owner);
        food = Food();
        Assert(pet.CheckFeed(owner, food), "later-era feeding works");
        Assert(pet.BondingBegin != DateTime.MinValue && !pet.IsBonded, "later-era feeding starts timer");
        pet.BondingBegin = DateTime.UtcNow.AddDays(-8);
        food = Food();
        Assert(pet.CheckFeed(owner, food), "later-era second feeding works");
        Assert(pet.IsBonded && pet.BondingBegin == DateTime.MinValue, "later-era feeding completes bond");
        pet.OnDeath(new Container((Serial)(0x40000000 + serial++)));
        Assert(pet.IsDeadPet && !pet.Deleted, "later-era bonded death creates ghost");
        pet.ResurrectPet();
        Assert(!pet.IsDeadPet && pet.Resurrections == 1 && pet.Hits == 10, "later-era pet resurrection retained");
        pet = new Pet();
        pet.OnDeath(new Container((Serial)(0x40000000 + serial++)));
        Assert(pet.Deleted && !pet.IsDeadPet, "later-era unbonded death remains ordinary");
        Console.WriteLine("PASS: " + assertions + " bonding, feeding, death and resurrection assertions.");
    }
}
