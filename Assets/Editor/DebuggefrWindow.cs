using System;
using System.Collections.Generic;
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
            GameObject rootPrefabObject = (GameObject) EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<GameObject>(prefabRootDirectoryPath), typeof(GameObject), true);
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
                    AssetDatabase.StartAssetEditing();
                    List<Transform> transformList = new List<Transform>(rootPrefabObject.GetComponentsInChildren<Transform>(true));
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
}
