using System;
using UnityEngine;

namespace Asteroids
{
    public interface IPickupEntity
    {
        public PickupType type { get; }
    }
    public class PickupBehaviourMB : MonoBehaviour, IPickupEntity
    {
        public PickupType pickupType;
        private IPickupFactory _parentFactory;
        private Transform _cachedTransform;
        private Rect _borderRect;

        public PickupType type { get; }

        public void Init(IPickupFactory parentFactory, Rect borderRect)
        {
            _parentFactory = parentFactory;
            _cachedTransform = transform;
            _borderRect = borderRect;
        }

        private void Update()
        {
            if (_cachedTransform != null)
            {
                Vector3 pos = _cachedTransform.position;
                if (pos.x > _borderRect.xMax) pos.x = _borderRect.xMin;
                if (pos.x < _borderRect.xMin) pos.x = _borderRect.xMax;
                if (pos.z > _borderRect.yMax) pos.z = _borderRect.yMin;
                if (pos.z < _borderRect.yMin) pos.z = _borderRect.yMax;
                _cachedTransform.position = pos;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!enabled) return;
            var handler = collision.gameObject.GetComponent<IPickupCollisionHandler>();
            if (handler != null && handler.HandleCollisionWithPickup(this))
            {
                _parentFactory.KillPickup(this);
            }
        }
    }
}