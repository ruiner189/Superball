using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ProLib.Managers;
using ProLib.Utility;
using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using static PachinkoBall;

namespace Superball
{
    [BepInPlugin(GUID, Name, Version)]

    [BepInDependency("com.ruiner.prolib", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {

        public const String GUID = "com.ruiner.superball";
        public const String Name = "Superball";
        public const String Version = "1.0.0";

        public static ConfigEntry<float> InitialVelocity;
        public static ConfigEntry<float> VelocityGained;

        public static ConfigEntry<OrbRarity> Rarity;

        public static ConfigEntry<int> LevelOneDamage;
        public static ConfigEntry<int> LevelOneCritDamage;

        public static ConfigEntry<int> LevelTwoDamage;
        public static ConfigEntry<int> LevelTwoCritDamage;

        public static ConfigEntry<float> LevelTwoHitMultiplier;
        public static ConfigEntry<int> LevelTwoHitAmount;

        public static ConfigEntry<int> LevelThreeDamage;
        public static ConfigEntry<int> LevelThreeCritDamage;
        public static ConfigEntry<float> LevelThreeHitMultiplier;
        public static ConfigEntry<int> LevelThreeHitAmount;

        private Harmony _harmony;
        public static ManualLogSource Log;
        public static ConfigFile ConfigFile;

        // Sprites
        public static Sprite Superball;
        public static Sprite SuperballAttack;


        private void Awake()
        {
            Log = Logger;
            ConfigFile = Config;

            LoadSprites();
            LoadConfig();

            _harmony = new Harmony(GUID);
            _harmony.PatchAll();

            OrbManager.Instance.RegisterAll();
        }

        private void LoadSprites()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            Superball = assembly.LoadSprite("Resources.Superball.png", 8);
            SuperballAttack = assembly.LoadSprite("Resources.Superball.png", 16);
        }

        private void LoadConfig()
        {
            InitialVelocity = Config.Bind<float>("Velocity", "IntialVelocity", 10f, "How much velocity does Superball start with.");
            VelocityGained = Config.Bind<float>("Velocity", "VelocityGained", 0.5f, "How much velocity does Superball gain per peg hit.");

            LevelOneDamage = Config.Bind<int>("Damage", "LevelOneDamage", 2);
            LevelOneCritDamage = Config.Bind<int>("Damage", "LevelOneCritDamage", 4);

            LevelTwoDamage = Config.Bind<int>("Damage", "LevelTwoDamage", 4);
            LevelTwoCritDamage = Config.Bind<int>("Damage", "LevelTwoCritDamage", 6);

            LevelThreeDamage = Config.Bind<int>("Damage", "LevelThreeDamage", 5);
            LevelThreeCritDamage = Config.Bind<int>("Damage", "LevelThreeCritDamage", 7);

            LevelTwoHitMultiplier = Config.Bind<float>("Multiplier", "LevelTwoMultiplierBonus", 0.05f, "How much additional damage is added per x pegs hit.");
            LevelTwoHitAmount = Config.Bind<int>("Multiplier", "LevelTwoHitAmount", 25, "How many pegs to hit before multiplier is applied. Stacks");

            LevelThreeHitMultiplier = Config.Bind<float>("Multiplier", "LevelThreeMultiplierBonus", 0.05f, "How much additional damage is added per x pegs hit. Stacks");
            LevelThreeHitAmount = Config.Bind<int>("Multiplier", "LevelThreeHitAmount", 25, "How many pegs to hit before multiplier is applied.");

            Rarity = Config.Bind<OrbRarity>("General", "Rarity", OrbRarity.COMMON, "How rare is the orb?");


        }

    }
}

