namespace Asteroids
{
    public interface IPickupCollisionHandler
    {
        bool HandleCollisionWithPickup(IPickupEntity pickupEntity);
    }
}