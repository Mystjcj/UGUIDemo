using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickMouse : MonoBehaviour, IPointerClickHandler
{
    private GraphicRaycaster _raycaster;

    void Start()
    {
        _raycaster = FindObjectOfType<GraphicRaycaster>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " " + Time.time);
        if (IsUI(eventData))
        {
            Debug.Log("点击在UI上！");
            //ExecuteAll(eventData);
        }
        else
        {
            Debug.Log("未点击在UI上！");
        }
    }
    private bool IsUI(PointerEventData data)
    {
        //PointerEventData data = new PointerEventData(EventSystem.current);
        //data.pressPosition = Input.mousePosition;
        //data.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(data, results);
        return results.Count > 0;
    }
    /// <summary>
    /// 执行所有的鼠标点击的事件
    /// </summary>
    /// <param name="eventData"></param>
    private void ExecuteAll(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject != gameObject)
            {
                ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerClickHandler);
            }
        }
    }
}
