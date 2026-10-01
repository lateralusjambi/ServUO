#region References
using System;

using Server.Items;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.SkillHandlers
{
    public class Provocation
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Provocation].Callback = OnUse;
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
            from.SendLocalizedMessage(501587); // Whom do you wish to incite?
            from.Target = new InternalFirstTarget(instrument);
        }

        public class InternalFirstTarget : Target
        {
            private readonly BaseInstrument m_Instrument;

            public InternalFirstTarget(BaseInstrument instrument)
                // No bard-specific range is imposed here. Publish 16's
                // skill-scaled GetBardRange() does not belong in this snapshot.
                : base(-1, false, TargetFlags.None)
            {
                m_Instrument = instrument;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                from.RevealingAction();

                BaseCreature creature = targeted as BaseCreature;

                if (creature == null || !from.CanBeHarmful(creature, true))
                {
                    from.SendLocalizedMessage(501589); // You can't incite that!
                    return;
                }

                if (!m_Instrument.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
                    return;
                }

                if (from is PlayerMobile && creature.Controlled)
                {
                    from.SendLocalizedMessage(501590); // They are too loyal to their master to be provoked.
                    return;
                }

                if (creature.Unprovokable)
                {
                    from.SendLocalizedMessage(1049446); // You have no chance of provoking those creatures.
                    return;
                }

                from.SendLocalizedMessage(1008085);
                // You play your music and your target becomes angered. Whom do you wish them to attack?
                from.Target = new InternalSecondTarget(m_Instrument, creature);
            }
        }

        public class InternalSecondTarget : Target
        {
            private readonly BaseCreature m_Creature;
            private readonly BaseInstrument m_Instrument;

            public InternalSecondTarget(BaseInstrument instrument, BaseCreature creature)
                // See InternalFirstTarget: ordinary targeting/client visibility
                // constrains selection; no Publish 16 bard range is applied.
                : base(-1, false, TargetFlags.None)
            {
                m_Instrument = instrument;
                m_Creature = creature;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                from.RevealingAction();

                BaseCreature target = targeted as BaseCreature;

                if (target == null)
                {
                    from.SendLocalizedMessage(501589); // You can't incite that!
                    return;
                }

                if (!m_Instrument.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
                    return;
                }

                if (m_Creature.Deleted || target.Deleted || m_Creature.Map != target.Map)
                {
                    from.SendLocalizedMessage(501589); // You can't incite that!
                    return;
                }

                if (m_Creature == target)
                {
                    from.SendLocalizedMessage(501593); // You can't tell someone to attack themselves!
                    return;
                }

                if (m_Creature.Unprovokable || target.Unprovokable)
                {
                    from.SendLocalizedMessage(1049446); // You have no chance of provoking those creatures.
                    return;
                }

                if (!from.CanBeHarmful(m_Creature, true) || !from.CanBeHarmful(target, true))
                    return;

                // Pre-Publish 16 barding uses two ordinary sequential checks:
                // Musicianship first, then Provocation. There is no creature
                // difficulty, averaged difficulty, +/-25 window, mastery bonus,
                // exceptional/slayer modifier, or instrument wear.
                if (from.Player && !BaseInstrument.CheckMusicianship(from))
                {
                    from.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
                    m_Instrument.PlayInstrumentBadly(from);
                    return;
                }

                if (!from.CheckSkill(SkillName.Provocation, 0.0, 100.0))
                {
                    from.SendLocalizedMessage(501599); // Your music fails to incite enough anger.
                    m_Instrument.PlayInstrumentBadly(from);

                    // Pre-Publish 16 failure behavior: the first creature may
                    // turn its anger on the bard.
                    m_Creature.Combatant = from;
                    return;
                }

                from.SendLocalizedMessage(501602); // Your music succeeds, as you start a fight.
                m_Instrument.PlayInstrumentWell(from);

                // Deliberately do not call BaseCreature.Provoke(). The modern
                // method creates reciprocal BardProvoked state, a 30-second
                // BardEndTime, forced focus, and bard damage attribution.
                // Historical Provocation initiates A -> B combat and then
                // ordinary creature combat/AI owns the encounter.
                m_Creature.Combatant = target;
            }
        }
    }
}
