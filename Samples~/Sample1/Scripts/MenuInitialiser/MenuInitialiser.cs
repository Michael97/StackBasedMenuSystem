using UnityEngine;
using StackBasedMenuSystem;

[RequireComponent(typeof(MenuInputHandler))]
public class MenuInitialiser : BaseMenuInitialiser
{
    //Start Prefab Menus
    [SerializeField] private WelcomeMenu welcomeMenuPrefab;
    [SerializeField] private MainMenu mainMenuPrefab;
    [SerializeField] private OptionsMenu optionsMenuPrefab;
    [SerializeField] private PauseMenu pauseMenuPrefab;
    //End Prefab Menus

    protected override void RegisterMenus()
    {
        //Start Prefab Register
        MenuManager.Instance.RegisterMenuPrefab(welcomeMenuPrefab);
        MenuManager.Instance.RegisterMenuPrefab(mainMenuPrefab);
        MenuManager.Instance.RegisterMenuPrefab(optionsMenuPrefab);
        MenuManager.Instance.RegisterMenuPrefab(pauseMenuPrefab);
        //End Prefab Register
    }
    

    protected override void InitialMenuShow()
    {
        WelcomeMenu.Show();
    }

    protected override void Subscribe()
    {
        MenuStack_GameManager.OnGameStart += HandleGameStart;
        MenuStack_GameManager.OnMenuLoad += HandleMenuLoad;
    }

    protected override void Unsubscribe()
    {
        MenuStack_GameManager.OnGameStart -= HandleGameStart;
        MenuStack_GameManager.OnMenuLoad -= HandleMenuLoad;
    }
}
