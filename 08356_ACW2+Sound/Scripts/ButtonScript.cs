using GameEngine.Components;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Objects;
using GameEngine.Scripts;

namespace OpenGL_Game.Scripts
{
    //Ben Mullenger
    class ButtonScript : Script
    {
        Method _OnLeftClick;
        bool highlighted;
        bool HighlightAcknowledged = false;
        Vector4 _highlightColour, _clickColour;
        public ButtonScript(Method pOnLeftClick, Vector4 pHighlightColour, Vector4 pClickColour)
            :base()
        {
            _OnLeftClick = pOnLeftClick;
            _highlightColour = pHighlightColour;
            _clickColour = pClickColour;
        }

        public override void OnMouseColide()
        {
            highlighted = true;
            HighlightAcknowledged = false;
            base.OnMouseColide();
        }
        public override void OnLeftClicked()
        {
            var col = (entity.GetComponent(ComponentTypes.COMPONENT_COLOUR) as ComponentColour);
            if (col != null)
            {
                col.Colour = _clickColour;
            }
            _OnLeftClick?.Invoke();
            base.OnLeftClicked();
        }

        public override void OnUpdate(float pDelta)
        {
            if(highlighted && !HighlightAcknowledged)
            {
                var col = (entity.GetComponent(ComponentTypes.COMPONENT_COLOUR) as ComponentColour);
                if (col != null)
                {
                    col.Colour = _highlightColour;
                }
                HighlightAcknowledged = true;
            }
            else if(!highlighted && HighlightAcknowledged)
            {
                var col = (entity.GetComponent(ComponentTypes.COMPONENT_COLOUR) as ComponentColour);
                if (col != null)
                {
                    col.Colour = col.BaseColour;
                }
                HighlightAcknowledged = false;
            }
            highlighted = false;
            base.OnUpdate(pDelta);
        }
    }
}
