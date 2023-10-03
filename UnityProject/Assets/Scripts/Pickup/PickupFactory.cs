using System.Collections.Generic;
using Asteroids;
using UnityEngine;

public enum PickupType
{
    Shield,
    Heart
}
public interface IPickupFactory
{
    void CreatePickup(PickupType pickupType, Vector2 position);
    void KillPickup(IPickupEntity pickup);
}

public class PickupFactory : IPickupFactory
{
    private readonly Dictionary<PickupType, PickupBehaviourMB> _prototypes;
    private Rect _borderRect;

    public PickupFactory(Dictionary<PickupType, PickupBehaviourMB> prototypes, Rect borderRect)
    {
        _prototypes = prototypes;
        _borderRect = borderRect;
    }
    public void CreatePickup(PickupType pickupType, Vector2 position)
    {
        var instance = Object.Instantiate(_prototypes[pickupType], new Vector3(position.x, 0, position.y), Quaternion.identity);
        instance.Init(this, _borderRect);
        Debug.Assert(instance.pickupType == pickupType);
    }

    public void KillPickup(IPickupEntity pickup)
    {
        Object.Destroy((pickup as PickupBehaviourMB).gameObject);
    }

}
