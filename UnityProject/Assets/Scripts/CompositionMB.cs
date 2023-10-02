using UnityEngine;

namespace Asteroids
{

    public class CompositionMB : MonoBehaviour
    {
        public GameSettingsHolderSO GameSettings;
        public PlayerControllerMB PlayerController;

        void Awake()
        {
            IInputState inputState = new InputState();
            PlayerController.Init(inputState, 
                PlayerController.GetComponent<IEntityMotion>(),
                this.GameSettings.Settings);
        }
    }
}