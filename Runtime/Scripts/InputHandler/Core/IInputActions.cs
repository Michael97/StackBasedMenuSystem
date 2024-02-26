namespace StackBasedMenuSystem
{
    public interface IInputActions
    {
        void EnableUIActions();
        void DisableUIActions();

        PlayerInputActions PlayerInputActions();
    }
}