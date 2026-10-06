using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx;
using Jotunn.Managers;
using UnityEngine;

namespace Moonforged.BannerCollection
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class MoonforgedBannerCollection : BaseUnityPlugin
    {
        public const string PluginGUID = "Moonforged.BannerCollection";
        public const string PluginName = "Moonforged Banner Collection";
        public const string PluginVersion = "1.0.0";

        private const string BundleResourcePath = "Moonforged.BannerCollection.mb_windows";

        private AssetBundle bannersBundle;

        private void Awake()
        {
            BannerConfigManager.Init(PluginGUID, Config);

            bannersBundle = EmbeddedAssetBundleLoader.LoadBundle(BundleResourcePath);

            if (bannersBundle == null)
            {
                Logger.LogError("Failed to load embedded AssetBundle: " + BundleResourcePath);
                return;
            }

            BannerRegistrar.InitConfig(Config);

            PrefabManager.OnPrefabsRegistered += OnPrefabsRegistered;
        }

        private void OnDestroy()
        {
            PrefabManager.OnPrefabsRegistered -= OnPrefabsRegistered;
        }

        private void OnPrefabsRegistered()
        {
            StartCoroutine(DelayedRegister(bannersBundle));
        }

        private IEnumerator DelayedRegister(AssetBundle bundle)
        {
            while (ZNetScene.instance == null)
            {
                yield return null;
            }

            BannerRegistrar.RegisterAllBanners(bundle);
        }
    }

    public static class EmbeddedAssetBundleLoader
    {
        public static AssetBundle LoadBundle(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    Debug.LogError("AssetBundle resource not found: " + resourcePath);
                    return null;
                }

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return AssetBundle.LoadFromMemory(memoryStream.ToArray());
                }
            }
        }
    }
}
