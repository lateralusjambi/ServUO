namespace Server.Misc
{
    public static class PrePublish16Summoning
    {
        // Explicitly selected by the eight original Magery summon spells.
        // No count cap is our documented pre-P16 reconstruction; other summon
        // systems keep their follower-budget checks. Accounting is unchanged.
        public static bool CanSummon(Mobile caster, int controlSlots, bool magerySummon)
        {
            return (magerySummon && SkillCheck.PrePublish16Rules) ||
                caster.Followers + controlSlots <= caster.FollowersMax;
        }
    }
}
