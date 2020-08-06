using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;

public class style : MonoBehaviour
{
    public float Width;
    public float Height;

    public bool SetPosition;
    public bool WeirdSet;

    public float X;
    public float Y;

    public void Setup()
    {
        var rt = transform.GetComponent<RectTransform>();
        var parent = transform.parent.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(utils.GetPercent(parent.sizeDelta.x, Width), utils.GetPercent(parent.sizeDelta.y, Height));

        if (SetPosition)
        {
            float newX, newY = 0.0f;
            if (WeirdSet)
            {
                newX = X - (parent.sizeDelta.x/2);
                newY = -(Y - (parent.sizeDelta.y/2));
            }
            else
            {
                newX = X;
                newY = Y;
            }

            rt.localPosition = new Vector3(newX, newY, rt.localPosition.z);
        }
    }
}