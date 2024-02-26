namespace StackBasedMenuSystem.Editor
{
    public static class EditorConstants
    {
        // File paths
        public const string PREFABS_PATH = "Assets/Prefabs";
        public const string SCRIPTS_PATH = "Assets/Scripts";
        public const string MENU_STACK_SYSTEM_PATH = "/MenuStackSystem";
        public const string MENU_STACK_SYSTEM_SCRIPTS_PATH = SCRIPTS_PATH + MENU_STACK_SYSTEM_PATH;
        public const string MENU_STACK_SYSTEM_PREFABS_PATH = PREFABS_PATH + MENU_STACK_SYSTEM_PATH;
        public const string MENUS_PREFABS_PATH = MENU_STACK_SYSTEM_PREFABS_PATH + "/Menus";
        public const string MENUS_SCRIPTS_PATH = MENU_STACK_SYSTEM_SCRIPTS_PATH + "/Menus";

        public const string PACKAGE_RESOURCES_PATH = "Packages/com.justtablesalt.stackbasedmenusystem/Editor/Resources";
        public const string PACKAGE_RUNTIME_PREFAB_PATH = "Packages/com.justtablesalt.stackbasedmenusystem/Runtime/Prefabs";
        public const string PACKAGE_RUNTIME_SCRIPTS_PATH = "Packages/com.justtablesalt.stackbasedmenusystem/Runtime/Scripts";
        public const string PACKAGE_TEMPLATE_SCRIPTS_PATH = PACKAGE_RUNTIME_SCRIPTS_PATH + "/Templates";



        public const string SAMPLE1_SCENE_PATH = "Assets/Samples/Stack Based Menu System/1.0.0/Sample1/Scenes";


        // Versioning
        public const string VERSION_NUMBER = "1.0.0";

        // Identifiers
        public const string MENU_MANAGER_PREFAB_NAME = "MenuManager.prefab";
        public const string MENU_MANAGER_SCRIPT_NAME = "MenuManager.cs";
        public const string MENU_INITIALISER_PREFAB_NAME = "MenuInitialiser.prefab";
        public const string MENU_INITIALISER_SCRIPT_NAME = "MenuInitialiser.cs";
        public const string GAME_MANAGER_PREFAB_NAME = "MenuStack_GameManager.prefab";
        public const string GAME_MANAGER_SCRIPT_NAME = "MenuStack_GameManager.cs";
        public const string MASTER_INPUT_HANDLER_PREFAB_NAME = "MasterInputHandler.prefab";
        public const string MASTER_INPUT_HANDLER_SCRIPT_NAME = "MasterInputHandler.cs";

        // Methods for dynamic path construction
        public static string GetMenuManagerPrefabPath() => MENU_STACK_SYSTEM_PREFABS_PATH + "/" + MENU_MANAGER_PREFAB_NAME;
        public static string GetMenuInitialiserPrefabPath() => MENU_STACK_SYSTEM_PREFABS_PATH + "/" + MENU_INITIALISER_PREFAB_NAME;
        public static string GetMenuInitialiserScriptPath() => MENU_STACK_SYSTEM_SCRIPTS_PATH + "/" + MENU_INITIALISER_SCRIPT_NAME;
        public static string GetGameManagerPrefabPath() => MENU_STACK_SYSTEM_PREFABS_PATH + "/" + GAME_MANAGER_PREFAB_NAME;
        public static string GetGameManagerScriptPath() => MENU_STACK_SYSTEM_SCRIPTS_PATH + "/" + GAME_MANAGER_SCRIPT_NAME;
        public static string GetMasterInputHandlerPrefabPath() => MENU_STACK_SYSTEM_PREFABS_PATH + "/" + MASTER_INPUT_HANDLER_PREFAB_NAME;
    }
}
