using StackBasedMenuSystem;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEditor.Compilation;
using System.Linq;

public class MenuSystemWindow : EditorWindow
{
    private Texture2D logo;
    private string menuName = "NewMenu";
    private bool updateMenuInitialiser = false;
    private BaseMenu.CloseType closeType = BaseMenu.CloseType.Destroy;
    private BaseMenu.MenuType menuType = BaseMenu.MenuType.Generic;
    private List<string> buttonNames = new List<string>(); // Store dynamic button names

    private GameObject canvas;
    private bool quickCreation = false;


    [MenuItem("Tools/MenuStackSystem/Menu System")]
    public static void ShowWindow()
    {
        var window = GetWindow<MenuSystemWindow>("Menu System");
        window.minSize = new Vector2(400, 550); // Set minimum size

    }

    private void OnEnable()
    {
        // Load the logo
        logo = EditorGUIUtility.Load("Packages/com.justtablesalt.stackbasedmenusystem/Editor/Resources/logo.png") as Texture2D;
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Centers content before the form

        // Vertical layout for logo and text
        GUILayout.BeginVertical();

        GUILayout.Space(20); // Spacer
        if (logo != null)
        {
            GUILayout.Label(logo, GUILayout.Width(200), GUILayout.Height(100)); // Adjust logo display
        }
        GUILayout.Label("Stack Based Menu System", EditorStyles.boldLabel);
        GUILayout.Label("By Just Table Salt", EditorStyles.miniBoldLabel);
        GUILayout.EndVertical();

        GUILayout.Space(10); // Spacer

        // Form area
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

        GUILayout.Space(20); // Spacer

        GUILayout.Label("Create New Menu", EditorStyles.boldLabel);

        GUILayout.Space(10); // Spacer

        // Inputs and buttons
        menuName = EditorGUILayout.TextField("Menu Name", menuName);
        updateMenuInitialiser = EditorGUILayout.Toggle("Add to Menu Initialiser", updateMenuInitialiser);
        closeType = (BaseMenu.CloseType)EditorGUILayout.EnumPopup("Close Type", closeType);
        menuType = (BaseMenu.MenuType)EditorGUILayout.EnumPopup("Menu Type", menuType);

        // Dynamic button names
        EditorGUILayout.LabelField("Button Names:");
        for (int i = 0; i < buttonNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            buttonNames[i] = EditorGUILayout.TextField(buttonNames[i]);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                buttonNames.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Button Name"))
        {
            buttonNames.Add("");
        }

        GUILayout.Space(20); // Spacer

        if (GUILayout.Button("Generate Script"))
        {
            DestroyImmediate(canvas);
            canvas = null;

            GenerateAndWriteScript(menuName, buttonNames);
            AssetDatabase.Refresh();
            CompilationPipeline.assemblyCompilationFinished += OnCompilationFinished;

            if (!quickCreation)
            {
                // Display a simple "Success" dialogue box with an "OK" button
                EditorUtility.DisplayDialog(
                    "Operation Completed", // Title
                    "Successfully created a the menu script. Please click the Create and Save Prefab button next.", // Message
                    "OK" // OK button text
                );
            }

        }

        if (GUILayout.Button("Create and Save Prefab"))
        {
            CreatePrefab(menuName, canvas);

            if (!quickCreation)
            {
                // Display a simple "Success" dialogue box with an "OK" button
                EditorUtility.DisplayDialog(
                    "Operation Completed", // Title
                    "Successfully created a new menu. Please update the references in the MenuInitialiser prefab.", // Message
                    "OK" // OK button text
                );
            }
        }

        GUILayout.Space(10); // Spacer
        EditorGUILayout.LabelField("Don't forget to update your references!");
        GUILayout.Space(10); // Spacer
        EditorGUILayout.LabelField("Advanced Settings");
        quickCreation = EditorGUILayout.Toggle("Quick Creation", quickCreation);

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void SetupProject()
    {
        
    }

    private void OnCompilationFinished(string assemblyPath, CompilerMessage[] compilerMessages)
    {
        var errorCount = 0;
        foreach (var message in compilerMessages)
        {
            if (message.type == CompilerMessageType.Error)
            {
                errorCount++;
            }
        }

        if (errorCount == 0)
        {
            if (EditorApplication.isCompiling == false)
            {
                //CreatePrefab(menuName, canvas);
            }
            else
            {
                //Debug.Log("Something is still compiling");
            }
   
        }

        Debug.Log("Script successful created, now click Create and Save Prefab");
        // Important: Remove the callback to prevent it from being called multiple times
        CompilationPipeline.assemblyCompilationFinished -= OnCompilationFinished;
    }
   



    void Discard()
    {
        DestroyImmediate(canvas);
        canvas = null;
    }

    void CreatePrefab(string menuName, GameObject canvas)
    {
        canvas = CreateCanvas(menuName);

        foreach (var buttonName in buttonNames)
        {
            CreateButton(canvas, buttonName);
        }

        TryAttachScript(menuName, canvas); // Attach your newly compiled script

        string folderPath = Path.Combine(Application.dataPath, "Prefabs/Menus");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"{menuName}.prefab");
        PrefabUtility.SaveAsPrefabAsset(canvas, filePath);

        DestroyImmediate(canvas);

        if (updateMenuInitialiser)
            UpdateMenuInitialiserScript(menuName);

        Debug.Log("Saved prefab successfully.");

        // Any other post-prefab creation actions
    }

    void UpdateMenuInitialiserScript(string menuName)
    {
        string menuInitialiserPath = Path.Combine(Application.dataPath, "Scripts/Menus/MenuInitialiser.cs");
        if (!File.Exists(menuInitialiserPath))
        {
            Debug.LogError("MenuInitialiser.cs not found.");
            return;
        }

        string menuClassName = $"{menuName}"; // Assuming the class name follows this pattern.
        string prefabFieldName = $"{menuName.ToLower()}Prefab"; // Field name for the prefab.
        string registerCall = $"MenuManager.Instance.RegisterMenuPrefab({prefabFieldName});";

        string[] lines = File.ReadAllLines(menuInitialiserPath);
        List<string> modifiedLines = new List<string>(lines);

        // Check if the menuName already exists in the file
        bool menuExists = modifiedLines.Any(line => line.Contains($" {menuClassName} ") || line.Contains($" {prefabFieldName};"));
        if (menuExists)
        {
            Debug.LogWarning($"Menu '{menuName}' already exists in MenuInitialiser.cs.");
            return;
        }

        // Find insertion points
        int prefabFieldInsertIndex = modifiedLines.FindIndex(x => x.Contains("//End Prefab Menus"));
        int registerCallInsertIndex = modifiedLines.FindIndex(x => x.Contains("//End Prefab Register")) + 1;

        // Prepare the new lines to insert
        string newPrefabFieldLine = $"    [SerializeField] private {menuClassName} {prefabFieldName};";
        string newRegisterCallLine = $"        {registerCall}";

        // Ensure we are inserting in the correct places
        if (prefabFieldInsertIndex != -1 && registerCallInsertIndex > prefabFieldInsertIndex) // Basic check
        {
            // Insert the new lines
            modifiedLines.Insert(prefabFieldInsertIndex, newPrefabFieldLine);
            modifiedLines.Insert(registerCallInsertIndex, newRegisterCallLine);

            // Write the modified content back to the file
            File.WriteAllLines(menuInitialiserPath, modifiedLines.ToArray());

            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("Failed to find correct insertion points in MenuInitialiser.cs.");
        }
    }


    void GenerateAndWriteScript(string menuName, List<string> buttonNames)
    {
        var scriptContent = BuildMenuScript(menuName, buttonNames);

        string folderPath = Path.Combine(Application.dataPath, "Scripts/Menus");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"{menuName}.cs");
        File.WriteAllText(filePath, scriptContent);

    }

    // Method to create canvas
    GameObject CreateCanvas(string canvasName)
    {
        GameObject canvasObj = new GameObject(canvasName);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.name = canvasName;
        return canvasObj;
    }

    // Method to create button
    void CreateButton(GameObject parent, string buttonName)
    {
        GameObject buttonObj = new GameObject(buttonName);
        buttonObj.transform.parent = parent.transform;
        // Configure the button component and RectTransform here
        Button button = buttonObj.AddComponent<Button>();
        // Optionally set button text, position, etc.
        button.name = buttonName;
    }

    private string BuildMenuScript(string menuName, List<string> buttons)
    {
        string buttonListeners = "";
        string buttonFunctions = "";

        foreach (var buttonName in buttons)
        {
            // Proper indentation for adding button listeners within the BindButtonActions method
            buttonListeners += $"        FindButtonAndAddListener(\"{buttonName}Button\", OnClick{buttonName});\n";

            // Properly indented button click handler methods
            buttonFunctions += $@"    public void OnClick{buttonName}()
    {{
        // Implement what happens when {buttonName} is clicked  
        Debug.Log(""{buttonName} clicked""); 
    }}

";
        }

        // Combining the parts with correct indentations to form the final script content
        string scriptContent = $@"using StackBasedMenuSystem;
using UnityEngine;

public class {menuName} : SimpleMenu<{menuName}>
{{
    public override void BindButtonActions()
    {{
        base.BindButtonActions();
{buttonListeners}    }}

{buttonFunctions}
}}";
        return scriptContent;
    }

    void WaitForCompilationToFinish(string menuName, GameObject canvas)
    {
        Debug.Log("Waiting for compilation to finish...");
        EditorApplication.CallbackFunction callback = null; // Define callback
        callback = () =>
        {
            if (!EditorApplication.isCompiling)
            {
                Debug.Log("Compilation finished.");
                EditorApplication.update -= callback; // Remove the callback

                // Now that compilation is finished, proceed with the rest of your logic
                PostCompilationActions(menuName, canvas);
            }
        };

        EditorApplication.update += callback; // Add the callback to the editor update
    }

    void PostCompilationActions(string menuName, GameObject canvas)
    {
        // Your logic here. For example:
        TryAttachScript(menuName, canvas); // Attach your newly compiled script

        string folderPath = Application.dataPath + "/Prefabs/Menus";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, $"{menuName}.prefab");
        PrefabUtility.SaveAsPrefabAsset(canvas, filePath);
        Debug.Log("Saved prefab");

        // Any other actions you need to take after compilation
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


}
