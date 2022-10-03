using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    #region Fields

    private Vector3 mousePositionOffset;

    #endregion

    #region Properties

    public bool dragging { get; protected set; }

    public bool dragActive { get; set; }

    #endregion

    #region Behaviour

    private void Awake()
    {
        dragActive = true;
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 10; // select distance = 10 units from the camera
        //Debug.Log(Camera.main.ScreenToWorldPoint(mousePos));
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void OnMouseDown()
    {
        if(dragActive == false || GameCore.Instance.GameEnd)
        {
            return;
        }
        Debug.Log("OnMouseDown");
        dragging = true;
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
    }

    public void OnMouseDrag()
    {
        if(dragActive == false || GameCore.Instance.GameEnd)
        {
            return;
        }
        Debug.Log("OnMouseDrag");
        transform.position = GetMouseWorldPosition() + mousePositionOffset;
    }

    public void OnMouseUp()
    {
        if (dragActive == false)
        {
            return;
        }
        Debug.Log("OnMouseUp");
        dragging = false;
    }

    /*void Update()
    {
        GetMouseWorldPosition();
    }*/

    #endregion
}
