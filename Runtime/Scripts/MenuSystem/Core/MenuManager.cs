namespace StackBasedMenuSystem
{
    public class MenuManager : BaseMenuManager
    {
        public static MenuManager Instance { get; set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialise();
            }
        }
        private void OnDestroy()
        {
            Instance = null;
        }
    }
}