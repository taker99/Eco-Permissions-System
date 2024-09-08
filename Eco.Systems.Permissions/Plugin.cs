using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Serialization;
using Eco.Core.Utils;
using Eco.Gameplay.Systems;
using Eco.Shared.Networking;
using Eco.Shared.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Eco.Systems.Permissions
{
    public class Plugin: Singleton<Plugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly string saveLocation = Path.Combine("Configs", "Mods");
        public const string appNameCon = "[ESP]";
        public const string appName = "<color=green>[ESP]</color>";
        private readonly PluginConfig<BaseConfig> config;
        public static string SaveLocation => GetRelevantDirectory();
        internal const string fileFormat = ".json";
        public static string AssemblyLocation => Directory.GetCurrentDirectory();

        public static double UTime => DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        public IPluginConfig PluginConfig => this.config;
        public BaseConfig Config => this.config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => this.config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Working";

        public string GetCategory() => "ESP";

        public override string ToString() => "ESP";
        public static void Initialize() { }
        public static void PostInitialize() { }
        public Plugin()
        {
            this.config = new PluginConfig<BaseConfig>("ESP-Settings");
            this.SaveConfig();
        }

        public static ModRegistration Register() => new()
        {
            ModName = "Eco Systems Permissions",
            ModDescription = "ESP is a Mod for creating custom Groups and Permissions system. it is the last Mod created by TheKye and is pulled from the EM Framework to be a standalone, Easy to maintain mod for the modding community.",
            ModDisplayName = "ESP",
        };


        static string GetRelevantDirectory()
        {
            if (saveLocation.StartsWith(Path.DirectorySeparatorChar))
            {
                return Path.Combine(AssemblyLocation, saveLocation);
            }
            return saveLocation;
        }

        static void CreateDirectoryIfNotExist() => CreateDirectoryIfNotExist(SaveLocation);
        public static void CreateDirectoryIfNotExist(string Path)
        {
            if (!Directory.Exists(SaveLocation))
            {
                Directory.CreateDirectory(SaveLocation);
            }
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }

        static string GetPathOf(string FileName)
        {
            if (FileName.Contains(fileFormat))
            {
                return Path.Combine(SaveLocation, FileName);
            }

            return Path.Combine(SaveLocation, FileName + fileFormat);
        }

        public static bool ConfigExists(string FileName) => File.Exists(GetPathOf(FileName));

        static string GetPath(string FileName)
        {
            if (!FileName.EndsWith(fileFormat))
            {
                FileName += fileFormat;
            }

            return Path.Combine(SaveLocation, FileName);
        }

    }
}

public class ESPJsonResolver : ExpandableObjectContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);
        if (!prop.Writable)
        {
            var property = member as PropertyInfo;
            var hasPrivateSetter = property?.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
        }
        return prop;
    }
}