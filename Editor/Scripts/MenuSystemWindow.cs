using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Compilation;
using System.Linq;
using TMPro; // Add this for TextMeshPro components

namespace StackBasedMenuSystem.Editor
{
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
            logo = EditorGUIUtility.Load(EditorConstants.PACKAGE_RESOURCES_PATH + "/logo.png") as Texture2D;
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
            // Form area with flexible width but with a minimum width constraint
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MinWidth(250));


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
            if (GUILayout.Button("Add Button Name") && !EditorApplication.isCompiling)
            {
                buttonNames.Add("");
            }

            GUILayout.Space(20); // Spacer

            if (GUILayout.Button("Generate Script") && !EditorApplication.isCompiling)
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

            if (GUILayout.Button("Create and Save Prefab") && !EditorApplication.isCompiling)
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

        void CreatePrefab(string menuName, GameObject canvas)
        {
            canvas = CreateCanvas(menuName);

            for (int i = 0; i < buttonNames.Count; i++)
            {
                CreateButton(canvas, buttonNames[i], i);
            }


            TryAttachScript(menuName, canvas); // Attach your newly compiled script

            if (!Directory.Exists(EditorConstants.MENUS_PREFABS_PATH))
            {
                Directory.CreateDirectory(EditorConstants.MENUS_PREFABS_PATH);
            }

            string filePath = EditorConstants.MENUS_PREFABS_PATH + $"/{menuName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(canvas, filePath);

            DestroyImmediate(canvas);

            if (updateMenuInitialiser)
                UpdateMenuInitialiserScript(menuName);
        }

        void UpdateMenuInitialiserScript(string menuName)
        {
            if (!File.Exists(EditorConstants.GetMenuInitialiserScriptPath()))
            {
                Debug.LogError($"{EditorConstants.MENU_INITIALISER_SCRIPT_NAME} not found. Please Generate in the script via the Setup Menu.");
                return;
            }

            string menuClassName = $"{menuName}"; // Assuming the class name follows this pattern.
            string prefabFieldName = $"{menuName.ToLower()}Prefab"; // Field name for the prefab.
            string registerCall = $"MenuManager.Instance.RegisterMenuPrefab({prefabFieldName});";

            string[] lines = File.ReadAllLines(EditorConstants.GetMenuInitialiserScriptPath());
            List<string> modifiedLines = new List<string>(lines);

            // Check if the menuName already exists in the file
            bool menuExists = modifiedLines.Any(line => line.Contains($" {menuClassName} ") || line.Contains($" {prefabFieldName};"));
            if (menuExists)
            {
                Debug.LogWarning($"Menu '{menuName}' already exists in {EditorConstants.MENU_INITIALISER_SCRIPT_NAME}.");
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
                File.WriteAllLines(EditorConstants.GetMenuInitialiserScriptPath(), modifiedLines.ToArray());

                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError($"Failed to find correct insertion points in {EditorConstants.MENU_INITIALISER_SCRIPT_NAME}.");
            }
        }


        void GenerateAndWriteScript(string menuName, List<string> buttonNames)
        {
            var scriptContent = BuildMenuScript(menuName, buttonNames);

            if (!Directory.Exists(EditorConstants.MENUS_SCRIPTS_PATH))
            {
                Directory.CreateDirectory(EditorConstants.MENUS_SCRIPTS_PATH);
            }

            string filePath = EditorConstants.MENUS_SCRIPTS_PATH + $"/{menuName}.cs";
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

        // Method to create a TextMeshPro button
        void CreateButton(GameObject parent, string buttonName, int index)
        {
            // Create the button GameObject
            GameObject buttonObj = new GameObject(buttonName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));

            // Set the button object as a child of the parent
            buttonObj.transform.SetParent(parent.transform, false);

            // Set RectTransform defaults for the button, including position
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;

            // Calculate the Y position based on the index
            // Assuming each button's height is 30 and spacing is 10 units
            float buttonHeight = 30f;
            float spacing = 10f;
            float yPos = -index * (buttonHeight + spacing); // Negative because UI coords go down

            rectTransform.anchoredPosition = new Vector2(0, yPos);
            rectTransform.sizeDelta = new Vector2(160, buttonHeight); // Default button size

            // Configure the Button component (optional settings)
            Button button = buttonObj.GetComponent<Button>();
            button.targetGraphic = buttonObj.GetComponent<Image>(); // Set the button's Target Graphic to its Image component
            buttonObj.GetComponent<Image>().color = Color.white; // Set default button image color

            // Add a TextMeshProUGUI component for the button text
            GameObject textObj = new GameObject("ButtonText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(buttonObj.transform, false);

            // Set RectTransform defaults for the text
            RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
            textRectTransform.localScale = Vector3.one;
            textRectTransform.anchoredPosition = Vector2.zero;
            textRectTransform.sizeDelta = new Vector2(160, 30); // Adjust as needed to fit the button

            // Configure TextMeshProUGUI component
            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = buttonName; // Set the button's text
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 24; // Adjust as needed
            text.color = Color.black;

            // Set the button's Text component to the newly created TextMeshProUGUI component (Optional: depends on button interactions)
            button.GetComponentInChildren<TextMeshProUGUI>(true).text = buttonName;

            // Note: Adjust text color, font, etc., as needed
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
}