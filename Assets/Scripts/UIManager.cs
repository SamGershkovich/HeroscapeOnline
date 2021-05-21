using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{

    public bool hovering = false;

    private void Update()
    {
        hovering = isHoveringUIElement;
    }
    public static bool isHoveringUIElement
    {
        get
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                return false;
            }

            string mouseOveredObjectName = "";
            string mouseOveredObjectTag = "";

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                mouseOveredObjectName = results[0].gameObject.name;
                mouseOveredObjectTag = results[0].gameObject.tag;

                return results[0].gameObject.layer == 5; // 5 is Unity's UI layer
            }

            return false;
        }
    }
}
