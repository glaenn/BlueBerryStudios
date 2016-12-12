using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Texture2D texture;
    static LoadingScreen instance;

    void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);            
            return;
        }

        instance = this;
        GUITexture guiTex = gameObject.AddComponent<GUITexture>();
        guiTex.enabled = false;
        guiTex.texture = texture;
        transform.position = new Vector3(0.5f, 0.5f, 1f);
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(!Application.isLoadingLevel)
        {
            SetActive(false);
        }
    }

    public static void SetActive(bool active = true)
    {
        if(!instance)
        {
            return;
        }
        instance.GetComponent<GUITexture>().enabled = active;
    }

    public void LoadLevel(int index)
    {
        SetActive(true);
        Application.LoadLevel(index);
    }

  


    



}
