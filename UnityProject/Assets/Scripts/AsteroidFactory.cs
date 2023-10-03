using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public interface IAsteroidFactory
    {
        void BuildAsteroid(int level, Vector2 position);
    }

    public class AsteroidFactory : IAsteroidFactory
    {
        private readonly Settings _settings;
        private readonly Rect _borderRect;

        [Serializable]
        public class Settings
        {
            //randomized sizes and rotation of parts
            public Vector2 xRange = new Vector2(0.75f, 1.5f);
            public Vector2 yRange = new Vector2(0.15f, 0.25f);
            public Vector2 xRotRange = new Vector2(-5f, 5f);
            public Vector2 yRotRange = new Vector3(-25f, 25f);
            public float Compactness = 0.8f;
            public Vector2 initialSpeedRange = new Vector2(1f, 2f);
            public Vector2 initialAngularVelocity = new Vector2(1f, 2f);
        }

        public AsteroidFactory(Settings settings, Rect borderRect)
        {
            _settings = settings;
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
            asteroid.Run(angularVelocity, velocity);
        }
        
        AsteroidStructureMB BuildAsteroidStructure(int level)
        {
            AsteroidStructureMB result = null;
            if (level == 0)
            {
                result = GameObject.CreatePrimitive(PrimitiveType.Cube)
                    .AddComponent<AsteroidStructureMB>();
                result.transform.localScale = new Vector3(
                    Random.Range(_settings.xRange.x, _settings.xRange.y),
                    Random.Range(_settings.yRange.y, _settings.yRange.y),
                    Random.Range(_settings.xRange.x, _settings.xRange.y));
            }
            else
            {
                result = new GameObject()
                    .AddComponent<AsteroidStructureMB>();
                result.Left = BuildAsteroidStructure(level - 1);
                result.Left.transform.SetParent(result.transform);
                result.Right = BuildAsteroidStructure(level - 1);
                result.Right.transform.SetParent(result.transform);
                //position the parts
                float positioningOffset = _settings.Compactness*Mathf.Pow(2, (level + 1) / 2);

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
                result.Left.transform.localRotation = Quaternion.Euler(
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y), 
                    Random.Range(_settings.yRotRange.x, _settings.yRotRange.y), 
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y));
                result.Right.transform.localRotation = Quaternion.Euler(
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y), 
                    Random.Range(_settings.yRotRange.x, _settings.yRotRange.y), 
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y));

            }

            result.Init(level, _borderRect);
            result.name = $"A_{level}_{Mathf.Abs(result.GetHashCode())}";

            return result;
        }
        
    }
}