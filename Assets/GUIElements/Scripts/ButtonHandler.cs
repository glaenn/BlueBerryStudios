using UnityEngine;
using System.Collections;

public class ButtonHandler : MonoBehaviour
{
    private bool trashAndPlayVisible = false;
    private bool interactionLayoutVisible = false;
    private bool insideProfile = false;
    private bool insideTrash = false;
    private bool insidePlay = false;
    private bool insideInteractionLayout = false;
    [SerializeField] private GameObject trashBin;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject interactionLayout;

    public bool TrashAndPlayVisible
    {
        get { return trashAndPlayVisible; }
        set { trashAndPlayVisible = value; }
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

    public bool InsideInteractionLayout
    {
        get { return insideInteractionLayout; }
        set { insideInteractionLayout = value; }
    }
    // Use this for initialization
    void Start ()
    {
        trashBin = GameObject.FindGameObjectWithTag("TrashCan");
        playButton = GameObject.FindGameObjectWithTag("PlayButton");
        interactionLayout = GameObject.FindGameObjectWithTag("InteractionLayout");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(InsideProfile || InsideTrash || InsidePlay)
        {
            trashAndPlayVisible = true;
        }
	    if(!InsideProfile && !InsideTrash && !InsidePlay)
        {
            trashAndPlayVisible = false;
        }

        if(trashAndPlayVisible)
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

        if(InsidePlay)
        {
            interactionLayout.SetActive(true);
        }

        else
        {
            interactionLayout.SetActive(false);
        }
	}
}
