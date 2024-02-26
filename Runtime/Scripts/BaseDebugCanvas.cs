using TMPro;
using UnityEngine;


namespace StackBasedMenuSystem
{
    public class BaseDebugCanvas : MonoBehaviour
    {
        public TextMeshProUGUI stackInfo;

        private void Awake()
        {
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
        }

        protected string IsMenuActive(BaseMenu menu)
        {
            return menu.gameObject.activeSelf ? "Active" : "Inactive";
        }

        protected string GetMenuCloseType(BaseMenu menu)
        {
            return menu.GetCloseType().ToString();
        }
    }

}