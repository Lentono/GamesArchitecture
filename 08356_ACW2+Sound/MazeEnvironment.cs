using System;
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
using GameEngine.Systems;
using GameEngine.Managers;
using GameEngine.Objects;
using OpenGL_Game.Scripts;

namespace OpenGL_Game
{
    // <C.L>
    /// <summary>
    /// Creates entities to form a Maze Environment
    /// Also generates the node and populates the corridorEnvironments
    /// </summary>
    static class MazeEnvironment
    {
        public static List<Entity> _entityList;

        private static List<Tuple<int, Vector3>> _doorNodeLocations;
        private static List<float> _wallLocations;

        public static float wallSegmentLength;
        
        private static Vector3 _mazeDimensions;

        private static Random _random;

        /// <summary>
        /// Method that creates the maze
        /// </summary>
        /// <param name="sceneManager">Reference to the sceneManager</param>
        /// <param name="mazeDimension">xz maze dimension size</param>
        /// <param name="inWallSegmentLength">distance between walls, must be modular to the maze dimension or it will around it up</param>
        public static void GenerateMaze(SceneManager sceneManager, float mazeDimension, float inWallSegmentLength)
        {
            //If a maze has already been generated we remove all existing maze entities from the list before we generate another maze
            if (_entityList != null)
            {
                foreach (var entity in _entityList)
                {
                    sceneManager.RemoveFromScene(entity, "Main");
                }
            }

            _entityList = new List<Entity>();

            _doorNodeLocations = new List<Tuple<int, Vector3>>();
            _wallLocations = new List<float>();
            wallSegmentLength = inWallSegmentLength;

            if (mazeDimension % wallSegmentLength != 0)
            {
                mazeDimension = ((float)Math.Round((double)mazeDimension / wallSegmentLength) * wallSegmentLength);
            }

            _mazeDimensions = new Vector3(mazeDimension, 2.5f, mazeDimension);

            _random = new Random();

            for (int i = 1; i <= mazeDimension / wallSegmentLength; i++)
            {
                _wallLocations.Add(i * wallSegmentLength);
            }

            //Create drone tracker
            _entityList.Add(new Entity("DroneTracker"));
            _entityList.Last().AddComponent(new ComponentPosition(0.0f, 2.0f, 0.0f));
            _entityList.Last().AddComponent(new ComponentScript(new DroneTrackerScript(sceneManager, _wallLocations.Count - 1)));

            GenerateFloor();
            GenerateStartingRoom();
            GenerateCorridors();
            GenerateOuterWalls();
            GenerateNodes();

            foreach (var entity in _entityList)
            {
                sceneManager.AddToScene(entity, "Main");
            }
        }

        /// <summary>
        /// Creates the floor entity
        /// </summary>
        private static void GenerateFloor()
        {
            _entityList.Add(new Entity("Floor"));
            _entityList.Last().AddComponent(new ComponentPosition(0.0f, -0.5f, 0.0f));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacefloor.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(_mazeDimensions.X, 1.0f, _mazeDimensions.Z));
        }

        /// <summary>
        /// Create starting room
        /// </summary>
        private static void GenerateStartingRoom()
        {
            //Three main walls

            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition(0.0f, 2.5f / 2.0f, (_wallLocations[0] / 2)));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(_wallLocations[0] + 1.0f, 2.5f, 1.0f));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3((_wallLocations[0] / 2) + 0.5f, 0.0f, 0.0f), new Vector3(-((_wallLocations[0] / 2) + 0.5f), 0.0f, 0.0f)));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));
            

            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition((_wallLocations[0] / 2), 2.5f / 2.0f, 0.0f));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(1.0f, 2.5f, _wallLocations[0] + 1.0f));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(0.0f, 0.0f, (_wallLocations[0] / 2) + 0.5f), new Vector3(0.0f, 0.0f, -((_wallLocations[0] / 2) + 0.5f))));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition(-(_wallLocations[0] / 2), 2.5f / 2.0f, 0.0f));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(1.0f, 2.5f, _wallLocations[0] + 1.0f));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(0.0f, 0.0f, -((_wallLocations[0] / 2) + 0.5f)), new Vector3(0.0f, 0.0f, (_wallLocations[0] / 2) + 0.5f)));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

            //2 walls with door opening

            Vector3 DoorLeftOpeningPosition = new Vector3(-(_wallLocations[0] / 2), 2.5f / 2.0f, (-_wallLocations[0] / 2));
            Vector3 DoorRightOpeningPosition = new Vector3((_wallLocations[0] / 2), 2.5f / 2.0f, (-_wallLocations[0] / 2));

            Vector3 WallOnePosition = Vector3.Lerp(DoorLeftOpeningPosition, DoorRightOpeningPosition, 0.25f);
            Vector3 WallTwoPosition = Vector3.Lerp(DoorLeftOpeningPosition, DoorRightOpeningPosition, 0.75f);

            WallOnePosition.X = WallOnePosition.X - 0.5f;
            WallTwoPosition.X = WallTwoPosition.X + 0.5f;

            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition(WallOnePosition));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(Lerp(DoorLeftOpeningPosition.X, WallOnePosition.X, 0.5f), 2.5f, 1.0f));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3((WallOnePosition.Z - WallOnePosition.X) - 0.5f, 0.0f, 0.0f), new Vector3(-(WallOnePosition.Z - WallOnePosition.X) + 0.5f, 0.0f, 0.0f)));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition(WallTwoPosition));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(Lerp(WallTwoPosition.X, DoorRightOpeningPosition.X, 0.5f), 2.5f, 1.0f));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3((-WallTwoPosition.Z - WallTwoPosition.X) + 0.5f, 0.0f, 0.0f), new Vector3(-(-WallTwoPosition.Z - WallTwoPosition.X) - 0.5f, 0.0f, 0.0f)));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));
        }

        /// <summary>
        /// Generate all walls, doorways, doors and door nodes depending on the size of the maze
        /// </summary>
        private static void GenerateCorridors()
        {
            Dictionary<Vector3, List<Tuple<int, Vector3>>> doorNodeLocations = new Dictionary<Vector3, List<Tuple<int, Vector3>>>();
            Vector3 doorScale = new Vector3(1.0f, 2.5f, 1.0f);

            for (int i = 1; i < (_mazeDimensions.X / wallSegmentLength) - 1; i++)
            {
                //Generate door locations

                float xLocation = (_wallLocations[i] / 2);
                float yLocation = doorScale.Y / 2.0f;
                float zLocation = -(_wallLocations[i] / 2) + RandomRange(_wallLocations[i]);

                Vector3 doorLocation = new Vector3(xLocation, yLocation, zLocation);

                doorNodeLocations.Add(doorLocation, new List<Tuple<int, Vector3>>());
                doorNodeLocations[doorLocation].Add(new Tuple<int, Vector3>(i - 1, new Vector3(xLocation - (wallSegmentLength / 4.0f), yLocation, zLocation)));
                doorNodeLocations[doorLocation].Add(new Tuple<int, Vector3>(i, new Vector3(xLocation + (wallSegmentLength / 4.0f), yLocation, zLocation)));

                xLocation = -(_wallLocations[i] / 2);
                zLocation = -(_wallLocations[i] / 2) + RandomRange(_wallLocations[i]);

                doorLocation = new Vector3(xLocation, yLocation, zLocation);

                doorNodeLocations.Add(doorLocation, new List<Tuple<int, Vector3>>());
                doorNodeLocations[doorLocation].Add(new Tuple<int, Vector3>(i, new Vector3(xLocation - (wallSegmentLength / 4.0f), yLocation, zLocation)));
                doorNodeLocations[doorLocation].Add(new Tuple<int, Vector3>(i - 1, new Vector3(xLocation + (wallSegmentLength / 4.0f), yLocation, zLocation)));

                xLocation = -(_wallLocations[i] / 2) + RandomRange(_wallLocations[i]);
                zLocation = (_wallLocations[i] / 2);

                doorLocation = new Vector3(xLocation, yLocation, zLocation);

                doorNodeLocations.Add(doorLocation, new List<Tuple<int, Vector3>>());
                doorNodeLocations[doorLocation].Add(new Tuple<int, Vector3>(i - 1, new Vector3(xLocation, yLocation, zLocation - (wallSegmentLength / 4.0f))));
                doorNodeLocations[doorLocation].Add(new Tuple<int, Vector3>(i, new Vector3(xLocation, yLocation, zLocation + (wallSegmentLength / 4.0f))));

                xLocation = -(_wallLocations[i] / 2) + RandomRange(_wallLocations[i]);
                zLocation = -(_wallLocations[i] / 2);

                doorLocation = new Vector3(xLocation, yLocation, zLocation);

                doorNodeLocations.Add(doorLocation, new List<Tuple<int, Vector3>>());
                doorNodeLocations[doorLocation].Add(new Tuple<int, Vector3>(i, new Vector3(xLocation, yLocation, zLocation - (wallSegmentLength / 4.0f))));
                doorNodeLocations[doorLocation].Add(new Tuple<int, Vector3>(i - 1, new Vector3(xLocation, yLocation, zLocation + (wallSegmentLength / 4.0f))));

                //Generate corridors

                //Get all doorLocations
                List<Vector3> doorLocations = doorNodeLocations.Keys.ToList();

                for (int j = 0; j < 4; j++)
                {
                    if (j < 2)
                    {
                        float axisLoc = Lerp(-(_wallLocations[i] / 2), doorLocations[j].Z, 0.5f);

                        _entityList.Add(new Entity("Wall" + _entityList.Count));
                        _entityList.Last().AddComponent(new ComponentPosition(doorLocations[j].X, doorLocations[j].Y, axisLoc));
                        _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
                        _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
                        _entityList.Last().AddComponent(new ComponentRotation());
                        _entityList.Last().AddComponent(new ComponentScale(1.0f, 2.5f, Math.Abs((axisLoc - doorLocations[j].Z) * 2) - doorScale.Z));
                        _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(0.0f, 0.0f, (axisLoc + (_wallLocations[i] / 2)) - (doorScale.Z / 2)), new Vector3(0.0f, 0.0f, -(axisLoc + (_wallLocations[i] / 2)))));
                        _entityList.Last().AddComponent(new ComponentRigidbody(true));
                        _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

                        axisLoc = Lerp((_wallLocations[i] / 2), doorLocations[j].Z, 0.5f);

                        _entityList.Add(new Entity("Wall" + _entityList.Count));
                        _entityList.Last().AddComponent(new ComponentPosition(doorLocations[j].X, doorLocations[j].Y, axisLoc));
                        _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
                        _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
                        _entityList.Last().AddComponent(new ComponentRotation());
                        _entityList.Last().AddComponent(new ComponentScale(1.0f, 2.5f, Math.Abs((axisLoc - doorLocations[j].Z) * 2) - doorScale.Z));
                        _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(0.0f, 0.0f, (axisLoc + (-_wallLocations[i] / 2)) + (doorScale.Z / 2)), new Vector3(0.0f, 0.0f, -(axisLoc + (-_wallLocations[i] / 2)))));
                        _entityList.Last().AddComponent(new ComponentRigidbody(true));
                        _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));
                    }
                    else
                    {
                        float axisLoc = Lerp(-(_wallLocations[i] / 2), doorLocations[j].X, 0.5f);

                        _entityList.Add(new Entity("Wall" + _entityList.Count));
                        _entityList.Last().AddComponent(new ComponentPosition(axisLoc, doorLocations[j].Y, doorLocations[j].Z));
                        _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
                        _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
                        _entityList.Last().AddComponent(new ComponentRotation());
                        _entityList.Last().AddComponent(new ComponentScale(Math.Abs((axisLoc - doorLocations[j].X) * 2) - doorScale.X, 2.5f, 1.0f));
                        _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3((axisLoc + (_wallLocations[i] / 2) - (doorScale.X / 2)), 0.0f, 0.0f), new Vector3(-(axisLoc + (_wallLocations[i] / 2)), 0.0f, 0.0f)));
                        _entityList.Last().AddComponent(new ComponentRigidbody(true));
                        _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

                        axisLoc = Lerp((_wallLocations[i] / 2), doorLocations[j].X, 0.5f);

                        _entityList.Add(new Entity("Wall" + _entityList.Count));
                        _entityList.Last().AddComponent(new ComponentPosition(axisLoc, doorLocations[j].Y, doorLocations[j].Z));
                        _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
                        _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
                        _entityList.Last().AddComponent(new ComponentRotation());
                        _entityList.Last().AddComponent(new ComponentScale(Math.Abs((axisLoc - doorLocations[j].X) * 2) - doorScale.X, 2.5f, 1.0f));
                        _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3((axisLoc + (-_wallLocations[i] / 2) + (doorScale.X / 2)), 0.0f, 0.0f), new Vector3(-(axisLoc + (-_wallLocations[i] / 2)), 0.0f, 0.0f)));
                        _entityList.Last().AddComponent(new ComponentRigidbody(true));
                        _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));
                    }
                }

                //Remove random amount of doors between 2 and 3
                int doorsToRemove = _random.Next(2, 4);

                //Remove doors until we have removed the required amount
                while (doorsToRemove > 0)
                {
                    //Shuffle the dictionary
                    doorNodeLocations = doorNodeLocations.OrderBy(x => _random.Next()).ToDictionary(item => item.Key, item => item.Value);

                    //Create a door where the last node is and then remove it from the dictionary
                    _entityList.Add(new Entity("Wall" + _entityList.Count));
                    _entityList.Last().AddComponent(new ComponentPosition(doorNodeLocations.Keys.Last()));
                    _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
                    _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
                    _entityList.Last().AddComponent(new ComponentRotation());
                    _entityList.Last().AddComponent(new ComponentScale(doorScale));

                    if (_wallLocations.Contains(Math.Abs(doorNodeLocations.Keys.Last().X)))
                    {
                        _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(0.0f, 0.0f, 0.5f)));
                    }
                    else
                    {
                        _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(-0.5f, 0.0f, 0.0f), new Vector3(0.5f, 0.0f, 0.0f)));
                    }
                    _entityList.Last().AddComponent(new ComponentRigidbody(true));
                    _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

                    doorNodeLocations.Remove(doorNodeLocations.Keys.Last());

                    --doorsToRemove;
                }

                //Add nodes from left over doorways to the node listy
                foreach (var doorWays in doorNodeLocations)
                {
                    _doorNodeLocations.AddRange(doorWays.Value.ToList());
                }

                doorNodeLocations.Clear();
            }
        }

        /// <summary>
        /// Create outer wall bounds
        /// </summary>
        private static void GenerateOuterWalls()
        {
            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition(0.0f, 2.5f / 2.0f, _wallLocations.Last() / 2));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(_wallLocations.Last(), 2.5f, 1.0f));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3((_wallLocations.Last() / 2) + 0.5f, 0.0f, 0.0f), new Vector3(-((_wallLocations.Last() / 2) + 0.5f), 0.0f, 0.0f)));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition(0.0f, 2.5f / 2.0f, -_wallLocations.Last() / 2));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(_wallLocations.Last(), 2.5f, 1.0f));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(-(_wallLocations.Last() / 2) + 0.5f, 0.0f, 0.0f), new Vector3(((_wallLocations.Last() / 2) + 0.5f), 0.0f, 0.0f)));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition(_wallLocations.Last() / 2, 2.5f / 2.0f, 0.0f));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(1.0f, 2.5f, _wallLocations.Last()));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(0.0f, 0.0f, (_wallLocations.Last() / 2) + 0.5f), new Vector3(0.0f, 0.0f, -((_wallLocations.Last() / 2) + 0.5f))));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));

            _entityList.Add(new Entity("Wall" + _entityList.Count));
            _entityList.Last().AddComponent(new ComponentPosition(-_wallLocations.Last() / 2, 2.5f / 2.0f, 0.0f));
            _entityList.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
            _entityList.Last().AddComponent(new ComponentTexture("spacewall.png"));
            _entityList.Last().AddComponent(new ComponentRotation());
            _entityList.Last().AddComponent(new ComponentScale(1.0f, 2.5f, _wallLocations.Last()));
            _entityList.Last().AddComponent(new ComponentLineCollider(0.5f, new Vector3(0.0f, 0.0f, -(_wallLocations.Last() / 2) + 0.5f), new Vector3(0.0f, 0.0f, ((_wallLocations.Last() / 2) + 0.5f))));
            _entityList.Last().AddComponent(new ComponentRigidbody(true));
            _entityList.Last().AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));
        }

        /// <summary>
        /// Generate all needed corridor nodes
        /// </summary>
        private static void GenerateNodes()
        {
            //Test CorridorNodePosition

            List<Entity> corridorNodes = new List<Entity>();

            int totalCorridorNodeCount = 0;

            for (int i = 1; i < _wallLocations.Count; i++)
            {
                corridorNodes.Add(new Entity(i + "/" + "CorridorNode/" + totalCorridorNodeCount++));
                corridorNodes.Last().AddComponent(new ComponentPosition((-_wallLocations[i] + (wallSegmentLength / 2.0f)) / 2.0f, 2.5f / 2.0f, (-_wallLocations[i] + (wallSegmentLength / 2.0f)) / 2.0f));
                corridorNodes.Last().AddComponent(new ComponentSphereCollider((wallSegmentLength / 2) - (1.0f + 1.0f) /* One is doorScale */));
                corridorNodes.Last().AddComponent(new ComponentNode());
                //Visualize
                //corridorNodes.Last().AddComponent(new ComponentScale((wallSegmentLength / 2) - (1.0f + 1.0f) /* One is doorScale */));
                //corridorNodes.Last().AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
                //corridorNodes.Last().AddComponent(new ComponentRotation());
                //corridorNodes.Last().AddComponent(new ComponentTexture("spacewall.png"));

                corridorNodes.Add(new Entity(i + "/" + "CorridorNode/" + totalCorridorNodeCount++));
                corridorNodes.Last().AddComponent(new ComponentPosition((_wallLocations[i] - (wallSegmentLength / 2.0f)) / 2.0f, 2.5f / 2.0f, (_wallLocations[i] - (wallSegmentLength / 2.0f)) / 2.0f));
                corridorNodes.Last().AddComponent(new ComponentSphereCollider((wallSegmentLength / 2) - (1.0f + 1.0f) /* One is doorScale */));
                corridorNodes.Last().AddComponent(new ComponentNode());

                corridorNodes.Add(new Entity(i + "/" + "CorridorNode/" + totalCorridorNodeCount++));
                corridorNodes.Last().AddComponent(new ComponentPosition((-_wallLocations[i] + (wallSegmentLength / 2.0f)) / 2.0f, 2.5f / 2.0f, (_wallLocations[i] - (wallSegmentLength / 2.0f)) / 2.0f));
                corridorNodes.Last().AddComponent(new ComponentSphereCollider((wallSegmentLength / 2) - (1.0f + 1.0f) /* One is doorScale */));
                corridorNodes.Last().AddComponent(new ComponentNode());

                corridorNodes.Add(new Entity(i + "/" + "CorridorNode/" + totalCorridorNodeCount++));
                corridorNodes.Last().AddComponent(new ComponentPosition((_wallLocations[i] - (wallSegmentLength / 2.0f)) / 2.0f, 2.5f / 2.0f, (-_wallLocations[i] + (wallSegmentLength / 2.0f)) / 2.0f));
                corridorNodes.Last().AddComponent(new ComponentSphereCollider((wallSegmentLength / 2) - (1.0f + 1.0f) /* One is doorScale */));
                corridorNodes.Last().AddComponent(new ComponentNode());

                _entityList.AddRange(corridorNodes);

                corridorNodes.Clear();
            }

            for (int i = 0; i < _doorNodeLocations.Count; i = i + 2)
            {
                Entity node1 = new Entity((_doorNodeLocations[i].Item1+1) + "/" + "DoorNode/" + i);
                node1.AddComponent(new ComponentPosition(_doorNodeLocations[i].Item2));
                node1.AddComponent(new ComponentSphereCollider((wallSegmentLength / 2) - (1.0f + 1.0f) /* One is doorScale */));
                node1.AddComponent(new ComponentNode());

                Entity node2 = new Entity((_doorNodeLocations[i + 1].Item1+1) + "/" + "DoorNode/" + (i + 1));
                node2.AddComponent(new ComponentPosition(_doorNodeLocations[i+1].Item2));
                node2.AddComponent(new ComponentSphereCollider((wallSegmentLength / 2) - (1.0f + 1.0f) /* One is doorScale */));
                node2.AddComponent(new ComponentNode());

                //Link the door nodes together
                (node1.GetComponent(ComponentTypes.COMPONENT_NODE) as ComponentNode).NodeLinks.Add(node2);
                (node2.GetComponent(ComponentTypes.COMPONENT_NODE) as ComponentNode).NodeLinks.Add(node1);

                _entityList.Add(node1);
                _entityList.Add(node2);
            }
        }

        public static void PlaceDrones(SceneManager sceneManager)
        {
            List<Entity> corridorNodes = new List<Entity>();

            for (int i = 1; i < _wallLocations.Count; i++)
            {
                corridorNodes.AddRange(_entityList.FindAll(delegate (Entity e)
                {
                    string[] split = e.Name.Split('/');

                    return (split[0] == i.ToString() && split[1] == "CorridorNode");
                }));

                int nodeChoice = _random.Next(0, corridorNodes.Count);

                Entity Drone = new Entity("Drone" + i);
                Drone.AddComponent(new ComponentPosition(((ComponentPosition)corridorNodes[nodeChoice].GetComponent(ComponentTypes.COMPONENT_POSITION)).Position));
                Drone.AddComponent(new ComponentGeometry("Geometry/test.obj"));
                Drone.AddComponent(new ComponentTexture("drone.png"));
                Drone.AddComponent(new ComponentAudio("idle-woah", true));
                Drone.AddComponent(new ComponentRotation());
                Drone.AddComponent(new ComponentScale(0.005f, 0.005f, 0.005f));
                Drone.AddComponent(new ComponentVelocity());
                Drone.AddComponent(new ComponentSpeedModifier(1.0f));
                Drone.AddComponent(new ComponentTargetNode(corridorNodes[nodeChoice]));
                Drone.AddComponent(new ComponentScript(new DroneHealthScript(sceneManager, 3)));
                Drone.AddComponent(new ComponentScript(new EnvironmentLocationScript(_mazeDimensions.X, wallSegmentLength)));
                Drone.AddComponent(new ComponentScript(new DroneMovementScript(sceneManager, 8.0f)));
                Drone.AddComponent(new ComponentSphereCollider(0.5f));
                _entityList.Add(Drone);
                sceneManager.AddToScene(Drone, "Main");
                
                Entity newEntity = new Entity("Drone Rep" + i);
                newEntity.AddComponent(new ComponentPosition(new Vector3(0, 0, 0)));
                newEntity.AddComponent(new ComponentRotation());
                newEntity.AddComponent(new ComponentTexture("Arrow.png"));
                newEntity.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
                newEntity.AddComponent(new ComponentScale(1.0f, 1.0f, 1.0f));
                newEntity.AddComponent(new ComponentColour(1,0,0,1));
                newEntity.AddComponent(new ComponentMaterial(ResourceManager.GetMaterial("TextureUnLit")));
                newEntity.AddComponent(new ComponentScript(new MapAvitar(Drone, new Vector3((float)Math.PI / 2, -(float)Math.PI / 2, 0))));
                newEntity.AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", false));
                _entityList.Add(newEntity);
                sceneManager.AddToScene(newEntity, "Main");

                corridorNodes.Clear();
            }
        }

        public static void PlacePowerUps(SceneManager sceneManager)
        {
            List<Entity> corridorNodes = new List<Entity>();

            for (int i = 1; i < _wallLocations.Count; i++)
            {
                corridorNodes.AddRange(_entityList.FindAll(delegate (Entity e)
                {
                    string[] split = e.Name.Split('/');

                    return (split[0] == i.ToString() && split[1] == "CorridorNode");
                }));

                int pickupCount = _random.Next(1, corridorNodes.Count);

                for (int k = 0; k < pickupCount; k++)
                {
                    int pickUp = _random.Next(0, corridorNodes.Count);
                    int pickUpType = _random.Next(1, 3);

                    #region PowerUpEntities
                    Entity pickup;
                    if (pickUpType == 1)
                    {
                        pickup = new Entity("HealthTest" + i + "/" + k);
                        pickup.AddComponent(new ComponentPosition(((ComponentPosition)corridorNodes[pickUp].GetComponent(ComponentTypes.COMPONENT_POSITION)).Position));
                        pickup.AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
                        pickup.AddComponent(new ComponentTexture("Red_cross.png"));
                        pickup.AddComponent(new ComponentScale(0.3f));
                        pickup.AddComponent(new ComponentAudio("collectable-emit", true));
                        pickup.AddComponent(new ComponentRotation());
                        pickup.AddComponent(new ComponentSphereCollider(0.3f));
                        pickup.AddComponent(new ComponentScript(new PickUp(PickUp.Type.Health, 1)));
                        pickup.AddComponent(new ComponentScript(new Bob(0.1f, 100f)));
                        pickup.AddComponent(new ComponentScript(new Rotate(new Vector3(0.3f, -0.3f, 0.5f))));
                        pickup.AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));
                        _entityList.Add(pickup);
                        sceneManager.AddToScene(pickup, "Main");
                    }
                    else
                    {
                        pickup = new Entity("AmmoTest" + i + "/" + k);
                        pickup.AddComponent(new ComponentPosition(((ComponentPosition)corridorNodes[pickUp].GetComponent(ComponentTypes.COMPONENT_POSITION)).Position));
                        pickup.AddComponent(new ComponentGeometry("Geometry/CubeGeometry.txt"));
                        pickup.AddComponent(new ComponentTexture("Bullet.png"));
                        pickup.AddComponent(new ComponentScale(0.3f));
                        pickup.AddComponent(new ComponentAudio("collectable-emit", true));
                        pickup.AddComponent(new ComponentRotation());
                        pickup.AddComponent(new ComponentSphereCollider(0.3f));
                        pickup.AddComponent(new ComponentScript(new PickUp(PickUp.Type.Ammo, 5)));
                        pickup.AddComponent(new ComponentScript(new Bob(0.1f, 100f)));
                        pickup.AddComponent(new ComponentScript(new Rotate(new Vector3(0.3f, -0.3f, 0.5f))));
                        pickup.AddComponent(new ComponentRenderToFrameBuffer("MapBuffer", true));
                        _entityList.Add(pickup);
                        sceneManager.AddToScene(pickup, "Main");
                    }

                    #endregion PowerUpEntities
                }

                corridorNodes.Clear();
            }
        }

        /// <summary>
        /// Returns a random range between a value
        /// </summary>
        /// <param name="wallLength"></param>
        /// <returns></returns>
        private static float RandomRange(float wallLength)
        {
            float random = (_random.Next(2, (((int)wallLength) - 2)));
            //Add a small offset, better for future calculations
            random += (float)_random.NextDouble();

            return random;
        }

        /// <summary>
        /// Single float value linear interpolation
        /// </summary>
        /// <returns></returns>
        private static float Lerp(float start, float end, float amount)
        {
            return start + (end - start) * amount;
        }
    }
}