using System;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace StackBasedMenuSystem
{
    public class MenuStack_GameManager : MonoBehaviour
    {
        public static MenuStack_GameManager Instance { get; private set; }

        public GameObject MasterInputHandler;

        public bool IsGamePaused { get; private set; }

        public bool InGame = false;

        public enum GameType
        {
            SingleScene,
            MultiScene
        }

        [SerializeField] private GameType gameType = GameType.SingleScene;

        public GameType GetGameType()
        {
            return gameType;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Instantiate(MasterInputHandler);
                LoadMenu();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static event Action OnGameStart;
        public static event Action OnMenuLoad;

        public void StartGame()
        {
            OnGameStart?.Invoke();
            IsGamePaused = false;
            InGame = true;

            if (gameType == GameType.SingleScene)
                return;

            SceneManager.LoadScene("GameScene");
        }

        public void LoadMenu()
        {
            OnMenuLoad?.Invoke();
            InGame = false;

            if (gameType == GameType.SingleScene)
                return;

            SceneManager.LoadScene("MainMenuScene");
        }

        public void TogglePause()
        {
            // Toggle the pause state based on the current state and the success of the operation.
            if (!IsGamePaused)
            {
                // If successfully showed the pause menu, pause the game.
                if (PauseMenu.Show())
                {
                    //Time.timeScale = 0f;
                    IsGamePaused = true;
                }
            }
            else
            {
                // If successfully hid the pause menu, unpause the game.
                if (PauseMenu.Hide())
                {
                    //Time.timeScale = 1f;
                    IsGamePaused = false;
                }
            }
        }
    }
}