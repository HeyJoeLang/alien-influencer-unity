using UnityEngine;
using UnityEditor;

public class SetSelectionParent : MonoBehaviour
{
    // Set Parent to 'Buildings' with hotkey Ctrl (Cmd) + Shift + P
    [MenuItem("Tools/Set Parent to 'Props' %#q")]
    static void SetParentToProps()
    {
        // Find the "Buildings" GameObject in the scene
        GameObject propsParent = GameObject.Find("Props");

        if (propsParent == null)
        {
            Debug.LogError("A GameObject named 'Props' was not found in the scene.");
            return;
        }

        // Get the selected objects in the editor
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogError("No objects selected. Please select the objects you want to reparent.");
            return;
        }

        // Change the parent of each selected object to 'Buildings'
        foreach (GameObject obj in selectedObjects)
        {
            Undo.RecordObject(obj.transform, "Change Parent");
            obj.transform.SetParent(propsParent.transform);
        }

        Debug.Log("Selected objects are now children of 'Props'.");
    }

    // Set Parent to 'Buildings' with hotkey Ctrl (Cmd) + Shift + P
    [MenuItem("Tools/Set Parent to 'Buildings' %#z")]
    static void SetParentToBuildings()
    {
        // Find the "Buildings" GameObject in the scene
        GameObject buildingsParent = GameObject.Find("Buildings");

        if (buildingsParent == null)
        {
            Debug.LogError("A GameObject named 'Buildings' was not found in the scene.");
            return;
        }

        // Get the selected objects in the editor
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogError("No objects selected. Please select the objects you want to reparent.");
            return;
        }

        // Change the parent of each selected object to 'Buildings'
        foreach (GameObject obj in selectedObjects)
        {
            Undo.RecordObject(obj.transform, "Change Parent");
            obj.transform.SetParent(buildingsParent.transform);
        }

        Debug.Log("Selected objects are now children of 'Buildings'.");
    }

    // Toggle the 'Buildings' GameObject active state with hotkey Ctrl (Cmd) + Shift + T
    [MenuItem("Tools/Toggle 'Buildings' Active State %#x")]
    static void ToggleBuildingsActiveState()
    {
        // Find the "Buildings" GameObject in the scene
        GameObject buildingsParent = GameObject.Find("Buildings");

        if (buildingsParent == null)
        {
            Debug.LogError("A GameObject named 'Buildings' was not found in the scene.");
            return;
        }

        // Toggle its active state
        bool isActive = buildingsParent.activeSelf;
        Undo.RecordObject(buildingsParent, "Toggle Buildings Active State");
        buildingsParent.SetActive(!isActive);

        Debug.Log($"'Buildings' is now {(buildingsParent.activeSelf ? "Active" : "Inactive")}.");
    }

}