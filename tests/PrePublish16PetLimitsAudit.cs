using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
public static class PetLimitsAudit
{
 static int serial=9000;
 static Mobile Owner() { var m=new Mobile((Serial)(serial++));m.DefaultMobileInit();return m; }
 static BaseCreature Pet() {var c=new BaseCreature((Serial)(serial++));c.DefaultMobileInit();typeof(BaseCreature).GetField("m_Owners",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(c,new List<Mobile>());c.Tamable=true;c.MinTameSkill=71.1;return c;}
 public static void Main()
 {
  typeof(World).GetField("m_Mobiles",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,new Dictionary<Serial,Mobile>());
  typeof(World).GetField("m_Items",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,new Dictionary<Serial,Item>());
  var data=typeof(World).GetField("_Data",BindingFlags.Static|BindingFlags.NonPublic);data.SetValue(null,Activator.CreateInstance(data.FieldType));
  MapDefinitions.Configure();Core.Expansion=Expansion.LBR;Config.Load();
  var entries=(Dictionary<string,Config.Entry>)typeof(Config).GetField("_Entries",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
  entries["Era.PrePublish16"]=new Config.Entry("Era.cfg",0,"Era","","PrePublish16","true",true,false);
  entries["Era.PrePublish16Control"]=new Config.Entry("Era.cfg",1,"Era","","PrePublish16Control","true",true,false);
  Console.WriteLine("era="+SkillCheck.PrePublish16Rules+", expansion="+Core.Expansion+", autoStable="+PlayerMobile.PetAutoStable);
  var owner=Owner();
  double[,] cases={{0,0,0},{53.3,53.3,53.3},{60,50,50},{80,60,60},{80,80,80},{100,100,100}};
  for(int i=0;i<cases.GetLength(0);i++){owner.Skills[SkillName.AnimalTaming].Base=cases[i,0];owner.Skills[SkillName.AnimalLore].Base=cases[i,1];owner.Skills[SkillName.Veterinary].Base=cases[i,2];Console.WriteLine("stable skills="+cases[i,0]+"/"+cases[i,1]+"/"+cases[i,2]+" max="+AnimalTrainer.GetMaxStabled(owner));}
  var trainer=new AnimalTrainer((Serial)(serial++));var pet=Pet();pet.ControlSlots=1;
  owner.Followers=5;Console.WriteLine("CanClaim at 5 slots + 1="+trainer.CanClaim(owner,pet));
  owner.Followers=0;pet.SetControlMaster(owner);Console.WriteLine("pre-stable level="+pet.Pre16OrnerinessLevel+" D="+pet.Pre16ControlDifficulty);
  pet.Internalize();pet.ClearControlMasterForStabling();pet.SummonMaster=null;pet.IsStabled=true;pet.StabledBy=owner;
  Console.WriteLine("replayed EndStable ownership sequence: level="+pet.Pre16OrnerinessLevel+" D="+pet.Pre16ControlDifficulty);
  pet.SetControlMaster(owner);pet.IsStabled=false;pet.StabledBy=null;
  Console.WriteLine("replayed claim: level="+pet.Pre16OrnerinessLevel+" D="+pet.Pre16ControlDifficulty);
  var buyer=Owner();for(int i=0;i<6;i++)Pet().SetControlMaster(buyer);
  Console.WriteLine("six ownership assignments: Followers="+buyer.Followers+" max="+buyer.FollowersMax);
  var valid=new List<BuyItemResponse>();var info=new GenericBuyInfo((Type)null,1,10,0,0);
  object[] args={new BuyItemResponse(Serial.Zero,1),info,valid,buyer.FollowersMax-buyer.Followers,true,0.0};
  typeof(BaseVendor).GetMethod("ProcessSinglePurchase",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(trainer,args);
  Console.WriteLine("ordinary goods, slots="+info.ControlSlots+", free="+args[3]+", accepted lines="+valid.Count+", fullPurchase="+args[4]);
 }
}
