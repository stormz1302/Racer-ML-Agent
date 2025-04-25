using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class DeprecatedAsset 
{

    private const string PREF_KEY =  "RTGDeprecatedAssetDoNotShow";

    static DeprecatedAsset()
    {
    
        if (!EditorPrefs.GetBool(Application.productName + PREF_KEY, false))
        {
            EditorApplication.update += ShowDeprecatedWarning;
        }
                
    }

    private static void ShowDeprecatedWarning()
    {
        
        EditorApplication.update -= ShowDeprecatedWarning; // Remove o update para não exibir mais de uma vez

        if (EditorUtility.DisplayDialog(
        "Upgrade Available: RTG Ultimate",
        "'Race Track Generator Ultimate' is now available.\n\nWould you like to learn more?",
        "Yes", "Not now"))
        {
            Application.OpenURL("https://assetstore.unity.com/packages/3d/environments/roadways/race-track-generator-ultimate-295584");
            EditorPrefs.SetBool(Application.productName + PREF_KEY, true);
        }

        
    }


}
