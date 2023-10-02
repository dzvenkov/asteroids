using UnityEngine.InputSystem;

namespace Asteroids
{
    public interface IInputState
    {
        float Rotation { get; }
        bool Thrust { get; }
        bool Fire { get; }
    }

    public class InputState : IInputState
    {
        public float Rotation
        {
            get
            {
                float result = 0;
                if (Keyboard.current.aKey.isPressed) result += -1;
                if (Keyboard.current.dKey.isPressed) result += 1;
                return result;
            }
        }
        
        public bool Thrust => Keyboard.current.wKey.isPressed;

        public bool Fire => Keyboard.current.spaceKey.isPressed;
    }
}
