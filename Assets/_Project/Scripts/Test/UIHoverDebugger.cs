using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIHoverDebugger : MonoBehaviour
{
    void Update()
    {
        // 마우스 왼쪽 버튼을 클릭하는 순간!
        if (Input.GetMouseButtonDown(0)) 
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // 마우스 광선에 맞은 UI가 하나라도 있다면? 제일 겉에 있는 놈의 이름을 불어라!
            if (results.Count > 0)
            {
                Debug.Log("🎯 마우스가 클릭한 범인 UI: " + results[0].gameObject.name);
            }
            else
            {
                Debug.Log("허공 클릭! (또는 마우스를 막는 UI가 아예 없음)");
            }
        }
    }
}