using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
    public static class PrePublish16ControlProbe
    {
        public static void Initialize()
        {
            CommandSystem.Register("PreP16Control", AccessLevel.GameMaster, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target a creature, then the player whose control chance you want to inspect.");
            e.Mobile.Target = new CreatureTarget();
        }

        private class CreatureTarget : Target
        {
            public CreatureTarget() : base(-1, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseCreature creature = targeted as BaseCreature;
                if (creature == null)
                {
                    from.SendMessage("Target a creature.");
                    return;
                }
                from.Target = new PlayerTarget(creature);
            }
        }

        private class PlayerTarget : Target
        {
            private readonly BaseCreature m_Creature;
            public PlayerTarget(BaseCreature creature) : base(-1, false, TargetFlags.None)
            {
                m_Creature = creature;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Mobile player = targeted as Mobile;
                if (player == null || !player.Player || m_Creature.Deleted)
                {
                    from.SendMessage("Target a player; the creature must still exist.");
                    return;
                }
                // Evaluate the player's skills without the GM account bypass.
                from.SendMessage("Provisional control enabled: {0}; Taming {1:F1}, Lore {2:F1}",
                    BaseCreature.Pre16ControlEnabled,
                    player.Skills[SkillName.AnimalTaming].Value,
                    player.Skills[SkillName.AnimalLore].Value);
                from.SendMessage("Difficulty {0:F1}; orneriness {1}; loyalty {2}/{3}; player chance {4:F2}%",
                    m_Creature.Pre16ControlDifficulty, m_Creature.Pre16OrnerinessLevel,
                    m_Creature.Loyalty, BaseCreature.MaxLoyalty,
                    m_Creature.GetPre16PlayerControlChance(player, false) * 100.0);
                from.SendMessage("Read-only calculation; skills, loyalty and ownership were not changed.");
            }
        }
    }
}
