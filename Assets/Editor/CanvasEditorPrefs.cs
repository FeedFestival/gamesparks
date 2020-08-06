using UnityEngine;
using Assets.Scripts.Utils;
using UnityEditor;

[CustomEditor(typeof(style))]
public class StyleObjectEditor : Editor
{
    style _style;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Refresh UI", GUILayout.Width(utils.GetPercent(Screen.width, 90)), GUILayout.Height(40)))
        {
            if (_style == null)
                _style = (style)target;

            _style.Setup();
        }
    }
}
