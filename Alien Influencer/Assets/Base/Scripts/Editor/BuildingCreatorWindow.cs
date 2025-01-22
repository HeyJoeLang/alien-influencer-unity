using UnityEngine;
using UnityEditor;

public class BuildingCreatorWindow : EditorWindow
{
    // Variables to store prefab and Building settings
    private GameObject prefab;
    private int maxDamage = 100;
    private int scoreValue = 10;
    private int particlesDamageScale = 1;
    private int selectedLayer = 0; // Default layer is 0 ("Default")

    [MenuItem("Tools/Building Creator")]
    public static void ShowWindow()
    {
        // Show the window
        GetWindow<BuildingCreatorWindow>("Building Creator");
    }

    private void OnGUI()
    {
        // Field to assign the prefab
        GUILayout.Label("Building Prefab", EditorStyles.boldLabel);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        // Input fields for maxDamage and scoreValue
        maxDamage = EditorGUILayout.IntField("Max Damage", maxDamage);
        scoreValue = EditorGUILayout.IntField("Score Value", scoreValue);

        // Input field for the Particles_Damage scale
        GUILayout.Label("Particles_Damage Scale", EditorStyles.boldLabel);
        particlesDamageScale = EditorGUILayout.IntField("Scale", particlesDamageScale);

        // Dropdown for selecting a layer
        GUILayout.Label("Set Layer", EditorStyles.boldLabel);
        selectedLayer = EditorGUILayout.LayerField("Layer", selectedLayer);

        // Create Building button
        if (GUILayout.Button("Create Building"))
        {
            CreateBuilding();
        }
    }

    private void CreateBuilding()
    {
        // Check if a prefab is assigned
        if (prefab == null)
        {
            Debug.LogError("No prefab selected! Please assign a prefab before pressing 'Create Building'.");
            return;
        }

        // Check if an object is selected in the scene
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            Debug.LogError("No game object selected! Please select a parent object in the scene.");
            return;
        }
        //Ensure MeshCollider is present
        if (selectedObject.GetComponent<MeshCollider>() == null)
        {
           // selectedLayer.AddComponent<MeshCollider>();
        }

        // Instantiate the prefab as a child of the currently selected object
        GameObject newBuilding = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        newBuilding.transform.SetParent(selectedObject.transform);

        // Set local position and rotation
        newBuilding.transform.localPosition = Vector3.zero;
        newBuilding.transform.localRotation = Quaternion.identity;

        // Swap the parent-child relationship
        newBuilding.transform.parent = selectedObject.transform.parent.transform;
        selectedObject.transform.SetParent(newBuilding.transform);

        // Access the Building component
        Building building = newBuilding.GetComponent<Building>();

        // Set building name to match the selected object name
        newBuilding.name = selectedObject.name;

        // Replace default standing building with selected object
        Transform standingBuildingTransform = newBuilding.transform.Find("Building_Standing");

        if (standingBuildingTransform != null)
        {
            GameObject buildingStanding = standingBuildingTransform.gameObject;

            // Copy 'Outline' component from 'Building_Standing' to the selected object
            Outline outline = buildingStanding.GetComponent<Outline>();
            if (outline != null)
            {
                // Add the Outline component to the selected object if it doesn't exist, then copy properties
                Outline newOutline = selectedObject.GetComponent<Outline>() ?? selectedObject.AddComponent<Outline>();

                // Copy relevant properties from the original Outline component
                CopyOutlineProperties(outline, newOutline);
            }

            // Set buildingStanding to the selected object and destroy the old standing building
            building.buildingStanding = selectedObject;
            DestroyImmediate(buildingStanding);
        }
        else
        {
            Debug.LogWarning("Building_Standing not found in the prefab.");
        }

        // Set the values for maxDamage and scoreValue
        building.maxDamage = maxDamage;
        building.scoreValue = scoreValue;

        // Find the Particles_Damage child and set its local scale
        Transform particlesDamage = newBuilding.transform.Find("Particles_Damage");
        if (particlesDamage != null)
        {
            particlesDamage.localScale = new Vector3(particlesDamageScale, particlesDamageScale, particlesDamageScale);
        }
        else
        {
            Debug.LogWarning("Particles_Damage child not found in the prefab.");
        }

        // Set the layer for both the selected object and the new building
        newBuilding.layer = selectedLayer;
        selectedObject.layer = selectedLayer;

        // Register undo for editor workflow
        Undo.RegisterCreatedObjectUndo(newBuilding, "Create Building");

        Debug.Log("Building created, parent relationship swapped, and layers set.");
    }

    // Helper method to copy properties of the Outline component
    private void CopyOutlineProperties(Outline source, Outline destination)
    {
        destination.enabled = source.enabled;
        destination.OutlineColor = source.OutlineColor;
        destination.OutlineMode = source.OutlineMode;
        destination.OutlineWidth = source.OutlineWidth;
    }
}
