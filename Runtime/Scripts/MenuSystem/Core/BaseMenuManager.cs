using System.Collections.Generic;
using UnityEngine;
using static StackBasedMenuSystem.BaseMenu;

namespace StackBasedMenuSystem
{
    public abstract class BaseMenuManager : MonoBehaviour
    {
        protected Stack<BaseMenu> menuStack = new Stack<BaseMenu>();

        // Dictionary to store menu prefabs
        protected Dictionary<System.Type, BaseMenu> menuPrefabs = new Dictionary<System.Type, BaseMenu>();

        [SerializeField] public BaseMenuInitialiser Initialiser;

        protected virtual void Initialise()
        {
            if (Initialiser == null)
            {
                Debug.LogError("Menu Initialiser is not set in the inspector");
                return;
            }
            Instantiate(Initialiser);
        }


        // Register menu prefabs method
        public void RegisterMenuPrefab<T>(T prefab) where T : BaseMenu
        {
            menuPrefabs[typeof(T)] = prefab;
            Debug.Log($"Registering menu prefab of type {typeof(T).Name}");

        }


        public void CreateInstance<T>() where T : BaseMenu
        {
            var prefab = GetPrefab<T>();

            Instantiate(prefab, transform);
        }


        // Get prefab from dictionary
        private T GetPrefab<T>() where T : BaseMenu
        {
            if (menuPrefabs.TryGetValue(typeof(T), out BaseMenu prefab))
            {
                return (T)prefab;
            }

            throw new MissingReferenceException("Prefab not found for type " + typeof(T));
        }

        public void OpenMenu(BaseMenu instance)
        {
            // De-activate top menu
            if (menuStack.Count > 0)
            {
                if (menuStack.Peek().GetCloseType() == BaseMenu.CloseType.Destroy)
                {
                    Destroy(menuStack.Pop().gameObject);
                    menuStack.Push(instance);
                    return;
                }
                else if (menuStack.Peek().GetCloseType() == BaseMenu.CloseType.Hide)
                {
                    menuStack.Peek().gameObject.SetActive(false);
                }

                var topCanvas = instance.GetComponent<Canvas>();
                var previousCanvas = menuStack.Peek().GetComponent<Canvas>();
                topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
            }
            menuStack.Push(instance);
        }

        public bool CloseMenu(BaseMenu menu)
        {
            if (menuStack.Count == 0)
            {
                Debug.LogWarningFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
                return false;
            }

            if (menuStack.Peek() != menu)
            {
                Debug.LogWarningFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
                return false;
            }


            return CloseTopMenu();
        }

        public bool CloseTopMenu()
        {
            var instance = menuStack.Pop();

            if (instance.GetCloseType() == BaseMenu.CloseType.Destroy)
            {
                Destroy(instance.gameObject);
                Debug.Log($"{instance.name} Destroyed (closed)");
            }
            else
            {
                instance.gameObject.SetActive(false);
                Debug.Log($"{instance.name} Deactivated (closed)");
            }

            // Re-activate the next menu on the stack if any
            if (menuStack.Count > 0)
            {
                var nextMenu = menuStack.Peek();
                nextMenu.gameObject.SetActive(true);
            }

            return true;
        }

        public bool GoBackONMenuStack()
        {
            if (menuStack.Count > 0)
            {
                menuStack.Peek().OnBackPressed();
                return true;
            }
            return false;
        }

        public Stack<BaseMenu> GetMenuStack()
        {
            return menuStack;
        }
    }
}