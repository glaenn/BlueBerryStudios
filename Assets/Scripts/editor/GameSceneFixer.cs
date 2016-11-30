using UnityEngine;
using UnityEditor;

public class GameSceneFixer : EditorWindow
{
    [MenuItem("MyTools/Game Scene Fixer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GameSceneFixer));
    }


    void OnGUI()
    {
        if(GUILayout.Button("Remove (number) endings"))
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject parent in allObjects)
            {
                if (parent.name.EndsWith(")"))
                {
                    int startIndex = parent.name.IndexOf("(");

                    parent.name = parent.name.Remove(startIndex);
                }
            }

        }
    }
 

}
