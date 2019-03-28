using GameEngine.Components;
using GameEngine.Managers;
using GameEngine.Objects;
using GameEngine.Scripts;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Systems
{
    //Ben Mullenger
    public class System2DInteraction : ISystem
    {

        private List<Entity> _entities = new List<Entity>();
        private const ComponentTypes COLLIDER_MASK = ComponentTypes.COMPONENT_2D_COLLIDER | ComponentTypes.COMPONENT_SCRIPT;

        public string Name
        {
            get
            {
                return "SystemInput";
            }
        }

        public void OnNewEntity(Entity entity)
        {
            if (entity.HasMask(COLLIDER_MASK))
            {
                _entities.Add(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        private bool _leftClick = false;
        private bool _rightClick = false;
        private bool _leftHeld = false;
        private bool _rightHeld = false;
        private Vector2 _mouseCoords = new Vector2();

        public System2DInteraction()
        {
            InputManager.AddMouseMoveTrigger(() =>
            {
                _mouseCoords = InputManager.MouseCoords;
            });
            InputManager.AddMouseTrigger(OpenTK.Input.MouseButton.Left, ButtonPosition.PressedOnce, () => _leftClick = true);
            InputManager.AddMouseTrigger(OpenTK.Input.MouseButton.Right, ButtonPosition.PressedOnce, () => _rightClick = true);
            InputManager.AddMouseTrigger(OpenTK.Input.MouseButton.Left, ButtonPosition.Held, () => _leftHeld = true);
            InputManager.AddMouseTrigger(OpenTK.Input.MouseButton.Right, ButtonPosition.Held, () => _rightHeld = true);
        }

        public void OnUpdate()
        {
            foreach (var e in _entities)
            {
                var collider = e.GetComponent(ComponentTypes.COMPONENT_2D_COLLIDER) as Component2DCollider;
                var script = e.GetComponents(ComponentTypes.COMPONENT_SCRIPT);
                var pos = e.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition;
                var position = pos != null ? pos.Position : Vector3.Zero;
                var rect = new System.Drawing.Rectangle(collider.rectangle.Left + (int)position.X, collider.rectangle.Top + (int)position.Y, collider.rectangle.Width, collider.rectangle.Height);

                if (rect.IntersectsWith(new System.Drawing.Rectangle((int)_mouseCoords.X, (int)_mouseCoords.Y, 1, 1)))
                {
                    foreach (var s in script)
                    {
                        var sci = (s as ComponentScript).script;
                        sci.OnMouseColide();

                        if (_leftHeld)
                        {
                            sci.OnLeftClickHeld();
                        }
                        else if (_leftClick)
                        {
                            sci.OnLeftClicked();
                        }
                        if (_rightHeld)
                        {
                            sci.OnRightClickHeld();
                        }
                        else if (_rightClick)
                        {
                            sci.OnRightClicked();
                        }
                    }
                }
            }
            _leftHeld = _leftClick = _rightHeld = _rightClick = false;

        }
        public void OnRender()
        {

        }
    }
}
