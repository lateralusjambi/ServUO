using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using Server;
using Server.Items;
using Server.Accounting;
using Server.Engines.VeteranRewards;
using Server.Misc;
using Server.Mobiles;

public static class PrePublish16EtherealTests
{
    private static int assertions, serial = 36000;

    private static void Assert(bool condition, string message)
    {
        ++assertions;
        if (!condition)
            throw new Exception(message);
    }

    private sealed class Pack : Container
    {
        public override double DefaultWeight { get { return 0; } }
        public Pack(Serial serial) : base(serial) { Layer = Layer.Backpack; }
    }

    private static Serial ItemSerial()
    {
        return (Serial)(0x40000000 + serial++);
    }

    private static PlayerMobile Player(int followers)
    {
        var player = new PlayerMobile { Player = true, Body = 400, Followers = followers };
        player.AddItem(new Pack(ItemSerial()));
        Assert(player.AccessLevel == AccessLevel.Player, "normal player");
        return player;
    }

    private static EtherealMount[] OriginalMounts()
    {
        return new EtherealMount[] { new EtherealHorse(ItemSerial()), new EtherealLlama(ItemSerial()), new EtherealOstard(ItemSerial()) };
    }

    private static void PutInPack(PlayerMobile player, EtherealMount mount)
    {
        mount.StatueID = 0x20DD;
        mount.ItemID = mount.StatueID;
        mount.TransparentMountedID = 0x3EAA;
        mount.Transparent = true;
        player.Backpack.AddItem(mount);
    }

    // Exercise item equip and dismount without requiring a UO installation.
    private static void LoadFixtureTileData()
    {
        string dir = Path.Combine(Path.GetTempPath(), "servuo-ethereal-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        Core.DataDirectories.Add(dir);
        try
        {
            using (var file = File.Create(Path.Combine(dir, "tiledata.mul")))
                file.SetLength(0x4000 * (26 + 37) + 2 * (0x4000 / 32) * 4);
            Assert(TileData.ItemTable.Length == 0x4000, "synthetic tile table loaded");
        }
        finally
        {
            Core.DataDirectories.Remove(dir);
            Directory.Delete(dir, true);
        }
    }

    private static void VerifyRewardTiers(bool pre16)
    {
        RewardSystem.SetupRewardTables();
        Assert(RewardSystem.RewardInterval == TimeSpan.FromDays(30), "30 calendar days per reward tier");
        var rider = Player(5);
        var account = new Account("ethereal-test-" + serial++, "fixture");
        rider.Account = account;

        foreach (var mount in OriginalMounts())
        {
            var entry = RewardSystem.Lists.SelectMany(l => l.Entries).Single(e => e.ItemType == mount.GetType());
            int days = !pre16 && mount is EtherealHorse ? 30 : 90;
            Assert(entry.List.Age == TimeSpan.FromDays(days), "exact catalog tier and one choice entry");
            PutInPack(rider, mount);
            mount.IsRewardItem = true;
            account.Created = DateTime.UtcNow.AddDays(1 - days);
            Assert(!RewardSystem.HasAccess(rider, entry), "claim unavailable below required account age");
            if (pre16)
                Assert(!mount.Validate(rider), "ineligible existing reward statuette cannot be used");
            account.Created = DateTime.UtcNow.AddDays(-days);
            Assert(RewardSystem.HasAccess(rider, entry), "claim available at required account age");
            if (pre16)
                Assert(mount.Validate(rider), "eligible reward statuette ignores full follower budget");
        }
    }

    public static void Main()
    {
        const BindingFlags worldField = BindingFlags.Static | BindingFlags.NonPublic;
        typeof(World).GetField("m_Mobiles", worldField).SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", worldField).SetValue(null, new Dictionary<Serial, Item>());
        var data = typeof(World).GetField("_Data", worldField);
        data.SetValue(null, Activator.CreateInstance(data.FieldType));
        LoadFixtureTileData();
        MapDefinitions.Configure();
        RaceDefinitions.Configure();
        Core.Expansion = Expansion.LBR;
        Config.Load();
        var entries = (Dictionary<string, Config.Entry>)typeof(Config).GetField("_Entries", worldField).GetValue(null);
        entries["Era.PrePublish16"] = new Config.Entry("Era.cfg", 0, "Era", "", "PrePublish16", "true", true, false);

        foreach (bool pre16 in new[] { true, false })
        {
            Config.Set("Era.PrePublish16", pre16);
            VerifyRewardTiers(pre16);
            foreach (int followers in new[] { 4, 5, 9 })
            {
                foreach (var mount in OriginalMounts())
                {
                    var rider = Player(followers);
                    Assert(!mount.Validate(rider), "loose statue is rejected");
                    PutInPack(rider, mount);
                    Assert(mount.FollowerSlots == (pre16 ? 0 : 1), "slot cost is era specific");
                    Assert(mount.Validate(rider) == (pre16 || followers < 5), $"mount admission is era specific: {pre16} {followers} {rider.FollowersMax} {mount.GetType().Name} parent={mount.Parent != null} race={rider.Race} mounted={rider.Mounted} bodymod={rider.IsBodyMod} human={rider.Body.IsHuman} reward={mount.IsRewardItem} trade={rider.HasTrade} blocked={BaseMount.GetMountPrevention(rider)}");
                    if (pre16 || followers >= 5)
                    {
                        mount.OnDoubleClick(rider);
                        Assert((mount.Rider == rider && rider.Mount == mount) == pre16, "actual click mounts immediately only under pre-P16 rules");
                        Assert(rider.Spell == null && rider.Followers == followers, "no casting or follower change on immediate ride or refused mount");
                    }
                    if (!pre16 && followers == 4)
                    {
                        mount.OnDoubleClick(rider);
                        Assert(mount.Rider == null && rider.Spell != null, "later-era click starts a timed cast");
                        EtherealMount.StopMounting(rider);
                    }
                    if (pre16)
                    {
                        mount.Rider = null;
                        Assert(!rider.Mounted && mount.Rider == null && mount.IsChildOf(rider.Backpack), "dismount returns statue");
                        Assert(rider.Followers == followers, "dismount preserves follower count");
                    }
                }
            }

            var laterMount = new EtherealKirin(ItemSerial());
            var full = Player(5);
            PutInPack(full, laterMount);
            Assert(laterMount.FollowerSlots == 1 && !laterMount.Validate(full), "other ethereals retain shared cap");
        }

        Config.Set("Era.PrePublish16", true);
        var ghost = Player(5);
        ghost.Body = 402;
        Assert(!ghost.Alive, "dead player fixture");
        var horse = new EtherealHorse(ItemSerial());
        PutInPack(ghost, horse);
        horse.OnDoubleClick(ghost);
        Assert(horse.Rider == null, "dead player cannot mount");
        Console.WriteLine("PASS: " + assertions + " ethereal mount assertions");
    }
}
