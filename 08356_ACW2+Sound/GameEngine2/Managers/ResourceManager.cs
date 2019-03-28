using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

using OpenTK.Graphics.OpenGL;
using GameEngine.Objects;
using OpenTK.Audio.OpenAL;
using OpenTK;

namespace GameEngine.Managers
{
    //Altered By Ben Mullenger and Callum Lenton
    static public class ResourceManager
    {
        static Dictionary<string, Geometry> geometryDictionary = new Dictionary<string, Geometry>();
        static Dictionary<string, Texture> textureDictionary = new Dictionary<string, Texture>();
        static Dictionary<string, int> textureDictionaryCubeMap = new Dictionary<string, int>();
        static Dictionary<string, Shader> shaderDictionary = new Dictionary<string, Shader>();
        static Dictionary<string, int> audioDictionary = new Dictionary<string, int>();
        static Dictionary<string, FrameBuffer> frameBufferDictionary = new Dictionary<string, FrameBuffer>();
        static Dictionary<string, Material> materialDictionary = new Dictionary<string, Material>();
        static Dictionary<string, PostProcessEffect> postProcessEffectDictionary = new Dictionary<string, PostProcessEffect>();

        //<D.M>
        public static Geometry LoadGeometry(string filename)
        {
            Geometry geometry;
            geometryDictionary.TryGetValue(filename, out geometry);
            if (geometry == null)
            {
                geometry = new Geometry();

                //If it's an obj file, we load it a different way
                if (filename.Contains(".obj"))
                {
                    geometry.LoadOBJ(filename);
                }
                else
                {
                    geometry.LoadObject(filename);
                }

                geometryDictionary.Add(filename, geometry);
            }

            return geometry;
        }

        //<B.M>
        /// <summary>
        /// Load a texture, if texture already loaded will not reload
        /// </summary>
        /// <param name="filename">File name of the texture, the textures should be located in the Textures folder</param>
        /// <returns>Texture</returns>
        public static Texture LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            if (!textureDictionary.ContainsKey(filename))
            {
                textureDictionary.Add(filename, new Texture(filename, true));
            }

            return textureDictionary[filename];
        }
        //<B.M>
        /// <summary>
        /// Generate a new empty texure, this will be managed and destroyed appropriatly
        /// </summary>
        /// <param name="name">Name for new texture</param>
        /// <returns>new Texture</returns>
        public static Texture GenEmptyTexture(int width, int height, Texture.Type type, string name, int unit = 0)
        {
            var t = new Texture(width, height, type, unit);

            textureDictionary.Add(name, t);

            return t;
        }

        //<B.M>
        /// <summary>
        /// Load a shader by passing a folder location containing all the shader programs to make up the shader.
        /// </summary>
        /// <param name="FolderLocation">Location of the folder containing all the shader programs</param>
        /// <returns>Shader, null if folderlocation could not be found or folder set up wrong</returns>
        public static Shader LoadShader(string FolderLocation)
        {
            string Name = FolderLocation;
            if (Name.StartsWith("Shaders\\"))
            {
                Name = Name.Remove(0, "Shaders\\".Length);
            }
            if (!shaderDictionary.ContainsKey(Name))
            {
                if (!Directory.Exists(FolderLocation))
                {
                    Logger.Write(LogMessageType.FatalError, "Could not file folder : " + FolderLocation);
                    return null;
                }
                var files = Directory.GetFiles(FolderLocation);

                var shaderSegments = new List<ShaderToLoad>();

                foreach (string f in files)
                {
                    string FileName = f.Substring(f.LastIndexOf("\\") + 1);
                    switch (FileName)
                    {
                        case "vs.glsl":
                            shaderSegments.Add(new ShaderToLoad() { ShaderFileType = ShaderType.VertexShader, FilePath = f });
                            break;
                        case "fs.glsl":
                            shaderSegments.Add(new ShaderToLoad() { ShaderFileType = ShaderType.FragmentShader, FilePath = f });
                            break;
                        case "gs.glsl":
                            shaderSegments.Add(new ShaderToLoad() { ShaderFileType = ShaderType.GeometryShader, FilePath = f });
                            break;
                        case "gse.glsl":
                            shaderSegments.Add(new ShaderToLoad() { ShaderFileType = ShaderType.GeometryShaderExt, FilePath = f });
                            break;
                        case "cs.glsl":
                            shaderSegments.Add(new ShaderToLoad() { ShaderFileType = ShaderType.ComputeShader, FilePath = f });
                            break;
                        case "tcs.glsl":
                            shaderSegments.Add(new ShaderToLoad() { ShaderFileType = ShaderType.TessControlShader, FilePath = f });
                            break;
                        case "tes.glsl":
                            shaderSegments.Add(new ShaderToLoad() { ShaderFileType = ShaderType.TessEvaluationShader, FilePath = f });
                            break;
                        default:
                            Logger.Write(LogMessageType.Error, "Could not load shader as contained file of incorrect name stucture, bad file: " + FileName);
                            return null;
                    }
                }
                shaderDictionary.Add(Name, new Shader(shaderSegments, Name));
            }
            return shaderDictionary[Name];
        }

        // <C.L>
        /// <summary>
        /// Load and manage audio files
        /// </summary>
        /// <param name="fileName">name of the audio file</param>
        /// <returns>Audio buffer handle</returns>
        public static int LoadAudio(string fileName)
        {
            int audioBuffer = 0, channels, bitsPerSample, sampleRate;
            byte[] audioData;
            ALFormat audioFormat;

            audioDictionary.TryGetValue(fileName, out audioBuffer);

            //If audio doesn't exist in the dictionary, we recreate it
            if (audioBuffer == 0)
            {
                // reserve a Handle for the audio file
                audioBuffer = AL.GenBuffer();

                Stream audioStream = File.Open("Audio/" + fileName + ".wav", FileMode.Open);

                if (audioStream == null)
                {
                    throw new ArgumentNullException("Audio stream is null, does the file exist? Check file name.");
                }

                //Load WAV file
                using (BinaryReader reader = new BinaryReader(audioStream))
                {
                    // RIFF header
                    string chunkID = new string(reader.ReadChars(4));

                    if (chunkID != "RIFF")
                    {
                        throw new NotSupportedException("Specified stream is not a wave file.");
                    }

                    int chunkSize = reader.ReadInt32();

                    string format = new string(reader.ReadChars(4));

                    if (format != "WAVE")
                    {
                        throw new NotSupportedException("Specified stream is not a wave file.");
                    }

                    // WAVE header
                    string fmtSubChunkID = new string(reader.ReadChars(4));

                    //Wav files can contain an extra sub chunk called "Junk", its contents are "uninteresting" so I'll just dispose of it
                    if (fmtSubChunkID == "JUNK")
                    {
                        int uselessDataSize = reader.ReadInt32();
                        reader.ReadBytes(uselessDataSize);
                        fmtSubChunkID = new string(reader.ReadChars(4));
                    }

                    if (fmtSubChunkID != "fmt ")
                    {
                        {
                            throw new NotSupportedException("Specified wave file is not supported.");
                        }
                    }

                    int fmtSubChunkSize = reader.ReadInt32();
                    int audio_format = reader.ReadInt16();
                    int num_channels = reader.ReadInt16();
                    int sample_rate = reader.ReadInt32();
                    int byte_rate = reader.ReadInt32();
                    int block_align = reader.ReadInt16();
                    int bits_per_sample = reader.ReadInt16();

                    Debug.WriteLine("format chunk size: " + fmtSubChunkSize);
                    Debug.WriteLine("audio format: " + audio_format);
                    Debug.WriteLine("num channels: " + num_channels);
                    Debug.WriteLine("sample rate: " + sample_rate);
                    Debug.WriteLine("byte rate: " + byte_rate);
                    Debug.WriteLine("block align: " + block_align);
                    Debug.WriteLine("bits per sample: " + bits_per_sample);

                    if(fmtSubChunkSize == 18)
                    {
                        int fmtExtraSize = reader.ReadInt16();
                        reader.ReadChars(fmtExtraSize);
                    }

                    string dataSubChunkID = new string(reader.ReadChars(4));

                    //Wrong wav format, contains extra data (LIST INFO)
                    if (dataSubChunkID == "LIST")
                    {
                        int uselessDataSize = reader.ReadInt32();

                        reader.ReadBytes(uselessDataSize);

                        dataSubChunkID = new string(reader.ReadChars(4));
                    }

                    //Useless potential extra data from other wav formats
                    if (dataSubChunkID == "fact")
                    {
                        int uselessDataSize = reader.ReadInt32();
                        reader.ReadBytes(uselessDataSize);
                        dataSubChunkID = new string(reader.ReadChars(4));
                    }

                    //Useless potential extra data from other wav formats
                    if (dataSubChunkID == "bext")
                    {
                        int uselessDataSize = reader.ReadInt32();
                        reader.ReadBytes(uselessDataSize);
                        dataSubChunkID = new string(reader.ReadChars(4));
                    }

                    //Useless potential extra data from other wav formats
                    if (dataSubChunkID == "acid")
                    {
                        int uselessDataSize = reader.ReadInt32();
                        reader.ReadBytes(uselessDataSize);
                        dataSubChunkID = new string(reader.ReadChars(4));
                    }

                    if (dataSubChunkID != "data")
                    {
                        throw new NotSupportedException("Specified wave file is not supported.");
                    }

                    int dataSubChunkSize = reader.ReadInt32();

                    channels = num_channels;
                    bitsPerSample = bits_per_sample;
                    sampleRate = sample_rate;

                    audioData = reader.ReadBytes((int)reader.BaseStream.Length);
                }

                audioFormat =
                channels == 1 && bitsPerSample == 8 ? ALFormat.Mono8 :
                channels == 1 && bitsPerSample == 16 ? ALFormat.Mono16 :
                channels == 2 && bitsPerSample == 8 ? ALFormat.Stereo8 :
                channels == 2 && bitsPerSample == 16 ? ALFormat.Stereo16 :
                (ALFormat)0; // unknown

                AL.BufferData(audioBuffer, audioFormat, audioData, audioData.Length, sampleRate);

                //Throw exception if it errors, if not we add the audio to the dictionary
                if (AL.GetError() != ALError.NoError)
                {
                    // respond to load error etc.
                    throw new Exception("Failed to load audio");
                }
                else
                {
                    audioDictionary.Add(fileName, audioBuffer);
                }
            }

            return audioBuffer;
        }

        //<B.M>
        public static Material GetMaterial(string v)
        {
            return materialDictionary.ContainsKey(v) ? materialDictionary[v] : null;
        }

        //<B.M>
        /// <summary>
        /// Generate a new frame buffer, this will manage the memory automatically
        /// </summary>
        /// <param name="frameBufferName">Name that it will be stored as</param>
        /// <param name="texture">the texture it will output to</param>
        /// <returns>new frame buffer object</returns>
        public static FrameBuffer GenFrameBuffer(string frameBufferName, string textureName, int width, int height, Matrix4 perspective, Matrix4 orthographic, Matrix4 view, Vector4 clearColor, int unit = 0)
        {
            if (frameBufferDictionary.ContainsKey(frameBufferName))
            {
                Logger.Write(LogMessageType.FatalError, "Tried to make a frame buffer with the name of a preexisting frame buffer");
            }
            if (!textureDictionary.ContainsKey(textureName))
            {
                GenEmptyTexture(width, height, Texture.Type.Colour, textureName, unit);
            }
            if (!textureDictionary.ContainsKey(textureName + "Depth"))
            {
                GenEmptyTexture(width, height, Texture.Type.Depth, textureName + "Depth", unit);
            }
            var texture = textureDictionary[textureName];
            var depthTexture = textureDictionary[textureName + "Depth"];
            var f = new FrameBuffer(frameBufferName, texture, depthTexture, perspective, orthographic, view, clearColor);
            frameBufferDictionary.Add(frameBufferName, f);
            return f;
        }

        //<B.M>
        /// <summary>
        /// Generate a new material
        /// </summary>
        /// <param name="materialName">Name for material</param>
        /// <param name="shaderName">Name of shader it will use</param>
        /// <param name="prepRender">The method that will extract data from an entity for rendering</param>
        /// <returns>New Material</returns>
        public static Material GenMaterial(string materialName, string shaderName, Material.EntityShaderMethod prepRender)
            => GenMaterial(materialName, shaderDictionary[shaderName], prepRender);

        //<B.M>
        /// <summary>
        /// Generate a new material
        /// </summary>
        /// <param name="materialName">Name for material</param>
        /// <param name="shader">Shader it will use</param>
        /// <param name="prepRender">The method that will extract data from an entity for rendering</param>
        /// <returns>New Material</returns>
        public static Material GenMaterial(string materialName, Shader shader, Material.EntityShaderMethod prepRender)
        {
            var m = new Material(shader, prepRender);

            materialDictionary.Add(materialName, m);

            return m;
        }

        //<B.M>
        public static PostProcessEffect GenPostProcessEffect(string effectName, string shaderName, PostProcessEffect.TextureShaderMethod prepRender)
            => GenPostProcessEffect(effectName, shaderDictionary[shaderName], prepRender);

        //<B.M>
        public static PostProcessEffect GenPostProcessEffect(string effectName, Shader shader, PostProcessEffect.TextureShaderMethod textureShaderMethod)
        {
            var ppe = new PostProcessEffect(shader, textureShaderMethod);

            postProcessEffectDictionary.Add(effectName, ppe);

            return ppe;
        }

        //<B.M>
        public static Dictionary<string, FrameBuffer> GetFrameBuffers() => frameBufferDictionary;

        //<B.M>
        public static Dictionary<string, PostProcessEffect> GetPostProccessEffects() => postProcessEffectDictionary;

        /// <summary>
        /// Cleans Up all resources
        /// </summary>
        public static void CleanUp()
        {
            foreach (var t in textureDictionary)
            {
                t.Value.Delete();
            }
            textureDictionary.Clear();
            foreach (var s in shaderDictionary)
            {
                s.Value.Delete();
            }
            shaderDictionary.Clear();
            foreach (var g in geometryDictionary)
            {
                g.Value.CleanUp();
            }
            geometryDictionary.Clear();
            foreach (var a in audioDictionary)
            {
                AL.DeleteBuffer(a.Value);
            }
            audioDictionary.Clear();
            foreach (var f in frameBufferDictionary)
            {
                f.Value.CleanUp();
            }
            frameBufferDictionary.Clear();
            materialDictionary.Clear();

        }
    
        //public static int LoadCubeMap(List<string> cubemap)
        //{
            //foreach (string s in cubemap)
            //{
            //    if (string.IsNullOrEmpty(s))
            //    {
            //        Console.WriteLine("Loading Texture Fail");
            //    }
            //}

            //string name = "Skybox";
            //Texture texture;
            //textureDictionary.TryGetValue(name, out texture);

            //if (texture == null)
            //{
            //    texture = GL.GenTexture();
            //    GL.BindTexture(TextureTarget.TextureCubeMap, texture);

            //    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            //    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            //    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            //    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);


            //    for (int i = 0; i < cubemap.Count; i++)
            //    {
            //        Bitmap bmp = new Bitmap(cubemap[i]);
            //        BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, Pixelpublic Format.Rgba, bmp_data.Width, bmp_data.Height, 0,
            //            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);


            //        bmp.UnlockBits(bmp_data);
            //    }

            //    //ADD TO DICTIONARY
            //    textureDictionary.Add(name, texture);
            //}

            //return texture;
        
    }
}
