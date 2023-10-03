using System.Collections.Generic;
using Asteroids;
using UnityEngine;

public interface IBulletFactory
{
    void Shoot();
    void Kill(IBulletEntity bulletEntity);
}

public class BulletFactory : IBulletFactory
{
    private readonly BulletBehaviourMB _bulletPrototype;
    private readonly Transform _muzzleTransform;
    private readonly BulletSettings _settings;
    private readonly Rect _borderRect;
    private readonly Stack<BulletBehaviourMB> _pool = new();

    public BulletFactory(BulletBehaviourMB bulletPrototype, Transform muzzle, BulletSettings settings, Rect borderRect)
    {
        _bulletPrototype = bulletPrototype;
        _muzzleTransform = muzzle;
        _settings = settings;
        _borderRect = borderRect;
        //populate pool;
        for (int i = 0; i < settings.poolSize; i++)
        {
            var bullet = Create();
            bullet.gameObject.SetActive(false);
            _pool.Push(bullet);
        }
    }
    
    public void Shoot()
    {
        BulletBehaviourMB result;
        if (_pool.Count > 0)
        {
            result = _pool.Pop();
            result.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("bullets pool exhausted");
            result = Create();
        }
        result.Rigidbody.position = _muzzleTransform.position;
        result.Rigidbody.velocity = _settings.shotSpeed * _muzzleTransform.forward;
    }

    private BulletBehaviourMB Create()
    {
        BulletBehaviourMB result = GameObject.Instantiate(_bulletPrototype);
        result.Init(_borderRect, this);
        return result;
    }

    public void Kill(IBulletEntity bulletEntity)
    {
        BulletBehaviourMB bullet = (BulletBehaviourMB)bulletEntity;
        bullet.gameObject.SetActive(false);
        _pool.Push(bullet);
    }
}
