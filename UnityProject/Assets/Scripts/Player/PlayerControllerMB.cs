using UnityEngine;

namespace Asteroids
{
    /**
     * Reads input and interprets it into setups for motion and shooting;
     * having this external from player behaviour allows for other sources of authority (e.g. cutscene)
     */
    public class PlayerControllerMB : MonoBehaviour, IAsteroidCollisionHandler
    {
        private IInputState _input;
        private IPlayerEntity _motion;
        private GameSettings _settings;
        private IBulletFactory _bulletFactory;
        private MatchState _matchState;

        private float _lastShotTime = 0; 
        
        public void Init(IInputState input, IPlayerEntity motion, IBulletFactory bulletFactory, 
            MatchState matchState, GameSettings settings)
        {
            _input = input;
            _motion = motion;
            _settings = settings;
            _bulletFactory = bulletFactory;
            _matchState = matchState;
        }

        void FixedUpdate()
        {
            if (_matchState != null && _matchState.State == IMatchState.Status.InProgress) //the price for Unity's initialization habits
            {
                if (_input.Thrust)
                {
                    float maxSpeedT = 1f - _motion.speed / _settings.MaxSpeed;//thrust drops to zero as we get closer to max speed
                    _motion.ApplyForce(maxSpeedT*_settings.BaseThrust*Time.fixedDeltaTime*_motion.forward);
                }

                if (_input.Rotation != 0f)
                {
                    _motion.Rotate(_settings.BaseRotationRate*Time.fixedDeltaTime*_input.Rotation);
                }

                if (_input.Fire && Time.time > _lastShotTime + _settings.BulletSettings.minShotInterval)
                {
                    _bulletFactory.Shoot();
                    _lastShotTime = Time.time;
                }
            }
        }
        
        public void HandleCollisionWithAsteroid(IAsteroidEntity asteroid)
        {
            _matchState.RegisterPlayerDeath();
            _motion.PlayDeathSequence(_matchState.State == IMatchState.Status.Lose);
        }
    }
    
}