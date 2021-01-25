using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Dreamcore.TransformUtility
{
    internal class ToolSettings : UserSetting<ToolSettingsData>
    {
        private static Settings _instance;
        
        public static Settings Instance =>
            _instance ?? (_instance = new Settings(Constants.PackageName));
        
        public ToolSettings(string key, ToolSettingsData value,
            SettingsScope scope = Constants.SettingsScope)
            : base(Instance, key, value, scope) { }

        public ToolSettings(Settings settings, string key, ToolSettingsData value,
            SettingsScope scope = Constants.SettingsScope)
            : base(settings, key, value, scope) { }
    }
}