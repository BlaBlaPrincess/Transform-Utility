using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: AssemblyIsEditorAssembly]
namespace Dreamcore.TransformUtility
{
    internal class ToolWindow : EditorWindow
    {
        #region Fields

        private static ToolSettings _settings =
            new ToolSettings(Constants.SettingsKey, new ToolSettingsData());

        private readonly GUILayoutOption[] _headerGuiOptions =
        {
            GUILayout.Width(80),
            GUILayout.ExpandWidth(false)
        };
        private readonly GUILayoutOption[] _columnGuiOptions =
        {
            GUILayout.MinWidth(50),
            GUILayout.MaxWidth(1000),
            GUILayout.ExpandWidth(true)
        };
    
        // Use value 30 to mimic inspector style
        private const int SpaceOffset = 0;
    
        #endregion
        
        #region Editor
    
        [MenuItem(Constants.MenuItemPath)]
        private static void ShowWindow()
        {
            var window = GetWindow<ToolWindow>();
            window.titleContent = new GUIContent("Tools");
            window.Show();
        }
    
        private void OnEnable()
        {
            ToolSettingsWindow.SettingsChanged += OnSettingsChanged;
            SceneView.duringSceneGui += OnDuringSceneGui;
        }

        private void OnDisable()
        {
            ToolSettingsWindow.SettingsChanged -= OnSettingsChanged;
            SceneView.duringSceneGui -= OnDuringSceneGui;
        }
    
        private void OnSettingsChanged()
        {
            _settings = new ToolSettings(Constants.SettingsKey, new ToolSettingsData());
            Repaint();
        }
        
        private void OnDuringSceneGui(SceneView vie)
        {
            var settings = _settings.value;
            if (settings.increaseEditorSmoothness && settings.coloringMode != ColoringMode.None)
            {
                Repaint();
            }
        }
    
        private void OnInspectorUpdate()
        {
            var settings = _settings.value;
            if (!settings.increaseEditorSmoothness && settings.coloringMode != ColoringMode.None)
            {
                Repaint();
            }
        }
    
        private void OnSelectionChange()
        {
            Repaint();
        }
    
        private Vector2 _scrollPos;
        private void OnGUI()
        {
            var transforms = Selection.transforms;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            _transformFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_transformFoldout, "Transform");
            if (_transformFoldout)
            {
                DrawRoundUpComponent(transforms);
                EditorGUILayout.Space();
                DrawMoveComponent(transforms);
                EditorGUILayout.Space();
                DrawRotationComponent(transforms);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
        }
    
        #endregion
    
        #region General methods
    
        private void SetGUIColor(Color color)
        {
            var settings = _settings.value;
            if (settings.coloringMode != ColoringMode.None)
            {
                GUI.backgroundColor = color;
            }
        }
    
        #endregion
    
        #region Transform component drawers
    
        private bool _transformFoldout = true;
        private int _rotationSensitivity = 30;
    
        private void DrawRoundUpComponent(Transform[] transforms)
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(SpaceOffset);
                EditorGUILayout.LabelField("Round Up", _headerGuiOptions);
                if (GUILayout.Button("Position", _columnGuiOptions))
                {
                    if (Selection.transforms.Length == 0) return;
                    Undo.RecordObjects(transforms as Object[], "Round Up");
                    foreach (var transform in transforms)
                    {
                        var pos = transform.position;
                        transform.position = new Vector3(
                            Mathf.Round(pos.x),
                            Mathf.Round(pos.y),
                            Mathf.Round(pos.z));
                        EditorUtility.SetDirty(transform);
                    }
                }
    
                if (GUILayout.Button("Rotation", _columnGuiOptions))
                {
                    if (Selection.transforms.Length == 0) return;
                    Undo.RecordObjects(transforms as Object[], "Round Up");
                    foreach (var transform in transforms)
                    {
                        var rotation = transform.rotation;
                        var x = rotation.eulerAngles.x;
                        var y = rotation.eulerAngles.y;
                        var z = rotation.eulerAngles.z;
                        transform.rotation = Quaternion.Euler(
                            Mathf.Round(x / _rotationSensitivity) * _rotationSensitivity % 360,
                            Mathf.Round(y / _rotationSensitivity) * _rotationSensitivity % 360,
                            Mathf.Round(z / _rotationSensitivity) * _rotationSensitivity % 360);
                        EditorUtility.SetDirty(transform);
                    }
                }
    
                if (GUILayout.Button("Scale", _columnGuiOptions))
                {
                    if (Selection.transforms.Length == 0) return;
                    Undo.RecordObjects(transforms as Object[], "Round Up");
                    foreach (var transform in transforms)
                    {
                        var scale = transform.localScale;
                        transform.localScale = new Vector3(
                            Mathf.Round(scale.x),
                            Mathf.Round(scale.y),
                            Mathf.Round(scale.z));
                        EditorUtility.SetDirty(transform);
                    }
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(SpaceOffset);
                EditorGUILayout.LabelField("Sensitivity", _headerGuiOptions);
                EditorGUILayout.LabelField(string.Empty, _columnGuiOptions);
                _rotationSensitivity = EditorGUILayout.IntPopup(_rotationSensitivity,
                    new[] {"1", "5", "10", "30", "45", "60", "90"},
                    new[] {1, 5, 10, 30, 45, 60, 90}, _columnGuiOptions);
                EditorGUILayout.LabelField(string.Empty, _columnGuiOptions);
            }
            GUILayout.EndHorizontal();
        }
    
        private void DrawMoveComponent(Transform[] transforms)
        {
            Vector3? newPosition = null;
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(SpaceOffset);
                EditorGUILayout.LabelField("Move", _headerGuiOptions);
                if (GUILayout.Button("To Camera Pivot", _columnGuiOptions))
                {
                    newPosition = SceneView.lastActiveSceneView.pivot;
                }
                if (GUILayout.Button("To Zeros", _columnGuiOptions))
                {
                    newPosition = Vector3.zero;
                }
            }
            GUILayout.EndHorizontal();
            
            if (newPosition is null) return;
            Undo.RecordObjects(transforms as Object[], "Move");
            foreach (var transform in transforms)
            {
                transform.position = (Vector3) newPosition;
                EditorUtility.SetDirty(transform);
            }
        }
    
        private void DrawRotationComponent(Transform[] transforms)
        {
            int x = -1;
            int y = -1;
            int z = -1;

            var settings = _settings.value;
            var defaultColor = GUI.backgroundColor;
            var camForward = SceneView.lastActiveSceneView.camera.transform.forward;
    
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(SpaceOffset);
                EditorGUILayout.LabelField("Rotation", _headerGuiOptions);
                EditorGUILayout.LabelField(string.Empty, _columnGuiOptions);
                SetGUIColor(camForward.y > 0
                    ? Color.Lerp(defaultColor, settings.coloringMode == ColoringMode.SingleColorTheme
                        ? settings.singleColorTheme
                        : Color.green, camForward.y)
                    : defaultColor);
                if (GUILayout.Button("Up", _columnGuiOptions))
                {
                    x = -90;
                }
                SetGUIColor(camForward.z > 0
                    ? Color.Lerp(defaultColor, settings.coloringMode == ColoringMode.SingleColorTheme
                        ? settings.singleColorTheme
                        : Color.blue, camForward.z)
                    : defaultColor);
                if (GUILayout.Button("Front", _columnGuiOptions))
                {
                    y = 0;
                    z = 0;
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(SpaceOffset);
                EditorGUILayout.LabelField(string.Empty, _headerGuiOptions);
                SetGUIColor(camForward.x < 0
                    ? Color.Lerp(defaultColor, settings.coloringMode == ColoringMode.SingleColorTheme
                        ? settings.singleColorTheme
                        : Color.red, -camForward.x)
                    : defaultColor);
                if (GUILayout.Button("Left", _columnGuiOptions))
                {
                    y = -90;
                    z = 0;
                }
                SetGUIColor(Color.Lerp(settings.coloringMode == ColoringMode.SingleColorTheme
                    ? settings.singleColorTheme
                    : Color.green, defaultColor, Mathf.Abs(camForward.y)));
                if (GUILayout.Button("Parallel", _columnGuiOptions))
                {
                    x = 0;
                }
                SetGUIColor(camForward.x > 0
                    ? Color.Lerp(defaultColor, settings.coloringMode == ColoringMode.SingleColorTheme
                        ? settings.singleColorTheme
                        : Color.red, camForward.x)
                    : defaultColor);
                if (GUILayout.Button("Right", _columnGuiOptions))
                {
                    y = 90;
                    z = 0;
                }
            }
            GUILayout.EndHorizontal();
                    
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(SpaceOffset);
                EditorGUILayout.LabelField(string.Empty, _headerGuiOptions);
                SetGUIColor(camForward.z < 0
                    ? Color.Lerp(defaultColor, settings.coloringMode == ColoringMode.SingleColorTheme
                        ? settings.singleColorTheme
                        : Color.blue, -camForward.z)
                    : defaultColor);
                if (GUILayout.Button("Back", _columnGuiOptions))
                {
                    y = 180;
                    z = 0;
                }
                SetGUIColor(camForward.y < 0
                    ? Color.Lerp(defaultColor, settings.coloringMode == ColoringMode.SingleColorTheme
                        ? settings.singleColorTheme
                        : Color.green, -camForward.y)
                    : defaultColor);
                if (GUILayout.Button("Down", _columnGuiOptions))
                {
                    x = 90;
                }
                EditorGUILayout.LabelField(string.Empty, _columnGuiOptions);
            }
            GUILayout.EndHorizontal();
            SetGUIColor(defaultColor);
            
            if (x == -1 && y == -1 && z == -1) return;
            Undo.RecordObjects(transforms as Object[], "Rotation");
            foreach (var transform in transforms)
            {
                var rotation = transform.rotation;
                transform.rotation = Quaternion.Euler(
                    x: x == -1 ? rotation.eulerAngles.x : x,
                    y: y == -1 ? rotation.eulerAngles.y : y,
                    z: z == -1 ? rotation.eulerAngles.z : z);
                EditorUtility.SetDirty(transform);
            }
        }
    
        #endregion
    }
}