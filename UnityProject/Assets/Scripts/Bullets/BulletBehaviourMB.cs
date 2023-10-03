using Asteroids;
using UnityEngine;

public interface IBulletEntity { }//just a handle type
public class BulletBehaviourMB : MonoBehaviour, IBulletEntity, IAsteroidCollisionHandler
{
    public Rigidbody rigidbody; //cached for speed

    private Rect _borderRect;
    private IBulletFactory _parentFactory;
    private Transform _cachedTransform;

    public void Awake()
    {
        _cachedTransform = transform;
    }

    public void Init(Rect borderRect, IBulletFactory parentFactory)
    {
        _borderRect = borderRect;
        _parentFactory = parentFactory;
    }

    public void Update()
    {
        if (_cachedTransform != null)
        {//killing offscreen bullets
            Vector3 pos = _cachedTransform.position;
            if (pos.x > _borderRect.xMax ||
                pos.x < _borderRect.xMin ||
                pos.z > _borderRect.yMax ||
                pos.z < _borderRect.yMin)
            {
                _parentFactory.Kill(this);
            }
        }
    }

    public void HandleCollisionWithAsteroid(IAsteroidEntity asteroid)
    {
        asteroid.DealDamage();
        _parentFactory.Kill(this);//my shift is over;
    }
}
