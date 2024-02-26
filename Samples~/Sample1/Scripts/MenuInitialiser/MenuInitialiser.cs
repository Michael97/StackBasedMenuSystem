using UnityEngine;
using StackBasedMenuSystem;

[RequireComponent(typeof(MenuInputHandler))]
public class MenuInitialiser : BaseMenuInitialiser
{
    [SerializeField] private WelcomeMenu welcomeMenuPrefab;
    [SerializeField] private MainMenu mainMenuPrefab;
    [SerializeField] private OptionsMenu optionsMenuPrefab;
    [SerializeField] private PauseMenu pauseMenuPrefab;

    protected override void RegisterMenus()
    {
        MenuManager.Instance.RegisterMenuPrefab(welcomeMenuPrefab);
        MenuManager.Instance.RegisterMenuPrefab(mainMenuPrefab);
        MenuManager.Instance.RegisterMenuPrefab(optionsMenuPrefab);
        MenuManager.Instance.RegisterMenuPrefab(pauseMenuPrefab);
    }   

    protected override void InitialMenuShow()
    {
        WelcomeMenu.Show();
    }

    protected override void Subscribe()
    {
        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnMenuLoad += HandleMenuLoad;
    }

    protected override void Unsubscribe()
    {
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnMenuLoad -= HandleMenuLoad;
    }
}
