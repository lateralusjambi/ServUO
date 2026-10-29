using Server.Mobiles;

namespace Server.Misc
{
    public static class PrePublish16PetLimits
    {
        // Ordinary tame pets predate the weighted P16 follower budget. Keep
        // unresolved summon, hireling and constructed-creature rules separate.
        public static bool IgnoresFollowerCap(BaseCreature creature)
        {
            return SkillCheck.PrePublish16Rules && creature != null && creature.Tamable &&
                !creature.Summoned && !(creature is BaseHire) && !(creature is Golem);
        }
    }
}
