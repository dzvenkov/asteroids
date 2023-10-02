using UnityEngine;

public interface IEntityMotion
{
    Vector2 forward { get; }
    void Rotate(float degrees);
    void ApplyForce(Vector2 direction);
}

[RequireComponent(typeof(Rigidbody))]
public class EntityMotionMB : MonoBehaviour, IEntityMotion
{
    private Transform _cachedTransform;
    private Rigidbody _cachedRigidbody;
    void Awake()
    {
        _cachedTransform = transform;
        _cachedRigidbody = GetComponent<Rigidbody>();
    }

    public Vector2 forward => new Vector2(_cachedTransform.forward.x, _cachedTransform.forward.z);
    
    public void Rotate(float degrees)
    {
        _cachedRigidbody.rotation *= Quaternion.Euler(0, degrees, 0);
    }

    public void ApplyForce(Vector2 direction)
    {
        _cachedRigidbody.AddForce(new Vector3(direction.x, 0, direction.y), ForceMode.VelocityChange);
    }
}
