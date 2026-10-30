using Server.Mobiles;

namespace Server.Misc
{
    public static class PrePublish16PetLimits
    {
        // Ordinary pets and constructed golems predate the weighted P16 budget.
        // Multiple active golems are documented; their exact earlier maximum
        // is unresolved. This bypass is a reconstruction, not an unlimited-OSI claim.
        public static bool IgnoresFollowerCap(BaseCreature creature)
        {
            return SkillCheck.PrePublish16Rules && creature != null &&
                !creature.Summoned && !(creature is BaseHire) &&
                (creature.Tamable || creature is Golem);
        }

        public static bool CanConstructGolem(Mobile owner)
        {
            // Preserve the existing four-slot assembly check outside pre-P16.
            // Production quotas are separate from this follower-budget decision.
            return SkillCheck.PrePublish16Rules || owner.Followers + 4 <= owner.FollowersMax;
        }
    }
}
