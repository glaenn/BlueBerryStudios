using UnityEngine;
using System.Collections;

public class ActivateChild : MonoBehaviour
{
    GameObject childObject;

    private bool isVisible = false;

    public bool IsVisible
    {
        get { return isVisible; }
        set { isVisible = value; }
    }
    // Use this for initialization
    void Start ()
    {
        //Find the first child of the object and save it
        childObject = this.gameObject.transform.GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Check if the child object should be visisble or not
	    if(IsVisible)
        {
            childObject.SetActive(true);
        }
        else
        {
            childObject.SetActive(false);
        }
	}
}
