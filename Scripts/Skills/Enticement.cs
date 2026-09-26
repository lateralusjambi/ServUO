using System;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.SkillHandlers
{
public class Enticement
{
public static void Initialize()
{
SkillInfo.Table[(int)SkillName.Enticement].Callback = new SkillUseCallback(OnUse);
}

	public static TimeSpan OnUse(Mobile m)
	{
		m.RevealingAction();

		BaseInstrument.PickInstrument(m, new InstrumentPickedCallback(OnPickedInstrument));

		return TimeSpan.FromSeconds(10.0);
	}

	public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
	{
		from.RevealingAction();

		from.SendAsciiMessage("Whom do you wish to entice?");
		from.Target = new EnticementTarget(from, instrument);
	}

	private class EnticementTarget : Target
	{
		private BaseInstrument m_Instrument;
		private bool m_SetSkillTime = true;

		public EnticementTarget(Mobile from, BaseInstrument instrument)
			: base(BaseInstrument.GetBardRange(from, SkillName.Enticement), false, TargetFlags.None)
		{
			m_Instrument = instrument;
		}

		protected override void OnTargetFinish(Mobile from)
		{
			if (m_SetSkillTime)
				from.NextSkillTime = Core.TickCount;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			from.RevealingAction();

			if (targeted == from)
			{
				from.SayTo(from, true, "You cannot entice yourself!");
			}
			else if (!m_Instrument.IsChildOf(from.Backpack))
			{
				from.SendAsciiMessage(
					"The instrument you are trying to play is no longer in your backpack!");
			}
			else if (targeted is Mobile)
			{
				Mobile targ = (Mobile)targeted;

				m_SetSkillTime = false;
				from.NextSkillTime = Core.TickCount + 10000;

				if (!BaseInstrument.CheckMusicianship(from))
				{
					targ.SayTo(
						targ,
						true,
						"You hear lovely music, and for a moment are drawn towards it.");

					targ.SayTo(
						from,
						true,
						"Your music fails to attract them.");

					m_Instrument.PlayInstrumentBadly(from);
					m_Instrument.ConsumeUse(from);
				}
				else if (!from.CheckSkill(SkillName.Enticement, 0.0, 100.0))
				{
					targ.SayTo(
						targ,
						true,
						"You hear lovely music, and for a moment are drawn towards it.");

					targ.SayTo(
						from,
						true,
						"Your music fails to attract them.");

					m_Instrument.PlayInstrumentBadly(from);
					m_Instrument.ConsumeUse(from);
				}
				else
				{
					m_Instrument.PlayInstrumentWell(from);
					m_Instrument.ConsumeUse(from);

					targ.SayTo(
						targ,
						true,
						"You hear lovely music, and are drawn towards it...");

					from.SayTo(
						from,
						true,
						"You play your hypnotic music, luring them near.");

					if (targ is PlayerMobile)
					{
						targ.SayTo(
							from,
							true,
							"What am I hearing?");

						targ.SayTo(
							from,
							true,
							"You might have better luck with sweet words.");
					}
					else if (targ is BaseVendor)
					{
						targ.SayTo(
							from,
							true,
							"What am I hearing?");

						targ.SayTo(
							from,
							true,
							"Oh, but I cannot wander too far from my shop!");
					}
					else if (targ is BaseCreature)
					{
						((BaseCreature)targ).TargetLocation =
							new Point2D((IPoint2D)from.Location);
					}
				}
			}
			else
			{
				from.SendAsciiMessage("You cannot entice that!");
			}
		}
	}
}

}
