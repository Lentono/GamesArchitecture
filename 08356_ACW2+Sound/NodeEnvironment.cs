using System;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using GameEngine.Components;
using GameEngine.Managers;

using GameEngine.Objects;

namespace OpenGL_Game
{
    //<C.L>
    /// <summary>
    /// 
    /// </summary>
    static class NodeEnvironment
    {
        private static List<Entity> _nodes;

        private static List<List<Entity>> _axisAlignedNodeList;

        private static List<float> _wallLocations;

        /// <summary>
        /// Method that links the node system together
        /// </summary>
        /// <param name="sceneManager">Reference to the sceneManager</param>
        /// <param name="mazeDimension">xz maze dimension size</param>
        /// <param name="wallSegmentLength">distance between walls, must be modular to the maze dimension or it will around it up</param>
        public static void LinkNodeEnvironment(SceneManager sceneManager, float mazeDimension, float wallSegmentLength)
        {
            _nodes = new List<Entity>();

            _wallLocations = new List<float>();

            if (mazeDimension % wallSegmentLength != 0)
            {
                mazeDimension = ((float)Math.Round((double)mazeDimension / wallSegmentLength) * wallSegmentLength);
            }

            for (int i = 1; i <= mazeDimension / wallSegmentLength; i++)
            {
                _wallLocations.Add(i * wallSegmentLength);
            }

            //Get all nodes
            _nodes.AddRange(sceneManager.Scenes["Main"].Entities.FindAll(delegate (Entity e)
            {
                return e.Name.Contains("Node");
            }));

            for (int i = 1; i < _wallLocations.Count; i++)
            {
                GroupAxisAlignedNodes(_nodes.FindAll(delegate(Entity e)
                {
                    string[] split = e.Name.Split('/');

                    return split[0] == i.ToString();
                }));

                foreach (var AlignedNodeList in _axisAlignedNodeList)
                {
                    LinkNodes(AlignedNodeList);
                }
            }
        }

        private static void GroupAxisAlignedNodes(List<Entity> nodes)
        {
            //First environment only one door on each aligned axis so they aren't aligned and doens't matter
            _axisAlignedNodeList = new List<List<Entity>>();

            List<Entity> posXAligned = new List<Entity>();
            List<Entity> negXAligned = new List<Entity>();
            List<Entity> posZAligned = new List<Entity>();
            List<Entity> negZAligned = new List<Entity>();

            for (int i = 0; i < nodes.Count; i++)
            {
                Vector3 nodePosition = ((ComponentPosition)nodes[i].GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;

                for (int j = 0; j < nodes.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Vector3 neighbourPosition = ((ComponentPosition)nodes[j].GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;

                    Vector3 test = neighbourPosition - nodePosition;

                    //I can probably do this way better with LINQ, will have to look into this later
                    if (nodePosition.X == neighbourPosition.X)
                    {
                        if (nodePosition.X < 0)
                        {
                            //There's a possibility of adding it multiple times, so we try to remove it first
                            negXAligned.Remove(nodes[i]);
                            negXAligned.Remove(nodes[j]);

                            negXAligned.Add(nodes[i]);
                            negXAligned.Add(nodes[j]);
                        }
                        else
                        {
                            //There's a possibility of adding it multiple times, so we try to remove it first
                            posXAligned.Remove(nodes[i]);
                            posXAligned.Remove(nodes[j]);

                            posXAligned.Add(nodes[i]);
                            posXAligned.Add(nodes[j]);
                        }
                    }
                    else if (nodePosition.Z == neighbourPosition.Z)
                    {
                        if (nodePosition.Z < 0)
                        {
                            //There's a possibility of adding it multiple times, so we try to remove it first
                            negZAligned.Remove(nodes[i]);
                            negZAligned.Remove(nodes[j]);

                            negZAligned.Add(nodes[i]);
                            negZAligned.Add(nodes[j]);
                        }
                        else
                        {
                            //There's a possibility of adding it multiple times, so we try to remove it first
                            posZAligned.Remove(nodes[i]);
                            posZAligned.Remove(nodes[j]);

                            posZAligned.Add(nodes[i]);
                            posZAligned.Add(nodes[j]);
                        }
                    }
                }
            }

            _axisAlignedNodeList.Add(posXAligned);
            _axisAlignedNodeList.Add(negXAligned);
            _axisAlignedNodeList.Add(posZAligned);
            _axisAlignedNodeList.Add(negZAligned);
        }

        private static void LinkNodes(List<Entity> nodes)
        {
            Entity closestNode = null;
            List<Entity> nodesToCheck = nodes.ToList();

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                Vector3 nodePosition = ((ComponentPosition)nodes[i].GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;

                double closestDistance = double.MaxValue;

                for (int j = 0; j < nodesToCheck.Count; j++)
                {
                    //We don't want to compare it with the same node as it will become the closest distance
                    if (nodes[i] == nodesToCheck[j])
                    {
                        continue;
                    }

                    Vector3 neighbourPosition = ((ComponentPosition)nodesToCheck[j].GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;

                    //Fastest way to calculate the distance
                    float deltaX = neighbourPosition.X - nodePosition.X;
                    float deltaY = neighbourPosition.Y - nodePosition.Y;
                    float deltaZ = neighbourPosition.Z - nodePosition.Z;

                    double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                    if (distance <= closestDistance)
                    {
                        //Store the current closest distance and add the node to the list of closest nodes
                        closestDistance = distance;
                        closestNode = nodesToCheck[j];
                    }
                }

                ((ComponentNode)nodes[i].GetComponent(ComponentTypes.COMPONENT_NODE)).NodeLinks.Add(closestNode);
                ((ComponentNode)closestNode.GetComponent(ComponentTypes.COMPONENT_NODE)).NodeLinks.Add(nodes[i]);

                nodesToCheck.Remove(nodes[i]);
                closestNode = null;
            }
        }
    }
}
