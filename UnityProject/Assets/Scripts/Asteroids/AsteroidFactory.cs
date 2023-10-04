using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public interface IAsteroidFactory
    {
        IReadOnlyList<IAsteroidEntity> Asteroids { get; } //all currently running asteroids
        void BuildAsteroid(int level, Vector2 position);
        void KillAsteroid(IAsteroidEntity asteroid);
        public void SplitAsteroid(IAsteroidEntity asteroidEntity);
    }

    public class AsteroidFactory : IAsteroidFactory
    {
        private readonly AsteroidSettings _settings;
        private readonly IPickupFactory _pickupFactory;
        private readonly MatchState _matchState;
        private readonly Rect _borderRect;
        private readonly List<IAsteroidEntity> _asteroids = new List<IAsteroidEntity>();
        public IReadOnlyList<IAsteroidEntity> Asteroids => _asteroids;

        public AsteroidFactory(AsteroidSettings settings, IPickupFactory pickupFactory, MatchState matchState, Rect borderRect)
        {
            _settings = settings;
            _pickupFactory = pickupFactory;
            _matchState = matchState;
            _borderRect = borderRect;
        }

        public void BuildAsteroid(int level, Vector2 position)
        {
            //build structure (combination of pairs of cubes)
            var asteroid = BuildAsteroidStructure(level);
            asteroid.transform.position = new Vector3(position.x, 0, position.y);
            //set initial motion
            float angularVelocity = Random.Range(_settings.initialAngularVelocity.x, _settings.initialAngularVelocity.y);
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            randomDir.Normalize();
            Vector3 velocity = Random.Range(_settings.initialSpeedRange.x, _settings.initialSpeedRange.y) * randomDir;
            //run
            Run(asteroid, angularVelocity, velocity);
        }

        public void KillAsteroid(IAsteroidEntity asteroid)
        {
            Debug.Assert(_asteroids.Contains(asteroid));
            _asteroids.Remove(asteroid);
            _matchState.RegisterAsteroidKill(asteroid.level);
            if (_asteroids.Count == 0)
            {
                _matchState.RegisterVictory();
            }
            if (Random.Range(0f, 1f) < _settings.probabilityToSpawnPickupOnKill)
            {
                _pickupFactory.CreatePickup((PickupType)Random.Range(0, 2), asteroid.position);
            }
            //I created you so I can kill you
            GameObject.Destroy((asteroid as AsteroidBehaviourMB).gameObject);
        }

        private AsteroidBehaviourMB BuildAsteroidStructure(int level)
        {
            AsteroidBehaviourMB result = null;
            if (level == 0)
            {
                result = GameObject.CreatePrimitive(PrimitiveType.Cube)
                    .AddComponent<AsteroidBehaviourMB>();
                result.transform.localScale = new Vector3(
                    Random.Range(_settings.xRange.x, _settings.xRange.y),
                    Random.Range(_settings.yRange.y, _settings.yRange.y),
                    Random.Range(_settings.xRange.x, _settings.xRange.y));
            }
            else
            {
                result = new GameObject()
                    .AddComponent<AsteroidBehaviourMB>();
                //recursively build two subparts
                result.Left = BuildAsteroidStructure(level - 1);
                result.Left.transform.SetParent(result.transform);
                result.Left.enabled = false;
                result.Right = BuildAsteroidStructure(level - 1);
                result.Right.transform.SetParent(result.transform);
                result.Right.enabled = false;
                //position the parts
                float positioningOffset = _settings.compactness*Mathf.Pow(2, (level + 1) / 2);

                if (level % 2 == 1)
                {
                    result.Left.transform.localPosition = new Vector3(-positioningOffset, 0, 0);
                    result.Right.transform.localPosition = new Vector3(positioningOffset, 0, 0);
                }
                else
                {
                    result.Left.transform.localPosition = new Vector3(0, 0, -positioningOffset);
                    result.Right.transform.localPosition = new Vector3(0, 0, positioningOffset);
                }
                //rotate them randomly to look a bit more interesting
                result.Left.transform.localRotation = Quaternion.Euler(
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y), 
                    Random.Range(_settings.yRotRange.x, _settings.yRotRange.y), 
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y));
                result.Right.transform.localRotation = Quaternion.Euler(
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y), 
                    Random.Range(_settings.yRotRange.x, _settings.yRotRange.y), 
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y));
            }

            result.Init(level, _borderRect, this);
            result.name = $"A_{level}_{Mathf.Abs(result.GetHashCode())}";
            result.enabled = false;
            return result;
        }
        
        public void SplitAsteroid(IAsteroidEntity asteroidEntity)
        {
            AsteroidBehaviourMB asteroid = asteroidEntity as AsteroidBehaviourMB;//we can do that cast as we're actually creating them
            Rigidbody rigidbody = asteroid.gameObject.GetComponent<Rigidbody>();

            Vector3 orthogonalDir = (asteroid.level %2 == 1)?asteroid.transform.forward:asteroid.transform.right;
            
            if (asteroid.Left != null)
            {
                Run(asteroid.Left, rigidbody.angularVelocity.y,
                    0.3f * rigidbody.velocity + 0.8f * orthogonalDir *rigidbody.velocity.magnitude);
            }

            if (asteroid.Right != null)
            {
                Run(asteroid.Right, -rigidbody.angularVelocity.y, 
                    0.3f*rigidbody.velocity - 0.8f*orthogonalDir *rigidbody.velocity.magnitude);
            }
            KillAsteroid(asteroid);
        }
        
        /* 
         * Asteroid behaviours can be 'sleeping'/disabled on subparts, here's the actual activation
         */
        private void Run(AsteroidBehaviourMB asteroid, float rotation, Vector3 velocity)
        {
            asteroid.transform.SetParent(null);
            asteroid.enabled = true;
            //setup rigidbody;
            Rigidbody rigidbody = asteroid.gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.constraints |= RigidbodyConstraints.FreezePositionY 
                                            | RigidbodyConstraints.FreezeRotationZ |
                                            RigidbodyConstraints.FreezeRotationX;
            rigidbody.drag = 0;
            rigidbody.angularDrag = 0;
            rigidbody.mass = Mathf.Pow(2, asteroid.level);

            asteroid.SetTargetMotion(rotation, velocity);
            
            rigidbody.velocity = _settings.initialSpeedBoostMultiplier * velocity;
            rigidbody.angularVelocity = Vector3.zero;
            
            _asteroids.Add(asteroid);
        }
    }
}