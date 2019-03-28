//using System;
//using System.Collections.Generic;
//using System.Linq;
//using OpenGL_Game.Components;
//using OpenGL_Game.Managers;
//using OpenGL_Game.Objects;
//using OpenGL_Game.Systems;
//using OpenTK;
//using OpenGL_Game;

//namespace OpenGL_Game
//{

//    // mjb not finished (or maybe not used in future//we already have a player?)
//    public class PlayerObject
//    {
//        enum stateOfPlayer { Moving, Dead, Stood }

//        private EntityManager _entityManager;
//        private Entity _playerEntity;
//        private Vector3 _startPosition;
//        private stateOfPlayer _currentState;
//        private int _health;
//        private int _powerDuration;
//        private float _timer;
//        static private bool _powerActives;

//        public PlayerObject()
//        {
//            _startPosition = new Vector3(0.0f, 2.0f, 0.0f);
//            _currentState = stateOfPlayer.Stood;
//            _playerEntity = null;
            
//          //  _entityManager = MyGame.gameInstance.entityManager;
//            _health = 3;
//            _powerDuration = 5;
//            _timer = 0;
//            _powerActives = false;
//        }

//        public void SpawnPlayer()
//        {
//            Entity newEntity;
//            newEntity = new Entity("Player");
//            newEntity.AddComponent(new ComponentColliderCircle(0.25f, false));
//            newEntity.AddComponent(new ComponentVelocity(+1.0f, 0.0f, 0.0f));
//            newEntity.AddComponent(new ComponentPosition(0.0f, 0.0f, 0.0f));
//            newEntity.AddComponent(new ComponentGeometry("CubeGeometry.txt"));
//            newEntity.AddComponent(new ComponentTexture("spacewall.png"));
//            newEntity.AddComponent(new ComponentRotation());
//            newEntity.AddComponent(new ComponentScale(1.0f, 1.0f, 1.0f));
//            newEntity.AddComponent(new ComponentUserInspectControl());
//            _playerEntity = newEntity;
//            _entityManager.AddEntity(newEntity);

//        }
        
//        public void Update()
//        {
            
//            if (_currentState == stateOfPlayer.Dead)
//                return;

//            ComponentColliderCircle colliderCircle = (ComponentColliderCircle)_playerEntity.Components.Find(pe => pe.ComponentType == ComponentTypes.COMPONENT_SPHERE_COLLIDER);
//            ComponentPosition posComp = (ComponentPosition)_playerEntity.Components.Find(pe => pe.ComponentType == ComponentTypes.COMPONENT_POSITION);
//            ComponentVelocity velComp = (ComponentVelocity)_playerEntity.Components.Find(pe => pe.ComponentType == ComponentTypes.COMPONENT_VELOCITY);
            
//        }

        
//        public Vector3 StartPosition
//        {
//            get { return _startPosition; }
//        }
        
        
//        static public bool PowerActives
//        {
//            get { return _powerActives; }
//        }
        
//    }

//}
