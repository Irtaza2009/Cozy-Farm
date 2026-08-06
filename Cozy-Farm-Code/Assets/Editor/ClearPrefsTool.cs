using UnityEngine;
using UnityEditor;

public class ClearPrefsTool
{
    [MenuItem("Tools/Clear Tutorial Pref")]
    public static void DeleteTutorialPref()
    {
        PlayerPrefs.DeleteKey("Tutorial_Completed");
        PlayerPrefs.Save();
        Debug.Log("Tutorial_Completed key deleted successfully!");
    }
}
