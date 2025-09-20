using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DebuggerWindow : EditorWindow
{
    bool foldoutDetachCompoments = true;
    string prefabRootDirectoryPath = "";

    Vector2 scrollPos = Vector2.zero;

    void OnEnable()
    {

    }

    [MenuItem("Tools/DebuggerWindow")]
    static void Open()
    {
        EditorWindow.GetWindow<DebuggerWindow>("DebuggerWindow");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        WithInFoldoutBlock("Detach Compoments", ref foldoutDetachCompoments, () =>
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("target RootPrefab");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            prefabRootDirectoryPath = (string)EditorGUILayout.TextField(prefabRootDirectoryPath);
            UnityEngine.Object rootPrefabObject = EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabRootDirectoryPath), typeof(UnityEngine.Object), true);
            if (rootPrefabObject != null)
            {
                prefabRootDirectoryPath = AssetDatabase.GetAssetPath(rootPrefabObject);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove All Collider"))
            {
                if (rootPrefabObject != null)
                {
                    List<string> prefabFilePaths = FindAllThreedSearchDirectory(prefabRootDirectoryPath, "prefab");
                    foreach (var prefabPath in prefabFilePaths)
                    {
                        var prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                        if (prefabObject == null)
                        {
                            continue;
                        }
                        AssetDatabase.StartAssetEditing();
                        List<Transform> transformList = new List<Transform>(prefabObject.GetComponentsInChildren<Transform>(true));
                        foreach (var child in transformList)
                        {
                            var compoment = child.GetComponent<Collider>();
                            if (compoment != null)
                            {
                                GameObject.DestroyImmediate(compoment, true);
                            }
                        }
                        AssetDatabase.StopAssetEditing();
                        foreach (var child in transformList)
                        {
                            EditorUtility.SetDirty(child);
                        }
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        });

        EditorGUILayout.EndScrollView();
    }

    void WithInFoldoutBlock(string title, ref bool foldout, Action callback)
    {
        EditorGUI.indentLevel = 0;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        foldout = EditorGUILayout.Foldout(foldout, title);
        if (foldout)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.Space();
            callback();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private List<string> FindAllThreedSearchDirectory(string searchRootDirectory, string extension)
    {
        List<string> seachedFilePathes = new List<string>();
        string[] pathes = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < pathes.Length; ++i)
        {
            string path = pathes[i];
            Match match = Regex.Match(path.ToLower(), @"" + searchRootDirectory.ToLower() + ".+." + extension);
            if (match.Success)
            {
                seachedFilePathes.Add(path);
            }
        }
        return seachedFilePathes;
    }
}
