using System;
using System.IO;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace GameEngine.Objects
{
    //Ben Mullenger
    /// <summary>
    /// struct that is used for loading shaders from config file
    /// </summary>
    public struct ShaderToLoad
    {
        public ShaderType ShaderFileType
        {
            get;
            set;
        }
        public string FilePath
        {
            get;
            set;
        }
        /// <summary>
        /// Struct used for loading shaders from config file
        /// </summary>
        /// <param name="p_typeDiscription">describes the shader type ie "Vertex"</param>
        /// <param name="p_filePath">where the actual shader is stored on disk</param>
        public ShaderToLoad(string p_typeDiscription, string p_filePath)
        {
            FilePath = p_filePath;
            switch (p_typeDiscription)
            {
                case "Vertex":
                    ShaderFileType = ShaderType.VertexShader;
                    break;
                case "Fragment":
                    ShaderFileType = ShaderType.FragmentShader;
                    break;
                case "Geometry":
                    ShaderFileType = ShaderType.GeometryShader;
                    break;
                case "GeometryExt":
                    ShaderFileType = ShaderType.GeometryShaderExt;
                    break;
                case "Computer":
                    ShaderFileType = ShaderType.ComputeShader;
                    break;
                case "TessControl":
                    ShaderFileType = ShaderType.TessControlShader;
                    break;
                case "TessElevation":
                    ShaderFileType = ShaderType.TessEvaluationShader;
                    break;
                default:
                    throw new Exception("Shader Config File Corrupt. tried to compile shader of type " + p_typeDiscription);
            }
        }
    }
    public class Shader
    {
        public int ID
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }

        private Dictionary<string, int> _uniformLocations;
        private Dictionary<string, int> _attributeLocations;

        List<int> ShaderIDs;
        /// <summary>
        /// Loads a shader to the graphics card with all the shader types passed
        /// </summary>
        /// <param name="pShaders">passed shader programs that will make up the one shader</param>
        public Shader(List<ShaderToLoad> pShaders, string pName)
        {
            Name = pName;

            _uniformLocations = new Dictionary<string, int>();
            _attributeLocations = new Dictionary<string, int>();

            StreamReader reader;

            ShaderIDs = new List<int>();

            int result;

            foreach (ShaderToLoad s in pShaders)
            {
                ShaderIDs.Add(0);
                ShaderIDs[ShaderIDs.Count - 1] = GL.CreateShader(s.ShaderFileType);
                reader = new StreamReader(s.FilePath);
                GL.ShaderSource(ShaderIDs[ShaderIDs.Count - 1], reader.ReadToEnd());
                reader.Close();
                GL.CompileShader(ShaderIDs[ShaderIDs.Count - 1]);

                GL.GetShader(ShaderIDs[ShaderIDs.Count - 1], ShaderParameter.CompileStatus, out result);
                if (result == 0)
                {
                    string name = s.ShaderFileType == ShaderType.VertexShader ? "Vertex" : "Fragment";
                    throw new Exception("Failed to compile " + name + " shader! " + GL.GetShaderInfoLog(ShaderIDs[ShaderIDs.Count - 1]));
                }
            }

            ID = GL.CreateProgram();
            foreach (int s in ShaderIDs)
            {
                GL.AttachShader(ID, s);
            }
            GL.LinkProgram(ID);

            int LinkResult = 0;

            GL.GetProgram(ID, GetProgramParameterName.LinkStatus, out LinkResult);
            if(LinkResult == 0)
            {
                throw new Exception("Failed to link shaders for " + pName + " !!!  Error::" + GL.GetProgramInfoLog(ID));
            }
        }

        public  void Use()
        {
            GL.UseProgram(ID);
        }

        /// <summary>
        /// find the location of an uniform in this shader, if it does not exist, then the method will return -1 and print an error message in the logfile
        /// </summary>
        /// <param name="pAttribName"></param>
        /// <returns></returns>
        public int GetUniformLocation(string pAttribName)
        {
            if(!_uniformLocations.ContainsKey(pAttribName))
            {
                _uniformLocations.Add(pAttribName, GL.GetUniformLocation(ID, pAttribName));
            }
            return _uniformLocations[pAttribName];
        }
        /// <summary>
        /// find the location of an Attribute in this shader, if it does not exist, then the method will return -1 and print an error message in the logfile
        /// </summary>
        /// <param name="pUniformName"></param>
        /// <returns></returns>
        public int GetAttributeLocation(string pUniformName)
        {
            if (!_attributeLocations.ContainsKey(pUniformName))
            {
                _attributeLocations.Add(pUniformName, GL.GetAttribLocation(ID, pUniformName));
            }
            return _attributeLocations[pUniformName];
        }

        public void Uniform1(string UniformName, float f)
        {
            GL.Uniform1(GetUniformLocation(UniformName), f);
        }
        public void Uniform2(string UniformName, Vector2 f)
        {
            GL.Uniform2(GetUniformLocation(UniformName), f);
        }
        public void Uniform3(string UniformName, Vector3 f)
        {

            GL.Uniform3(GetUniformLocation(UniformName), f);
        }
        public void Uniform4(string UniformName, Vector4 f)
        {
            GL.Uniform4(GetUniformLocation(UniformName), f);
        }
        public void UniformMat4(string UniformName, Matrix4 f)
        {
            GL.UniformMatrix4(GetUniformLocation(UniformName), false, ref f);
        }
        /// <summary>
        /// Cleans up the shader program off of the gpu
        /// </summary>
        public void Delete()
        {
            foreach (int s in ShaderIDs)
            {
                GL.DetachShader(ID, s);
                GL.DeleteShader(s);
            }
            GL.DeleteProgram(ID);
        }

    }
}
