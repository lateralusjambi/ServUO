using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using Server;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;
using Server.Targeting;

// Standalone executable against the built server assemblies, not server startup.
public static class PrePublish16AnimalLoreTests
{
    private static int assertions, serial = 5000, rolls;
    private static bool rollResult = true;
    private static double lastMin, lastMax;
    private static void Assert(bool value, string message)
    {
        assertions++;
        if (!value) throw new Exception(message);
    }
    private static void Field(object instance, Type type, string name, object value)
    {
        type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
    }
    private sealed class Viewer : Mobile
    {
        public bool Living = true;
        public override bool Alive { get { return Living; } }
        public Viewer() : base((Serial)(serial++)) { DefaultMobileInit(); }
    }
    private sealed class Creature : BaseCreature
    {
        public FoodType Diet = FoodType.GrainsAndHay;
        public override FoodType FavoriteFood { get { return Diet; } }
        public override int Meat { get { return 1; } }
        public Creature() : base((Serial)(serial++))
        {
            DefaultMobileInit();
            Field(this, typeof(BaseCreature), "m_Owners", new List<Mobile>());
            Body = 0xE8;
            Tamable = true;
        }
    }
    private sealed class Capture : NetState
    {
        // Instances are allocated without opening a network connection below.
        private Capture() : base(default(SocketState)) { }
        public List<int> Messages;
        public List<int> Packets;
        public override void Send(Packet packet)
        {
            Packets.Add(packet.PacketID);
            if (packet.PacketID == 0xC1)
            {
                int length;
                byte[] data = packet.Compile(false, out length);
                Messages.Add((data[14] << 24) | (data[15] << 16) | (data[16] << 8) | data[17]);
            }
        }
        public void Clear() { Messages.Clear(); Packets.Clear(); Gumps.Clear(); }
    }
    private static void Era(bool pre16) { Config.Set("Era.PrePublish16", pre16); }
    private static void Target(Viewer viewer, object creature)
    {
        rolls = 0;
        AnimalLore.OnUse(viewer);
        Assert(viewer.Target != null && viewer.Target.Range == 8, "target cursor retains range");
        // Dispatch the actual handler after shared targeting validation. Distance,
        // LOS and socket I/O remain client acceptance, not simulated here.
        viewer.Target.GetType().GetMethod("OnTarget", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(viewer.Target, new[] { (object)viewer, creature });
    }
    private static int Category(int message)
    {
        if (message == 1042947 || (message >= 1042933 && message <= 1042939)) return 0;
        if (message >= 1042925 && message <= 1042932) return 1;
        if (message >= 1042940 && message <= 1042946) return 2;
        if (message == 1042948 || message == 1042949 || message == 1044477 ||
            (message >= 1042903 && message <= 1042908)) return 3;
        if (message == 1048138 || (message >= 1042909 && message <= 1042915)) return 4;
        return -1;
    }
    public static void Main()
    {
        typeof(World).GetField("m_Mobiles", BindingFlags.Static | BindingFlags.NonPublic)
            .SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", BindingFlags.Static | BindingFlags.NonPublic)
            .SetValue(null, new Dictionary<Serial, Item>());
        var worldData = typeof(World).GetField("_Data", BindingFlags.Static | BindingFlags.NonPublic);
        worldData.SetValue(null, Activator.CreateInstance(worldData.FieldType));
        MapDefinitions.Configure();
        Core.Expansion = Expansion.LBR;
        Config.Load();
        var entries = (Dictionary<string, Config.Entry>)typeof(Config)
            .GetField("_Entries", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        entries["Era.PrePublish16"] = new Config.Entry("Era.cfg", 0, "Era", "", "PrePublish16", "true", true, false);
        entries["Era.PrePublish16Control"] = new Config.Entry("Era.cfg", 1, "Era", "", "PrePublish16Control", "true", true, false);
        Mobile.SkillCheckTargetHandler = (from, skill, target, min, max) =>
        {
            Assert(skill == SkillName.AnimalLore, "correct checked skill");
            rolls++; lastMin = min; lastMax = max;
            return rollResult;
        };
        var viewer = new Viewer();
        var net = (Capture)FormatterServices.GetUninitializedObject(typeof(Capture));
        // Mobile.NetState removes states with a null socket. An unconnected
        // socket keeps the in-memory packet collector attached; Send is overridden.
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        typeof(NetState).GetProperty("Socket").SetValue(net, socket, null);
        net.Messages = new List<int>(); net.Packets = new List<int>();
        Field(net, typeof(NetState), "m_Gumps", new List<Gump>());
        Field(viewer, typeof(Mobile), "m_NetState", net);
        var creature = new Creature();
        Era(true);
        viewer.Skills[SkillName.AnimalLore].Base = 50.0;
        Target(viewer, creature);
        Assert(rolls == 1 && lastMin == 0 && lastMax == 100, "one pre-P16 skill roll");
        Assert(net.Messages.Count == 3 && net.Messages[0] == 500328, "wild target gives prompt plus two facts below GM");
        Assert(net.Gumps.Count == 0 && !net.Packets.Contains(0xB0) && !net.Packets.Contains(0xDD), "no numeric gump packet");
        net.Clear(); rollResult = false;
        Target(viewer, creature);
        Assert(rolls == 1 && net.Messages.SequenceEqual(new[] { 500328, 500334 }), "failure reveals no facts");
        rollResult = true;
        foreach (object invalid in new object[] { new object(), new Creature { Body = 0x190 }, new Creature { IsDeadPet = true } })
        {
            net.Clear(); Target(viewer, invalid);
            Assert(rolls == 0 && net.Messages.Count == 2, "invalid target does not roll or reveal traits");
            Assert(net.Messages[1] == (invalid is BaseCreature && ((BaseCreature)invalid).IsDeadPet ? 500331 : 500329), "invalid target message");
        }
        net.Clear(); viewer.Living = false;
        Target(viewer, creature);
        Assert(rolls == 0 && net.Messages.Last() == 500331, "dead viewer rejected");
        viewer.Living = true;

        creature.SetControlMaster(viewer);
        Assert(creature.Pre16DistinctOwnerCount == 1, "first known master");
        var other = new Viewer();
        creature.SetControlMaster(other);
        Assert(creature.Pre16DistinctOwnerCount == 2 && creature.Owners.Count == 0,
            "transfer tracked without altering taming eligibility owners");
        creature.SetControlMaster(viewer);
        Assert(creature.Pre16DistinctOwnerCount == 2 && creature.Pre16OrnerinessLevel == 0,
            "repeat owner and lore reads do not create release penalties");
        creature.Loyalty = 50;
        net.Clear(); Target(viewer, creature);
        Assert(net.Messages.Count == 4 && net.Messages.Last() == 1043276, "tamed report appends loyalty");
        for (int i = 0; i < 500; i++)
        {
            int[] report = PrePublish16AnimalLore.CreateReport(creature);
            Assert(report.Length == 3 && report[2] == 1043276, "two facts plus loyalty every time");
            int first = Category(report[0]), second = Category(report[1]);
            Assert(first >= 0 && second >= 0 && first != second, "two distinct valid categories");
            Assert(!report.Contains(1042940) && !report.Contains(1042946), "owner fact uses transfer-aware count");
        }
        Assert(creature.Loyalty == 50 && creature.Pre16OrnerinessLevel == 0 &&
            creature.Pre16DistinctOwnerCount == 2 && creature.Owners.Count == 0,
            "reading traits does not change loyalty, control penalties or taming history");
        var eagle = new Eagle((Serial)(serial++));
        eagle.DefaultMobileInit(); eagle.Tamable = true;
        int[] eagleFacts = { 1042947, 1042925, 1042946, 1042907, 1042908, 1042909, 1048138 };
        for (int i = 0; i < 100; i++)
        {
            int[] report = PrePublish16AnimalLore.CreateReport(eagle);
            Assert(report.Length == 2 && report.All(eagleFacts.Contains),
                "eagle reports only eligible facts, using corrected P15 feather ID");
        }
        creature.Controlled = false; creature.Tamable = false;
        for (int i = 0; i < 100; i++)
        {
            int[] report = PrePublish16AnimalLore.CreateReport(creature);
            Assert(report.Length == 2 && report.All(m => Category(m) != 1 && Category(m) != 2),
                "untamable wild target excludes loyalty, training and owner facts");
        }
        double[] cuts = { 0, 11, 26, 41, 61, 75, 86, 100 };
        for (int i = 0; i < cuts.Length; i++)
        {
            Assert(PrePublish16AnimalLore.SkillBand(cuts[i]) == i, "chosen skill band boundary");
            if (i > 0) Assert(PrePublish16AnimalLore.SkillBand(cuts[i] - .1) == i - 1, "below chosen band boundary");
        }
        Assert(PrePublish16AnimalLore.MagicMessage(0) == 1042947 && PrePublish16AnimalLore.MagicMessage(100) == 1042939, "magic endpoints");
        Assert(PrePublish16AnimalLore.OwnerMessage(0) == 1042946 && PrePublish16AnimalLore.OwnerMessage(1) == 1042940 &&
            PrePublish16AnimalLore.OwnerMessage(6) == 1042945 && PrePublish16AnimalLore.OwnerMessage(10) == 1042945, "owner endpoints and cap");
        Assert(PrePublish16AnimalLore.LoyaltyMessage(-1) == 1043271 && PrePublish16AnimalLore.LoyaltyMessage(9) == 1043271 &&
            PrePublish16AnimalLore.LoyaltyMessage(10) == 1043272 && PrePublish16AnimalLore.LoyaltyMessage(99) == 1043280 &&
            PrePublish16AnimalLore.LoyaltyMessage(101) == 1043281, "loyalty bands and clamp");

        // The P16 tier checks and numeric gump remain reachable only in later mode.
        Era(false); net.Clear();
        Target(viewer, creature);
        Assert(rolls == 0 && net.Messages.Last() == 1049674, "later-era under-GM wild restriction retained");
        viewer.Skills[SkillName.AnimalLore].Base = 100;
        net.Clear(); Target(viewer, creature);
        Assert(rolls == 0 && net.Messages.Last() == 1049675, "later-era GM untamable restriction retained");
        creature.Tamable = true;
        net.Clear(); Target(viewer, creature);
        Assert(net.Gumps.Any(g => g is AnimalLoreGump) && net.Packets.Contains(0xB0), "later-era numeric gump retained");
        Assert(!net.Messages.Any(m => Category(m) >= 0), "no descriptive report leaks into later mode");
        net.Clear(); creature.Tamable = false;
        viewer.Skills[SkillName.AnimalLore].Cap = 120;
        viewer.Skills[SkillName.AnimalLore].Base = 110;
        Target(viewer, creature);
        Assert(net.Gumps.Any(g => g is AnimalLoreGump), "later-era 110 tier still lores untamable creature");
        Era(true); net.Clear();
        Core.Expansion = Expansion.TOL;
        var laterGump = (Gump)FormatterServices.GetUninitializedObject(typeof(NewAnimalLoreGump));
        // This type-only throttle fixture never ran BaseGump's constructor.
        GC.SuppressFinalize(laterGump);
        net.Gumps.Add(laterGump);
        AnimalLore.OnUse(viewer);
        Assert(net.Messages.Last() == 500328, "pre-P16 cursor ignores later pet-training gump throttle");
        socket.Dispose();
        Console.WriteLine("PASS: " + assertions + " Animal Lore handler and report assertions.");
    }
}
