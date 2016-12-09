using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    [SerializeField] private bool isClicked = false;

    public void ClickedStart()
    {
        if(Input.GetMouseButton(0) && isClicked)
        {
            Debug.Log(gameObject.name + "was clicked ");
            isClicked = true;
        }
    }


}
