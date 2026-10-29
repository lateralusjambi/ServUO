using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

// Compile/run separately against the built assemblies; never during server startup.
public static class PrePublish16PetAcquisitionTests
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
    private sealed class TradePet : BaseCreature
    {
        public bool AcceptsMaster = true;
        public TradePet() : base((Serial)(serial++))
        {
            DefaultMobileInit(); Body=0xE8; Tamable=true; ControlSlots=1; MinTameSkill=71.1;
            Field(this, typeof(BaseCreature), "m_Owners", new List<Mobile>());
        }
        public override bool CanBeControlledBy(Mobile m) { return AcceptsMaster; }
    }
    private static Item Trade(BaseCreature pet)
    {
        var type=typeof(BaseAI).GetNestedType("TransferItem", BindingFlags.NonPublic);
        var item=(Item)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, new object[] { (Serial)(0x40000000+serial++) }, null);
        Field(item,type,"m_Creature",pet); return item;
    }
    private sealed class Sale : AnimalBuyInfo
    {
        public Sale() : base(1,typeof(TradePet),7,10,0,0) { }
        public override IEntity GetEntity()
        {
            var pet=new TradePet(); World.AddMobile(pet); return pet;
        }
    }
    private sealed class OtherBuy : GenericBuyInfo
    {
        public override int ControlSlots { get { return 2; } }
        public OtherBuy() : base((Type)null,7,10,0,0) { }
    }
    private static object[] PurchaseLine(AnimalTrainer vendor, IBuyItemInfo info, int budget, int count)
    {
        object[] args={new BuyItemResponse(Serial.Zero,count),info,new List<BuyItemResponse>(),budget,true,0.0};
        typeof(BaseVendor).GetMethod("ProcessSinglePurchase",BindingFlags.Instance | BindingFlags.NonPublic).Invoke(vendor,args);
        return args;
    }
    public static void Main()
    {
        typeof(World).GetField("m_Mobiles", BindingFlags.Static | BindingFlags.NonPublic)
            .SetValue(null, new Dictionary<Serial, Mobile>());
        typeof(World).GetField("m_Items", BindingFlags.Static | BindingFlags.NonPublic)
            .SetValue(null, new Dictionary<Serial, Item>());
        var data=typeof(World).GetField("_Data",BindingFlags.Static | BindingFlags.NonPublic);
        data.SetValue(null,Activator.CreateInstance(data.FieldType));
        MapDefinitions.Configure(); Core.Expansion=Expansion.LBR; Config.Load();
        var entries=(Dictionary<string, Config.Entry>)typeof(Config).GetField("_Entries",BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        entries["Era.PrePublish16"]=new Config.Entry("Era.cfg",0,"Era","","PrePublish16","true",true,false);
        entries["Era.PrePublish16Control"]=new Config.Entry("Era.cfg",1,"Era","","PrePublish16Control","true",true,false);
        var trainer=new AnimalTrainer((Serial)(serial++)); trainer.DefaultMobileInit();
        foreach (bool pre16 in new[] {true,false})
        {
            Config.Set("Era.PrePublish16",pre16);
            foreach (int followers in new[] {0,4,5,6,12})
            {
                var from=Owner(0); var to=Owner(0); to.Followers=followers;
                var pet=new TradePet(); pet.SetControlMaster(from); pet.Owners.Add(from);
                var item=Trade(pet); bool allowed=item.AllowSecureTrade(from,to,to,true);
                Assert(allowed==(pre16 || followers<5),"trade acceptance respects era");
                if (allowed)
                {
                    item.OnSecureTrade(from,to,to,true);
                    Assert(pet.ControlMaster==to && to.Followers==followers+1 && from.Followers==0,"actual transfer and accounting");
                    Assert(pet.Pre16OrnerinessLevel==0 && pet.Pre16DistinctOwnerCount==2 && pet.Owners.Count==1,"transfer history stays independent from tame owners");
                }
                var claimOwner=Owner(30); var stored=Pet(claimOwner); trainer.EndStable(claimOwner,stored);
                claimOwner.Followers=followers; trainer.EndClaimList(claimOwner,stored);
                Assert(stored.Controlled==(pre16 || followers<5),"individual claim respects era");
                Assert(stored.IsStabled==(!pre16 && followers>=5),"refused claim remains stored");
                Assert(stored.Pre16OrnerinessLevel==0,"claim does not consume release milestone");
                // Summons and non-tamable creatures retain their existing weighted restriction.
                stored.Summoned=true;
                Assert(trainer.CanClaim(claimOwner,stored)==(claimOwner.Followers+stored.ControlSlots<=claimOwner.FollowersMax),"summon claim restriction retained");
                stored.Summoned=false; stored.Tamable=false;
                Assert(trainer.CanClaim(claimOwner,stored)==(claimOwner.Followers+stored.ControlSlots<=claimOwner.FollowersMax),"non-tamable claim restriction retained");
            }
            var heavyFrom=Owner(0); var heavyTo=Owner(0); heavyTo.Followers=4;
            var heavy=new TradePet(); heavy.ControlSlots=3; heavy.SetControlMaster(heavyFrom);
            var heavyToken=Trade(heavy);
            Assert(heavyToken.AllowSecureTrade(heavyFrom,heavyTo,heavyTo,true)==pre16,"three-slot transfer uses era gate");
            Assert(trainer.CanClaim(heavyTo,heavy)==pre16,"three-slot claim uses era gate");
            if (pre16)
            {
                heavyToken.OnSecureTrade(heavyFrom,heavyTo,heavyTo,true);
                Assert(heavyTo.Followers==7 && heavy.ControlSlots==3,"heavy pet accounting remains weighted");
            }
            foreach (int budget in new[] {-7,-1,0,1,2,5})
            {
                var sale=new Sale(); object[] line=PurchaseLine(trainer,sale,budget,2);
                bool allowed=pre16 || budget>=2;
                Assert(((List<BuyItemResponse>)line[2]).Count==(allowed?1:0),"animal purchase line admission");
                Assert((double)line[5]==(allowed?14:0),"normal animal price");
                Assert((int)line[3]==(allowed && !pre16?budget-2:budget),"era budget reservation");
                line=PurchaseLine(trainer,new OtherBuy(),budget,2);
                Assert(((List<BuyItemResponse>)line[2]).Count==(budget>=4?1:0),"unknown positive-slot buy category stays restricted");
                sale.Amount=1; line=PurchaseLine(trainer,sale,budget,2);
                Assert((double)line[5]==(pre16 || budget>=1?7:0),"animal stock clamp retains price");
                line=PurchaseLine(trainer,sale,budget,0);
                Assert(((List<BuyItemResponse>)line[2]).Count==0,"zero purchase rejected");
            }
            // The real full transaction: gold consumption, stock, entity creation,
            // ownership and follower accounting, without a network shop UI.
            foreach (int followers in new[] {0,5,6})
            foreach (int startingGold in new[] {1,100})
            {
                var vendor=new AnimalTrainer((Serial)(serial++)); vendor.DefaultMobileInit();
                var buyer=Owner(startingGold); buyer.Followers=followers;
                var sale=new Sale(); var display=new TradePet(); World.AddMobile(display);
                Field(sale,typeof(GenericBuyInfo),"m_DisplayEntity",display);
                Field(vendor,typeof(BaseVendor),"m_ArmorBuyInfo",new ArrayList {sale});
                bool result=vendor.OnBuyItems(buyer,new List<BuyItemResponse> {new BuyItemResponse(display.Serial,2)});
                bool allowed=(pre16 || followers+2<=5) && startingGold>=14;
                Assert(result==allowed,"actual paid animal transaction admission");
                Assert(buyer.Backpack.GetAmount(typeof(Gold))==(allowed?startingGold-14:startingGold),"actual gold deduction or refusal");
                Assert(sale.Amount==(allowed?8:10),"actual stock decrement");
                Assert(buyer.Followers==followers+(allowed?2:0),"actual delivered pets counted");
                int owned=0;
                foreach (Mobile m in World.Mobiles.Values)
                    if (m is TradePet && ((TradePet)m).ControlMaster==buyer) owned++;
                Assert(owned==(allowed?2:0),"actual pet delivery and ownership");
            }
        }
        Config.Set("Era.PrePublish16",true);
        // Both remaining NPC claim entry points use the same era-aware check.
        foreach (bool byName in new[] {false,true})
        {
            var owner=Owner(150);
            owner.Skills[SkillName.AnimalTaming].Base=100;
            owner.Skills[SkillName.AnimalLore].Base=100;
            owner.Skills[SkillName.Veterinary].Base=100;
            for (int i=0;i<5;i++) { var pet=Pet(owner); pet.Name="Bull"; trainer.EndStable(owner,pet); }
            owner.Followers=5; trainer.Claim(owner,byName?"Bull":null);
            Assert(owner.Stabled.Count==0 && owner.Followers==10,"claim all/by-name works above weighted cap");
        }
        // Other accepted-trade guards remain active above the cap.
        var sender=Owner(0); var receiver=Owner(0); receiver.Followers=6;
        var guarded=new TradePet(); guarded.SetControlMaster(sender);
        var token=Trade(guarded); guarded.AcceptsMaster=false;
        Assert(!token.AllowSecureTrade(sender,receiver,receiver,true),"uncontrollable recipient still rejected");
        guarded.AcceptsMaster=true; guarded.Summoned=true;
        Assert(!token.AllowSecureTrade(sender,receiver,receiver,true),"summoned transfer still capped");
        guarded.Summoned=false; guarded.Tamable=false;
        Assert(!token.AllowSecureTrade(sender,receiver,receiver,true),"non-tamable transfer still capped");
        guarded.Tamable=true;
        guarded.Aggressors.Add(AggressorInfo.Create(sender,guarded,false));
        Assert(!token.AllowSecureTrade(sender,receiver,receiver,true),"combat guard still checked above cap");
        guarded.Aggressors.Clear();
        Assert(!token.AllowSecureTrade(receiver,sender,sender,true),"wrong owner still rejected");
        token.OnSecureTrade(sender,receiver,receiver,false);
        Assert(guarded.ControlMaster==sender,"cancelled transfer preserves ownership");
        // Genuine release after an over-cap transfer still uses existing milestones.
        var first=Owner(0); var second=Owner(0); second.Followers=6;
        var transferred=new TradePet(); transferred.SetControlMaster(first); transferred.Owners.Add(first);
        var trade=Trade(transferred);
        Assert(trade.AllowSecureTrade(first,second,second,true),"over-cap transfer before release");
        trade.OnSecureTrade(first,second,second,true); new AnimalAI(transferred).DoOrderRelease();
        Assert(transferred.Pre16OrnerinessLevel==2,"second owner genuine release milestone preserved");
        Console.WriteLine("PASS: "+assertions+" pet-acquisition assertions");
    }
}
