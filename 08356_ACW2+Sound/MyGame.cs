using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using GameEngine.Components;
using GameEngine.Systems;
using GameEngine.Managers;
using GameEngine.Objects;
using GameEngine.Scripts;
using OpenTK.Graphics;
using OpenGL_Game.Scripts;

namespace OpenGL_Game
{
    //All
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class MyGame : GameWindow
    {
        EntityManager entityManager;
        SystemManager systemManager;
        SceneManager sceneManager;

        private Entity _playerEntity;

        private Vector4 ＶＡＰＯＲＷＡＶＥ = new Vector4(0.933f, 0.24f, 1f, 1);  //Very Important!!!

        public static MyGame gameInstance;

        public static int width = 1400, height = 700;

        private float _mazeDimensionXZ = 25.0f;
        //Must be modular with maze dimension, if not the maze dimension will be rounded up to compensate
        private float _corridorWidth = 6.5f; // Results in maze dimension actually being 26 instead of 25

        private string vaporText = "VAPORWAVE no";
        private bool isＶＡＰＯＲＷＡＶＥ = true;

        //PlayerObject playerObject;
        //public static float dt;
        /// <summary>
        /// Get current Scene's orthographic projection
        /// </summary>
        public Matrix4 Orthographic
        {
            get
            {
                return sceneManager.GetOrthographic();
            }
            set
            {
                sceneManager.SetOrthographic(value);
            }
        }
        /// <summary>
        /// Get current Scene's perspective projection
        /// </summary>
        public Matrix4 Perspective
        {
            get
            {
                return sceneManager.GetPerspective();
            }
            set
            {
                sceneManager.SetPerspective(value);
            }
        }
        /// <summary>
        /// Get current Scene's view projection
        /// </summary>
        public Matrix4 View
        {
            get
            {
                return sceneManager.GetView();
            }
            set
            {
                sceneManager.SetView(value);
            }
        }
        /// <summary>
        /// Get current Scene's camera index
        /// </summary>
        public int CameraNum
        {
            get
            {
                return sceneManager.GetCameraNum();
            }
            set
            {
                sceneManager.SetCameraNum(value);
            }
        }

        ViewProjectionManager _viewProjectionManager;

        internal static MyGame GameInstance
        {
            get => gameInstance;
            set => gameInstance = value;
        }

        public MyGame() : base()
        {
            GameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            sceneManager = new SceneManager(entityManager);
            //playerObject = new PlayerObject();
            AudioContext AC = new AudioContext();
            X = 0;
            Y = 0;
            Width = width;
            Height = height;
            var p = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), width / height, 0.01f, 100f);
            var o = Matrix4.CreateOrthographicOffCenter(0, width, 0, height, -1000f, 1000f);
            _viewProjectionManager = new ViewProjectionManager(Matrix4.Identity, p, o);
        }

        private void CreateEntities()
        {
            _playerEntity = new Entity("Camera1");

            MazeEnvironment.GenerateMaze(sceneManager, _mazeDimensionXZ, _corridorWidth);
            NodeEnvironment.LinkNodeEnvironment(sceneManager, _mazeDimensionXZ, _corridorWidth);
            MazeEnvironment.PlaceDrones(sceneManager);
            MazeEnvironment.PlacePowerUps(sceneManager);

            //playerObject.SpawnPlayer();

            Entity newEntity;

            Entity musicEntity = new Entity("MusicObject");
            musicEntity.AddComponent(new ComponentAudio("Doom-Tronica", true));
            musicEntity.AddComponent(new ComponentPosition(0.0f, -0.5f, 0.0f));
            entityManager.AddEntity(musicEntity);

            newEntity = new Entity("MusicObject2");
            newEntity.AddComponent(new ComponentAudio("drone-collide", false));
            newEntity.AddComponent(new ComponentPosition(0.0f, 0.0f, 0.0f));
            sceneManager.AddToScene(newEntity, "Main");

            newEntity = new Entity("OBJTest");
            newEntity.AddComponent(new ComponentTexture("spacewall.png"));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Sphere.obj"));
            newEntity.AddComponent(new ComponentMaterial(ResourceManager.GetMaterial("TextureUnLit")));
            newEntity.AddComponent(new ComponentPosition(0, 0, 0));
            newEntity.AddComponent(new ComponentRotation(0, 0, 0));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentScript(new Rotate(new Vector3(0.01f, -0.01f, 0.025f))));
            newEntity.AddComponent(new ComponentScript(new FollowPosition(_playerEntity)));
            newEntity.AddComponent(new ComponentScale(0.1f, 0.1f, 0.1f));
            sceneManager.AddToScene(newEntity, "Main");

            newEntity = new Entity("FrameBufferTest");
            newEntity.AddComponent(new ComponentTexture("FrameBufferTexture"));
            newEntity.AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            newEntity.AddComponent(new ComponentPosition(Height / 6, 600, 0));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentUI());
            newEntity.AddComponent(new ComponentScale(Height / 3, Height / 3, 1.0f));
            sceneManager.AddToScene(newEntity, "Main");

            var player = new Player(3, 10, 8, new Vector3(0.0f, 1.25f, 0.0f), sceneManager);

            newEntity = new Entity("HealthText");
            newEntity.AddComponent(new ComponentPosition(15, 15, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(7.0f, 7.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentText(() => { return player.HealthDisplay; }, ResourceManager.LoadTexture("samplefont.gif"), new Vector2(16, 14)));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Main");

            newEntity = new Entity("Ammo Text");
            newEntity.AddComponent(new ComponentPosition(15, 45, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(7.0f, 7.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentText(() => { return player.AmmoDisplay; }, ResourceManager.LoadTexture("samplefont.gif"), new Vector2(16, 14)));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Main");

            _playerEntity.AddComponent(new ComponentCamera(CameraTypes.FPS, 45, 100, 0.1f, width, height, new Vector3(0, 0.75f, 0)));
            _playerEntity.AddComponent(new ComponentPosition(new Vector3(0, 1.25f, 0)));
            _playerEntity.AddComponent(new ComponentRotation());
            _playerEntity.AddComponent(new ComponentGeometry("Geometry/gun.obj"));
            _playerEntity.AddComponent(new ComponentScale(0.05f, 0.05f, 0.05f));
            _playerEntity.AddComponent(new ComponentScript(new EnvironmentLocationScript(_mazeDimensionXZ, _corridorWidth)));
            _playerEntity.AddComponent(new ComponentScript(player));
            _playerEntity.AddComponent(new ComponentScript(new PlayerNodeCollisionScript(sceneManager)));
            _playerEntity.AddComponent(new ComponentSphereCollider(0.5f));
            _playerEntity.AddComponent(new ComponentRigidbody(false));
            sceneManager.AddToScene(_playerEntity, "Main");


            newEntity = new Entity("MapPlayer");
            newEntity.AddComponent(new ComponentPosition(new Vector3(0, 0, 0)));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentTexture("Arrow.png"));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentScale(1.0f, 1.0f, 1.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentMaterial(ResourceManager.GetMaterial("TextureUnLit")));
            newEntity.AddComponent(new ComponentScript(new MapAvitar(_playerEntity, new Vector3((float)Math.PI / 2,0,0))));
            newEntity.AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", false));
            sceneManager.AddToScene(newEntity, "Main");


            //Give the audio system the player reference
            var audio = (SystemAudio)systemManager.FindSystem("SystemAudio");
            audio.AddPlayerEntity(newEntity);

            newEntity = new Entity("StartBack");
            newEntity.AddComponent(new ComponentPosition(200, 200, -1));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture("neutralLand.png"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(100.0f, 30.0f, 1.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentUI());
            newEntity.AddComponent(new Component2DCollider(-100, -30, 200, 60));
            newEntity.AddComponent(new ComponentScript(new ButtonScript(() => sceneManager.SetScene("Main"), new Vector4(0.5f, 0.5f, 0.5f, 1), new Vector4(0, 0, 0, 1))));
            sceneManager.AddToScene(newEntity, "Menu");
            sceneManager.AddToScene(newEntity, "LevelComplete");
            

            newEntity = new Entity("StartText");
            newEntity.AddComponent(new ComponentPosition(160, 200, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(10.0f, 10.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(0, 0, 0, 1));
            newEntity.AddComponent(new ComponentText("Start", ResourceManager.LoadTexture("samplefont.gif")));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Menu");
            sceneManager.AddToScene(newEntity, "LevelComplete");

            newEntity = new Entity("OptionBack");
            newEntity.AddComponent(new ComponentPosition(200, 300, -1));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture("neutralLand.png"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(100.0f, 30.0f, 1.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentUI());
            newEntity.AddComponent(new Component2DCollider(-100, -30, 200, 60));
            newEntity.AddComponent(new ComponentScript(new ButtonScript(() => sceneManager.SetScene("Options"), new Vector4(0.5f, 0.5f, 0.5f, 1), new Vector4(0, 0, 0, 1))));
            sceneManager.AddToScene(newEntity, "Menu");

            newEntity = new Entity("OptionsText");
            newEntity.AddComponent(new ComponentPosition(160, 300, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(10.0f, 10.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(0, 0, 0, 1));
            newEntity.AddComponent(new ComponentText("Options", ResourceManager.LoadTexture("samplefont.gif")));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Menu");

            newEntity = new Entity("ExitBack");
            newEntity.AddComponent(new ComponentPosition(200, 400, -1));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture("neutralLand.png"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(100.0f, 30.0f, 1.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentUI());
            newEntity.AddComponent(new Component2DCollider(-100, -30, 200, 60));
            newEntity.AddComponent(new ComponentScript(new ButtonScript(Exit, new Vector4(0.5f, 0.5f, 0.5f, 1), new Vector4(0, 0, 0, 1))));
            sceneManager.AddToScene(newEntity, "Menu");
            sceneManager.AddToScene(newEntity, "GameOver");
            sceneManager.AddToScene(newEntity, "LevelComplete");

            newEntity = new Entity("ExitText");
            newEntity.AddComponent(new ComponentPosition(160, 400, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(10.0f, 10.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(0, 0, 0, 1));
            newEntity.AddComponent(new ComponentText("Exit", ResourceManager.LoadTexture("samplefont.gif")));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Menu");
            sceneManager.AddToScene(newEntity, "GameOver");
            sceneManager.AddToScene(newEntity, "LevelComplete");

            newEntity = new Entity("ExitBack");
            newEntity.AddComponent(new ComponentPosition(200, 400, -1));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture("neutralLand.png"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(100.0f, 30.0f, 1.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentUI());
            newEntity.AddComponent(new Component2DCollider(-100, -30, 200, 60));
            newEntity.AddComponent(new ComponentScript(new ButtonScript(() => sceneManager.SetScene("Menu"), new Vector4(0.5f, 0.5f, 0.5f, 1), new Vector4(0, 0, 0, 1))));
            sceneManager.AddToScene(newEntity, "Options");

            newEntity = new Entity("ExitText");
            newEntity.AddComponent(new ComponentPosition(160, 400, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(10.0f, 10.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(0, 0, 0, 1));
            newEntity.AddComponent(new ComponentText("Exit", ResourceManager.LoadTexture("samplefont.gif")));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Options");

            newEntity = new Entity("VaporToggle");
            newEntity.AddComponent(new ComponentPosition(1000, 200, -1));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture("neutralLand.png"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(100.0f, 30.0f, 1.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentUI());
            newEntity.AddComponent(new Component2DCollider(-100, -30, 200, 60));
            newEntity.AddComponent(new ComponentScript(new ButtonScript(() =>
            {
                var ppes = ResourceManager.GetPostProccessEffects();
                isＶＡＰＯＲＷＡＶＥ = !isＶＡＰＯＲＷＡＶＥ;
                vaporText = "VAPORWAVE " + (isＶＡＰＯＲＷＡＶＥ ? "no" : "yes");
                ppes["Blur"].Active = isＶＡＰＯＲＷＡＶＥ;
                ppes["Offset"].Active = isＶＡＰＯＲＷＡＶＥ;
                ppes["Colour"].Active = isＶＡＰＯＲＷＡＶＥ;

            }, new Vector4(0.5f, 0.5f, 0.5f, 1), new Vector4(0, 0, 0, 1))));
            sceneManager.AddToScene(newEntity, "Options");

            newEntity = new Entity("VaporToggleText");
            newEntity.AddComponent(new ComponentPosition(920, 200, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(7.0f, 7.0f, 7.0f));
            newEntity.AddComponent(new ComponentColour(0, 0, 0, 1));
            newEntity.AddComponent(new ComponentText(() => vaporText, ResourceManager.LoadTexture("samplefont.gif"), new Vector2(16, 14)));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Options");

            newEntity = new Entity("MuteMusic");
            newEntity.AddComponent(new ComponentPosition(1000, 300, -1));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture("neutralLand.png"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(100.0f, 30.0f, 1.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentUI());
            newEntity.AddComponent(new Component2DCollider(-100, -30, 200, 60));
            newEntity.AddComponent(new ComponentScript(new ButtonScript(() =>
            {
                ComponentAudio entityAudio = (ComponentAudio)musicEntity.GetComponent(ComponentTypes.COMPONENT_AUDIO);

                entityAudio.MuteAudio(!entityAudio.mute);

            }, new Vector4(0.5f, 0.5f, 0.5f, 1), new Vector4(0, 0, 0, 1))));
            sceneManager.AddToScene(newEntity, "Options");

            newEntity = new Entity("MuteMusicText");
            newEntity.AddComponent(new ComponentPosition(940, 300, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(8.0f, 8.0f, 8.0f));
            newEntity.AddComponent(new ComponentColour(0, 0, 0, 1));
            newEntity.AddComponent(new ComponentText("Mute Music", ResourceManager.LoadTexture("samplefont.gif")));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Options");

            newEntity = new Entity("Controls List Spawn");
            newEntity.AddComponent(new ComponentScript(new SpawnButtonListScript(sceneManager, "Options", (systemManager.FindSystem("SystemInput") as SystemInput).Controls, ResourceManager.LoadTexture("samplefont.gif"))));

            newEntity = new Entity("SplashText");
            newEntity.AddComponent(new ComponentPosition(400, 200, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(50.0f, 50.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentText("TEAM 3!WOW!", ResourceManager.LoadTexture("samplefont.gif")));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "Menu");

            newEntity = new Entity("GameOverSplashText");
            newEntity.AddComponent(new ComponentPosition(400, 200, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(50.0f, 50.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentText("GAME OVER!", ResourceManager.LoadTexture("samplefont.gif")));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "GameOver");

            newEntity = new Entity("PicardFacepalm");
            newEntity.AddComponent(new ComponentPosition(1000, 500, -1));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture("Picard.png"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(200.0f, 200.0f, 1.0f));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "GameOver");

            newEntity = new Entity("PicardHappy");
            newEntity.AddComponent(new ComponentPosition(1000, 500, -1));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture("Picard2.png"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(200.0f, 200.0f, 1.0f));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "LevelComplete");


            newEntity = new Entity("CameraMenu");
            newEntity.AddComponent(new ComponentCamera(CameraTypes.Stationary, 45, 100, 1f, width, height, new Vector3(0, 2, 0)));
            newEntity.AddComponent(new ComponentPosition(new Vector3(0, 0, 0)));
            newEntity.AddComponent(new ComponentRotation());
            sceneManager.AddToScene(newEntity, "Menu");

            newEntity = new Entity("LevelCompleteSplashText");
            newEntity.AddComponent(new ComponentPosition(400, 200, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            newEntity.AddComponent(new ComponentRotation());
            newEntity.AddComponent(new ComponentScale(25.0f, 25.0f, 10.0f));
            newEntity.AddComponent(new ComponentColour(ＶＡＰＯＲＷＡＶＥ));
            newEntity.AddComponent(new ComponentText("LEVEL COMPLETE!", ResourceManager.LoadTexture("samplefont.gif")));
            newEntity.AddComponent(new ComponentUI());
            sceneManager.AddToScene(newEntity, "LevelComplete");
        }

        private void CreateSystems()
        {
            ISystem newSystem;

            newSystem = new SystemRender(this, _viewProjectionManager);
            systemManager.AddSystem(newSystem);

            newSystem = new SystemInput();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemCamera(_viewProjectionManager);
            systemManager.AddSystem(newSystem);

            newSystem = new System2DInteraction();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemCollision();
            systemManager.AddSystem(newSystem);

            //Need a reference to the player for the listener position
            //We can do this another way but I'm just doing this for now
            newSystem = new SystemAudio(sceneManager);
            systemManager.AddSystem(newSystem);
        }

        /// <summary>
        /// Allows the game to setup the environment and matrices.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            InputManager.Initialise(this);

            var p = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), width / height, 0.01f, 100f);
            var o = Matrix4.CreateOrthographicOffCenter(0, width, 0, height, -1000f, 1000f);

            int MapOrSize = 20;
            var o2 = Matrix4.CreateOrthographicOffCenter(-MapOrSize, MapOrSize, -MapOrSize, MapOrSize, -50, 50);
            var v = Matrix4.LookAt(new Vector3(0, 40, 0), new Vector3(0, -1, 0), new Vector3(0, 0, 1));

            ResourceManager.GenFrameBuffer("MapBuffer", "FrameBufferTexture", Width, Height, o2, o, v, new Vector4(1, 1, 1, 1));

            sceneManager.AddScene("Menu", new Scene(p, o, () =>
            {
                InputManager.LockMouseToScreen = false;

                InputManager.IsMouseVisible = true;
            }
            , null));
            sceneManager.AddScene("Options", new Scene(p, o, () =>
            {
                InputManager.LockMouseToScreen = false;

                InputManager.IsMouseVisible = true;
            }, null));

            void meth() //Needed to make this so I could remove it from the check list when it was needed.
            {
                sceneManager.SetScene("Menu");
            }

            sceneManager.AddScene("Main", new Scene(p, o, () =>
            {
                InputManager.LockMouseToScreen = true;

                InputManager.IsMouseVisible = false;

                InputManager.AddKeyboardTrigger(Key.Escape, ButtonPosition.PressedOnce, meth);
            }, () => InputManager.RemoveKeyboardTrigger(Key.Escape, ButtonPosition.PressedOnce, meth)));

            sceneManager.AddScene("GameOver", new Scene(p,o, () =>
            {
                ResourceManager.GetPostProccessEffects()["AttackShake"].Active = false;

                InputManager.LockMouseToScreen = false;

                InputManager.IsMouseVisible = true;
            }
            , null));

            sceneManager.AddScene("LevelComplete", new Scene(p, o, () =>
            {
                ResourceManager.GetPostProccessEffects()["AttackShake"].Active = false;

                InputManager.LockMouseToScreen = false;

                InputManager.IsMouseVisible = true;

                _mazeDimensionXZ += _corridorWidth;

                //Reset player position
                ComponentPosition position = _playerEntity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition;

                position.Position = new Vector3(0.0f, 1.25f, 0.0f);

                //Update player script
                var scripts = _playerEntity.GetComponents(ComponentTypes.COMPONENT_SCRIPT);

                foreach (ComponentScript script in scripts)
                {
                    if (script.script is EnvironmentLocationScript)
                    {
                        (script.script as EnvironmentLocationScript).UpdateValues(_mazeDimensionXZ, _corridorWidth);
                    }
                }

                //Regenerate the maze
                MazeEnvironment.GenerateMaze(sceneManager, _mazeDimensionXZ, _corridorWidth);
                NodeEnvironment.LinkNodeEnvironment(sceneManager, _mazeDimensionXZ, _corridorWidth);
                MazeEnvironment.PlaceDrones(sceneManager);
                MazeEnvironment.PlacePowerUps(sceneManager);
            }
            , null));

            sceneManager.SetScene("Menu");

            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            CreateSystems();
            CreateEntities();
        }

        /// <summary>
        /// Logic called when window unloads
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            ResourceManager.CleanUp();
            base.OnUnload(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            //dt = (float)e.Time;
            //if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Key.Escape))
            //    Exit();

            InputManager.Update();

            systemManager.Update();

            entityManager.Update((float)e.Time);
            sceneManager.CleanUpDestoyed();
            systemManager.ManagerEntityChanges(entityManager);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            entityManager.Render();
            systemManager.Render();

            GL.Flush();
            SwapBuffers();
        }
    }
}
