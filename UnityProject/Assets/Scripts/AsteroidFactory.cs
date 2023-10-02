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

        [Serializable]
        public class Settings
        {
            //randomized sizes and rotation of parts
            public Vector2 xRange = new Vector2(0.75f, 1.5f);
            public Vector2 yRange = new Vector2(0.15f, 0.25f);
            public Vector2 xRotRange = new Vector2(-5f, 5f);
            public Vector3 yRotRange = new Vector3(-25f, 25f);
            public float Compactness = 0.8f;
        }

        public AsteroidFactory(Settings settings)
        {
            _settings = settings;
        }

        public void BuildAsteroid(int level, Vector2 position)
        {
            var asteroid = BuildAsteroidImpl(level);
            asteroid.transform.position = new Vector3(position.x, 0, position.y);
        }
        
        AsteroidStructureMB BuildAsteroidImpl(int level)
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
                result.left = BuildAsteroidImpl(level - 1);
                result.left.transform.SetParent(result.transform);
                result.right = BuildAsteroidImpl(level - 1);
                result.right.transform.SetParent(result.transform);
                //position the parts
                float positioningOffset = _settings.Compactness*Mathf.Pow(2, (level + 1) / 2);

                if (level % 2 == 1)
                {
                    result.left.transform.localPosition = new Vector3(-positioningOffset, 0, 0);
                    result.right.transform.localPosition = new Vector3(positioningOffset, 0, 0);
                }
                else
                {
                    result.left.transform.localPosition = new Vector3(0, 0, -positioningOffset);
                    result.right.transform.localPosition = new Vector3(0, 0, positioningOffset);
                }
                result.left.transform.localRotation = Quaternion.Euler(
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y), 
                    Random.Range(_settings.yRotRange.x, _settings.yRotRange.y), 
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y));
                result.right.transform.localRotation = Quaternion.Euler(
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y), 
                    Random.Range(_settings.yRotRange.x, _settings.yRotRange.y), 
                    Random.Range(_settings.xRotRange.x, _settings.xRotRange.y));

            }
            result.name = $"A_{level}_{Mathf.Abs(result.GetHashCode())}";

            return result;
        }
        
    }
}