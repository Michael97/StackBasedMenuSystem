using UnityEngine;
using StackBasedMenuSystem;

//The first menu the user sees
public class WelcomeMenu : SimpleMenu<WelcomeMenu>
{
    public override void BindButtonActions()
    {
        // Always call the base method to bind the "BackButton"
        base.BindButtonActions();

        // Bind additional buttons
        FindButtonAndAddListener("EnterButton", OnEnterButtonClicked);
        FindButtonAndAddListener("ExitButton", OnExitButtonClicked);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    public void OnEnterButtonClicked()
    {
        MainMenu.Show();
    }
}
