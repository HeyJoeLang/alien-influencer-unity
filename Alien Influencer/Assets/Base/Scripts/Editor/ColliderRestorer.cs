using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ColliderRestorerWindow : EditorWindow
{
    // Dictionary to store removed colliders for each GameObject
    private static Dictionary<GameObject, List<ColliderData>> removedColliders = new Dictionary<GameObject, List<ColliderData>>();

    // Struct to hold collider information before removal
    private struct ColliderData
    {
        public Vector3 center;
        public Vector3 size;
        public Quaternion rotation;
        public ColliderType type;
    }

    // Enum to represent different collider types
    private enum ColliderType
    {
        Box,
        Sphere,
        Capsule,
        Mesh
    }

    // Parent GameObject field for the user to specify
    public GameObject parentObject;

    [MenuItem("Tools/Collider Restorer")]
    public static void ShowWindow()
    {
        GetWindow<ColliderRestorerWindow>("Collider Restorer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Collider Restorer", EditorStyles.boldLabel);
        GUILayout.Label("Specify the parent object:");

        // Object field to specify the parent GameObject
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        // If the parentObject is set, show buttons to remove or restore colliders
        if (parentObject != null)
        {
            if (GUILayout.Button("Remove Colliders"))
            {
                RemoveColliders(parentObject);
            }

            if (GUILayout.Button("Restore Colliders"))
            {
                RestoreColliders(parentObject);
            }
        }
        else
        {
            GUILayout.Label("Please assign a parent object.");
        }
    }

    // Method to remove colliders from a GameObject and its children, and store the removed colliders
    private void RemoveColliders(GameObject targetObject)
    {
        Collider[] colliders = targetObject.GetComponentsInChildren<Collider>();
        List<ColliderData> colliderDataList = new List<ColliderData>();

        foreach (Collider col in colliders)
        {
            ColliderData data = new ColliderData
            {
                center = Vector3.zero,
                size = Vector3.zero,
                rotation = Quaternion.identity
            };

            if (col is BoxCollider boxCollider)
            {
                data.type = ColliderType.Box;
                data.center = boxCollider.center;
                data.size = boxCollider.size;
            }
            else if (col is SphereCollider sphereCollider)
            {
                data.type = ColliderType.Sphere;
                data.center = sphereCollider.center;
                data.size = Vector3.one * sphereCollider.radius;
            }
            else if (col is CapsuleCollider capsuleCollider)
            {
                data.type = ColliderType.Capsule;
                data.center = capsuleCollider.center;
                data.size = Vector3.one * capsuleCollider.height;
            }
            else if (col is MeshCollider meshCollider)
            {
                data.type = ColliderType.Mesh;
            }

            // Save the removed collider's data
            colliderDataList.Add(data);

            // Remove the collider
            DestroyImmediate(col);
        }

        // Store removed colliders in the dictionary
        removedColliders[targetObject] = colliderDataList;
        Debug.Log("Colliders removed for " + targetObject.name);
    }

    // Method to restore the removed colliders for a specific GameObject, its ancestors, and children
    private void RestoreColliders(GameObject targetObject)
    {
        // First restore colliders for the specified object and its children
        RestoreCollidersForGameObjectAndChildren(targetObject);

        // Now walk up the hierarchy and restore colliders for ancestors
        Transform currentTransform = targetObject.transform.parent;
        while (currentTransform != null)
        {
            RestoreCollidersForGameObjectAndChildren(currentTransform.gameObject);
            currentTransform = currentTransform.parent;
        }
    }

    // Helper method to restore colliders for a GameObject and its children
    private void RestoreCollidersForGameObjectAndChildren(GameObject targetObject)
    {
        if (removedColliders.TryGetValue(targetObject, out List<ColliderData> colliderDataList))
        {
            foreach (ColliderData data in colliderDataList)
            {
                switch (data.type)
                {
                    case ColliderType.Box:
                        BoxCollider boxCollider = targetObject.AddComponent<BoxCollider>();
                        boxCollider.center = data.center;
                        boxCollider.size = data.size;
                        break;
                    case ColliderType.Sphere:
                        SphereCollider sphereCollider = targetObject.AddComponent<SphereCollider>();
                        sphereCollider.center = data.center;
                        sphereCollider.radius = data.size.x / 2f; // Assuming the radius was stored as size.x
                        break;
                    case ColliderType.Capsule:
                        CapsuleCollider capsuleCollider = targetObject.AddComponent<CapsuleCollider>();
                        capsuleCollider.center = data.center;
                        capsuleCollider.height = data.size.x; // Assuming height stored as size.x
                        break;
                    case ColliderType.Mesh:
                        targetObject.AddComponent<MeshCollider>();
                        break;
                }
            }

            Debug.Log("Colliders restored for " + targetObject.name);
        }
        else
        {
            Debug.LogWarning("No colliders to restore for " + targetObject.name);
        }
    }
}
