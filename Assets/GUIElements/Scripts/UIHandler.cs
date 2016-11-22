//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;

//[RequireComponent(typeof(Renderer))]
//public class UIHandler : MonoBehaviour
//{

//    [SerializeField] private GameObject trashCan;
//    [SerializeField] private GameObject playButton;

//    [SerializeField] private bool isVisible = false;

//    public Renderer rend;

//    void Start()
//    {
//        rend = GetComponent<Renderer>();
//        //Debug.Log("Inside interaction area");
//        playButton = GameObject.FindGameObjectWithTag("PlayButton");
//        trashCan = GameObject.FindGameObjectWithTag("TrashCan");
//    }

//    void OnMouseEnter()
//    {
//        Debug.Log("Mouse is over!");
//        isVisible = false;
//    }

//    //void FixedUpdate()
//    //{
//    //    if(isVisible)
//    //    {
//    //        Debug.Log("Is active");
//    //        trashCan.SetActive(true);
//    //        playButton.SetActive(true);
             
//    //    }

//    //    else
//    //    {
//    //        //Debug.Log("Is not active");
//    //        trashCan.SetActive(false);
//    //        playButton.SetActive(false);
//    //    }
//    //}

//    //void OnMouseExit()
//    //{
//    //    isVisible = false;
//    //}











//}
