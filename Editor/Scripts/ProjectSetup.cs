using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using System;

namespace StackBasedMenuSystem.Editor
{
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
            logo = EditorGUIUtility.Load(EditorConstants.PACKAGE_RESOURCES_PATH + "/logo.png") as Texture2D;
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
            GUILayout.Label("Stack Based Menu System", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter });
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
            if (GUILayout.Button("View Documentation") && !EditorApplication.isCompiling)
            {
                Application.OpenURL("https://github.com/Michael97/StackBasedMenuSystem/blob/main/README.md");
            }


            GUILayout.Space(20); // Spacer

            if (GUILayout.Button("Generate Scripts") && !EditorApplication.isCompiling)
            {
                GenerateScripts();
            }

            GUILayout.Space(10); // Spacer

            if (GUILayout.Button("Generate Prefabs") && !EditorApplication.isCompiling)
            {
                GeneratePrefabs();
            }

            GUILayout.Space(10); // Spacer

            if (GUILayout.Button("Fix references") && !EditorApplication.isCompiling)
            {
                FixReferences();
            }

            GUILayout.Space(10); // Spacer

            if (GUILayout.Button("Add Sample Scenes to Build") && !EditorApplication.isCompiling)
            {
                AddScenesToBuild();
            }

            GUILayout.Space(20); // Spacer

            // Change the button's background color to red
            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("Purge Menu System") && !EditorApplication.isCompiling)
            {
                bool userResponse = EditorUtility.DisplayDialog(
                    "Confirm Action",
                    "Are you sure you want to do this? All files under Assets/Prefabs/MenusStackSystem, and Assets/Scripts/MenusStackSystem will be deleted. You cannot undo this action.",
                    "Yes",
                    "No"
                );

                if (userResponse)
                {
                    Purge();
                }
                else
                {
                    // Optionally handle the "No" response
                    Debug.Log("Operation cancelled by the user.");
                }

            }

            // Reset the background color for other GUI elements
            GUI.backgroundColor = Color.white;

            GUILayout.EndVertical(); // End nested vertical layout

            GUILayout.FlexibleSpace(); // Add flexible space on the right
            GUILayout.EndHorizontal(); // End horizontal layout

            GUILayout.FlexibleSpace(); // Add flexible space below
            GUILayout.EndVertical(); // End vertical layout
        }

        void FixReferences()
        {
            // Load the MenuManager prefab
            GameObject menuManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorConstants.GetMenuManagerPrefabPath());
            if (menuManagerPrefab == null)
            {
                Debug.LogError("MenuManager prefab not found.");
                return;
            }

            // Load the MenuInitialiser prefab
            GameObject menuInitialiserPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorConstants.GetMenuInitialiserPrefabPath());
            if (menuInitialiserPrefab == null)
            {
                Debug.LogError("MenuInitialiser prefab not found.");
                return;
            }

            // Access the MenuManager component
            MenuManager menuManagerComponent = menuManagerPrefab.GetComponent<MenuManager>();

            AutoAssignMenuPrefabs(menuInitialiserPrefab.GetComponent<BaseMenuInitialiser>());

            if (menuManagerComponent == null)
            {
                Debug.LogError("MenuManager component not found on the MenuManager prefab.");
                return;
            }

            // Start recording changes for undo functionality
            Undo.RecordObject(menuManagerComponent, "Set Initialiser Reference");

            // Set the Initialiser variable
            menuManagerComponent.Initialiser = menuInitialiserPrefab.GetComponent<BaseMenuInitialiser>();

            // Save changes to the prefab
            EditorUtility.SetDirty(menuManagerComponent);

            // Optionally, if you need to save the prefab explicitly (Unity 2018.3 and later)
            // PrefabUtility.SavePrefabAsset(menuManagerPrefab);

            Debug.Log("Menu System references fixed.");
        }


        void AutoAssignMenuPrefabs(BaseMenuInitialiser menuInitialiser)
        {
            var serializedObject = new SerializedObject(menuInitialiser);

            // Iterate through all fields of the BaseMenuInitialiser
            var fields = menuInitialiser.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => typeof(BaseMenu).IsAssignableFrom(f.FieldType) && f.GetCustomAttribute<SerializeField>() != null);

            foreach (var field in fields)
            {
                // Assume the prefab name matches the class name of the field type
                string prefabName = field.FieldType.Name;
                string prefabPath = $"{EditorConstants.MENUS_PREFABS_PATH}/{prefabName}.prefab";

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                {
                    // Find the serialized property that corresponds to the current field
                    var property = serializedObject.FindProperty(field.Name);
                    if (property != null)
                    {
                        property.objectReferenceValue = prefab;
                        Debug.Log($"Assigned {prefabName} to {field.Name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"No prefab found for {prefabName} at path {prefabPath}");
                }
            }

            serializedObject.ApplyModifiedProperties(); // Apply the changes to the serialized object
        }

        void AddScenesToBuild()
        {
            // Example list of scene names you want to add to the build settings
            string[] scenesToAdd = {
                EditorConstants.SAMPLE1_SCENE_PATH + "/SetupScene.unity",
                EditorConstants.SAMPLE1_SCENE_PATH + "/MainMenuScene.unity",
                EditorConstants.SAMPLE1_SCENE_PATH + "/GameScene.unity",
                EditorConstants.SAMPLE1_SCENE_PATH + "/SingleSceneExample.unity" };

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
            System.IO.Directory.Delete(EditorConstants.MENU_STACK_SYSTEM_SCRIPTS_PATH, true);
            System.IO.Directory.Delete(EditorConstants.MENU_STACK_SYSTEM_PREFABS_PATH, true);
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

            if (!Directory.Exists(EditorConstants.MENUS_SCRIPTS_PATH))
            {
                Directory.CreateDirectory(EditorConstants.MENUS_SCRIPTS_PATH);
            }

            File.WriteAllText(EditorConstants.GetMenuInitialiserScriptPath(), content);
            AssetDatabase.Refresh();
        }

        private void GenerateScripts()
        {
            CreateMenuInitialiser();
            DuplicateScriptTemplates();
        }

        private void GeneratePrefabs()
        {
            CreateMenuInitialiserObject();

            var masterInputhandler = new GameObject();
            masterInputhandler.name = "MasterInputHandler";
            masterInputhandler.AddComponent<MasterInputHandler>();

            // Save the masterInputHandler as a prefab and get a reference to it
            GameObject masterInputHandlerPrefab = PrefabUtility.SaveAsPrefabAsset(masterInputhandler, EditorConstants.GetMasterInputHandlerPrefabPath());
            DestroyImmediate(masterInputhandler);
            Debug.Log($"Saved Master Input Handler prefab to {EditorConstants.MENU_STACK_SYSTEM_PREFABS_PATH}");

            var menuManagerPrefab = new GameObject();
            menuManagerPrefab.name = "MenuManager";
            AddComponentToPrefab<MenuManager>(menuManagerPrefab);
            menuManagerPrefab.GetComponent<MenuManager>().Initialiser = AssetDatabase.LoadAssetAtPath<BaseMenuInitialiser>(EditorConstants.GetMenuInitialiserPrefabPath());
            PrefabUtility.SaveAsPrefabAsset(menuManagerPrefab, EditorConstants.GetMenuManagerPrefabPath());
            DestroyImmediate(menuManagerPrefab);
            Debug.Log($"Saved MenuManager prefab to {EditorConstants.MENU_STACK_SYSTEM_PREFABS_PATH}");

            var gameManagerPrefab = new GameObject();
            gameManagerPrefab.name = "MenuStack_GameManager";
            TryAttachScript("MenuStack_GameManager", gameManagerPrefab);

            // Attach the masterInputHandlerPrefab to the MenuStack_GameManager's MasterInputHandler variable using reflection
            AttachMasterInputHandlerUsingReflection(gameManagerPrefab, masterInputHandlerPrefab);

            PrefabUtility.SaveAsPrefabAsset(gameManagerPrefab, EditorConstants.GetGameManagerPrefabPath());
            DestroyImmediate(gameManagerPrefab);
            Debug.Log($"Saved Game Manager prefab to {EditorConstants.MENU_STACK_SYSTEM_PREFABS_PATH}");
        }
        
        private void AttachMasterInputHandlerUsingReflection(GameObject gameManagerPrefab, GameObject masterInputHandlerPrefab)
        {
            // Assuming the script component is the first component, adjust if necessary
            var gameManagerScript = gameManagerPrefab.GetComponents<Component>().FirstOrDefault(c => c.GetType().Name == "MenuStack_GameManager");

            if (gameManagerScript != null)
            {
                // Get the type of the script
                Type scriptType = gameManagerScript.GetType();

                // Find the MasterInputHandler field
                FieldInfo masterInputHandlerField = scriptType.GetField("MasterInputHandler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (masterInputHandlerField != null)
                {
                    // Set the value of the MasterInputHandler field to the masterInputHandlerPrefab
                    masterInputHandlerField.SetValue(gameManagerScript, masterInputHandlerPrefab.GetComponent<MasterInputHandler>());
                }
                else
                {
                    Debug.LogError("MasterInputHandler field not found in MenuStack_GameManager script.");
                }
            }
            else
            {
                Debug.LogError("MenuStack_GameManager script not found on the gameManagerPrefab.");
            }
        }
   


        void CreateMenuInitialiserObject()
        {
            var name = "MenuInitialiser";
            var prefab = new GameObject(name);

            TryAttachScript(name, prefab);

            if (!Directory.Exists(EditorConstants.MENU_STACK_SYSTEM_PREFABS_PATH))
                Directory.CreateDirectory(EditorConstants.MENU_STACK_SYSTEM_PREFABS_PATH);

            PrefabUtility.SaveAsPrefabAsset(prefab, EditorConstants.GetMenuInitialiserPrefabPath());

            DestroyImmediate(prefab);

            Debug.Log($"Saved MenuInitialiser prefab to {EditorConstants.MENU_STACK_SYSTEM_PREFABS_PATH}");
        }

        void TryAttachScript(string menuName, GameObject targetGameObject)
        {
            //var scriptType = GetType(menuName);
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

        System.Type GetType(string typeName)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Debug.Log($"Checking assembly: {assembly.FullName}");
                var type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }


        System.Type GetTypeDirect(string typeName)
        {
            return System.Type.GetType(typeName + ", Assembly-CSharp");
        }

        public static void AddComponentToPrefab<T>(GameObject prefab) where T : Component
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null.");
                return;
            }

            // Check if the prefab already has the component to avoid duplicates
            if (!prefab.GetComponent<T>())
            {
                // Start recording changes for undo functionality
                Undo.RecordObject(prefab, $"Add {typeof(T).Name} to {prefab.name}");

                // Directly add the component to the prefab
                prefab.AddComponent<T>();

                // Mark the prefab as dirty so the change is saved
                EditorUtility.SetDirty(prefab);
            }
            else
            {
                Debug.LogWarning($"{prefab.name} already has a {typeof(T).Name} component.");
            }
        }

        void DuplicateScriptTemplates()
        {
            string templatePath = EditorConstants.PACKAGE_TEMPLATE_SCRIPTS_PATH;
            string targetPath = EditorConstants.MENU_STACK_SYSTEM_SCRIPTS_PATH;


            // Get all script template files in the directory
            string[] templateFiles = System.IO.Directory.GetFiles(templatePath, "*Template.cs");

            foreach (string filePath in templateFiles)
            {
                string filename = System.IO.Path.GetFileName(filePath);
                string destinationFilename = filename.Replace("Template", "");
                string destinationPath = System.IO.Path.Combine(targetPath, destinationFilename);

                string fileContents = System.IO.File.ReadAllText(filePath);

                // Use regex to replace class names ending with 'Template'
                string modifiedContents = Regex.Replace(fileContents, @"(\w+)Template", m => m.Groups[1].Value);

                string destinationDir = System.IO.Path.GetDirectoryName(destinationPath);
                if (!System.IO.Directory.Exists(destinationDir))
                {
                    System.IO.Directory.CreateDirectory(destinationDir);
                }

                System.IO.File.WriteAllText(destinationPath, modifiedContents);

                AssetDatabase.ImportAsset(destinationPath, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Script template {filename} copied and modified to: {destinationPath}");
            }

            AssetDatabase.Refresh();
        }


    }
}