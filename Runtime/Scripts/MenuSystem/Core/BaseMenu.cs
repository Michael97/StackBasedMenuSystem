using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

namespace StackBasedMenuSystem
{
    public abstract class BaseMenu<T> : BaseMenu where T : BaseMenu<T>
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            // Ensure this is the only instance in the scene, otherwise, destroy the new one.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = (T)this;
                SceneManager.sceneLoaded += OnSceneLoaded;
                BindButtonActions();
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public virtual void OnEnable()
        {
            GetComponent<Canvas>().sortingLayerName = "Foreground";

            if (GetComponent<Canvas>().worldCamera == null)
                GetComponent<Canvas>().worldCamera = Camera.main;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Instance == null || Instance.gameObject.scene != gameObject.scene)
            {
                Instance = (T)this; // Reassign instance if in a new scene and no current instance
            }
        }

        protected static bool Open()
        {
            if (Instance == null)
            {
                if (MenuManager.Instance == null)
                {
                    Debug.LogError("No MenuManager found in scene. " + Instance.name);
                    return false;
                }
                MenuManager.Instance.CreateInstance<T>();
            }
            else if (!Instance.gameObject.activeSelf)
            {
                Instance.gameObject.SetActive(true);
            }
            else
            {
                return false; // Menu is already active
            }

            MenuManager.Instance.OpenMenu(Instance);
            return true; // Successfully opened or made active
        }

        protected static bool ForceClose()
        {
            if (Instance != null)
            {
                return MenuManager.Instance.CloseTopMenu();  // Successfully closed
            }
            else
            {
                Debug.LogWarningFormat("Attempted to close menu {0} but it was already null.", typeof(T).Name);
                return false; // Failed to close because it was null
            }
        }

        protected static bool Close()
        {
            if (Instance != null)
            {
                return MenuManager.Instance.CloseMenu(Instance);  // Successfully closed
            }
            else
            {
                Debug.LogWarningFormat("Attempted to close menu {0} but it was already null.", typeof(T).Name);
                return false; // Failed to close because it was null
            }
        }

        public override void OnBackPressed()
        {
            var nonClosableTypes = new[] { MenuType.Main, MenuType.GameOver, MenuType.Welcome };

            if (nonClosableTypes.Contains(Instance.GetMenuType()))
            {
                Debug.LogWarningFormat(Instance, "Attempted to close the {0}. (VIA BACK BUTTON) This is not allowed when it is on top of the stack", Instance.GetType());
                return;
            }

            Close();
        }

        public override void BindButtonActions()
        {
            // Always bind the "BackButton" in the base class
            FindButtonAndAddListener("BackButton", OnBackPressed);
        }

    }


    public abstract class BaseMenu : MonoBehaviour
    {
        public enum CloseType
        {
            Destroy, //The GameObject is destroyed when closed.
            Hide,    //The GameObject is hidden when closed.
            Disabled //The GameObject is not usable when closed, but can still be seen.
        }

        [SerializeField] private CloseType closeType = CloseType.Destroy;

        public CloseType GetCloseType()
        {
            return closeType;
        }

        public void SetCloseType(CloseType type)
        {
            closeType = type;
        }

        public enum MenuType
        {
            Generic,
            Main,
            Options,
            Pause,
            Inventory,
            Dialogue,
            Interaction,
            GameOver,
            Welcome
        }

        [SerializeField] private MenuType menuType = MenuType.Generic;

        public MenuType GetMenuType()
        {
            return menuType;
        }

        public void SetMenuType(MenuType type)
        {
            menuType = type;
        }

        public abstract void OnBackPressed();

        public abstract void BindButtonActions();

        protected void FindButtonAndAddListener(string buttonName, UnityEngine.Events.UnityAction action)
        {
            Button button = FindButtonInChild(transform, buttonName);

            if (button != null)
            {
                button.onClick.AddListener(action);
            }
            else
            {
                Debug.LogWarningFormat(this, "{0} has no {1}. Was this intended?", GetType(), buttonName);
            }
        }

        private Button FindButtonInChild(Transform parent, string buttonName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == buttonName)
                {
                    Button button = child.GetComponent<Button>();
                    if (button != null)
                    {
                        return button;
                    }
                }

                Button buttonInChildren = FindButtonInChild(child, buttonName);
                if (buttonInChildren != null)
                {
                    return buttonInChildren;
                }
            }

            return null;
        }

    }
}