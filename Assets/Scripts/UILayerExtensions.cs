using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UILayerExtensions
{
    public static bool IsPointerOverUIElement(LayerMask layerMask)
    {
        return IsPointerOverUIElement(layerMask, out _);
    }
    
    public static bool IsPointerOverUIElement(LayerMask layerMask, out List<RaycastResult> results)
    {
        results = new List<RaycastResult>();
        
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        
        EventSystem.current.RaycastAll(eventData, results);
        
        var resultsCount = results.Count(x => (1 << x.gameObject.layer) == layerMask);
        var isPointerOverUI = resultsCount > 0;
        return isPointerOverUI;
    }

    public static GameObject GetFirstPointerUIElement(LayerMask layerMask)
    {
        if (!IsPointerOverUIElement(layerMask, out var results))
            return null;

        return results.First().gameObject;
    }
}