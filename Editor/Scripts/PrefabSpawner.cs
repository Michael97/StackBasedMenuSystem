using UnityEngine;
using UnityEditor;

namespace StackBasedMenuSystem.Editor
{
    public class PrefabSpawner : MonoBehaviour
    {
        [MenuItem("GameObject/MenuSystem/EventSystem", false, 10)]
        static void SpawnEventSystem(MenuCommand menuCommand)
        {
            SpawnObject(EditorConstants.PACKAGE_RUNTIME_PREFAB_PATH + "/EventSystem.prefab", menuCommand);
        }
/*
        [MenuItem("GameObject/MenuSystem/MenuManager", false, 10)]
        static void SpawnMenuManager(MenuCommand menuCommand)
        {
            SpawnObject(EditorConstants.PACKAGE_RUNTIME_PREFAB_PATH + "/MenuManager.prefab", menuCommand);
        }*/

        [MenuItem("GameObject/MenuSystem/DebugCanvas", false, 10)]
        static void SpawnDebugCanvas(MenuCommand menuCommand)
        {
            SpawnObject(EditorConstants.PACKAGE_RUNTIME_PREFAB_PATH + "/DebugCanvas.prefab", menuCommand);
        }

        /*
        [MenuItem("GameObject/MenuSystem/GameManager", false, 10)]
        static void SpawnGameManager(MenuCommand menuCommand)
        {
            SpawnObject(EditorConstants.PACKAGE_RUNTIME_PREFAB_PATH + "/MenuStack_GameManager.prefab", menuCommand);
        }*/


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

            Selection.activeObject = spawnedObject;
        }
    }
}