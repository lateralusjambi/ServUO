using System;

namespace Server.Misc
{
    // Five creations per approximately 24 hours is documented in November 2001.
    // Character scope, exact timing and persistence are reconstruction choices.
    public sealed class PrePublish16GolemQuota
    {
        public const int Limit = 5;
        // Wording preserved in a 2016 Sphere script (1071942), not verified in LBR.
        // Send literal text so the modern client cannot substitute revised cliloc wording.
        public const string RefusalMessage = "You are too drained to make another golem. You must wait approximately one day to construct another.";

        private int m_Creations;
        private DateTime m_ResetAt;

        public PrePublish16GolemQuota()
        {
        }

        public PrePublish16GolemQuota(GenericReader reader)
        {
            m_Creations = reader.ReadInt();
            m_ResetAt = reader.ReadDateTime();
        }

        public int CreationsAt(DateTime now)
        {
            return now >= m_ResetAt ? 0 : m_Creations;
        }

        public DateTime ResetAt { get { return m_ResetAt; } }

        public bool CanCreate(DateTime now)
        {
            return CreationsAt(now) < Limit;
        }

        public void RecordCreation(DateTime now)
        {
            if (!CanCreate(now))
                throw new InvalidOperationException("The golem production quota is exhausted.");

            if (now >= m_ResetAt)
            {
                m_Creations = 0;
                m_ResetAt = now.AddHours(24);
            }

            ++m_Creations;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_Creations);
            writer.Write(m_ResetAt);
        }
    }
}

namespace Server.Mobiles
{
    public partial class PlayerMobile
    {
        private Server.Misc.PrePublish16GolemQuota m_GolemProductionQuota = new Server.Misc.PrePublish16GolemQuota();

        public Server.Misc.PrePublish16GolemQuota GolemProductionQuota { get { return m_GolemProductionQuota; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PrePublish16GolemsCreated { get { return m_GolemProductionQuota.CreationsAt(DateTime.UtcNow); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PrePublish16GolemQuotaReset { get { return m_GolemProductionQuota.ResetAt; } }
    }
}
