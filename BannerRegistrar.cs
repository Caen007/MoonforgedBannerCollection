using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace Moonforged.BannerCollection
{
    public class BannerRegistration
    {
        public string PrefabName;
        public string DisplayName;
        public RequirementConfig[] Requirements;
        public string Description;
        public string Category;
        public int Comfort;

        public BannerRegistration(string prefab, string display, RequirementConfig[] reqs, string desc, string cat, int comfort = 0)
        {
            PrefabName = prefab;
            DisplayName = display;
            Requirements = reqs;
            Description = desc;
            Category = cat;
            Comfort = comfort;
        }
    }

    public static class BannerRegistrar
    {
        private static bool wasAlreadyRegistered = false; // HOTFIX stays untouched

        private static ConfigEntry<string> BannerCategoryConfig;

        public static readonly List<BannerRegistration> AllRegistrations = new()
        {
            // ---------------------- Boss Banners ---------------------

            // 0
            new BannerRegistration("M_Eikthyr_Banner","Moonforged Eikthyr Banner", new[]{
                new RequirementConfig("Wood",10), new RequirementConfig("Raspberry",4), new RequirementConfig("LeatherScraps",5)
            },"","banner"),

            // 1
            new BannerRegistration("M_TheElder_Banner","Moonforged Elder Banner", new[]{
                new RequirementConfig("RoundLog",5), new RequirementConfig("Blueberries",4), new RequirementConfig("TrollHide",5)
            },"","banner"),

            // 2
            new BannerRegistration("M_Bonemass_Banner","Moonforged Bonemass Banner", new[]{
                new RequirementConfig("ElderBark",10), new RequirementConfig("Guck",4), new RequirementConfig("DeerHide",5)
            },"","banner"),

            // 3
            new BannerRegistration("M_Moder_Banner","Moonforged Moder Banner", new[]{
                new RequirementConfig("FineWood",10), new RequirementConfig("FreezeGland",4), new RequirementConfig("WolfPelt",5)
            },"","banner"),

            // 4
            new BannerRegistration("M_Yagluth_Banner","Moonforged Yagluth Banner", new[]{
                new RequirementConfig("FineWood",10), new RequirementConfig("Cloudberry",4), new RequirementConfig("LoxPelt",5)
            },"","banner"),

            // 5
            new BannerRegistration("M_TheQueen_Banner","Moonforged Queen Banner", new[]{
                new RequirementConfig("YggdrasilWood",5), new RequirementConfig("MushroomMagecap",4), new RequirementConfig("ScaleHide",5)
            },"","banner"),

            // 6
            new BannerRegistration("M_Fader_Banner","Moonforged Fader Banner", new[]{
                new RequirementConfig("Blackwood",10), new RequirementConfig("CharredBone",4), new RequirementConfig("Fiddleheadfern",5), new RequirementConfig("AskHide",5)
            },"","banner"),

            // 6.1
            new BannerRegistration("M_KallFimbulbringer_Banner","Moonforged Kall Fimbulbringer Banner", new[]{
                new RequirementConfig("Frostwood",10), new RequirementConfig("Ice",10), new RequirementConfig("MooseHide",5)
            },"","banner"),

            // ---------------------- Vendor Banners ---------------------

            // 7
            new BannerRegistration("M_Haldor_Banner","Moonforged Haldor Banner", new[]{
                new RequirementConfig("Wood",4), new RequirementConfig("Raspberry",4), new RequirementConfig("DeerHide",4)
            },"","banner"),

            // 8
            new BannerRegistration("M_Hildir_Banner","Moonforged Hildir Banner", new[]{
                new RequirementConfig("FineWood",4), new RequirementConfig("Cloudberry",4), new RequirementConfig("LoxPelt",4)
            },"","banner"),

            // 9
            new BannerRegistration("M_BogWitch_Banner","Moonforged Bog Witch Banner", new[]{
                new RequirementConfig("ElderBark",4), new RequirementConfig("MushroomYellow",4), new RequirementConfig("TrophyDraugr",4)
            },"","banner"),

            // ---------------------- Miniboss Banners ---------------------

            // 10
            new BannerRegistration("M_Brennan_Banner","Moonforged Brenna Banner", new[]{
                new RequirementConfig("RoundLog",4), new RequirementConfig("TrophySkeleton",4), new RequirementConfig("TrollHide",4)
            },"","banner"),

            // 11
            new BannerRegistration("M_Geirrhafa_Banner","Moonforged Geirrhafa Banner", new[]{
                new RequirementConfig("FineWood",4), new RequirementConfig("WolfFang",4), new RequirementConfig("TrophyHatchling",2)
            },"","banner"),

            // 12
            new BannerRegistration("M_ZilThungr_Banner","Moonforged Zil & Thungr Banner", new[]{
                new RequirementConfig("FineWood",4), new RequirementConfig("TrophyGoblin",1), new RequirementConfig("TrophyGoblinBrute",1), new RequirementConfig("LoxPelt",5)
            },"","banner"),

            // ---------------------- Biome Banners ---------------------

           // 13
            new BannerRegistration("M_Meadows","Moonforged Meadows Banner", new[]{
               new RequirementConfig("Wood",10), new RequirementConfig("Raspberry",4), new RequirementConfig("LeatherScraps",5)
                },"","banner"),

            // 14
            new BannerRegistration("M_BlackForest","Moonforged Black Forest Banner", new[]{
               new RequirementConfig("RoundLog",5), new RequirementConfig("Blueberries",4), new RequirementConfig("TrollHide",5)
                },"","banner"),

           // 15
            new BannerRegistration("M_Swamp","Moonforged Swamp Banner", new[]{
               new RequirementConfig("ElderBark",10), new RequirementConfig("Guck",4), new RequirementConfig("DeerHide",5)
                },"","banner"),
                        
            // 16
            new BannerRegistration("M_Mountains","Moonforged Mountains Banner", new[]{
               new RequirementConfig("FineWood",10), new RequirementConfig("FreezeGland",4), new RequirementConfig("WolfPelt",5)
                },"","banner"),

            // 17
            new BannerRegistration("M_Plains","Moonforged Plains Banner", new[]{
               new RequirementConfig("FineWood",10), new RequirementConfig("Cloudberry",4), new RequirementConfig("LoxPelt",5)
                },"","banner"),

            // 18
            new BannerRegistration("M_Mistlands","Moonforged Mistlands Banner", new[]{
               new RequirementConfig("YggdrasilWood",5), new RequirementConfig("MushroomMagecap",4), new RequirementConfig("ScaleHide",5)
                },"","banner"),

            // 19
            new BannerRegistration("M_Ashlands","Moonforged Ashlands Banner", new[]{
                new RequirementConfig("Blackwood",10), new RequirementConfig("CharredBone",4), new RequirementConfig("Fiddleheadfern",5), new RequirementConfig("AskHide",5)
                },"","banner"),

            // 20
            new BannerRegistration("M_DeepNorth","Moonforged Deepnorth Banner", new[]{
               new RequirementConfig("Frostwood",10), new RequirementConfig("Ice",10), new RequirementConfig("MooseHide",5)
                },"","banner")


        };

        public static void InitConfig(ConfigFile cfg)
        {
            BannerCategoryConfig = BannerConfigManager.AddEntry(
                cfg, "Categories", "BannerCategory",
                "Moonforged Banner Collection",
                "Hammer category name for all Moonforged banners."
            );
        }

        private static string CategoryToTab(string category) =>
            category.ToLower() switch
            {
                "banner" => BannerCategoryConfig != null ? BannerCategoryConfig.Value : "Moonforged Banner Collection",
                _ => category
            };

        public static void RegisterAllBanners(AssetBundle bundle)
        {
            if (wasAlreadyRegistered) return;
            foreach (var reg in AllRegistrations)
                RegisterBanner(bundle, reg);
            wasAlreadyRegistered = true;
        }

        private static void RegisterBanner(AssetBundle bundle, BannerRegistration reg)
        {
            if (bundle == null) return;
            GameObject prefab = bundle.LoadAsset<GameObject>(reg.PrefabName);
            if (prefab == null)
            {
                Debug.LogWarning($"[Moonforged Banner Collection] Missing prefab in AssetBundle: {reg.PrefabName}");
                return;
            }

            prefab.name = reg.PrefabName;

            var znv = prefab.GetComponent<ZNetView>() ?? prefab.AddComponent<ZNetView>();
            znv.m_persistent = true;
            znv.m_syncInitialScale = true;

            if (!prefab.GetComponent<ZSyncTransform>())
                prefab.AddComponent<ZSyncTransform>();

            if (!prefab.GetComponent<BannerIndoorClothStopper>())
                prefab.AddComponent<BannerIndoorClothStopper>();

            Piece piece = prefab.GetComponent<Piece>() ?? prefab.AddComponent<Piece>();
            piece.m_name = reg.DisplayName;
            piece.m_description = reg.Description;
            piece.m_groundOnly = false;

            GameObject vfxPlace = ZNetScene.instance?.GetPrefab("vfx_Place_wood");
            GameObject sfxPlace = ZNetScene.instance?.GetPrefab("sfx_build_hammer_wood");
            GameObject destroyVFX = ZNetScene.instance?.GetPrefab("vfx_destroyed");
            GameObject destroySFX = ZNetScene.instance?.GetPrefab("sfx_wood_break");

            var placeFX = new EffectList();
            var placeList = new List<EffectList.EffectData>();
            if (vfxPlace != null) placeList.Add(new EffectList.EffectData { m_prefab = vfxPlace });
            if (sfxPlace != null) placeList.Add(new EffectList.EffectData { m_prefab = sfxPlace });
            placeFX.m_effectPrefabs = placeList.ToArray();
            piece.m_placeEffect = placeFX;

            WearNTear wear = prefab.GetComponent<WearNTear>() ?? prefab.AddComponent<WearNTear>();
            wear.m_health = 1000f;
            wear.m_noRoofWear = true;

            var destroyFX = new EffectList();
            var destroyList = new List<EffectList.EffectData>();
            if (destroyVFX != null) destroyList.Add(new EffectList.EffectData { m_prefab = destroyVFX });
            if (destroySFX != null) destroyList.Add(new EffectList.EffectData { m_prefab = destroySFX });
            destroyFX.m_effectPrefabs = destroyList.ToArray();
            wear.m_destroyedEffect = destroyFX;

            if (reg.Comfort > 0)
                piece.m_comfort = reg.Comfort;

            Sprite icon = bundle.LoadAsset<Sprite>(reg.PrefabName);
            if (icon != null)
                piece.m_icon = icon;

            bool requiresIron = reg.Requirements.Any(r => r.Item.ToLower() == "iron");
            string craftingStation = requiresIron ? "forge" : "piece_workbench";

            var config = new PieceConfig
            {
                PieceTable = PieceTables.Hammer,
                Category = CategoryToTab(reg.Category),
                CraftingStation = craftingStation,
                Requirements = reg.Requirements
            };

            PieceManager.Instance.AddPiece(new CustomPiece(prefab, true, config));
        }
    }
}
