using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ColliderRemoverWindow : EditorWindow
{
    // List to hold GameObjects you want to exempt from collider removal
    public List<GameObject> exemptObjects = new List<GameObject>();

    [MenuItem("Tools/Collider Remover")]
    public static void ShowWindow()
    {
        GetWindow<ColliderRemoverWindow>("Collider Remover");
    }

    private void OnGUI()
    {
        GUILayout.Label("Collider Remover", EditorStyles.boldLabel);
        GUILayout.Label("Select objects to exempt from collider removal:");

        // Create a scroll view for the exempt list
        for (int i = 0; i < exemptObjects.Count; i++)
        {
            exemptObjects[i] = (GameObject)EditorGUILayout.ObjectField(exemptObjects[i], typeof(GameObject), true);
        }

        if (GUILayout.Button("Add Exempt Object"))
        {
            exemptObjects.Add(null);
        }

        if (GUILayout.Button("Remove Last Exempt Object"))
        {
            if (exemptObjects.Count > 0)
            {
                exemptObjects.RemoveAt(exemptObjects.Count - 1);
            }
        }

        if (GUILayout.Button("Remove All Colliders"))
        {
            RemoveAllColliders();
        }
    }

    private void RemoveAllColliders()
    {
        // Get all GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Skip if the object or any of its ancestors are in the exempt list
            if (IsExempt(obj)) continue;

            // Remove all colliders from the object and its children
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                DestroyImmediate(col);
            }
        }

        Debug.Log("Colliders removed, except for exempt objects and their children.");
    }

    // Check if the object or any of its ancestors are in the exempt list
    private bool IsExempt(GameObject obj)
    {
        foreach (GameObject exemptObj in exemptObjects)
        {
            // If the object is the exempt object or its child, return true
            if (obj == exemptObj || obj.transform.IsChildOf(exemptObj.transform))
            {
                return true;
            }
        }
        return false;
    }
}
