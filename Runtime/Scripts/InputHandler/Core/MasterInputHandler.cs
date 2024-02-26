using System;
using UnityEngine;

namespace StackBasedMenuSystem
{
    public class MasterInputHandler : MonoBehaviour, IInputActions
    {
        public static MasterInputHandler Instance { get; private set; }
        private PlayerInputActions playerInputActions;

        private void Awake()
        {
            // Singleton setup
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                playerInputActions = new PlayerInputActions();
            }
        }

        public PlayerInputActions PlayerInputActions()
        {
            return playerInputActions;
        }

        public void EnableUIActions()
        {
            playerInputActions.UI.Enable();
        }

        public void DisableUIActions()
        {
            playerInputActions.UI.Disable();
        }

        private void OnEnable() => playerInputActions.Enable();
        private void OnDisable() => playerInputActions.Disable();
    }
}