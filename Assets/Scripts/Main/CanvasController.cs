using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Assets.Scripts.Utils;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private Main _main;
    
    private div _body;

    //public OrderedDictionary divs;

    private int index = 0;

    public void Init(Main main)
    {
        _main = main;

        GetBasicElements();
    }

    public int InspectorScreenWidth;
    public int InspectorScreenHeight;
    public string InspectorScreenName;

    public void Build(bool inspector)
    {
        if (inspector)
            GetBasicElements();

        style_utils.buildDiv(_body);
    }

    public void GetBasicElements()
    {
        //divs = new OrderedDictionary();

        _body = new div
        {
            Id = index.ToString(),
            elementName = transform.gameObject.name,
            children = new List<div>(),
            type = divType.body,
            element = transform.GetComponent<RectTransform>()
        };
        style_utils.SetAnchor(AnchorType.TopLeft, _body.element);
        _body.element.localPosition = new Vector3(-(_body.element.sizeDelta.x / 2), (_body.element.sizeDelta.y / 2), 0);

        getDivs(transform, _body);
    }

    private void getDivs(Transform parent, div div)
    {
        var i = 0;
        foreach (Transform child in parent)
        {
            var s = child.GetComponent<style>();
            if (s == null)
                continue;
            
            s.Refresh();
            s.Div.childIndex = i;
            s.Div.parent = div;

            getDivs(child, s.Div);
            
            div.children.Add(s.Div);

            i++;

            //divs.Add(s.Div.Id, s.Div);
        }
    }

    public div FindDiv(string name)
    {
        return new div();
    }
}