using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using OpenTK.Graphics.OpenGL;

using GameEngine.Managers;

namespace GameEngine.Objects
{
    //Ben Mullenger
    public class Texture
    {
        public enum Type
        {
            Colour,
            Depth,
            Stencil
        }
        public static readonly string TEXTURE_DIRECTORY = Directory.GetCurrentDirectory() + "/Textures/";

        private Bitmap _bitmap;
        private int _textureLocation;
        public int TextureLocation
        {
            get
            {
                return _textureLocation;
            }
        }
        private int _unit;
        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }

        public void SetUnit(int i)
        {
            _unit = i;
        }

        /// <summary>
        /// Make a new empty texture
        /// </summary>
        /// <param name="pUnit">default unit the texture should use to render</param>
        public Texture(int width, int height, Type type, int pUnit)
        {
            _unit = pUnit;
            //MakeNewTexture(_unit);
            if (type == Type.Colour)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + _unit);
                GL.Enable(EnableCap.Texture2D);
            }
            GL.GenTextures(1, out _textureLocation);
            GL.BindTexture(TextureTarget.Texture2D, _textureLocation);
            switch (type)
            {
                case Type.Colour:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)null);
                    break;
                case Type.Depth:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.UnsignedInt, (IntPtr)null);
                    break;
                case Type.Stencil:
                    throw new NotImplementedException();
                    //break;
                default:
                    break;
            }
            SetTexParams();
        }

        /// <summary>
        /// Loads a texture from a bitmap
        /// </summary>
        /// <param name="p_bitmap">bitmap to load textrue from</param>
        /// <param name="pUnit">default unit the texture should use to render</param>
        public Texture(Bitmap pBitmap, int pUnit = 0)
        {
            if (pBitmap != null)
                LoadBitMapData(pBitmap, pUnit);
            _bitmap = pBitmap;
            _unit = pUnit;
        }
        /// <summary>
        /// Make a new texture from a file
        /// </summary>
        /// <param name="fileName">name of the file that is stored in the Assets/Textures folder</param>
        /// <param name="pShaderID">the shader that will use this texture</param>
        /// <param name="p_unit">the unit location that this texture will be used in when beig used</param>
        public Texture(string fileName, bool p_useTextureDirectory, int p_width = 0, int p_height = 0, int p_unit = 0)
        {
            string filePath = p_useTextureDirectory ? TEXTURE_DIRECTORY + fileName : fileName;
            if (File.Exists(filePath))
            {
                Bitmap textureBitMap = new Bitmap(filePath);

                LoadBitMapData(textureBitMap, p_unit, p_height, p_width);
            }
            else
            {
                Logger.Write(LogMessageType.FatalError, "Could not find texture called " + filePath + " make sure that it is being copied into the directory.");
                throw new Exception("Could not find texture called " + filePath + " make sure that it is being copied into the directory.");
            }
        }
        /// <summary>
        /// Loads the bitmap onto the graphics card.
        /// </summary>
        /// <param name="p_bitmap">bitmap to load</param>
        /// <param name="p_Unit">unit it should use as default</param>
        private void LoadBitMapData(Bitmap p_bitmap, int p_Unit, int p_width = 0, int p_height = 0)
        {
            //p_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData TextureData = p_bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, p_bitmap.Width, p_bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            MakeNewTexture(p_Unit);

            p_width = p_width == 0 ? TextureData.Width : p_width;
            p_height = p_height == 0 ? TextureData.Height : p_height;

            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, p_width,
                p_height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte,
                TextureData.Scan0);

            SetTexParams();

            p_bitmap.UnlockBits(TextureData);

            Width = p_width;
            Height = p_height;
        }

        private void MakeNewTexture(int pUnit)
        {

            GL.ActiveTexture(TextureUnit.Texture0 + pUnit);
            GL.Enable(EnableCap.Texture2D);
            GL.GenTextures(1, out _textureLocation);
            GL.BindTexture(TextureTarget.Texture2D, _textureLocation);

            _unit = pUnit;
        }

        public void SaveBitmap(string p_fileName)
        {
            _bitmap?.Save(p_fileName);
        }

        private void SetTexParams()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }
        /// <summary>
        /// Bind the texture to a shader at a a specific attribut location
        /// </summary>
        /// <param name="pShader">shader the texture will be bound to</param>
        /// <param name="pUniformName">the attribute the texture will be stored in</param>
        public void BindTextureUniform(Shader pShader, string pUniformName)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + _unit);
            GL.BindTexture(TextureTarget.Texture2D, _textureLocation);
            pShader.Uniform1(pUniformName, 0);
        }

        public  void Delete()
        {
            GL.DeleteTexture(_textureLocation);
        }
    }
}
