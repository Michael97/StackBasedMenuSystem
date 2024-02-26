namespace StackBasedMenuSystem
{
    public abstract class SimpleMenu<T> : BaseMenu<T> where T : SimpleMenu<T>
    {
        public static bool Show()
        {
            return Open();
        }

        public static bool Hide()
        {
            return Close();
        }
    }
}