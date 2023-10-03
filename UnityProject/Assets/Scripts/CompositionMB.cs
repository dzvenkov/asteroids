using System.Collections.Generic;
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
        public PlayerBehaviourMB playerEntity;
        public BulletBehaviourMB BulletPrototype;
        public Transform MuzzleTransform;
        public Camera MainCamera;
        public HUDViewMB HudView;

        public PickupBehaviourMB PickupHealthProto;
        public PickupBehaviourMB PickupShieldProto;

        private IAsteroidFactory _asteroidsFactory;
        private MatchState _matchState;
        void Awake()
        {
            //This is the App's composition root
            
            //* World - borders and cameras
            Rect borderRect = CalculateBorders(MainCamera);
            CreateAndSetupOverlayCameras(MainCamera, borderRect);   
            Debug.Log(borderRect);
            
            //* Misc parts
            IInputState inputState = new InputState();
            _matchState = new MatchState(GameSettings.Settings);
            
            //* HUD
            HudView.Init(_matchState);
            
            //* Bullets
            IBulletFactory bulletFactory = new BulletFactory(BulletPrototype, MuzzleTransform,
                GameSettings.Settings.BulletSettings, borderRect);
            //* Pickups
            IPickupFactory pickupFactory = new PickupFactory(new Dictionary<PickupType, PickupBehaviourMB>()
            {
                { PickupType.Heart, PickupHealthProto },
                { PickupType.Shield, PickupShieldProto }
            }, borderRect);
            //* Player
            playerEntity.Init(borderRect);
            PlayerController.Init(inputState, 
                playerEntity,
                bulletFactory,
                _matchState,
                this.GameSettings.Settings);
            //* Asteroids
            _asteroidsFactory = new AsteroidFactory(GameSettings.Settings.AsteroidsSettings, _matchState, borderRect);
            
            //* place initial asteroids and pickups (hardcoded for now)
            _asteroidsFactory.BuildAsteroid(4, 0.5f*borderRect.size.x/2*Vector2.right - 0.3f*borderRect.size.y/2*Vector2.up);
            _asteroidsFactory.BuildAsteroid(4, -0.5f*borderRect.size.x/2*Vector2.right);
            _asteroidsFactory.BuildAsteroid(3, 0.75f*borderRect.size.x/2*Vector2.right + 0.3f*borderRect.size.y/2*Vector2.up);
            
            pickupFactory.CreatePickup(PickupType.Heart, -0.5f*borderRect.size.x/2*Vector2.right - 0.3f*borderRect.size.y/2*Vector2.up);
            pickupFactory.CreatePickup(PickupType.Shield, -0.5f*borderRect.size.x/2*Vector2.right + 0.3f*borderRect.size.y/2*Vector2.up);
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
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                _matchState.CheatAddHealth();
            }
        }

        static void CreateAndSetupOverlayCameras(Camera mainCamera, Rect borderRect)
        {
            //URP overlay cameras are used to properly render warping entities (so they are seen at both edges, not teleporting)
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
                overlayCamera = Instantiate(mainCamera, mainCamera.transform, true);
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