using ProLib.Managers;
using ProLib.Orbs;
using ProLib.Utility;
using UnityEngine;

namespace Superball
{
    public sealed class SuperballOrb : CustomOrb
    {
        public static SuperballOrb Instance { private set; get; }


        private SuperballOrb() : base("superball")
        {
            CreateLocalization();
        }

        public static void Register()
        {
            if (Instance != null) return;

            Instance = new SuperballOrb();
        }

        public override void CreatePrefabs()
        {
            CustomShotBuilder shotBuilder = new CustomShotBuilder()
                .SetName($"{GetName()}shot")
                .SetSprite(Plugin.SuperballAttack);

            GameObject normalShot = shotBuilder.Build();
            GameObject overflowShot = shotBuilder.Build();

            overflowShot.GetComponent<ShotBehavior>()._shotType = ShotBehavior.ShotType.OVERFLOW;

            CustomOrbBuilder levelOne = new CustomOrbBuilder()
                .SetName("SuperBall")
                .SetDamage(Plugin.LevelOneDamage.Value, Plugin.LevelOneCritDamage.Value)
                .SetLevel(1)
                .SetDescription("sb_increase_velocity")
                .SetRarity(Plugin.Rarity.Value)
                .SetSprite(Plugin.Superball)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .IncludeInOrbPool(true)
                .SetShot(normalShot);

            this[1] = levelOne.Build();
            this[1].AddComponent<Speed>();

            CustomOrbBuilder levelTwo = levelOne.Clone()
                .WithPrefab(this[1])
                .SetDamage(Plugin.LevelTwoDamage.Value, Plugin.LevelTwoCritDamage.Value)
                .SetDescription("sb_increase_velocity", "sb_multiplier_on_hit")
                .AddParameter("SB_MULTIPLIER", (GetMultiplier(2) - 1).ToString("P"))
                .AddParameter("SB_M_PEG_AMOUNT", GetPegHitAmount(2).ToString())
                .IncludeInOrbPool(false)
                .SetLevel(2);

            this[2] = levelTwo.Build();
            this[2].AddComponent<Multiplier>();

            CustomOrbBuilder levelThree = levelTwo.Clone()
                .WithPrefab(this[2])
                .SetDamage(Plugin.LevelThreeDamage.Value, Plugin.LevelThreeCritDamage.Value)
                .SetDescription("overflow2", "sb_random_overflow", "sb_increase_velocity", "sb_multiplier_on_hit")
                .AddParameter("SB_MULTIPLIER", (GetMultiplier(3) - 1).ToString("P"))
                .AddParameter("SB_M_PEG_AMOUNT", GetPegHitAmount(3).ToString())
                .SetLevel(3)
                .SetShot(overflowShot);

            this[3] = levelThree.Build();

            this[3].AddComponent<OverflowRandomTarget>();

            CustomOrbBuilder.JoinLevels(this[1], this[2], this[3]);
        }

        public static float GetMultiplier(int level)
        {
            float multiplier = 1;
            if(level == 2)
            {
                multiplier += Plugin.LevelTwoHitMultiplier.Value;
            } else if (level == 3)
            {
                multiplier += Plugin.LevelThreeHitMultiplier.Value;
            }

            return multiplier;
        }

        public static int GetPegHitAmount(int level)
        {
            if (level == 2) return Plugin.LevelTwoHitAmount.Value;
            if (level == 3) return Plugin.LevelThreeHitAmount.Value;
            return (int) Plugin.LevelTwoHitAmount.DefaultValue;
        }

        public void CreateLocalization()
        {
            LanguageManager manager = LanguageManager.Instance;
            manager.LoadLocalizationSource(new Localization("Orbs/superball_name", "Superball"));
            manager.LoadLocalizationSource(new Localization("Orbs/sb_increase_velocity", "Increases velocity every <sprite name=\"PEG\"> Hit"));
            manager.LoadLocalizationSource(new Localization("Orbs/sb_multiplier_on_hit", "<style=dmg_bonus>Gain {[SB_MULTIPLIER]} more damage</style> every {[SB_M_PEG_AMOUNT]} <sprite name=\"PEG\"> hit"));
            manager.LoadLocalizationSource(new Localization("Orbs/sb_random_overflow", "Attacks a random enemy. When target is hit, goes to a new random target"));
        }

        public override bool IsEnabled()
        {
            return true;
        }

    }
}
