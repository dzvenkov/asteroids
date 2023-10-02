using UnityEngine;

namespace Asteroids
{
    public class CompositionMB : MonoBehaviour
    {
        public GameSettingsHolderSO GameSettings;
        public PlayerControllerMB PlayerController;
        public EntityMotionMB PlayerEntityMotion;
        public Camera MainCamera;

        void Awake()
        {
            Rect borderRect = CalculateBorders(MainCamera);
            Debug.Log(borderRect);
            IInputState inputState = new InputState();
            PlayerEntityMotion.Init(borderRect);
            PlayerController.Init(inputState, 
                PlayerEntityMotion,
                this.GameSettings.Settings);
            IAsteroidFactory asteroidsFactory = new AsteroidFactory(GameSettings.Settings.AsteroidsFactorySettings);
            asteroidsFactory.BuildAsteroid(5, 10*Vector2.right);
        }

        Rect CalculateBorders(Camera camera)
        {
            //assumes camera is looking down
            Debug.Assert(Mathf.Approximately(camera.transform.forward.y, -1f));
            Plane ground = new Plane(Vector3.up, Vector3.zero);
            Ray ray00 = camera.ScreenPointToRay(new Vector3(0, 0, 0));
            Ray ray11 = camera.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));
            ground.Raycast(ray00, out var enter);
            Vector3 corner00 = ray00.GetPoint(enter);
            ground.Raycast(ray11, out enter);
            Vector3 corner11 = ray11.GetPoint(enter);
            Rect result = new Rect();
            result.size = new Vector2(corner11.x - corner00.x, corner11.z - corner00.z);
            result.center = new Vector2(corner11.x + corner00.x, corner11.z + corner00.z) / 2;
            return result;
        }
    }
}