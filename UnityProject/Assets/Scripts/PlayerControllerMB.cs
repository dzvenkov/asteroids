using UnityEngine;

namespace Asteroids
{
    /**
     * Reads input and interprets it into setups for motion and shooting;
     */
    public class PlayerControllerMB : MonoBehaviour
    {
        private IInputState _input;
        private IEntityMotion _motion;
        private GameSettings _settings;
        
        public void Init(IInputState input, IEntityMotion motion, GameSettings settings)
        {
            _input = input;
            _motion = motion;
            _settings = settings;
        }

        void FixedUpdate()
        {
            if (_input != null) //the price for Unity's initialization habits
            {
                if (_input.Thrust)
                {
                    _motion.ApplyForce(_settings.BaseThrust*Time.fixedDeltaTime*_motion.forward);
                }

                if (_input.Rotation != 0f)
                {
                    _motion.Rotate(_settings.BaseRotation*Time.fixedDeltaTime*_input.Rotation);
                }
            }
        }
    }
    
}