using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFieldOfView : MonoBehaviour {

    private Camera cam;

    // Use this for initialization
    void Start()
    {
        GameController.OnFieldOfViewUpdate += new GameController.FieldOfViewUpdate(ChangeFieldOfView);
        cam = GetComponent<Camera>();
        ChangeFieldOfView(GameController.instance.FieldOfView);
    }

    // Runs evertime the field of view is changed in the settings
    void ChangeFieldOfView(float fieldOfView)
    {
        cam.fieldOfView = fieldOfView;
    }
}
