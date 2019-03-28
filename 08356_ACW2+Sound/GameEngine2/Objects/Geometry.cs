using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Objects
{
    public class Geometry
    {
        List<float> vertices = new List<float>();
        int numberOfTriangles;

        // Graphics
        private int vao_Handle;
        private int vbo_verts;

        private List<Vector3> positions = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector2> texCoords = new List<Vector2>();

        public Geometry()
        {
        }

        //<C.L>
        public void LoadOBJ(string fileName)
        {
            string line = null;

            positions = new List<Vector3>();
            normals = new List<Vector3>();
            texCoords = new List<Vector2>();

            try
            {
                FileStream fin = File.OpenRead(fileName);
                StreamReader sr = new StreamReader(fin);

                GL.GenVertexArrays(1, out vao_Handle);
                GL.BindVertexArray(vao_Handle);
                GL.GenBuffers(1, out vbo_verts);

                char[] separator = new char[] { ' ', '/'};

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] values = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length != 0)
                    {
                        if (values[0] == "v")
                        {
                            positions.Add(new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
                        }
                        else if (values[0] == "vt")
                        {
                            texCoords.Add(new Vector2(float.Parse(values[1]), float.Parse(values[2])));
                        }
                        else if (values[0] == "vn")
                        {
                            normals.Add(new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
                        }
                        else if (values[0] == "f")
                        {
                            numberOfTriangles++;

                            if (values.Length > 4)
                            {
                                vertices.Add(positions[(int.Parse(values[1])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].Z);
                                vertices.Add(normals[(int.Parse(values[3])) - 1].X);
                                vertices.Add(normals[(int.Parse(values[3])) - 1].Y);
                                vertices.Add(normals[(int.Parse(values[3])) - 1].Z);
                                vertices.Add(texCoords[(int.Parse(values[2])) - 1].X);
                                vertices.Add(texCoords[(int.Parse(values[2])) - 1].Y);

                                vertices.Add(positions[(int.Parse(values[4])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[4])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[4])) - 1].Z);
                                vertices.Add(normals[(int.Parse(values[6])) - 1].X);
                                vertices.Add(normals[(int.Parse(values[6])) - 1].Y);
                                vertices.Add(normals[(int.Parse(values[6])) - 1].Z);
                                vertices.Add(texCoords[(int.Parse(values[5])) - 1].X);
                                vertices.Add(texCoords[(int.Parse(values[5])) - 1].Y);

                                vertices.Add(positions[(int.Parse(values[7])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[7])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[7])) - 1].Z);
                                vertices.Add(normals[(int.Parse(values[9])) - 1].X);
                                vertices.Add(normals[(int.Parse(values[9])) - 1].Y);
                                vertices.Add(normals[(int.Parse(values[9])) - 1].Z);
                                vertices.Add(texCoords[(int.Parse(values[8])) - 1].X);
                                vertices.Add(texCoords[(int.Parse(values[8])) - 1].Y);
                            }
                            else
                            {
                                vertices.Add(positions[(int.Parse(values[1])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].Z);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].Z);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[1])) - 1].Y);

                                vertices.Add(positions[(int.Parse(values[2])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[2])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[2])) - 1].Z);
                                vertices.Add(positions[(int.Parse(values[2])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[2])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[2])) - 1].Z);
                                vertices.Add(positions[(int.Parse(values[2])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[2])) - 1].Y);

                                vertices.Add(positions[(int.Parse(values[3])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[3])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[3])) - 1].Z);
                                vertices.Add(positions[(int.Parse(values[3])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[3])) - 1].Y);
                                vertices.Add(positions[(int.Parse(values[3])) - 1].Z);
                                vertices.Add(positions[(int.Parse(values[3])) - 1].X);
                                vertices.Add(positions[(int.Parse(values[3])) - 1].Y);
                            }
                        }
                    }
                }

                sr.Close();

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_verts);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Count * sizeof(float)), vertices.ToArray<float>(), BufferUsageHint.StaticDraw);

                // Positions
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                //Normals 
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                // Tex Coords
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

                GL.BindVertexArray(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(line);
            }
        }

        public void LoadObject(string filename)
        {
            string line;

            try
            {
                FileStream fin = File.OpenRead(filename);
                StreamReader sr = new StreamReader(fin);

                GL.GenVertexArrays(1, out vao_Handle);
                GL.BindVertexArray(vao_Handle);
                GL.GenBuffers(1, out vbo_verts);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] values = line.Split(',');

                    if (values[0].StartsWith("NUM_OF_TRIANGLES"))
                    {
                        numberOfTriangles = int.Parse(values[0].Remove(0, "NUM_OF_TRIANGLES".Length));
                        continue;
                    }
                    if (values[0].StartsWith("//") || values.Length < 8) continue;

                    vertices.Add(float.Parse(values[0]));
                    vertices.Add(float.Parse(values[1]));
                    vertices.Add(float.Parse(values[2]));
                    vertices.Add(float.Parse(values[3]));
                    vertices.Add(float.Parse(values[4]));
                    vertices.Add(float.Parse(values[5]));
                    vertices.Add(float.Parse(values[6]));
                    vertices.Add(float.Parse(values[7]));
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_verts);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Count * sizeof(float)), vertices.ToArray<float>(), BufferUsageHint.StaticDraw);

                // Positions
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                //Normals 
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                // Tex Coords
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

                GL.BindVertexArray(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Render()
        {
            GL.BindVertexArray(vao_Handle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numberOfTriangles * 3);
        }

        public  void CleanUp()
        {
            GL.BindVertexArray(vao_Handle);
            GL.DeleteBuffer(vbo_verts);
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vao_Handle);
        }
    }
}
