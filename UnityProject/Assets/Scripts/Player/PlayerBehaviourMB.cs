using System.Collections;
using UnityEngine;

public interface IPlayerEntity
{
    Vector2 forward { get; }
    float speed { get; }
    void Rotate(float degrees);
    void ApplyForce(Vector2 direction);
    void PlayDeathSequence(bool final);
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviourMB : MonoBehaviour, IPlayerEntity
{
    public GameObject ModelRoot;
    public GameObject Corpse;
    
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

    public void PlayDeathSequence(bool final)
    {
        StartCoroutine(DeathSequence(final));
    }

    IEnumerator DeathSequence(bool final)
    {
        ModelRoot.SetActive(false); 
        var corpse = Instantiate(Corpse, ModelRoot.transform.position, ModelRoot.transform.rotation);
        foreach (var corpseRB in corpse.GetComponentsInChildren<Rigidbody>())
        {
            corpseRB.AddForce(Random.insideUnitSphere, ForceMode.VelocityChange);
        }
        yield return new WaitForSeconds(3);
        transform.SetPositionAndRotation(new Vector3(_borderRect.center.x, 0, _borderRect.center.y), 
            Quaternion.identity);
        if (!final)
        {
            Destroy(corpse);//leave last corpse hanging around for dramatic effect
            ModelRoot.SetActive(true);
        }
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
}
