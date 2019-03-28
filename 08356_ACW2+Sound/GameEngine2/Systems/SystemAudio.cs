using System;
using System.IO;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;

using GameEngine.Components;
using GameEngine.Objects;
using GameEngine.Managers;

namespace GameEngine.Systems
{
    //Author: <C.L>
    /// <summary>
    /// 
    /// </summary>
    public class SystemAudio : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_AUDIO);

        private List<Entity> _entities;
        private Entity _player;

        /// <summary>
        /// Constructor
        /// </summary>
        public SystemAudio(SceneManager sceneManager)
        {
            _entities = new List<Entity>();
        }

        public string Name
        {
            get { return "SystemAudio"; }
        }

        public void AddPlayerEntity(Entity player)
        {
            _player = player;
        }

        public void OnNewEntity(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                _entities.Add(entity);
                ComponentAudio audio = ((ComponentAudio)entity.GetComponent(ComponentTypes.COMPONENT_AUDIO));
                audio.MuteAudio(false);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                _entities.Remove(entity);
                ComponentAudio audio = ((ComponentAudio)entity.GetComponent(ComponentTypes.COMPONENT_AUDIO));

                if (audio != null)
                {
                    audio.MuteAudio(true);
                }
            }
        }

        public void OnUpdate()
        {
            foreach (var entity in _entities)
            {
                ComponentAudio audio = ((ComponentAudio)entity.GetComponent(ComponentTypes.COMPONENT_AUDIO));

                Vector3 position = ((ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;

                Vector3 listenerPosition = ((ComponentPosition)_player.GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;
                Vector3 listenerDirection = -Vector3.Normalize(position - listenerPosition);
                Vector3 listenerUp = Vector3.UnitY;

                AL.Source(audio.AudioSource, ALSource3f.Position, ref position);
                AL.Listener(ALListener3f.Position, ref listenerPosition);
                AL.Listener(ALListenerfv.Orientation, ref listenerDirection, ref listenerUp);
            }
        }

        public void OnRender()
        {

        }
    }
}
