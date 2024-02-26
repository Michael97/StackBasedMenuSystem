using StackBasedMenuSystem;

public class MainMenu : SimpleMenu<MainMenu>
{
    public override void BindButtonActions()
    {
        base.BindButtonActions();

        FindButtonAndAddListener("OptionsButton", OnClickOptions);
        FindButtonAndAddListener("PlayButton", OnClickPlay);
    }

    public void OnClickOptions()
    {
        OptionsMenu.Show();
    }

    public void OnClickPlay()
    {
        if (Close())
            GameManager.Instance.StartGame();
    }

}
