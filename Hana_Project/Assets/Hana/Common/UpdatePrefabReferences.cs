using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class UpdatePrefabReferences : EditorWindow
{
    [MenuItem("Tools/Update Prefab References")]
    static void UpdateReferences()
    {
        string resourcePath = "Assets/Hana/Resources/";

        // 모든 프리팹 가져오기
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Hana/Prefabs" });

        foreach (string guid in prefabGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                    {
                        Material mat = renderer.sharedMaterials[i];
                        if (mat != null)
                        {
                            string matPath = AssetDatabase.GetAssetPath(mat);
                            string newMatPath = resourcePath + Path.GetFileName(matPath);

                            Material newMat = AssetDatabase.LoadAssetAtPath<Material>(newMatPath);
                            if (newMat != null)
                            {
                                renderer.sharedMaterials[i] = newMat;
                            }
                        }
                    }
                }

                // 프리팹 저장
                PrefabUtility.SavePrefabAsset(prefab);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ 모든 프리팹이 새로운 리소스를 참조하도록 업데이트되었습니다!");
    }
}
