using System;
using Asteroids;

[Serializable]
public class GameSettings
{
    public float BaseRotationRate = 270f;
    public float BaseThrust = 100f;
    public float MaxSpeed = 30f;
    public AsteroidFactory.Settings AsteroidsFactorySettings;
}