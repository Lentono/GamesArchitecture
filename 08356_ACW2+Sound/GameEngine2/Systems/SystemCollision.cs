using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using OpenTK;

using GameEngine.Objects;
using GameEngine.Components;

namespace GameEngine.Systems
{
    //Ben Mullenger, Callum Lenton
    public class SystemCollision : ISystem
    {
        const ComponentTypes SPHERE_MASK = ComponentTypes.COMPONENT_SPHERE_COLLIDER;
        const ComponentTypes LINE_MASK = ComponentTypes.COMPONENT_LINE_COLLIDER;
        public string Name => "SystemCollision";

        private List<Entity> _entities = new List<Entity>();

        public void OnNewEntity(Entity entity)
        {
            if (entity.HasMask(SPHERE_MASK) || entity.HasMask(LINE_MASK))
            {
                _entities.Add(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void OnRender()
        {
            
        }

        public void OnUpdate()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                for (int j = i + 1; j < _entities.Count; j++)
                {
                    var e = _entities[i];
                    var e2 = _entities[j];

                    if (CheckCollision(e, e2, out float overlap))
                    {
                        if (e.HasMask(ComponentTypes.COMPONENT_RIGIDBODY) && e2.HasMask(ComponentTypes.COMPONENT_RIGIDBODY))
                        {
                            var isKin1 = (e.GetComponent(ComponentTypes.COMPONENT_RIGIDBODY) as ComponentRigidbody).IsKinematic;
                            var isKin2 = (e2.GetComponent(ComponentTypes.COMPONENT_RIGIDBODY) as ComponentRigidbody).IsKinematic;

                            if (!isKin1)
                            {
                                var posCom = e.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition;

                                var direction = Vector3.Normalize(posCom.Position - posCom.OldPosition);

                                if (e2.HasMask(ComponentTypes.COMPONENT_LINE_COLLIDER))
                                {
                                    var lineComp = e2.GetComponent(ComponentTypes.COMPONENT_LINE_COLLIDER) as ComponentLineCollider;
                                    var wallVector = lineComp.Start - lineComp.End;
                                    var dir = posCom.OldPosition - posCom.Position;
                                    var cross = Vector3.Cross(wallVector, new Vector3(0, 1, 0));
                                    cross *= Vector3.Dot(dir, cross) > 0 ? 1 : -1;
                                    posCom.Position += cross.Normalized() * overlap;
                                    j--;
                                }
                                else
                                {
                                    posCom.Position = posCom.OldPosition;
                                }
                            }

                            if (!isKin2)
                            {
                                var posCom = e2.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition;

                                var direction = Vector3.Normalize(posCom.Position - posCom.OldPosition);

                                if (e.HasMask(ComponentTypes.COMPONENT_LINE_COLLIDER))
                                {
                                    var lineComp = e.GetComponent(ComponentTypes.COMPONENT_LINE_COLLIDER) as ComponentLineCollider;
                                    var wallVector = lineComp.Start - lineComp.End;
                                    var dir = posCom.OldPosition - posCom.Position;
                                    var cross = Vector3.Cross(wallVector, new Vector3(0, 1, 0));
                                    cross *= Vector3.Dot(dir, cross) > 0 ? 1 : -1;
                                    posCom.Position += cross.Normalized() * overlap;
                                    j--;
                                }
                                else
                                {
                                    posCom.Position = posCom.OldPosition;
                                }
                            }
                        }

                        var scripts = e.GetComponents(ComponentTypes.COMPONENT_SCRIPT);
                        foreach (ComponentScript s in scripts)
                        {
                            s.script.OnCollision(e2);
                        }
                        scripts = e2.GetComponents(ComponentTypes.COMPONENT_SCRIPT);
                        foreach (ComponentScript s in scripts)
                        {
                            s.script.OnCollision(e);
                        }
                    }
                }
            }
        }

        private bool CheckCollision(Entity e, Entity e2, out float overlap)
        {
            bool eSphere = e.HasMask(SPHERE_MASK);
            bool e2Sphere = e2.HasMask(SPHERE_MASK);

            if(eSphere != e2Sphere)
            {
                if(eSphere)
                {
                    return SphereLine(e, e2, out overlap);
                }
                else
                {
                    return SphereLine(e2, e, out overlap);
                }
            }
            else if(eSphere)
            {
                return SphereSphere(e, e2, out overlap);
            }
            else
            {
                return LineLine(e, e2, out overlap);
            }
        }

        private bool SphereLine(Entity sphere, Entity line, out float overlap)
        {
            var spherePos = (sphere.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;
            var linePos = (line.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

            var sphereCol = sphere.GetComponent(ComponentTypes.COMPONENT_SPHERE_COLLIDER) as ComponentSphereCollider;
            var lineCol = line.GetComponent(ComponentTypes.COMPONENT_LINE_COLLIDER) as ComponentLineCollider;

            spherePos += sphereCol.Offset;
            linePos += lineCol.Offset;

            var mStart = lineCol.Start + linePos;
            var mEnd = lineCol.End + linePos;

            float AnoN = Vector3.Dot(spherePos - mEnd, Vector3.Normalize(mStart - mEnd));
            Vector3 A = AnoN * Vector3.Normalize(mStart - mEnd);

            if (A.Length < (mStart - mEnd).Length && AnoN > 0)
            {
                if (((mEnd + A) - spherePos).Length < sphereCol.Radius + lineCol.Radius)
                {
                    overlap = sphereCol.Radius + lineCol.Radius - ((mEnd + A) - spherePos).Length;
                    return true;
                }
            }
            overlap = 0;
            return false;
        }

        private bool SphereSphere(Entity e, Entity e2, out float overlap)
        {
            var pos1 = (e.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;
            var pos2 = (e2.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

            var col1 = e.GetComponent(ComponentTypes.COMPONENT_SPHERE_COLLIDER) as ComponentSphereCollider;
            var col2 = e.GetComponent(ComponentTypes.COMPONENT_SPHERE_COLLIDER) as ComponentSphereCollider;

            pos1 += col1.Offset;
            pos2 += col2.Offset;

            //Fastest way to calculate the distance
            float deltaX = pos1.X - pos2.X;
            float deltaY = pos1.Y - pos2.Y;
            float deltaZ = pos1.Z - pos2.Z;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            overlap = (col1.Radius + col2.Radius) - (float)distance;

            return distance < (col1.Radius + col2.Radius);
        }

        private bool LineLine(Entity e, Entity e2, out float overlap)
        {
            overlap = 0;
            return false;
        }
    }
}
