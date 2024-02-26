using System.Collections;
using UnityEngine;


namespace StackBasedMenuSystem
{
    public abstract class BaseMenuInitialiser : MonoBehaviour
    {
        protected BaseMenuInputHandler inputHandler;

        private static BaseMenuInitialiser Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                inputHandler = GetComponent<BaseMenuInputHandler>();

                Subscribe();

                RegisterMenus();
                StartCoroutine(Initialise());
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Subscribe()
        {
            //GameManager.OnGameStart += HandleGameStart;
            //GameManager.OnMenuLoad += HandleMenuLoad;
        }

        protected virtual void Unsubscribe()
        {
            //GameManager.OnGameStart -= HandleGameStart;
            //GameManager.OnMenuLoad -= HandleMenuLoad;
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        protected virtual void HandleGameStart()
        {

        }

        protected virtual void HandleMenuLoad()
        {
            StartCoroutine(Initialise());
        }

        protected abstract void RegisterMenus();

        private IEnumerator Initialise()
        {
            yield return null;
            InitialMenuShow();
        }

        protected abstract void InitialMenuShow();
    }
}