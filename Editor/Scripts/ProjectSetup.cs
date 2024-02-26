using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using Codice.CM.Common.Purge;

public class ProjectSetup : EditorWindow
{
    private Texture2D logo;

    [MenuItem("Tools/MenuStackSystem/Setup")]
    private static void ShowWindow()
    {
        var window = GetWindow<ProjectSetup>("Project Setup");
        window.minSize = new Vector2(300, 60); // Set the minimum size of the window
    }

    private void OnEnable()
    {
        // Load the logo
        logo = EditorGUIUtility.Load("Packages/com.justtablesalt.stackbasedmenusystem/Editor/Resources/logo.png") as Texture2D;
    }

    void OnGUI()
    {
        GUILayout.BeginVertical(); // Start a vertical layout
        GUILayout.FlexibleSpace(); // Add flexible space above to center content vertically

        GUILayout.BeginHorizontal(); // Start a horizontal layout
        GUILayout.FlexibleSpace(); // Add flexible space on the left to center content horizontally

        // Your content here
        GUILayout.BeginVertical(); // Nested vertical layout for tighter control
        if (logo != null)
        {
            var centeredStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUILayout.Label(logo, centeredStyle, GUILayout.Width(200), GUILayout.Height(100));
        }
        GUILayout.Label("Stack Based Menu System",  new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter });
        GUILayout.Label("By Just Table Salt", new GUIStyle(EditorStyles.miniBoldLabel) { alignment = TextAnchor.MiddleCenter });

        GUILayout.EndVertical(); // End nested vertical layout

        GUILayout.FlexibleSpace(); // Add flexible space on the right
        GUILayout.EndHorizontal(); // End horizontal layout

        GUILayout.FlexibleSpace(); // Add flexible space below
        GUILayout.EndVertical(); // End vertical layout

        GUILayout.BeginVertical(); // Start a vertical layout
        GUILayout.FlexibleSpace(); // Add flexible space above to center content vertically

        GUILayout.BeginHorizontal(); // Start a horizontal layout
        GUILayout.FlexibleSpace(); // Add flexible space on the left to center content horizontally

        // Your content here
        GUILayout.BeginVertical(); // Nested vertical layout for tighter control

        // Generic text about reading the documentation and downloading samples
        GUILayout.Label("Welcome to the Stack Based Menu System. For a comprehensive understanding of how to use this system, please refer to the documentation. If this is your first time using the project, we highly recommend downloading the samples to get started.", new GUIStyle(EditorStyles.wordWrappedLabel) { alignment = TextAnchor.MiddleCenter });

        // Displaying a clickable link to documentation
        if (GUILayout.Button("View Documentation"))
        {
            Application.OpenURL("https://github.com/Michael97/StackBasedMenuSystem/blob/main/README.md");
        }


        GUILayout.Space(20); // Spacer

        if (GUILayout.Button("Generate Scripts"))
        {
            GenerateScripts();
        }

        GUILayout.Space(10); // Spacer

        if (GUILayout.Button("Generate Prefabs"))
        {
            GeneratePrefabs();
        }

        GUILayout.Space(10); // Spacer

        if (GUILayout.Button("Add Sample Scenes to Build"))
        {
            AddScenesToBuild();
        }

        /* if (GUILayout.Button("Purge Menu System"))
         {
             Purge();
         }
        */
        GUILayout.EndVertical(); // End nested vertical layout

        GUILayout.FlexibleSpace(); // Add flexible space on the right
        GUILayout.EndHorizontal(); // End horizontal layout

        GUILayout.FlexibleSpace(); // Add flexible space below
        GUILayout.EndVertical(); // End vertical layout
    }

    const string SCENE_PATH = "Assets/Samples/Stack Based Menu System/1.0.0/Sample1/Scenes";

    void AddScenesToBuild()
    {
        // Example list of scene names you want to add to the build settings
        string[] scenesToAdd = { SCENE_PATH + "/SetupScene.unity", SCENE_PATH + "/MainMenuScene.unity", SCENE_PATH + "/GameScene.unity", SCENE_PATH + "/SingleSceneExample.unity" };

        // Get the current list of scenes in the build settings
        var originalScenes = EditorBuildSettings.scenes;

        foreach (var scenePath in scenesToAdd)
        {
            if (!File.Exists(scenePath))
            {
                Debug.LogWarning($"Scene at path {scenePath} does not exist and will not be added to the build settings.");
                continue; // Skip to the next scene in the list
            }

            bool sceneAlreadyPresent = false;
            foreach (var scene in originalScenes)
            {
                if (scene.path == scenePath)
                {
                    sceneAlreadyPresent = true;
                    break;
                }
            }

            if (!sceneAlreadyPresent)
            {
                var newScenesList = new System.Collections.Generic.List<EditorBuildSettingsScene>(originalScenes)
                {
                    new EditorBuildSettingsScene(scenePath, true)
                };
                EditorBuildSettings.scenes = newScenesList.ToArray();
                Debug.Log($"Scene added to build settings: {scenePath}");
            }
        }
    }

    void Purge()
    {
        System.IO.Directory.Delete("Assets/Prefabs/Menus", true);
        System.IO.Directory.Delete("Assets/Scripts/Menus", true);
        AssetDatabase.Refresh();
    }

    void CreateMenuInitialiser()
    {
        string content = $@"using UnityEngine;
using StackBasedMenuSystem;

[RequireComponent(typeof(MenuInputHandler))]
public class MenuInitialiser : BaseMenuInitialiser
{{
    /// MACHINE GENERATED CODE DO NOT MODIFY ///
    //Start Prefab Menus
    //End Prefab Menus
    /// MACHINE GENERATED CODE DO NOT MODIFY ///
    
    protected override void RegisterMenus()
    {{
        /// MACHINE GENERATED CODE DO NOT MODIFY ///
        //Start Prefab Register
        //End Prefab Register
        /// MACHINE GENERATED CODE DO NOT MODIFY ///
    }}


    protected override void InitialMenuShow()
    {{

    }}

    protected override void Subscribe()
    {{

    }}

    protected override void Unsubscribe()
    {{

    }}
}}
";

        string folderPath = Path.Combine(Application.dataPath, "Scripts/Menus");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"MenuInitialiser.cs");
        File.WriteAllText(filePath, content);
        AssetDatabase.Refresh();
    }

    private void GenerateScripts()
    {
        CreateMenuInitialiser();

    }

    private void GeneratePrefabs()
    {
        CreateMenuInitialiserObject();
    }

    void CreateMenuInitialiserObject()
    {
        var name = "MenuInitialiser";
        var prefab = new GameObject(name);

        TryAttachScript(name, prefab);

        string folderPath = Application.dataPath + "/Prefabs/Menus";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, $"{name}.prefab");
        PrefabUtility.SaveAsPrefabAsset(prefab, filePath);
        DestroyImmediate(prefab);
        Debug.Log("Saved prefab");
    }

    void TryAttachScript(string menuName, GameObject targetGameObject)
    {
        var scriptType = GetTypeDirect(menuName);
        if (scriptType != null)
        {
            targetGameObject.AddComponent(scriptType);
            Debug.Log($"{menuName} script attached to {targetGameObject.name}.");
        }
        else
        {
            Debug.LogError($"Script {menuName} not found or not compiled.");
        }
    }


    System.Type GetTypeDirect(string typeName)
    {
        return System.Type.GetType(typeName + ", Assembly-CSharp");
    }

    private void SetupProject()
    {
        // Check if the specific path exists
        if (Directory.Exists(SCENE_PATH))
        {
            AddScenesToBuild();
        }
        else
        {
            CreateMenuInitialiser();
            Debug.Log($"{SCENE_PATH} does not exist. Skipping scene addition step.");
        }

        // Implement the actual setup logic here
        Debug.Log("Project setup completed.");
    }
}