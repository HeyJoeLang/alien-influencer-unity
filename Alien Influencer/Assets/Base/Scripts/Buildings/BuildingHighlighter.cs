using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHighlighter : Singleton<BuildingHighlighter>
{
    Outline curr_highlight, curr_select;
    public GameObject curr_HighlightObj, curr_SelectObj;

    public void SelectObject()
    {
        if (curr_HighlightObj == null) // Nothing to select
        {
            return;
        }
        SelectObject(curr_HighlightObj.GetComponent<Outline>());
    }
    public void HighlightObject(GameObject highlightObj)
    {
        if (highlightObj == null || curr_HighlightObj == highlightObj)
        {
            return;
        }
        if(curr_highlight != null)
        {
            curr_highlight.OutlineWidth = 0;
        }
        curr_highlight = highlightObj.GetComponent<Outline>();
        curr_highlight.OutlineWidth = 5.0f;
        curr_highlight.OutlineColor = Color.yellow;
        curr_HighlightObj = highlightObj;
    }
    private void SelectObject(Outline obj)
    {
        if (obj == null)
        {
            return;
        }

        if (curr_select != null) 
        {
            curr_select.OutlineWidth = 0;
        }
        obj.OutlineColor = Color.green; 
        obj.OutlineWidth = 5.0f;
        curr_select = obj;
        curr_SelectObj = obj.gameObject;
        MinionManager.Instance.NewBuildingSelection(curr_SelectObj);
    }
}