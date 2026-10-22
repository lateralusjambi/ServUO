#region References
using System;

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.SkillHandlers
{
    public class Peacemaking
    {
        // The exact April 22, 2002 area radius remains unresolved.
        // Keeping it isolated here prevents a provisional reconstruction
        // value from becoming entangled with the skill architecture.
        private const int PeaceRange = 12;

        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Peacemaking].Callback = OnUse;
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            BaseInstrument.PickInstrument(m, OnPickedInstrument);

            // Pre-Publish 16: the reuse timer begins when the skill is invoked.
            // Ten seconds is retained provisionally until the exact April 2002
            // reuse delay is resolved.
            return TimeSpan.FromSeconds(10.0);
        }

        public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
        {
            from.RevealingAction();

            if (!instrument.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
                return;
            }

            // Pre-Publish 16 Peacemaking is an immediate area skill. There is
            // no target cursor and no persistent BardPacified state.
            if (from.Player && !BaseInstrument.CheckMusicianship(from))
            {
                from.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
                instrument.PlayInstrumentBadly(from);
                return;
            }

            if (!from.CheckSkill(SkillName.Peacemaking, 0.0, 100.0))
            {
                from.SendLocalizedMessage(500613); // You attempt to calm everyone, but fail.
                instrument.PlayInstrumentBadly(from);
                return;
            }

            instrument.PlayInstrumentWell(from);

            Map map = from.Map;

            if (map == null)
                return;

            bool calmed = false;
            IPooledEnumerable eable = from.GetMobilesInRange(PeaceRange);

            foreach (Mobile mobile in eable)
            {
                if (mobile == from)
                    continue;

                BaseCreature creature = mobile as BaseCreature;

                if ((creature != null && (creature.Uncalmable || creature.AreaPeaceImmune)) ||
                    !from.CanBeHarmful(mobile, false))
                {
                    continue;
                }

                if (mobile.Combatant == null && !mobile.Warmode)
                    continue;

                calmed = true;

                mobile.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!
                mobile.Combatant = null;
                mobile.Warmode = false;
            }

            eable.Free();

            if (calmed)
                from.SendLocalizedMessage(500615); // You play your hypnotic music, stopping the battle.
            else
                from.SendLocalizedMessage(1049648); // You play hypnotic music, but there is nothing in range for you to calm.
        }

        // Retained for callers elsewhere in ServUO. Historical Peacemaking
        // itself never creates BardPacified state.
        public static bool UnderEffects(Mobile m)
        {
            return m is BaseCreature && ((BaseCreature)m).BardPacified;
        }
    }
}
