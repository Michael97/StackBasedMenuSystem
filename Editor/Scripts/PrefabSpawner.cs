using UnityEngine;
using UnityEditor;

public class PrefabSpawner : MonoBehaviour
{
    const string PREFAB_PATH = "Packages/com.justtablesalt.stackbasedmenusystem/Runtime/Prefabs";

    [MenuItem("GameObject/MenuSystem/EventSystem", false, 10)]
    static void SpawnEventSystem(MenuCommand menuCommand)
    {
        // Path to your prefab within the package (adjust the path as necessary)
        SpawnObject(PREFAB_PATH + "/EventSystem.prefab", menuCommand);
    }

    [MenuItem("GameObject/MenuSystem/MenuManager", false, 10)]
    static void SpawnMenuManager(MenuCommand menuCommand)
    {
        // Path to your prefab within the package (adjust the path as necessary)
        SpawnObject(PREFAB_PATH + "/MenuManager.prefab", menuCommand);
    }

    [MenuItem("GameObject/MenuSystem/DebugCanvas", false, 10)]
    static void SpawnDebugCanvas(MenuCommand menuCommand)
    {
        // Path to your prefab within the package (adjust the path as necessary)
        SpawnObject(PREFAB_PATH + "/DebugCanvas.prefab", menuCommand);
    }

    static void SpawnObject(string path, MenuCommand menuCommand)
    {
        // Load the prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Prefab not found at: " + path);
            return;
        }

        // Instantiate the prefab
        var spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(spawnedObject, menuCommand.context as GameObject);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(spawnedObject, "Create " + spawnedObject.name);

        // Optionally, select the newly created object
        Selection.activeObject = spawnedObject;
    }
}
