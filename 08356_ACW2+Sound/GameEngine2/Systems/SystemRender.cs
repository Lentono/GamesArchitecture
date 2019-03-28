using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GameEngine.Components;
using GameEngine.Objects;
using GameEngine.Managers;
using System.IO;
using System;

namespace GameEngine.Systems
{
    //Heavily Altered By Ben Mullenger
    public class SystemRender : ISystem
    {
        const ComponentTypes MASK_3D = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_TEXTURE | ComponentTypes.COMPONENT_ROTATION | ComponentTypes.COMPONENT_SCALE);
        const ComponentTypes MASK_TEXT = (ComponentTypes.COMPONENT_TEXT | ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_ROTATION | ComponentTypes.COMPONENT_SCALE);
        private GameWindow MyGame;
        private ViewProjectionManager _viewProjectionManager;
        private List<Entity> _entities;
        public Dictionary<string, List<Entity>> frameBufferRenders;
        private Matrix4 _persp;
        private Matrix4 _ortho;
        private Matrix4 _view;
        private Random _random;

        public SystemRender(GameWindow myGame, ViewProjectionManager viewProjectionManager)
        {
            MyGame = myGame;
            _viewProjectionManager = viewProjectionManager;
            _entities = new List<Entity>();
            frameBufferRenders = new Dictionary<string, List<Entity>>();

            var shaders = Directory.GetDirectories("Shaders");
            foreach (var s in shaders)
            {
                ResourceManager.LoadShader(s);
            }

            var p = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), MyGame.Width / MyGame.Height, 0.01f, 100f);
            var o = Matrix4.CreateOrthographicOffCenter(0, MyGame.Width, 0, MyGame.Height, -1000f, 1000f);

            ResourceManager.GenFrameBuffer("Master", "MasterTexture", MyGame.Width, MyGame.Height, p, o, Matrix4.Identity, new Vector4(0.1f, 0, 0.1f, 1));
            ResourceManager.GenPostProcessEffect("Last", "Last", (t, s) => t.BindTextureUniform(s, "s_texture"));

            _random = new Random();
            ResourceManager.GenPostProcessEffect("Blur", "Blur", (t, s) => { t.BindTextureUniform(s, "s_texture"); s.Uniform1("pixelSize", 0.0005f); }); 
            var shake = ResourceManager.GenPostProcessEffect("AttackShake", "Offset", (t, s) => 
            {
                t.BindTextureUniform(s, "s_texture");
                s.Uniform2("offset", new Vector2(0.01f * (float)_random.NextDouble(), 0.01f * (float)_random.NextDouble()));
            });
            shake.Active = false;
            ResourceManager.GenPostProcessEffect("Offset", "Offset", (t, s) => { t.BindTextureUniform(s, "s_texture"); s.Uniform2("offset", new Vector2(0.001f * (float)_random.NextDouble(), 0.001f * (float)_random.NextDouble())); });
            ResourceManager.GenPostProcessEffect("Colour", "Colour", (t, s) => { t.BindTextureUniform(s, "s_texture"); s.Uniform2("delta", new Vector2(0.001f, 0.001f)); });

            for (int i = 1; i < ResourceManager.GetPostProccessEffects().Count + 1; i++)
            {
                ResourceManager.GenFrameBuffer("Master" + i, "MasterTexture" + i, MyGame.Width, MyGame.Height, p, o, Matrix4.Identity, new Vector4(0.1f, 0, 0.1f, 1));
            }

            var DefaultLitMater = ResourceManager.GenMaterial("TextureLit", "Texture", (e, s) =>
            {
                DefaultTexturePrep(e, s);
                s.Uniform3("EyePos", _viewProjectionManager.View.ExtractTranslation());
                s.Uniform3("LightPos", new Vector3(0, 5, 0));
            });
            var DefaultUnlitMater = ResourceManager.GenMaterial("TextureUnLit", "TextureUnLit", (e, s) => DefaultTexturePrep(e, s));

            var DefaultLitTextMater = ResourceManager.GenMaterial("TextLit", "Text", (e, s) =>
            {
                DefaultTextPrep(e, s);
                s.Uniform3("EyePos", _viewProjectionManager.View.ExtractTranslation());
                s.Uniform3("LightPos", new Vector3(0, 5, 0));
            });

            var DefaultUnlitText = ResourceManager.GenMaterial("TextUnLit", "TextUnLit", (e, s) => DefaultTextPrep(e, s));
        }

        public string Name
        {
            get { return "SystemRender"; }
        }

        public void OnNewEntity(Entity entity)
        {
            if (entity.HasMask(ComponentTypes.COMPONENT_RENDER_TO_FRAME_BUFFER))
            {
                var fList = entity.GetComponents(ComponentTypes.COMPONENT_RENDER_TO_FRAME_BUFFER);
                bool renderToMain = false;
                foreach (var fL in fList)
                {
                    var f = fL as ComponentRenderToFrameBuffer;
                    if (!frameBufferRenders.ContainsKey(f.BufferName))
                    {
                        frameBufferRenders.Add(f.BufferName, new List<Entity>());
                    }
                    frameBufferRenders[f.BufferName].Add(entity);
                    renderToMain = f.RenderToMainBuffer;
                }
                if (!renderToMain)
                {
                    return;
                }
            }
            if (entity.HasMask(MASK_3D) || entity.HasMask(MASK_TEXT))
            {
                _entities.Add(entity);
            }
        }

        public void OnUpdate() { }

        public void OnRender()
        {
            foreach (var fb in ResourceManager.GetFrameBuffers())
            {
                if (!frameBufferRenders.ContainsKey(fb.Key))
                {
                    continue;
                }

                fb.Value.Bind();

                GL.ClearColor(fb.Value.ClearColour.X, fb.Value.ClearColour.Y, fb.Value.ClearColour.Z, fb.Value.ClearColour.W);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                _view = fb.Value.View;
                _persp = fb.Value.Perspective;
                _ortho = fb.Value.Orthographic;

                foreach (var e in frameBufferRenders[fb.Key])
                {
                    SendToRender(e);
                }

                fb.Value.Unbind();
            }

            var frame = ResourceManager.GetFrameBuffers()["Master"];
            frame.Bind();

            GL.ClearColor(frame.ClearColour.X, frame.ClearColour.Y, frame.ClearColour.Z, frame.ClearColour.W);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _persp = _viewProjectionManager.Perspective;
            _ortho = _viewProjectionManager.Orthographic;
            _view = _viewProjectionManager.View;

            foreach (var e in _entities)
            {
                SendToRender(e);
            }

            var geometry = ResourceManager.LoadGeometry("Geometry/SquareGeometry.txt");

            var tex = ResourceManager.LoadTexture("MasterTexture");
            int count = 1;
            foreach (var pps in ResourceManager.GetPostProccessEffects())
            {
                if (pps.Key == "Last" || !pps.Value.Active)
                {
                    continue;
                }
                frame = ResourceManager.GetFrameBuffers()["Master" + count];
                frame.Bind();
                pps.Value.PrepRender(tex);
                geometry.Render();
                GL.BindVertexArray(0);
                GL.UseProgram(0);
                frame.Unbind();
                tex = ResourceManager.LoadTexture("MasterTexture" + count);
                count++;
            }
            frame.Unbind();
            //tex = ResourceManager.LoadTexture("MasterTexture2");
            ResourceManager.GetPostProccessEffects()["Last"].PrepRender(tex);
            geometry.Render();
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        private void SendToRender(Entity e)
        {
            Material material;
            bool isText = e.HasMask(ComponentTypes.COMPONENT_TEXT);
            bool isUI = e.HasMask(ComponentTypes.COMPONENT_UI);
            if (e.GetComponent(ComponentTypes.COMPONENT_MATERIAL) is ComponentMaterial materialComp)
            {
                material = materialComp.material;
                material.PrepRender(e);
            }
            else
            {
                string MatName = isText ? isUI ? "TextUnLit" : "TextLit" : isUI ? "TextureUnLit" : "TextureLit";
                material = ResourceManager.GetMaterial(MatName);
                material.PrepRender(e);
            }
            if (isText)
            {
                DrawText(e, material.Shader);
                return;
            }
            var geom = (e.GetComponent(ComponentTypes.COMPONENT_GEOMETRY) as ComponentGeometry);
            geom.Geometry().Render();
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        private void DefaultTextPrep(Entity e, Shader s)
        {
            ExtractDefaultValues(e, out Vector3 scale, out Vector3 rotation, out Vector3 position, out Vector4 colour, out Matrix4 proj, out Matrix4 view);
            var textComp = ((ComponentText)e.GetComponent(ComponentTypes.COMPONENT_TEXT));

            var geometry = ResourceManager.LoadGeometry("Geometry/SquareGeometry.txt");

            textComp.Font.BindTextureUniform(s, "s_texture");

            s.UniformMat4("Proj", proj);
            s.UniformMat4("View", view);
            s.Uniform4("uColour", colour);
            s.Uniform2("uTexScale", new Vector2(1 / 16.0f, 1 / 14.0f));
        }

        private void DefaultTexturePrep(Entity e, Shader s)
        {
            ExtractDefaultValues(e, out Vector3 scale, out Vector3 rotation, out Vector3 position, out Vector4 colour, out Matrix4 proj, out Matrix4 view);
            DefaultExtract3D(e, scale, rotation, position, out Matrix4 uModel, out Texture texture);

            texture.BindTextureUniform(s, "s_texture");

            s.UniformMat4("Proj", proj);
            s.UniformMat4("View", view);
            s.UniformMat4("uModel", uModel);
            s.Uniform4("uColour", colour);
        }

        private void DefaultExtract3D(Entity e, Vector3 scale, Vector3 rotation, Vector3 position, out Matrix4 uModel, out Texture texture)
        {
            uModel = Matrix4.CreateScale(scale) * Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.CreateTranslation(position);
            texture = ((ComponentTexture)e.GetComponent(ComponentTypes.COMPONENT_TEXTURE)).Texture;
        }

        private void ExtractDefaultValues(Entity e, out Vector3 scale, out Vector3 rotation, out Vector3 position, out Vector4 Colour, out Matrix4 proj, out Matrix4 view)
        {
            scale = ((ComponentScale)e.GetComponent(ComponentTypes.COMPONENT_SCALE)).Scale;
            rotation = ((ComponentRotation)e.GetComponent(ComponentTypes.COMPONENT_ROTATION)).Rotation;
            position = ((ComponentPosition)e.GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;
            var colComp = ((ComponentColour)e.GetComponent(ComponentTypes.COMPONENT_COLOUR));

            Colour = colComp == null ? Vector4.One : colComp.Colour;
            bool UI = e.HasMask(ComponentTypes.COMPONENT_UI);
            proj = UI ? _ortho : _persp;
            view = UI ? Matrix4.Identity : _view;
            if (UI)
            {
                position.Y = (MyGame.Height) - position.Y;
                GL.DepthFunc(DepthFunction.Always);
            }
            else
            {
                GL.DepthFunc(DepthFunction.Lequal);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
            if(entity.HasMask(ComponentTypes.COMPONENT_RENDER_TO_FRAME_BUFFER))
            {                
                foreach(var f in frameBufferRenders)
                {
                    f.Value.Remove(entity);
                }
            }
        }

        public void DrawText(Entity e, Shader shader)
        {
            bool isUI = e.HasMask(ComponentTypes.COMPONENT_UI);

            var text = (e.GetComponent(ComponentTypes.COMPONENT_TEXT) as ComponentText);

            var position = (e.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;
            position.Y = isUI ? (MyGame.Height) - position.Y : position.Y;

            var scale = (e.GetComponent(ComponentTypes.COMPONENT_SCALE) as ComponentScale).Scale;

            //Corrects the shape if the width and height is different
            var correctionMat = isUI ? Matrix4.CreateScale(new Vector3(1, (MyGame.Width / (float)MyGame.Height), 1)) : Matrix4.Identity;

            var geometry = ResourceManager.LoadGeometry("Geometry/SquareGeometry.txt");

            var Text = text.Text;

            for (int i = 0; i < Text.Length; i++)
            {
                shader.Uniform2("uTexOffset", text.CharOffsets[i]);

                Matrix4 uModel = correctionMat * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(position + new Vector3(2 * i * scale.X, 0, 0));
                shader.UniformMat4("uModel", uModel);

                geometry.Render();
            }
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        // MJB  - Render a Skybox 
        public void DrawSkyBox()
        {
            List<string> cubeMap = new List<string>();
            //1 = down left = 2, top = 3, bottom = 4, back = 5, front = 6
            cubeMap.Add("Textures/craterlake_dn.tga");
            cubeMap.Add("Textures/craterlake_lf.tga");
            cubeMap.Add("Textures/craterlake_up.tga");
            cubeMap.Add("Textures/craterlake_rt.tga");
            cubeMap.Add("Textures/craterlake_bk.tga");
            cubeMap.Add("Textures/craterlake_ft.tga");

            Entity newEntity;
            newEntity = new Entity("Skybox");
            newEntity.AddComponent(new ComponentPosition(new Vector3(0f, 0f, 0f)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SkyboxGeometry.txt"));
            newEntity.AddComponent(new ComponentTexture(cubeMap));
        }
    }
}
