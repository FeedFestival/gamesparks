using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;

public class style : MonoBehaviour
{
    private div _div;
    public div Div
    {
        get
        {
            if (_div == null)
                _div = new div
                {
                    Id = Name,
                    elementName = gameObject.name,
                    type = divType.div,
                    element = GetComponent<RectTransform>(),
                    children = new List<div>()
                };
            return _div;
        }
    }

    public string Name;

    public string Class;
    
    public value Height
    {
        get
        {
            return Div.height;
        }
    }

    public value[] Margin;

    public void Refresh()
    {
        _div = new div
        {
            Id = Name,
            elementName = gameObject.name,
            type = divType.div,
            element = GetComponent<RectTransform>(),
            children = new List<div>()
        };
    }
}