using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Asteroids
{
    public class CompositionMB : MonoBehaviour
    {
        public GameSettingsHolderSO GameSettings;
        public PlayerControllerMB PlayerController;
        public PlayerBehaviourMB PlayerBehaviour;
        public Camera MainCamera;


        private IAsteroidFactory _asteroidsFactory;
        void Awake()
        {
            //This is the App's composition root
            Rect borderRect = CalculateBorders(MainCamera);
            Debug.Log(borderRect);
            IInputState inputState = new InputState();
            
            //1. Player
            PlayerBehaviour.Init(borderRect);
            PlayerController.Init(inputState, 
                PlayerBehaviour,
                this.GameSettings.Settings);
            //2. Asteroids
            _asteroidsFactory = new AsteroidFactory(GameSettings.Settings.AsteroidsSettings, borderRect);
            _asteroidsFactory.BuildAsteroid(5, 10*Vector2.right);//test
            CreateAndSetupOverlayCameras(MainCamera, borderRect);   
        }

        public void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                foreach (var asteroid in _asteroidsFactory.Asteroids.ToList())
                {
                    _asteroidsFactory.SplitAsteroid(asteroid);
                }
            }
        }

        static void CreateAndSetupOverlayCameras(Camera mainCamera, Rect borderRect)
        {
            //URP overlay cameras are used to 
            UniversalAdditionalCameraData mainCameraData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
            
            Camera overlayCamera;
            UniversalAdditionalCameraData overlayCameraData;
            foreach (Vector3 offset in new Vector3[]
                     {
                         new Vector3(0, 0, borderRect.size.y),
                         new Vector3(0, 0, -borderRect.size.y),
                         new Vector3(borderRect.size.x, 0, 0),
                         new Vector3(-borderRect.size.x, 0, 0),
                     })
            {
                overlayCamera = Instantiate(mainCamera);
                Destroy(overlayCamera.GetComponent<AudioListener>());
                overlayCamera.transform.position += offset;
                overlayCameraData = overlayCamera.GetComponent<UniversalAdditionalCameraData>();
                overlayCameraData.renderPostProcessing = false;
                overlayCameraData.renderType = CameraRenderType.Overlay;
                mainCameraData.cameraStack.Add(overlayCamera);
            }            
        }
        
        static Rect CalculateBorders(Camera camera)
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