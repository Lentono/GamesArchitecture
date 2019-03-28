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
    /// 
    /// </summary>
    class DroneMovementScript : Script
    {
        //Mask
        private const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_SPEEDMOD | ComponentTypes.COMPONENT_TARGET | ComponentTypes.COMPONENT_VELOCITY | ComponentTypes.COMPONENT_ROTATION | ComponentTypes.COMPONENT_SCRIPT | ComponentTypes.COMPONENT_AUDIO);

        /// <summary>
        /// Drone State Types
        /// </summary>
        public enum DroneStateTypes
        {
            Idle,
            Aggressive,
            Dead,
            Disabled
        }

        //Drone state
        public DroneStateTypes droneState;

        SceneManager _sceneManager;

        private Entity _player;
        private List<Entity> _nodePath;

        private List<Vector3> _previousPositions;

        private Random _random;

        private float _viewRange;

        private float _timeSinceTrigger;

        private bool _targettingPlayer = false;
        private bool _targettingNodePath = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sceneManager">To access the node system</param>
        public DroneMovementScript(SceneManager sceneManager, float viewRange) : base()
        {
            _sceneManager = sceneManager;

            droneState = DroneStateTypes.Idle;

            _nodePath = new List<Entity>();

            _previousPositions = new List<Vector3>();

            _random = new Random();

            _viewRange = viewRange;

            _timeSinceTrigger = 0.0f;
        }

        public override void OnSceneAdded()
        {
            InputManager.AddKeyboardTrigger(OpenTK.Input.Key.D, ButtonPosition.PressedOnce, () => droneState = droneState == DroneStateTypes.Disabled ? DroneStateTypes.Idle : DroneStateTypes.Disabled);
            base.OnSceneAdded();
        }

        public override void OnSceneRemoved()
        {

            InputManager.RemoveKeyboardTrigger(OpenTK.Input.Key.D, ButtonPosition.PressedOnce, () => droneState = droneState == DroneStateTypes.Disabled ? DroneStateTypes.Idle : DroneStateTypes.Disabled);
            base.OnSceneRemoved();
        }

        public void AddEntityToPath(Entity node)
        {
            if (!_nodePath.Contains(node))
            {
                Debug.WriteLine("Added Node");
                _nodePath.Add(node);
            }
        }

        public override void OnUpdate(float pDelta)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                ComponentTargetNode targetNode = entity.GetComponent(ComponentTypes.COMPONENT_TARGET) as ComponentTargetNode;
                ComponentPosition position = entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition;
                ComponentRotation rotation = entity.GetComponent(ComponentTypes.COMPONENT_ROTATION) as ComponentRotation;
                ComponentVelocity velocity = entity.GetComponent(ComponentTypes.COMPONENT_VELOCITY) as ComponentVelocity;
                ComponentSpeedModifier speedMod = entity.GetComponent(ComponentTypes.COMPONENT_SPEEDMOD) as ComponentSpeedModifier;
                ComponentAudio audio = entity.GetComponent(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;

                EnvironmentLocationScript droneLocation = null;

                List<IComponent> scripts = entity.GetComponents(ComponentTypes.COMPONENT_SCRIPT);

                foreach (ComponentScript script in scripts)
                {
                    if (script.script is EnvironmentLocationScript)
                    {
                        droneLocation = script.script as EnvironmentLocationScript;
                    }
                }

                if (droneState == DroneStateTypes.Idle)
                {
                    IdleDroneLogic(targetNode, position, rotation, velocity, speedMod, audio, droneLocation, pDelta);
                }
                else if (droneState == DroneStateTypes.Aggressive)
                {
                    AggressiveDroneLogic(targetNode, position, rotation, velocity, speedMod, audio, droneLocation, pDelta);
                }
            }
            else
            {
                throw new Exception("Drone doesn't have all required components");
            }
        }

        private void IdleDroneLogic(ComponentTargetNode targetNode, ComponentPosition inPos, ComponentRotation inRot, ComponentVelocity inVel, ComponentSpeedModifier inSpeed, ComponentAudio inAudio, EnvironmentLocationScript inLoc, float inDelta)
        {
            Vector3 targetPosition = (targetNode.TargetNode.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

            if (_player != null)
            {
                Vector3 playerPosition = (_player.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;
                EnvironmentLocationScript playerLocation = null;

                List<IComponent> scripts = _player.GetComponents(ComponentTypes.COMPONENT_SCRIPT);

                foreach (ComponentScript script in scripts)
                {
                    if (script.script is EnvironmentLocationScript)
                    {
                        playerLocation = script.script as EnvironmentLocationScript;
                        continue;
                    }
                }

                //Fastest way to calculate the distance
                float deltaX = playerPosition.X - inPos.Position.X;
                float deltaY = playerPosition.Y - inPos.Position.Y;
                float deltaZ = playerPosition.Z - inPos.Position.Z;

                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                //If the distance between the player and drone is less than 5 then we change state
                if (distance < _viewRange && inLoc.EnvironmentLocation == playerLocation.EnvironmentLocation && CanDroneSeeTarget(inPos.Position, targetPosition, playerPosition))
                {
                    //Other logic (speed mod change)
                    inSpeed.SpeedMod = 3.0f;

                    //Change drone state to aggressive
                    droneState = DroneStateTypes.Aggressive;

                    entity.AddComponent(new ComponentRigidbody());

                    //Change sound to the trigger sound
                    inAudio.SetAudioBuffer("trigger-woah", false);

                    //Change vaporwave post process effects active variable to true
                    ResourceManager.GetPostProccessEffects()["AttackShake"].Active = true;

                    _targettingPlayer = true;
                }
                else if (targetPosition == inPos.Position)
                {
                    //If we've reached our target, update to a new target
                    FindNewTarget(targetNode, inPos, inVel, inRot);
                }

                //Update position
                Vector3 travelDistance = ((inVel.Velocity * inSpeed.SpeedMod) * inDelta);

                //Fastest way to calculate the distance
                deltaX = targetPosition.X - inPos.Position.X;
                deltaY = targetPosition.Y - inPos.Position.Y;
                deltaZ = targetPosition.Z - inPos.Position.Z;

                double distanceToNode = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                if (travelDistance.Length > distanceToNode)
                {
                    inPos.Position = targetPosition;
                }
                else
                {
                    inPos.Position += travelDistance;
                }

                _previousPositions.Add(inPos.Position);

                if (_previousPositions.Count > 6)
                {
                    _previousPositions.Remove(_previousPositions.First());

                    if (CheckIfStuck())
                    {
                        //Since we can't see the player anymore, we'll find the nearest node in our environment to follow
                        targetNode.TargetNode = FindNearestEnvironmentNode(inPos, inLoc, "CorridorNode");
                        targetPosition = (targetNode.TargetNode.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

                        //Other logic (speed mod change, reset velocity to target node (temp))
                        inVel.Velocity = -Vector3.Normalize(inPos.Position - targetPosition);

                        //Calculate new rotation
                        inRot.Rotation = LookAt(inPos.Position, targetPosition);
                    }
                }
            }
            else
            {
                _player = _sceneManager.Scenes["Main"].Entities.Find(delegate (Entity e)
                {
                    return e.Name == "Camera1";
                });
            }
        }

        /// <summary>
        /// Checks if the drone is stuck, if true we recalculate velocity
        /// </summary>
        /// <returns></returns>
        private bool CheckIfStuck()
        {
            if (_previousPositions.Count == 6)
            {
                double totalDistance = 0.0f;

                for (int i = 0; i < _previousPositions.Count; i = i + 2)
                {
                    double deltaX = _previousPositions[i].X - _previousPositions[i+1].X;
                    double deltaY = _previousPositions[i].Y - _previousPositions[i+1].Y;
                    double deltaZ = _previousPositions[i].Z - _previousPositions[i+1].Z;

                    totalDistance += Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
                }

                if (totalDistance < 0.02f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Decide on a new target node to reach (neighbour)
        /// </summary>
        private void FindNewTarget(ComponentTargetNode targetNode, ComponentPosition inPos, ComponentVelocity inVel, ComponentRotation inRot)
        {
            //Get target nodes neighbouring nodes
            List<Entity> neighbouringNodes = (targetNode.TargetNode.GetComponent(ComponentTypes.COMPONENT_NODE) as ComponentNode).NodeLinks;

            //Choose new target option
            int nodeChoice = _random.Next(0, neighbouringNodes.Count);

            //Set target option
            targetNode.TargetNode = neighbouringNodes[nodeChoice];

            //Calculate new velocity
            Vector3 position = inPos.Position;
            Vector3 targetPosition = (targetNode.TargetNode.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

            inVel.Velocity = -Vector3.Normalize(position - targetPosition);

            //Calculate new rotation

            inRot.Rotation = LookAt(position, targetPosition);
        }

        private void AggressiveDroneLogic(ComponentTargetNode targetNode, ComponentPosition inPos, ComponentRotation inRot, ComponentVelocity inVel, ComponentSpeedModifier inSpeed, ComponentAudio inAudio, EnvironmentLocationScript inLoc, float inDelta)
        {
            Vector3 playerPosition = (_player.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;
            EnvironmentLocationScript playerLocation = null;

            List<IComponent> scripts = _player.GetComponents(ComponentTypes.COMPONENT_SCRIPT);

            foreach (ComponentScript script in scripts)
            {
                if (script.script is EnvironmentLocationScript)
                {
                    playerLocation = script.script as EnvironmentLocationScript;
                }
            }

            _timeSinceTrigger += inDelta;

            //Once the trigger sound has finished, we changed to the angry sound
            if (_timeSinceTrigger > 1.1f && _timeSinceTrigger < 1.2f)
            {
                inAudio.SetAudioBuffer("angry-woah", true);
            }

            //Fastest way to calculate the distance
            float deltaX = playerPosition.X - inPos.Position.X;
            float deltaY = playerPosition.Y - inPos.Position.Y;
            float deltaZ = playerPosition.Z - inPos.Position.Z;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            Vector3 newPPos = new Vector3(playerPosition.X, inPos.Position.Y, playerPosition.Z);

            //Calculate new rotation
            inRot.Rotation = LookAt(inPos.Position, newPPos);

            if (_targettingPlayer)
            {
                inVel.Velocity = -Vector3.Normalize(inPos.Position - playerPosition);

                //Update position
                inPos.Position += ((inVel.Velocity * inSpeed.SpeedMod) * inDelta);

                if (_nodePath.Count != 0)
                {
                    _targettingPlayer = false;
                    _targettingNodePath = true;

                    targetNode.TargetNode = _nodePath.First();
                }
            }
            else if (_targettingNodePath)
            {
                Vector3 targetPosition = (targetNode.TargetNode.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

                inVel.Velocity = -Vector3.Normalize(inPos.Position - targetPosition);

                //Update position
                inPos.Position += ((inVel.Velocity * inSpeed.SpeedMod) * inDelta);

                _previousPositions.Add(inPos.Position);

                if (_previousPositions.Count > 6)
                {
                    _previousPositions.Remove(_previousPositions.First());

                    if (CheckIfStuck())
                    {
                        //Recalculate velocity
                        inVel.Velocity = -Vector3.Normalize(inPos.Position - targetPosition);
                    }
                }

                //Distance the drone is going to travel
                Vector3 travelDistance = ((inVel.Velocity * inSpeed.SpeedMod) * inDelta);

                //Fastest way to calculate the distance
                deltaX = targetPosition.X - inPos.Position.X;
                deltaY = targetPosition.Y - inPos.Position.Y;
                deltaZ = targetPosition.Z - inPos.Position.Z;

                double distanceToNode = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                if (travelDistance.Length > distanceToNode)
                {
                    inPos.Position = targetPosition;
                }
                else
                {
                    inPos.Position += travelDistance;
                }

                //If drone reached temporary node position then we target the player again
                if (targetPosition == inPos.Position)
                {
                    //If we've reached our target, target player again
                    _nodePath.Remove(_nodePath.First());

                    if (_nodePath.Count != 0)
                    {
                        targetNode.TargetNode = _nodePath.First();
                    }
                    else
                    {
                        _targettingPlayer = true;
                        _targettingNodePath = false;
                    }
                }
            }
            //If we're targetting the player, we will see if there are any nodes between myself and the player and target the nodes instead
            //if (_targettingPlayerAgg)
            //{
            //    inVel.Velocity = -Vector3.Normalize(inPos.Position - playerPosition);

            //    //Update position
            //    inPos.Position = inPos.Position + ((inVel.Velocity * inSpeed.SpeedMod) * inDelta);

            //    Tuple<bool, Entity> closestNode = NodeCloserThanPlayer(inPos.Position, newPPos, inLoc);

            //    if (closestNode.Item1)
            //    {
            //        _targettingPlayerAgg = false;
            //        _targettingNodeAgg = true;

            //        targetNode.TargetNode = closestNode.Item2;

            //        Vector3 targetPosition = (targetNode.TargetNode.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

            //        inVel.Velocity = -Vector3.Normalize(inPos.Position - targetPosition);
            //    }
            //}

            //If we found nodes between the drone and the player, we will target the node and then retarget the player
            //if (_targettingNodeAgg)
            //{
            //    //Update position
            //    inPos.Position = inPos.Position + ((inVel.Velocity * inSpeed.SpeedMod) * inDelta);

            //    Vector3 targetPosition = (targetNode.TargetNode.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

            //    _previousPositions.Add(inPos.Position);

            //    if (_previousPositions.Count > 6)
            //    {
            //        _previousPositions.Remove(_previousPositions.First());

            //        if (CheckIfStuck())
            //        {
            //            //Recalculate velocity
            //            inVel.Velocity = -Vector3.Normalize(inPos.Position - targetPosition);
            //        }
            //    }

            //    //Update position
            //    Vector3 travelDistance = ((inVel.Velocity * inSpeed.SpeedMod) * inDelta);

            //    //Fastest way to calculate the distance
            //    deltaX = targetPosition.X - inPos.Position.X;
            //    deltaY = targetPosition.Y - inPos.Position.Y;
            //    deltaZ = targetPosition.Z - inPos.Position.Z;

            //    double distanceToNode = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            //    if (travelDistance.Length > distanceToNode)
            //    {
            //        inPos.Position = targetPosition;
            //    }
            //    else
            //    {
            //        inPos.Position += travelDistance;
            //    }

            //    //If drone reached temporary node position then we target the player again
            //    if (targetPosition == inPos.Position)
            //    {
            //        //If we've reached our target, target player again
            //        _targettingNodeAgg = false;
            //        _targettingPlayerAgg = true;
            //    }
            //}

            //If the distance between the player and drone is greater than the range then we change state, or if they are not in the same environment (can't be seen)
            if (distance > _viewRange && _nodePath.Count == 0 || playerLocation.EnvironmentLocation <= 0)
            {
                //Since we can't see the player anymore, we'll find the nearest node in our environment to follow
                targetNode.TargetNode = FindNearestEnvironmentNode(inPos, inLoc);
                Vector3 targetPosition = (targetNode.TargetNode.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

                //Other logic (speed mod change, reset velocity to target node (temp))
                inSpeed.SpeedMod = 1.0f;
                inVel.Velocity = -Vector3.Normalize(inPos.Position - targetPosition);

                //Calculate new rotation
                inRot.Rotation = LookAt(inPos.Position, targetPosition);

                //Change drone state to idle
                droneState = DroneStateTypes.Idle;

                entity.RemoveComponent(ComponentTypes.COMPONENT_RIGIDBODY);

                _timeSinceTrigger = 0.0f;
                inAudio.SetAudioBuffer("idle-woah", true);

                _targettingPlayer = false;
                _targettingNodePath = false;

                //Change vaporwave post process effects active variable to false
                ResourceManager.GetPostProccessEffects()["AttackShake"].Active = false;
            }
        }

        /// <summary>
        /// Checks if there is any nodes between it and the player, if so, that's the new target
        /// </summary>
        /// <param name="dronePosition">drones position</param>
        /// <param name="playerPosition">player position</param>
        /// <returns>true if there is a node between it and the player</returns>
        //private Tuple<bool, Entity> NodeCloserThanPlayer(Vector3 dronePosition, Vector3 playerPosition, EnvironmentLocationScript inLoc)
        //{
        //    int currentLocation = inLoc.EnvironmentLocation;

        //    List<Entity> environmentNodes = new List<Entity>();

        //    //Find all nodes in current environment
        //    environmentNodes.AddRange(_sceneManager.Scenes["Main"].Entities.FindAll(delegate (Entity e)
        //    {
        //        string[] split = e.Name.Split('/');

        //        return (split[0] == currentLocation.ToString() && split[1].Contains("Node"));
        //    }));

        //    List<Entity> NodesInPlayerDirection = new List<Entity>();

        //    //Find all nodes in the same direction as the player
        //    foreach (var node in environmentNodes)
        //    {
        //        Vector3 nodePosition = (node.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

        //        if (CanDroneSeeTarget(dronePosition, playerPosition, nodePosition))
        //        {
        //            NodesInPlayerDirection.Add(node);
        //        }
        //    }

        //    //Fastest way to calculate the distance
        //    //Distance to player
        //    float deltaX = playerPosition.X - dronePosition.X;
        //    float deltaY = playerPosition.Y - dronePosition.Y;
        //    float deltaZ = playerPosition.Z - dronePosition.Z;

        //    double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

        //    //If any of the nodes are closer than the player, return true and the node that is the closest
        //    Entity closestNode = null;

        //    foreach (var node in NodesInPlayerDirection)
        //    {
        //        Vector3 nodePosition = (node.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

        //        //Fastest way to calculate the distance
        //        //Distance to node
        //        deltaX = nodePosition.X - dronePosition.X;
        //        deltaY = nodePosition.Y - dronePosition.Y;
        //        deltaZ = nodePosition.Z - dronePosition.Z;

        //        double nodeDistance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

        //        if (nodeDistance < distance)
        //        {
        //            closestNode = node;
        //            distance = nodeDistance;
        //        }
        //    }

        //    if (closestNode != null)
        //    {
        //        return new Tuple<bool, Entity>(true, closestNode);
        //    }
        //    else
        //    {
        //        return new Tuple<bool, Entity>(false, closestNode);
        //    }
        //}

        /// <summary>
        /// Checks if the player is in front of the drone
        /// </summary>
        /// <returns> true or false</returns>
        private bool CanDroneSeeTarget(Vector3 dronePosition, Vector3 droneTargetPosition, Vector3 otherTargetPosition)
        {
            Vector3 droneDirection = Vector3.Normalize(dronePosition - droneTargetPosition);
            Vector3 directionToPlayer = Vector3.Normalize(dronePosition - otherTargetPosition);

            List<float> directionList = new List<float>
            {
                (float)Math.Round(droneDirection.X, 1),
                (float)Math.Round(directionToPlayer.X, 1),
                (float)Math.Round(droneDirection.Z, 1),
                (float)Math.Round(directionToPlayer.Z, 1)
            };

            for (int i = 0; i < directionList.Count; i++)
            {
                if (directionList[i] == 0.1f || directionList[i] == -0.1f)
                {
                    directionList[i] = 0.0f;
                }
            }

            if (new Vector2(directionList[0] - directionList[1], directionList[2] - directionList[3]) == Vector2.Zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Finds the nearest node in the environment and returns it for the new target
        /// </summary>
        /// <returns>Closest node in the environment</returns>
        private Entity FindNearestEnvironmentNode(ComponentPosition inPos, EnvironmentLocationScript inLoc, string name = "Node")
        {
            int currentLocation = inLoc.EnvironmentLocation;

            List<Entity> environmentNodes = new List<Entity>();

            //Find all nodes in current environment
            environmentNodes.AddRange(_sceneManager.Scenes["Main"].Entities.FindAll(delegate (Entity e)
            {
                string[] split = e.Name.Split('/');

                return (split[0] == currentLocation.ToString() && split[1].Contains(name));
            }));

            Entity closestNode = null;

            double closestDistance = double.MaxValue;

            //Find closest node to me
            foreach (var node in environmentNodes)
            {
                Vector3 nodePosition = ((ComponentPosition)node.GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;

                //Fastest way to calculate the distance
                float deltaX = nodePosition.X - inPos.Position.X;
                float deltaY = nodePosition.Y - inPos.Position.Y;
                float deltaZ = nodePosition.Z - inPos.Position.Z;

                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }

            //Return it to be used as new target
            return closestNode;
        }

        /// <summary>
        /// Creates a rotation vector that is looking at the target
        /// </summary>
        /// <param name="position">current object position</param>
        /// <param name="targetPosition">targets position</param>
        /// <returns>rotation vector towards target</returns>
        private Vector3 LookAt(Vector3 position, Vector3 targetPosition)
        {
            Vector3 forward = Vector3.Normalize(targetPosition - position);

            float dot = Vector3.Dot(Vector3.UnitZ, forward);

            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            {
                return Vector3.UnitY * (float)Math.PI;
            }

            if (Math.Abs(dot - (1.0f)) < 0.000001f)
            {
                return new Vector3(0.0f, 0.0f, 0.0f) * 1.0f;
            }

            float angle = (float)Math.Acos(dot);
            Vector3 axis = Vector3.Cross(Vector3.UnitZ, forward);
            axis = Vector3.Normalize(axis);

            return axis * angle;
        }
    }
}
