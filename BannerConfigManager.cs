using BepInEx.Configuration;

namespace Moonforged.BannerCollection
{

    public static class BannerConfigManager
    {
        public static void Init(string modName, ConfigFile config)
        {
        }

        public static ConfigEntry<T> AddEntry<T>(ConfigFile cfg, string section, string key, T defaultValue, string description)
        {
            return cfg.Bind(section, key, defaultValue, description);
        }
    }
}
