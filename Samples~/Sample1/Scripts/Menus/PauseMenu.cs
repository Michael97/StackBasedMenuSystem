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
            GameManager.Instance.LoadMenu();
    }

    public override void OnBackPressed()
    {
        GameManager.Instance.TogglePause();
    }
}