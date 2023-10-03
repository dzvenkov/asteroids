using UnityEngine;

namespace Asteroids
{
    public interface IAsteroidEntity
    {
        int level { get; }
    }
    public class AsteroidBehaviourMB : MonoBehaviour, IAsteroidEntity
    {
        public AsteroidBehaviourMB Left;
        public AsteroidBehaviourMB Right;

        private Rect _borderRect;
        private Rigidbody _cachedRigidbody;
        private Transform _cachedTransform;
        private float _targetRotation;
        private Vector3 _targetVelocity;

        public int level { get; private set; }
        public void Init(int level, Rect borderRect)
        {
            this.level = level;
            _borderRect = borderRect;
            _cachedRigidbody = GetComponent<Rigidbody>();
            _cachedTransform = GetComponent<Transform>();
        }

        public void SetTargetMotion(float rotation, Vector3 velocity)
        {
            _targetRotation = rotation;
            _targetVelocity = velocity;
        }
        
        private void FixedUpdate()
        {
            if (_cachedRigidbody != null)
            {
                _cachedRigidbody.velocity = Vector3.Lerp(_cachedRigidbody.velocity, _targetVelocity, 0.2f);
                _cachedRigidbody.angularVelocity = Vector3.Lerp(_cachedRigidbody.angularVelocity, 
                    new Vector3(0, _targetRotation, 0), 0.01f);
            }
        }

        public void Update()
        {
            if (_cachedTransform != null)//this actually duplicates the warp functionality at player so to be extracted
            {
                Vector3 pos = _cachedTransform.position;
                if (pos.x > _borderRect.xMax) pos.x = _borderRect.xMin;
                if (pos.x < _borderRect.xMin) pos.x = _borderRect.xMax;
                if (pos.z > _borderRect.yMax) pos.z = _borderRect.yMin;
                if (pos.z < _borderRect.yMin) pos.z = _borderRect.yMax;
                _cachedTransform.position = pos;
            }
        }
    }
}