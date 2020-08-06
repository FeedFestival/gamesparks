using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils
{
    public enum AnchorType
    {
        TopRight, TopLeft,
        LeftCenter,
        Center
    }

    public enum LayoutType
    {
        Square,
        Full
    }

    public enum AspectRatio
    {
        Unregistered,
        Aspect_16_9
    }

    public enum bootstrap
    {
        boot,
        px,
        percent,
        none
    }

    public enum divType
    {
        div,
        label,
        body
    }

    public class value
    {
        public int natural;
        public float floating;
        public bootstrap boot;
        public bool auto;
    }

    public class div
    {
        public string Id;
        public string elementName;
        public divType type;
        public int childIndex;

        public div parent;
        //public List<boot> children;

        public List<div> children;

        public value width;
        public value height;

        public value[] margin;

        public value[] padding;

        private RectTransform _element;
        public RectTransform element
        {
            get { return _element; }
            set
            {
                _element = value;

                if (type != divType.body)
                {
                    if (elementName.Contains("-("))
                    {
                        var s = elementName.Split('(', ')');
                        var values = s[1].Split(',');

                        formatSize(values[0], out width);

                        if (values.Length == 1)
                            formatSize("p-100", out height);
                        else
                            formatSize(values[1], out height);
                    }
                    if (elementName.Contains("-|"))
                    {
                        var s = elementName.Split('|', '|');
                        var values = s[1].Split(',');

                        padding = setupValues(values);
                    }
                    if (elementName.Contains("-["))
                    {
                        var s = elementName.Split('[', ']');
                        var values = s[1].Split(',');

                        margin = setupValues(values);
                    }
                }

                if (style_base.isLabel(elementName))
                {
                    type = divType.label;
                    style_utils.SetAnchor(AnchorType.TopLeft, _element);
                }
                else
                    style_utils.SetAnchor(AnchorType.TopLeft, _element);

                if (type == divType.label && margin == null)
                {
                    margin = new value[4];
                    formatSize("auto", out margin[0]);
                    formatSize("auto", out margin[1]);
                    formatSize("auto", out margin[2]);
                    formatSize("auto", out margin[3]);
                }
            }
        }

        private value[] setupValues(string[] values)
        {
            var array = new value[4];

            switch (values.Length)
            {
                case 1:
                    formatSize(values[0], out array[0]);
                    formatSize(values[0], out array[2]);
                    formatSize(values[0], out array[1]);
                    formatSize(values[0], out array[3]);
                    break;
                case 2:
                    formatSize(values[0], out array[0]);
                    formatSize(values[0], out array[2]);
                    formatSize(values[1], out array[1]);
                    formatSize(values[1], out array[3]);
                    break;
                case 3:
                    formatSize(values[0], out array[0]);
                    formatSize(values[1], out array[2]);
                    formatSize(values[2], out array[1]);
                    formatSize(values[2], out array[3]);
                    break;
                case 4:
                    formatSize(values[0], out array[0]);
                    formatSize(values[1], out array[1]);
                    formatSize(values[2], out array[2]);
                    formatSize(values[3], out array[3]);
                    break;
            }
            return array;
        }

        private void formatSize(string value, out value val)
        {
            val = new value();
            if (value.Equals("auto"))
            {
                val.auto = true;
            }
            else if (value.Contains("b-"))
            {
                value = value.Replace("b-", "");
                val.boot = bootstrap.boot;
                int.TryParse(value, out val.natural);
            }
            else if (value.Contains("p-"))
            {
                value = value.Replace("p-", "");
                val.boot = bootstrap.percent;
                int.TryParse(value, out val.natural);
            }
            else
            {
                val.boot = bootstrap.px;
                float.TryParse(value, out val.floating);
            }
        }
    }

    public static class style_base
    {
        public static int ScreenWidth;
        public static int ScreenHeight;

        public static readonly Color32 Transparent = new Color32(0, 0, 0, 0);
        public static readonly Color32 Black = new Color32(0, 0, 0, 255);
        public static readonly Color32 White = new Color32(255, 255, 255, 255);
        public static readonly Color32 WhiteTransparent = new Color32(255, 255, 255, 200);

        public static readonly string circle = "";

        public static value getValue(value value)
        {
            if (value == null)
                value = new value { natural = 12, boot = bootstrap.boot };

            if (value.boot == bootstrap.boot)
                value.floating = ComputeSize(value.natural);

            return value;
        }

        private static float ComputeSize(float size)
        {
            float val = 100.0f;
            switch ((int)size)
            {
                case 12:
                    val = 100.0f;
                    break;
                case 11:
                    val = 91.66f;
                    break;
                case 10:
                    val = 83.33f;
                    break;
                case 9:
                    val = 75.0f;
                    break;
                case 8:
                    val = 66.66f;
                    break;
                case 7:
                    val = 58.33f;
                    break;
                case 6:
                    val = 50.0f;
                    break;
                case 5:
                    val = 41.66f;
                    break;
                case 4:
                    val = 33.33f;
                    break;
                case 3:
                    val = 25.0f;
                    break;
                case 2:
                    val = 16.66f;
                    break;
                case 1:
                    val = 8.33f;
                    break;
            }
            return val;
        }

        public static bool isBody(string s)
        {
            if (s.Contains("<body"))
                return true;

            return false;
        }

        public static bool isDiv(Transform t)
        {
            if (t.gameObject.name.Contains("<boot") ||
                t.gameObject.name.Contains("<label"))
                return true;

            return false;
        }

        public static bool isLabel(string s)
        {
            if (s.Contains("<label"))
                return true;

            return false;
        }

        public static void SetSize(float x, float y, RectTransform rt)
        {
            var parent = rt.transform.parent.GetComponent<RectTransform>();

            var w = style_utils.GetPercent(parent.sizeDelta.x, x);
            var h = style_utils.GetPercent(parent.sizeDelta.y, y);

            rt.GetComponent<RectTransform>().sizeDelta = new Vector3(w, h);
        }

        public static void SetSize(float x, float y, Button button)
        {
            SetSize(x, y, button.GetComponent<RectTransform>());
            for (int i = 0; i < button.transform.childCount; i++)
            {
                var text = button.transform.GetChild(i).GetComponent<Text>();
                if (text != null)
                    SetTextSize(text, 45);
            }
        }

        public static void SetSize(LayoutType layoutType, RectTransform rt)
        {
            if (layoutType == LayoutType.Full)
            {
                rt.sizeDelta = new Vector3(ScreenWidth, ScreenHeight);
            }
        }

        public static void SetSize(float x, LayoutType layoutType, RectTransform rt)
        {
            var parent = rt.transform.parent.GetComponent<RectTransform>();

            var w = style_utils.GetPercent(parent.sizeDelta.x, x);
            float h = w;

            rt.sizeDelta = new Vector3(w, h);
        }

        public static void SetSize(LayoutType layoutType, float y, RectTransform rt)
        {
            var parent = rt.transform.parent.GetComponent<RectTransform>();

            float w;
            var h = style_utils.GetPercent(parent.sizeDelta.y, y);
            w = h;

            rt.sizeDelta = new Vector3(w, h);
        }

        public static void SetFixedSize(float x, float y, RectTransform rt)
        {
            rt.GetComponent<RectTransform>().sizeDelta = new Vector3(x, y);
        }

        public static void SetPosition(float x, float y, RectTransform rt)
        {
            var parent = rt.transform.parent.GetComponent<RectTransform>();

            var w = style_utils.GetPercent(parent.sizeDelta.x, x);
            var h = style_utils.GetPercent(parent.sizeDelta.y, y);

            rt.GetComponent<RectTransform>().localPosition = new Vector3(w, -h, 0f);
        }

        public static void SetFixedPosition(float x, float y, RectTransform rt)
        {
            rt.GetComponent<RectTransform>().localPosition = new Vector3(x, -y, 0f);
        }

        public static void SetAnchor(AnchorType at, RectTransform rt)
        {
            Vector2 anchor;
            Vector2 pivot;

            if (at == AnchorType.TopLeft)
            {
                anchor = new Vector2(0, 1);
                pivot = new Vector2(0, 1);
            }
            else if (at == AnchorType.LeftCenter)
            {
                anchor = new Vector2(0, 1f);
                pivot = new Vector2(0f, 0.5f);
            }
            else if (at == AnchorType.Center)
            {
                anchor = new Vector2(0.5f, 0.5f);
                pivot = new Vector2(0.5f, 0.5f);
            }
            else
            {
                anchor = new Vector2(1, 1);
                pivot = new Vector2(1, 1);
            }

            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
        }

        public static void SetIconSize(Text text)
        {
            var parent = text.transform.parent.GetComponent<RectTransform>();

            text.fontSize = (int)parent.sizeDelta.y;
        }

        public static void SetTextSize(Text text, float predefinedValue = 0)
        {
            var parentHeight = text.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
            var textHeight = text.transform.GetComponent<RectTransform>().sizeDelta.y;

            if (predefinedValue > 1)
            {
                text.fontSize = (int)style_utils.GetPercent(parentHeight, predefinedValue);
                return;
            }
            else if (Math.Abs(textHeight) < 1)
            {
                text.fontSize = (int)style_utils.GetPercent(parentHeight, 99);
                return;
            }
            var ratio = style_utils.GetValuePercent(textHeight, parentHeight);
            text.fontSize = (int)style_utils.GetPercent(textHeight, ratio);
        }

        public static float GCD(float x, float y)
        {
            return Math.Abs(y) < 1 ? x : GCD(y, x % y);
        }

        public static AspectRatio GetAspectRatio()
        {
            var x = ScreenWidth / GCD(ScreenWidth, ScreenHeight);
            var y = ScreenHeight / GCD(ScreenWidth, ScreenHeight);

            if ((Math.Abs(x - 16) < 0.1f && Math.Abs(y - 9) < 0.1f) ||
                (Math.Abs(x - 427) < 2 && Math.Abs(y - 240) < 2) ||
                (Math.Abs(x - 80) < 2 && Math.Abs(y - 39) < 2) ||  //  480	× 234
                (Math.Abs(x - 30) < 2 && Math.Abs(y - 17) < 2) ||  //  480	×	272
                (Math.Abs(x - 53) < 2 && Math.Abs(y - 30) < 2) ||
                (Math.Abs(x - 128) < 2 && Math.Abs(y - 75) < 2) ||
                (Math.Abs(x - 71) < 2 && Math.Abs(y - 40) < 2) ||
                (Math.Abs(x - 667) < 2 && Math.Abs(y - 375) < 2) ||
                (Math.Abs(x - 683) < 2 && Math.Abs(y - 384) < 2) ||
                (Math.Abs(x - 222) < 2 && Math.Abs(y - 125) < 2))
            {
                return AspectRatio.Aspect_16_9;
            }

            return AspectRatio.Unregistered;
        }
    }
}
