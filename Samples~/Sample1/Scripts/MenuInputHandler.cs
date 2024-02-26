
namespace StackBasedMenuSystem
{
    public class MenuInputHandler : BaseMenuInputHandler
    {
        protected override void SubscribeToInputActions()
        {
            inputHandler.PlayerInputActions().UI.Cancel.performed += _ => EscapePressed();
        }

        protected override void UnsubscribeFromInputActions()
        {
            inputHandler.PlayerInputActions().UI.Cancel.performed -= _ => EscapePressed();

        }

        private void EscapePressed()
        {
            //If we are in game we probably wanna pause the game
            if (MenuStack_GameManager.Instance.InGame)
            {
                var menuStack = MenuManager.Instance.GetMenuStack();
                // Toggle pause if no menus are open or the top menu is the pause menu
                if (menuStack.Count == 0 || menuStack.Peek().GetMenuType() == BaseMenu.MenuType.Pause)
                {
                    MenuStack_GameManager.Instance.TogglePause();
                }
                //If we get here, it means we are in game and we have a menu open on top of the pause menu, so we close it
                else
                {
                    // Close the topmost non-pause menu
                    MenuManager.Instance.CloseMenu(menuStack.Peek());
                }
            }
            //If we are in the main menu, just go back on the stack
            else
                MenuManager.Instance.GoBackONMenuStack();
        }
    }
}