using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using b = Assets.Scripts.Utils.style_base;

namespace Assets.Scripts.Utils
{
    public static class style_utils
    {
        public static float GetPercent(float value, float percent)
        {
            return (value / 100f) * percent;
        }

        public static float GetValuePercent(float value, float maxValue)
        {
            return (value * 100f) / maxValue;
        }

        public static float XPercent(float percent)
        {
            return (b.ScreenWidth / 100f) * percent;
        }
        public static float YPercent(float percent)
        {
            return (b.ScreenHeight / 100f) * percent;
        }

        public static void BootstrapSize(value width, value height, RectTransform rt)
        {
            width = b.getValue(width);
            height = b.getValue(height);

            if (width.boot == bootstrap.boot && height.boot == bootstrap.boot)
                b.SetSize(width.floating, height.floating, rt);
            else
            {
                var parent = rt.transform.parent.GetComponent<RectTransform>();
                if (width.boot == bootstrap.boot)
                    width.floating = GetPercent(parent.sizeDelta.x, width.floating);
                else if (width.boot == bootstrap.percent)
                    width.floating = GetPercent(parent.sizeDelta.x, width.natural);
                if (height.boot != bootstrap.px)
                    height.floating = GetPercent(parent.sizeDelta.y, height.natural);
                rt.GetComponent<RectTransform>().sizeDelta = new Vector3(width.floating, height.floating);
            }
        }

        public static void SetPosition(float x, float y, Button button)
        {
            b.SetPosition(x, y, button.GetComponent<RectTransform>());
        }

        public static void SetPosition(float x, float y, Image image)
        {
            b.SetPosition(x, y, image.GetComponent<RectTransform>());
        }

        public static void SetAnchor(AnchorType at, RectTransform rt)
        {
            b.SetAnchor(at, rt);
        }

        public static void SetAnchor(AnchorType at, Button button)
        {
            b.SetAnchor(at, button.GetComponent<RectTransform>());
        }

        public static void SetAnchor(AnchorType at, Image image)
        {
            b.SetAnchor(at, image.GetComponent<RectTransform>());
        }

        public static void buildDiv(div div)
        {
            if (b.isBody(div.elementName) == false)
                build(div);
            if (div.children.Count > 0)
                foreach (div d in div.children)
                {
                    buildDiv(d);
                }
        }

        private static void build(div div)
        {
            if (div.type != divType.label)
                BootstrapSize(div.width,
                            div.height,
                            div.element);

            if (div.margin != null)
            {
                setupMargins(ref div);
            }
            else if (div.childIndex == 0)
                div.element.localPosition = new Vector3(0f, 0f, 0f);
            else
            {
                var x = getUsedSpace(div, true);

                if (divFitsInline(x, div.element.sizeDelta.x, div.parent.element.sizeDelta.x))
                    div.element.localPosition = new Vector3(x, 0f, 0f);
                else
                {
                    var y = getUsedSpace(div);
                    div.element.localPosition = new Vector3(0, -y, 0f);
                }
            }
        }

        private static bool divFitsInline(float curX, float elX, float parentX)
        {
            if (curX + elX <= parentX)
                return true;
            return false;
        }

        private static float getUsedSpace(div div, bool xAxis = false)
        {
            if (xAxis)
            {
                float x = 0f;
                for (var i = 0; i < div.childIndex; i++)
                {
                    var blk = div.parent.children[i];
                    x = x + blk.element.sizeDelta.x;
                }

                return x;
            }

            int level = -1;
            List<float> levelsHeight = new List<float>();
            for (var i = 0; i < div.childIndex; i++)
            {
                var blk = div.parent.children[i];
                var previousBlk = i == 0 ? null : div.parent.children[i - 1];

                if (i == 0 || (int)previousBlk.element.localPosition.y != (int)blk.element.localPosition.y)
                {
                    level++;
                    levelsHeight.Add(blk.element.sizeDelta.y);
                    continue;
                }
                if (previousBlk.element.sizeDelta.y < blk.element.sizeDelta.y)
                    levelsHeight[level] = blk.element.sizeDelta.y;
            }

            float y = 0;
            foreach (float f in levelsHeight)
            {
                y = y + f;
            }
            return y;
        }

        public static void setupMargins(ref div div)
        {
            var parentSize = div.parent.element.sizeDelta.x;
            var elSize = div.element.sizeDelta.x;

            var parentHeight = div.parent.element.sizeDelta.y;
            var elHeight = div.element.sizeDelta.y;

            if (parentSize > elSize &&
                div.margin[3].auto && div.margin[1].auto) // it means we center this boot horizontally.
            {
                var remainingSpace = parentSize - elSize;
                div.margin[3].floating = remainingSpace / 2;
            }
            else
            {
                div.margin[3].floating = 0;
            }

            if (parentHeight > elHeight &&
                div.margin[0].auto && div.margin[2].auto)
            {
                var remainingSpace = parentHeight - elHeight;
                div.margin[0].floating = remainingSpace / 2;
            }

            div.element.localPosition = new Vector3(div.margin[3].floating, -div.margin[0].floating, 0f);
        }
    }
}