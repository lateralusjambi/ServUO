using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

public static class PrePublish16HirelingTests
{
    private const BindingFlags InstanceField = BindingFlags.Instance | BindingFlags.NonPublic;
    private static int assertions, serial = 31000;

    private static void Assert(bool condition, string message)
    {
        ++assertions;
        if (!condition)
            throw new Exception(message);
    }

    private static T Init<T>(T creature) where T : BaseCreature
    {
        creature.DefaultMobileInit();
        typeof(BaseCreature).GetField("m_Owners", InstanceField).SetValue(creature, new List<Mobile>());
        return creature;
    }

    private static BaseHire Hireling()
    {
        var hire = Init(new BaseHire((Serial)serial++));
        hire.ControlSlots = 2;
        typeof(BaseHire).GetField("m_Pay", InstanceField).SetValue(hire, 60);
        return hire;
    }

    private static Gold Payment(int amount)
    {
        var gold = new Gold((Serial)(0x40000000 + serial++));
        var info = typeof(Item).GetMethod("AcquireCompactInfo", InstanceField).Invoke(gold, null);
        info.GetType().GetField("m_Weight").SetValue(info, 0.0);
        gold.Stackable = true;
        gold.Amount = amount;
        return gold;
    }

    private static BaseEscortable Escort()
    {
        var escort = Init(new BaseEscortable((Serial)serial++));
        typeof(BaseEscortable).GetField("m_DestinationString", InstanceField).SetValue(escort, "Jhelom");
        typeof(BaseEscortable).GetField("m_Destination", InstanceField)
            .SetValue(escort, new EscortDestinationInfo("Jhelom", null));
        return escort;
    }

    public static void Main()
    {
        const BindingFlags worldField = BindingFlags.Static | BindingFlags.NonPublic;
        typeof(World).GetField("m_Mobiles", worldField).SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", worldField).SetValue(null, new Dictionary<Serial, Item>());
        var data = typeof(World).GetField("_Data", worldField);
        data.SetValue(null, Activator.CreateInstance(data.FieldType));
        MapDefinitions.Configure();
        Core.Expansion = Expansion.LBR;
        Config.Load();
        var entries = (Dictionary<string, Config.Entry>)typeof(Config).GetField("_Entries", worldField).GetValue(null);
        entries["Era.PrePublish16"] = new Config.Entry("Era.cfg", 0, "Era", "", "PrePublish16", "true", true, false);

        var heldGold = typeof(BaseHire).GetField("m_HoldGold", InstanceField);
        var payTimer = typeof(BaseHire).GetField("m_PayTimer", InstanceField);
        foreach (bool pre16 in new[] { true, false })
        {
            Config.Set("Era.PrePublish16", pre16);
            var owner = new PlayerMobile();
            Assert(owner.AccessLevel == AccessLevel.Player && owner.FollowersMax == 5, "ordinary player");
            var pet = Init(new BaseCreature((Serial)serial++));
            pet.Tamable = true;
            pet.ControlSlots = 5;
            pet.SetControlMaster(owner);
            Assert(owner.Followers == 5 && pet.ControlMaster == owner, "pet fills follower budget");

            for (int i = 0; i < 2; ++i)
            {
                var hire = Hireling();
                var payment = Payment(60);
                int before = owner.Followers, wages = (int)heldGold.GetValue(hire);
                Assert(hire.OnDragDrop(owner, payment) == pre16, "paid hire admission follows era");
                Assert(hire.Controlled == pre16 && hire.ControlMaster == (pre16 ? owner : null), "ownership follows admission");
                Assert(owner.Followers == before + (pre16 ? 2 : 0), "weighted accounting is retained");
                Assert((int)heldGold.GetValue(hire) == wages + (pre16 ? 60 : 0), "refusal cannot credit wages");
                Assert((payTimer.GetValue(hire) != null) == pre16, "only accepted hire starts payday timer");
                if (!pre16)
                    Assert(!payment.Deleted && payment.Amount == 60, "refused gold stays intact for bounce");
                else
                    Assert(!hire.OnDragDrop(new PlayerMobile(), Payment(60)) && hire.ControlMaster == owner,
                        "another player cannot buy an already employed hireling");
            }

            var below = new PlayerMobile();
            var candidate = Hireling();
            Assert(!candidate.OnDragDrop(below, Payment(59)) && candidate.ControlMaster == null, "insufficient gold rejected");
            Assert(candidate.OnDragDrop(below, Payment(60)) && below.Followers == 2, "ordinary paid hire still works");

            var available = Hireling();
            Assert(!available.IsHumanInTown(), "fixture represents an unguarded location");
            var speaker = new PlayerMobile { Player = true };
            speaker.Location = new Point3D(4, 0, 0);
            Assert(!available.HandlesOnSpeech(speaker), "hireling does not listen at four tiles");
            speaker.Location = new Point3D(3, 0, 0);
            Assert(available.HandlesOnSpeech(speaker) == pre16, "nearby normal speech reaches unguarded hireling only in pre-P16 mode");
            var greeting = new SpeechEventArgs(speaker, "unrelated", MessageType.Regular, 0, new int[0]);
            var request = new SpeechEventArgs(speaker, "hire", MessageType.Regular, 0, new[] { 0x0162 });
            if (available.HandlesOnSpeech(speaker))
            {
                available.OnSpeech(greeting);
                available.OnSpeech(request);
            }
            Assert(!greeting.Handled, "unrelated speech is not consumed");
            Assert(request.Handled == pre16, "hire request reaches wage quote handler");
            Assert(!available.Controlled && available.ControlMaster == null, "quote does not hire NPC");
            Assert(!candidate.HandlesOnSpeech(speaker), "unrelated player gets no new speech access to an employed hireling");

            var first = Escort();
            var second = Escort();
            Assert(first.AcceptEscorter(owner), "escort can start independently of hiring");
            owner.LastEscortTime = DateTime.MinValue;
            Assert(!second.AcceptEscorter(owner) && BaseEscortable.EscortTable[owner] == first,
                "second escort still refused after removing cooldown as a cause");
        }
        Console.WriteLine("PASS: " + assertions + " hireling and escort assertions");
    }
}
