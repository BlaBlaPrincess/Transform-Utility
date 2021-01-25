using System;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Dreamcore.TransformUtility
{
    internal class ToolSettingsWindow : EditorWindow
    {
        private static readonly ToolSettings Settings =
            new ToolSettings(Constants.SettingsKey, new ToolSettingsData());

        public static event Action SettingsChanged;
        
        [UserSettingBlock("General")]
        public static void ConditionalGUIValue(string searchContext)
        {
            EditorGUI.BeginChangeCheck();

            var settings = Settings.value;
            
            settings.coloringMode = (ColoringMode) EditorGUILayout.EnumPopup("Coloring Mode",
                settings.coloringMode);
            if (settings.coloringMode == ColoringMode.SingleColorTheme)
            {
                settings.singleColorTheme = EditorGUILayout.ColorField("Single Color Theme",
                    settings.singleColorTheme);
            }
            if (settings.coloringMode != ColoringMode.None)
            {
                settings.increaseEditorSmoothness =
                    EditorGUILayout.Toggle("Increase Smoothness",
                        settings.increaseEditorSmoothness, GUILayout.ExpandWidth(false));
                if (settings.increaseEditorSmoothness)
                {
                    EditorGUILayout.HelpBox("Increased smoothness may degrade editor performance.",
                        MessageType.Warning);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                Settings.SetValue(settings, true);
                SettingsChanged?.Invoke();
            }
        }
    }
}