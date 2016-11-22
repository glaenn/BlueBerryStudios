using UnityEngine;
using System.Collections;

public class ButtonHandler : MonoBehaviour {
    private bool isVisible = false;
    private bool insideProfile = false;
    private bool insideTrash = false;
    private bool insidePlay = false;
    [SerializeField] private GameObject trashBin;
    [SerializeField] private GameObject playButton;

    public bool IsVisible
    {
        get { return isVisible; }
        set { isVisible = value; }
    }

    public bool InsideProfile
    {
        get { return insideProfile; }
        set { insideProfile = value; }
    }

    public bool InsideTrash
    {
        get { return insideTrash; }
        set { insideTrash = value; }
    }

    public bool InsidePlay
    {
        get { return insidePlay; }
        set { insidePlay = value; }
    }
    // Use this for initialization
    void Start ()
    {
        trashBin = GameObject.FindGameObjectWithTag("TrashCan");
        playButton = GameObject.FindGameObjectWithTag("PlayButton");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(InsideProfile || InsideTrash || InsidePlay)
        {
            isVisible = true;
        }
	    if(!InsideProfile && !InsideTrash && !InsidePlay)
        {
            isVisible = false;
        }

        if(isVisible)
        {
            //TODO: find the trashcan and the play button and add them to the script
            trashBin.SetActive(true);
            playButton.SetActive(true);
        }

        else
        {
            trashBin.SetActive(false);
            playButton.SetActive(false);
        }
	}
}
