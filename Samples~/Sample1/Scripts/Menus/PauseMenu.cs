using StackBasedMenuSystem;
public class PauseMenu : SimpleMenu<PauseMenu>
{
    public override void BindButtonActions()
    {
        base.BindButtonActions();

        FindButtonAndAddListener("OptionsButton", OnClickOptions);
        FindButtonAndAddListener("ExitButton", OnClickExit);
    }

    public void OnClickOptions()
    {
        OptionsMenu.Show();
    }

    public void OnClickExit()
    {
        if (Close())
            MenuStack_GameManager.Instance.LoadMenu();
    }

    public override void OnBackPressed()
    {
        MenuStack_GameManager.Instance.TogglePause();
    }
}