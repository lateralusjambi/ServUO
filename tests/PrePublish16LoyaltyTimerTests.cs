using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using Server;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

// Runs the real timer and map line-of-sight checks without a client or saves.
public static class PrePublish16LoyaltyTimerTests
{
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.NonPublic;
    private const BindingFlags Static = BindingFlags.Static | BindingFlags.NonPublic;
    private static int assertions, serial = 41000;

    private static void Assert(bool value, string message)
    {
        ++assertions;
        if (!value) throw new Exception(message);
    }

    private static void Set(Type type, object target, string field, object value)
    {
        type.GetField(field, Instance).SetValue(target, value);
    }

    private static void Place(Mobile mobile, Map map, int x, int y)
    {
        Set(typeof(Mobile), mobile, "m_Map", map);
        Set(typeof(Mobile), mobile, "m_Location", new Point3D(x, y, 0));
    }

    private static void SetWall(bool blocked)
    {
        var tiles = Map.Felucca.Tiles;
        var block = new StaticTile[8][][];
        for (int x = 0; x < 8; x++)
        {
            block[x] = new StaticTile[8][];
            for (int y = 0; y < 8; y++)
                block[x][y] = new StaticTile[0];
        }
        if (blocked)
            block[3][2] = new[] { new StaticTile(1, 0) };
        tiles.SetStaticBlock(2, 2, block);
    }

    public static void Main()
    {
        typeof(World).GetField("m_Mobiles", Static).SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", Static).SetValue(null, new Dictionary<Serial, Item>());
        MapDefinitions.Configure();
        Core.Expansion = Expansion.LBR;
        Config.Load();
        var entries = (Dictionary<string, Config.Entry>)typeof(Config).GetField("_Entries", Static).GetValue(null);
        entries["Era.PrePublish16"] = new Config.Entry("Era.cfg", 0, "Era", "", "PrePublish16", "true", true, false);
        string dir = Path.Combine(Path.GetTempPath(), "servuo-los-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        Core.DataDirectories.Add(dir);
        try
        {
            using (var f = File.Create(Path.Combine(dir, "tiledata.mul")))
                f.SetLength(0x4000 * (26 + 37) + 2 * (0x4000 / 32) * 4);
            TileData.ItemTable[1] = new ItemData("wall", TileFlag.NoShoot, 0, 0, 0, 0, 20);
            File.WriteAllBytes(Path.Combine(dir, "staidx0.mul"), new byte[0]);
            File.WriteAllBytes(Path.Combine(dir, "statics0.mul"), new byte[0]);
            Map.Felucca.Tiles.SetLandBlock(2, 2, new LandTile[64]);
            var owner = new Mobile((Serial)serial++);
            owner.DefaultMobileInit();
            Place(owner, Map.Felucca, 20, 18);

            // Attach only the fields used by the online check. No listener or client.
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var state = (NetState)FormatterServices.GetUninitializedObject(typeof(NetState));
                typeof(NetState).GetProperty("Socket").GetSetMethod(true).Invoke(state, new object[] { socket });
                Set(typeof(Mobile), owner, "m_NetState", state);
                var pet = new BaseCreature((Serial)serial++);
                pet.DefaultMobileInit();
                pet.Tamable = true;
                Place(pet, Map.Felucca, 18, 18);
                Set(typeof(BaseCreature), pet, "m_bControlled", true);
                Set(typeof(BaseCreature), pet, "m_ControlMaster", owner);
                World.AddMobile(pet);

                var timer = new LoyaltyTimer();
                var tick = typeof(LoyaltyTimer).GetMethod("OnTick", Instance);
                Action<bool, int, string> check = (hourly, expected, label) =>
                {
                    pet.Loyalty = 100;
                    Set(typeof(LoyaltyTimer), timer, "m_NextHourlyCheck", DateTime.UtcNow.AddMinutes(hourly ? -1 : 10));
                    tick.Invoke(timer, null);
                    Assert(pet.Loyalty == expected, label + ": " + pet.Loyalty);
                };

                Config.Set("Era.PrePublish16", true);
                SetWall(false);
                Assert(pet.InLOS(owner), "open line of sight");
                check(false, 100, "nearby and visible between hourly checks");
                check(true, 90, "visible hourly loss");
                SetWall(true);
                Assert(!pet.InLOS(owner), "wall blocks actual line of sight");
                check(false, 80, "wall causes faster decay");
                check(true, 80, "hourly loss does not stack with faster decay");

                Set(typeof(Mobile), owner, "m_NetState", null);
                check(false, 100, "offline owner retains old cadence");
                check(true, 90, "offline hourly loss unchanged");
                Set(typeof(Mobile), owner, "m_NetState", state);
                Place(owner, Map.Trammel, 20, 18);
                check(false, 100, "cross-facet owner retains old cadence");
                check(true, 90, "cross-facet hourly loss unchanged");
                Place(owner, Map.Felucca, 18 + Map.MaxLOSDistance + 1, 18);
                check(false, 100, "distance alone outside LOS range is not an obstruction");
                Place(owner, Map.Felucca, 20, 18);
                pet.Summoned = true;
                check(false, 100, "summons excluded");
                pet.Summoned = false;
                pet.Tamable = false;
                check(false, 100, "non-tamable followers excluded");
                pet.Tamable = true;

                Config.Set("Era.PrePublish16", false);
                check(false, 100, "later era skips non-hourly check");
                check(true, 90, "later era retains old hourly loss behind wall");
            }
        }
        finally
        {
            Core.DataDirectories.Remove(dir);
            Directory.Delete(dir, true);
        }
        Console.WriteLine("PASS: " + assertions + " loyalty timer assertions");
    }
}
