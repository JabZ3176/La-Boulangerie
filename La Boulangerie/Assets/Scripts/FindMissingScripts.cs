using UnityEngine;
using UnityEditor;

public class FindMissingScripts : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Scripts")]
    static void FindMissing()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;

        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component c in components)
            {
                if (c == null)
                {
                    Debug.LogWarning("Missing script found on: " + go.name, go);
                    count++;
                }
            }
        }

        if (count == 0)
            Debug.Log("No missing scripts found!");
        else
            Debug.Log(count + " missing script(s) found — check warnings above");
    }
}