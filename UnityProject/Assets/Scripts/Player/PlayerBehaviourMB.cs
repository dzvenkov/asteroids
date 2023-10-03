using UnityEngine;

public interface IPlayerBehaviour
{
    Vector2 forward { get; }
    float speed { get; }
    void Rotate(float degrees);
    void ApplyForce(Vector2 direction);
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviourMB : MonoBehaviour, IPlayerBehaviour
{
    private Transform _cachedTransform;
    private Rigidbody _cachedRigidbody;
    private Rect _borderRect;
    public void Init(Rect borderRect)
    {
        _borderRect = borderRect;
        _cachedTransform = transform;
        _cachedRigidbody = GetComponent<Rigidbody>();
    }

    public Vector2 forward => new Vector2(_cachedTransform.forward.x, _cachedTransform.forward.z);
    public float speed => _cachedRigidbody.velocity.magnitude;
    
    public void Rotate(float degrees)
    {
        if (Mathf.Abs(degrees) > 0.0001f) _cachedRigidbody.angularVelocity = Vector3.zero;
        _cachedRigidbody.rotation *= Quaternion.Euler(0, degrees, 0);
    }

    public void ApplyForce(Vector2 direction)
    {
        _cachedRigidbody.AddForce(new Vector3(direction.x, 0, direction.y), ForceMode.VelocityChange);
    }

    public void Update()
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
}
