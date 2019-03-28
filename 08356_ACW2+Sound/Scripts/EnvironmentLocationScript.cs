using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using GameEngine.Components;
using GameEngine.Objects;
using GameEngine.Managers;
using GameEngine.Scripts;

namespace OpenGL_Game.Scripts
{
    //Author: <C.L>
    /// <summary>
    /// Calculates the environment location of itself and sets it in the component
    /// </summary>
    class EnvironmentLocationScript : Script
    {
        private const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION);

        private List<float> _wallLocations;
        private float _mazeDimension;
        private float _corridorWidth;

        private int _environmentLocation;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sceneManager">To access the node system</param>
        public EnvironmentLocationScript(float mazeDimension, float corridorWidth) : base()
        {
            _wallLocations = new List<float>();

            _mazeDimension = mazeDimension;
            _corridorWidth = corridorWidth;

            if (mazeDimension % _corridorWidth != 0)
            {
                mazeDimension = ((float)Math.Round((double)mazeDimension / _corridorWidth) * _corridorWidth);
            }

            for (int i = 1; i <= (mazeDimension / _corridorWidth) + 1; i++)
            {
                _wallLocations.Add(i * (_corridorWidth / 4.0f));
            }
        }

        public void UpdateValues(float mazeDimension, float corridorWidth)
        {
            _wallLocations = new List<float>();

            _mazeDimension = mazeDimension;
            _corridorWidth = corridorWidth;

            if (mazeDimension % _corridorWidth != 0)
            {
                mazeDimension = ((float)Math.Round((double)mazeDimension / _corridorWidth) * _corridorWidth);
            }

            for (int i = 1; i <= (mazeDimension / _corridorWidth) + 1; i++)
            {
                _wallLocations.Add(i * (_corridorWidth / 4.0f));
            }
        }

        public int EnvironmentLocation
        {
            get
            {
                return _environmentLocation;
            }
        }

        public override void OnUpdate(float pDelta)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                Vector3 position = ((ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;

                int location = -1;

                for (int i = 0; i < _wallLocations.Count - 1; i++)
                {
                    if ((Math.Abs(position.X) / 2.0f) < _wallLocations[i] && (Math.Abs(position.X) / 2.0f) > _wallLocations[i] - (_corridorWidth / 4.0f))
                    {
                        if (i > location)
                        {
                            location = i;
                        }
                    }
                    if ((Math.Abs(position.Z) / 2.0f) < _wallLocations[i] && (Math.Abs(position.Z) / 2.0f) > _wallLocations[i] - (_corridorWidth / 4.0f))
                    {
                        if (i > location)
                        {
                            location = i;
                        }
                    }
                }

                _environmentLocation = location;
            }
            else
            {
                throw new Exception("Entity doesn't have all the required components");
            }
        }
    }
}
