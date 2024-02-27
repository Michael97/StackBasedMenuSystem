using TMPro;
using UnityEngine;
using StackBasedMenuSystem;

public class DebugCanvas : BaseDebugCanvas
{
    public TextMeshProUGUI pausedInfo;
    public TextMeshProUGUI gameStatusInfo;

    private DebugCanvas Instance;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        var menuStack = MenuManager.Instance?.GetMenuStack();

        if (menuStack == null)
            return;

        stackInfo.text = "Menu Stack Count: " + menuStack.Count;
        stackInfo.text += "\n";

        foreach (var menu in menuStack)
        {
            stackInfo.text += menu.name + " - " + IsMenuActive(menu) + " - " + GetMenuCloseType(menu) + "\n";
        }
        pausedInfo.text = MenuStack_GameManager.Instance.IsGamePaused ? "Game Paused" : "Game Running";
        gameStatusInfo.text = MenuStack_GameManager.Instance.InGame ? "In Game" : "In Main Menu";
    }
}
