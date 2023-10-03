using UnityEngine;

namespace Asteroids
{
    public interface IAsteroidStructure
    {
        int level { get; }
        void Run(float rotation, Vector3 velocity);
    }
    
    public class AsteroidStructureMB : MonoBehaviour
    {
        public AsteroidStructureMB Left;
        public AsteroidStructureMB Right;

        private Rect _borderRect;

        public int level { get; private set; }
        public void Init(int level, Rect borderRect)
        {
            this.level = level;
            _borderRect = borderRect;

        }
        
        public void Run(float rotation, Vector3 velocity)
        {
            //setup rigidbody;
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.constraints |= RigidbodyConstraints.FreezePositionY 
                                            | RigidbodyConstraints.FreezeRotationZ |
                                            RigidbodyConstraints.FreezeRotationX;
            rigidbody.drag = 0;
            rigidbody.angularDrag = 0;
            rigidbody.angularVelocity = new Vector3(0, rotation, 0);
            rigidbody.mass = Mathf.Pow(2, level);
            rigidbody.velocity = velocity;
            
            //init motion
            EntityMotionMB motion = gameObject.AddComponent<EntityMotionMB>();
            motion.Init(_borderRect);            
        }
    }
}