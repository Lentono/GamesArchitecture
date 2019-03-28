using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Audio.OpenAL;

using GameEngine.Managers;

namespace GameEngine.Components
{
    //<C.L>
    /// <summary>
    /// Component for loading and retrieving the handle of the audio buffer
    /// </summary>
    public class ComponentAudio : IComponent
    {
        private int _audioSource;
        //Holds the pointer handle of the audio buffer loaded in through the resource manager
        private int _audioBuffer;

        public bool mute
        {
            get;
            private set;
        }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public ComponentAudio()
        {
            mute = false;
            _audioSource = 0;
            _audioBuffer = 0;
        }

        /// <summary>
        /// Create an audio component and load the audio file
        /// </summary>
        /// <param name="fileName">Name of the audio file</param>
        public ComponentAudio(string fileName, bool loop)
        {
            _audioSource = AL.GenSource();
            _audioBuffer = ResourceManager.LoadAudio(fileName);

            AL.Source(_audioSource, ALSourcei.Buffer, _audioBuffer);
            AL.Source(_audioSource, ALSourceb.Looping, loop);
            
            //if (!_mute)
            //{
            //    AL.SourcePlay(_audioSource);
            //}
        }

        /// <summary>
        /// Retrieve the audio source handle
        /// </summary>
        public int AudioSource
        {
            get
            {
                return _audioSource;
            }
        }

        /// <summary>
        /// Retrieve the audio buffer handle
        /// </summary>
        public int AudioBuffer
        {
            get
            {
                return _audioBuffer;
            }
        }

        //Set the components audio buffer
        public void SetAudioBuffer(string fileName, bool loop)
        {
            //Stop old source and delete it
            AL.SourceStop(_audioSource);
            AL.DeleteSource(_audioSource);

            //Generate new source and get requested buffer
            _audioSource = AL.GenSource();
            _audioBuffer = ResourceManager.LoadAudio(fileName);

            //Set source to buffer and play
            AL.Source(_audioSource, ALSourcei.Buffer, _audioBuffer);
            AL.Source(_audioSource, ALSourceb.Looping, loop);

            if (!mute)
            {
                AL.SourcePlay(_audioSource);
            }
        }

        public void MuteAudio(bool mute)
        {
            this.mute = mute;

            if (!this.mute)
            {
                AL.SourcePlay(_audioSource);
            }
            else
            {
                AL.SourceStop(_audioSource);
            }
        }

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_AUDIO;
            }
        }
    }
}
