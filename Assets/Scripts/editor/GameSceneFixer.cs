using UnityEngine;
using UnityEditor;
using System.Collections;

public class GameSceneFixer : EditorWindow
{

    [MenuItem("MyTools/Game Scene Fixer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GameSceneFixer));
    }


    void OnGUI()
    {
        if(GUILayout.Button("Fix Scene"))
        {
            GameObject[] allParents = UnityEngine.Object.FindObjectsOfType<GameObject>();

            foreach (GameObject parent in allParents)
            {
                foreach (Transform child in parent.transform)
                {
                    foreach (Transform childchild in child.transform)
                    {
                        foreach (Transform childchildchild in childchild.transform)
                        {
                            Debug.Log(childchild.name);
                        }
                    }
                }
            }  
        }
    }

}
