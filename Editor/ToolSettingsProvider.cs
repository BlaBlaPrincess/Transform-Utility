using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Dreamcore.TransformUtility
{
    internal static class ToolSettingsProvider
    {
        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new UserSettingsProvider(Constants.PreferencesPath, ToolSettings.Instance,
                new[] {typeof(ToolSettingsProvider).Assembly});
        }
    }
}