using System;
using UnityEngine;

namespace Asteroids
{
    [Serializable]
    public class GameSettings
    {
        public float BaseRotationRate = 270f;
        public float BaseThrust = 100f;
        public float MaxSpeed = 30f;
        public int StartHearts = 3;
        public AsteroidSettings AsteroidsSettings;
        public BulletSettings BulletSettings;
    }

    [Serializable]
    public class AsteroidSettings
    {
        //randomized sizes and rotation of parts
        public Vector2 xRange = new Vector2(0.5f, 2f);
        public Vector2 yRange = new Vector2(0.15f, 0.5f);
        public Vector2 xRotRange = new Vector2(-5f, 5f);
        public Vector2 yRotRange = new Vector2(-25f, 25f);
        //affects packing of subparts
        public float compactness = 0.25f;
        //initial movement randomized ranges
        public Vector2 initialSpeedRange = new Vector2(1f, 2f);
        public Vector2 initialAngularVelocity = new Vector2(0.7f, 1.5f);
        //speedup at start or split factor (helps with subparts not getting stuck with each other)
        public float initialSpeedBoostMultiplier = 2f;
    }

    [Serializable]
    public class BulletSettings
    {
        public int poolSize = 100;
        public float shotSpeed = 5;
        public float minShotInterval = 1f;
    }
}