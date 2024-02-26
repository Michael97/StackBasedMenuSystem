using UnityEngine;
namespace StackBasedMenuSystem
{
    public abstract class BaseMenuInputHandler : MonoBehaviour
    {
        protected IInputActions inputHandler;

        protected virtual void Start()
        {
            inputHandler = MasterInputHandler.Instance;
            inputHandler.EnableUIActions();
            SubscribeToInputActions();
        }

        protected virtual void OnDestroy()
        {
            if (inputHandler != null)
            {
                UnsubscribeFromInputActions();
                inputHandler.DisableUIActions();
            }
        }

        protected abstract void SubscribeToInputActions();
        protected abstract void UnsubscribeFromInputActions();
    }
}