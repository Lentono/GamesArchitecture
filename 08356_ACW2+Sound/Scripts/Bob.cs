using GameEngine.Components;
using GameEngine.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Scripts
{
    //Ben Mullenger
    class Bob : Script
    {
        private float _bobDepth;
        private float _CurrentBob;
        private bool _up;
        private int count = 0;
        private bool _half_up;
        private float _bobSpeed;

        public Bob(float bobDepth, float bobSpeed)
        {
            _bobDepth = bobDepth;
            _bobSpeed = bobSpeed;
            _CurrentBob = 0;
            _up = true;
        }

        public override void OnUpdate(float pDelta)
        {
            float difference = (_bobDepth - _CurrentBob) / 1000;
            float amount = (float)Math.Max(Math.Abs(difference), 0.00001) * _bobSpeed;
            _CurrentBob += _half_up?  amount : -amount;
            var pos = (entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition);
            pos.Position += new OpenTK.Vector3(0, _up ? amount : -amount, 0);
            if(_CurrentBob < 0 || _CurrentBob > _bobDepth)
            {
                _half_up = !_half_up;
                _CurrentBob = _CurrentBob < 0 ? 0 : _CurrentBob;
                _CurrentBob = _CurrentBob > _bobDepth ? _bobDepth : _CurrentBob;
                count++;
            }
            if(count == 2)
            {
                _up = !_up;
                count = 0;
            }
            base.OnUpdate(pDelta);
        }
    }
}
