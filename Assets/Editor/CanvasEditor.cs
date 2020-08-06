using System;
using UnityEngine;
using System.Collections;
using Assets.Editor;
using Assets.Scripts.Utils;
using UnityEditor;

[CustomEditor(typeof(CanvasController))]
public class CanvasEditor : Editor
{
    private CanvasController _script;

    private AndroidViewType _androidViewType;
    public AndroidViewType AndroidViewType
    {
        set
        {
            _androidViewType = value;
            EditorUtils.AndroidViewType = _androidViewType;
        }
        get { return _androidViewType; }
    }

    private GUIStyle _redStyle;
    public string Error;

    public override void OnInspectorGUI()
    {
        if (_script == null)
        {
            _script = (CanvasController)target;
            utils.CanvasController = _script;
        }


        if (_redStyle == null)
            _redStyle = new GUIStyle();
        _redStyle.normal.textColor = Color.red;

        GUILayout.Space(10);
        if (GUILayout.Button("Refresh UI", GUILayout.Width(style_utils.GetPercent(Screen.width, 90)), GUILayout.Height(50)))
        {
            //_script.RefreshCameraTransform();
            _script.Build(true);
        }

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");
        GUILayout.Space(5);

        GUILayout.Label("Resolution");
        GUILayout.Space(5);

        _script.InspectorScreenWidth = EditorGUILayout.IntField("Width :", _script.InspectorScreenWidth);
        _script.InspectorScreenHeight = EditorGUILayout.IntField("Height :", _script.InspectorScreenHeight);

        if (string.IsNullOrEmpty(Error) == false)
        {
            GUILayout.Space(10);
            GUILayout.Label(Error, _redStyle);
            GUILayout.Space(10);
        }
        _script.InspectorScreenName = EditorGUILayout.TextField("Name :", _script.InspectorScreenName);

        GUILayout.Space(10);
        if (GUILayout.Button("Add View Size"))
        {
            AddScreenResolution(_script.InspectorScreenName, _script.InspectorScreenWidth, _script.InspectorScreenHeight);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Populate"))
        {
            if (AndroidViewType == AndroidViewType.Landscape)
            {
                AddScreenResolution("4K UHD (Landscape)", 3840, 2160);
                AddScreenResolution("WQXGA + (Landscape)", 3200, 1800);
                AddScreenResolution("iPad Pro (Landscape)", 2732, 2048);
                AddScreenResolution("Samsung S6 (Landscape)", 2560, 1440);
                AddScreenResolution("Full HD (Landscape)", 1920, 1080);

                AddScreenResolution("HD (222:125) (Landscape)", 1776, 1000);

                AddScreenResolution("900p HD+ (Landscape)", 1600, 900);

                AddScreenResolution("HD ready (683:384) (Landscape)", 1366, 768);
                AddScreenResolution("iPhone 6 (667:375) (Landscape)", 1334, 750);

                AddScreenResolution("WXGA-H (Landscape)", 1280, 720);

                AddScreenResolution("iPhone 5 (71:40) (Landscape)", 1136, 640);
                AddScreenResolution("WSVGA (128:75) (Landscape)", 1024, 600);
                AddScreenResolution("PV (30:17) (Landscape)", 960, 544);
                AddScreenResolution("PV (53:30) (Landscape)", 848, 480);

                AddScreenResolution("IPhone 6 s(Landscape)", 667, 375);

                AddScreenResolution("nHD (Landscape)", 640, 360);

                AddScreenResolution("iPhone 5 s (71:40)(Landscape)", 568, 320);

                AddScreenResolution("Low (80:39) (Landscape)", 480, 234);
            }
        }

        GUILayout.Space(10);

        AndroidViewType = (AndroidViewType)EditorGUILayout.EnumPopup("Android View Type:", AndroidViewType);

        GUILayout.Space(10);

        foreach (var viewSize in EditorUtils.ViewSizes)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(viewSize.Name);
            GUILayout.Label(viewSize.Width.ToString());
            GUILayout.Label(viewSize.Height.ToString());

            if (GUILayout.Button("Set VS"))
            {
                EditorUtils.SetSizeToScreen(GameViewSizeGroupType.Android, viewSize.Width, viewSize.Height);

                _script.InspectorScreenWidth = viewSize.Width;
                _script.InspectorScreenHeight = viewSize.Height;
                _script.InspectorScreenName = viewSize.Name;

                //_script.RefreshCameraTransform();
                //_script.RefreshUi(true);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
        }

        GUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }

    private void AddScreenResolution(string rezName, int xSize, int ySize)
    {
        if (string.IsNullOrEmpty(rezName))
            Error = "Name is null or incorect. " + Environment.NewLine;
        else if (rezName.Contains(xSize.ToString()) ||
            rezName.Contains(xSize.ToString()))
            Error = "Not allowed to contain Width and Height in the elementName." + Environment.NewLine;
        else if (rezName.Contains(AndroidViewType.ToString()) == false)
            Error = "You must have the ViewType(ex: Landscape)," + Environment.NewLine;
        else
            Error = null;

        if (string.IsNullOrEmpty(Error) == false)
            return;

        if (AndroidViewType == AndroidViewType.Landscape &&
            xSize > ySize && xSize > 280)
        {
            foreach (var vs in EditorUtils.ViewSizes)
            {
                if (vs.Width == xSize && vs.Height == ySize)
                    Error = vs.Width + ":" + vs.Height + "ViewSize allready exists.";
            }
            if (string.IsNullOrEmpty(Error))
                EditorUtils.AddCustomSize(GameViewSizeType.FixedResolution, GameViewSizeGroupType.Android,
                    xSize, ySize, rezName);
        }
    }
}

[CustomEditor(typeof(style))]
public class StyleEditor : Editor
{
    private style _script;

    public override void OnInspectorGUI()
    {
        if (_script == null)
        {
            _script = (style)target;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Refresh UI", GUILayout.Width(style_utils.GetPercent(Screen.width, 90)), GUILayout.Height(40)))
        {
            //_script.RefreshCameraTransform();
            _script.Refresh();
        }

        GUILayout.Space(10);


        EditorGUILayout.BeginVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("Name: ");

        _script.Name = EditorGUILayout.TextField(_script.Name,
                GUILayout.Width(style_utils.GetPercent(Screen.width, 20)));
        
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);








        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Width: " + (_script.Div.width.auto ?
            "auto" : _script.Div.width.boot == bootstrap.px ?
                                _script.Div.width.floating.ToString() : _script.Div.width.natural.ToString()));
        if (GUILayout.Button("auto", GUILayout.Width(style_utils.GetPercent(Screen.width, 10))))
        {
            _script.Div.width.auto = !_script.Div.width.auto;
            if (_script.Div.width.auto)
                _script.Div.width.boot = bootstrap.none;
            else
                _script.Div.width.boot = bootstrap.px;
        }
        if (GUILayout.Button("boot", GUILayout.Width(style_utils.GetPercent(Screen.width, 10))))
        {
            _script.Div.width.boot = bootstrap.boot;
            _script.Div.width.auto = false;
        }
        if (_script.Div.width.boot == bootstrap.boot)
        {
            _script.Div.width.natural = EditorGUILayout.IntField(_script.Div.width.natural,
                GUILayout.Width(style_utils.GetPercent(Screen.width, 15)));
            GUILayout.Space(10);
        }

        if (GUILayout.Button("%", GUILayout.Width(style_utils.GetPercent(Screen.width, 10))))
        {
            _script.Div.width.boot = bootstrap.percent;
            _script.Div.width.auto = false;
        }
        if (_script.Div.width.boot == bootstrap.percent)
        {
            _script.Div.width.natural = EditorGUILayout.IntField(_script.Div.width.natural,
                GUILayout.Width(style_utils.GetPercent(Screen.width, 15)));
            GUILayout.Space(10);
        }

        if (GUILayout.Button("px", GUILayout.Width(style_utils.GetPercent(Screen.width, 10))))
        {
            _script.Div.width.boot = bootstrap.px;
            _script.Div.width.auto = false;
        }
        if (_script.Div.width.boot == bootstrap.px)
        {
            _script.Div.width.floating = EditorGUILayout.FloatField(_script.Div.width.floating,
                GUILayout.Width(style_utils.GetPercent(Screen.width, 15)));
        }
        EditorGUILayout.EndHorizontal();


        GUILayout.Space(5);


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Height: " + (_script.Div.height.auto ?
            "auto" : _script.Div.height.boot == bootstrap.px ?
                                _script.Div.height.floating.ToString() : _script.Div.height.natural.ToString()));
        if (GUILayout.Button("auto", GUILayout.Width(style_utils.GetPercent(Screen.width, 10))))
        {
            _script.Div.height.auto = !_script.Div.height.auto;
            if (_script.Div.height.auto)
                _script.Div.height.boot = bootstrap.none;
            else
                _script.Div.height.boot = bootstrap.px;
        }
        if (GUILayout.Button("boot", GUILayout.Width(style_utils.GetPercent(Screen.width, 10))))
        {
            _script.Div.height.boot = bootstrap.boot;
            _script.Div.height.auto = false;
        }
        if (_script.Div.height.boot == bootstrap.boot)
        {
            _script.Div.height.natural = EditorGUILayout.IntField(_script.Div.height.natural,
                GUILayout.Width(style_utils.GetPercent(Screen.width, 15)));
            GUILayout.Space(10);
        }

        if (GUILayout.Button("%", GUILayout.Width(style_utils.GetPercent(Screen.width, 10))))
        {
            _script.Div.height.boot = bootstrap.percent;
            _script.Div.height.auto = false;
        }
        if (_script.Div.height.boot == bootstrap.percent)
        {
            _script.Div.height.natural = EditorGUILayout.IntField(_script.Div.height.natural,
                GUILayout.Width(style_utils.GetPercent(Screen.width, 15)));
            GUILayout.Space(10);
        }

        if (GUILayout.Button("px", GUILayout.Width(style_utils.GetPercent(Screen.width, 10))))
        {
            _script.Div.height.boot = bootstrap.px;
            _script.Div.height.auto = false;
        }
        if (_script.Div.height.boot == bootstrap.px)
        {
            _script.Div.height.floating = EditorGUILayout.FloatField(_script.Div.height.floating,
                GUILayout.Width(style_utils.GetPercent(Screen.width, 15)));
        }
        EditorGUILayout.EndHorizontal();




        GUILayout.Space(10);
        //_script.InspectorScreenHeight = EditorGUILayout.IntField("Height :", _script.InspectorScreenHeight);

        GUILayout.Space(10);

        GUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }
}