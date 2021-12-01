using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrivialHxGLTF;
using Microsoft.Xna.Framework.Input;
using GLTFFile = HxGLTF.GLTFFile;
using GLTFLoader = HxGLTF.GLTFLoader;
using Texture = Microsoft.Xna.Framework.Graphics.Texture;

namespace TestRendering
{
    public class TrivialGame : Game
    {

        public static GameWindow GameWindow;
        
        private RenderTarget2D _dsScreen;
        
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        
        private Matrix world, view, proj;
        private Effect _effect;

        private Camera _camera;
        
        private Texture2D _testTexture2D;
        private Texture2D _ass;
        private Texture2D _tits;
        private Texture2D _face;

        private GLTFModel _model;
        private GLTFModel _helm;
        private List<GLTFModel> _chars = new List<GLTFModel>();
        private GLTFModel _sky;

        private bool _controlLight = false;
        
        /*private GLTFFile _gltfFile;
        private List<VertexPositionTexture> _vertexPositionsTextures = new List<VertexPositionTexture>();
        private int _primitiveCount = 0;
        private readonly List<Mesh> _meshes = new List<Mesh>();
        private Dictionary<string, Texture2D> _loadedTextures = new Dictionary<string, Texture2D>();*/

        public TrivialGame()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

            _graphicsDeviceManager.PreferredBackBufferHeight = 720;
            _graphicsDeviceManager.PreferredBackBufferWidth = 1280;
            
            _graphicsDeviceManager.ApplyChanges();

            GameWindow = Window;
        }
        
        

        protected override void Initialize()
        {

           // _dsScreen = new RenderTarget2D(GraphicsDevice, 256, 192, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _dsScreen = new RenderTarget2D(GraphicsDevice, 1280, 720, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _camera = new Camera(GraphicsDevice);
            
            //var gltfFile = GLTFLoader.Load(@"C:\Users\asame\Documents\ModelExporter\black2\output_assets\map06_20\map06_20.glb");
            //gltfFile = GLTFLoader.Load(@"C:\Users\asame\Documents\ModelExporter\black2\output_assets\map13_03\map13_03.glb");
            //gltfFile = GLTFLoader.Load(@"C:\Users\asame\Documents\ModelExporter\Platin\output_assets\map05_21c\map05_21c.gltf");
            //var gltfFile = GLTFLoader.Load(@"C:\Users\asame\Documents\ModelExporter\Platin\output_assets\m_comp03_00_00c\m_comp03_00_00c.gltf");
            //gltfFile = GLTFLoader.Load(@"C:\Users\asame\Documents\ModelExporter\Platin\output_assets\ball\ball.gltf");
            //gltfFile = GLTFLoader.Load(@"C:\Users\asame\Documents\ModelExporter\Platin\output_assets\ashiato\ashiato.gltf");
            //gltfFile = GLTFLoader.Load(@"C:\Users\asame\Documents\ModelExporter\black2\output_assets\ak_00\ak_00.glb");
            //_gltfFile = GLTFLoader.Load(@"G:\Opera GX - Downloads\glTF\Sponza.gltf");
            //gltfFile = GLTFLoader.Load(@"G:\Opera GX - Downloads\glTF\avocado\Avocado.gltf");
            //gltfFile = GLTFLoader.Load(@"C:\Users\asame\Documents\ModelExporter\Platin\output_assets\psel_all\psel_all.gltf");

            /*Console.WriteLine(_gltfFile.Asset.Version);
            var scene = _gltfFile.Scene;

            foreach (var image in _gltfFile.Images)
            {
                if (Path.IsPathRooted(image.Uri))
                {
                    _loadedTextures.Add(image.Uri, LoadFormFile(image.Uri));
                }
                else
                {
                    var combinedPath = Path.Combine(Path.GetDirectoryName(_gltfFile.Path) ?? string.Empty, image.Uri);
                    _loadedTextures.Add(image.Uri, LoadFormFile(combinedPath));
                }
            }

            foreach (var node in _gltfFile.Scenes[scene].Nodes)
            {
                var i = _gltfFile.Nodes[node].Mesh;
                if (!i.HasValue) continue;

                var mesh = new Mesh();
                foreach (var primitive in _gltfFile.Meshes[i.Value].Primitives)
                {
                    var isIndexed = primitive.Indices.HasValue;
                    Accessor indexAccessor = null;
                    if (isIndexed)
                    {
                        indexAccessor = _gltfFile.Accessors[primitive.Indices.Value];
                    }

                    var primitives = new Primitives();
                    foreach (var (key, value) in primitive.Attributes)
                    {
                        var dataAccessor = _gltfFile.Accessors[value];

                        primitives.Dictionary.Add(key,
                            isIndexed
                                ? _gltfFile.ReadAccessorIndexed(dataAccessor, indexAccessor)
                                : _gltfFile.ReadAccessor(dataAccessor));
                    }

                    var matIndex = primitive.Material ?? -1;
                    if (matIndex != -1 && _gltfFile.Materials[matIndex].PbrMetallicRoughness.BaseColorTexture != null)
                    {
                        var textureIndex = _gltfFile.Materials[matIndex].PbrMetallicRoughness.BaseColorTexture.Index;
                        var imageIndex = _gltfFile.Textures[textureIndex].Source;
                        primitives.Material = new Material() { Uri = _gltfFile.Images[imageIndex].Uri };
                    }
                    
                    primitives.Init(GraphicsDevice);
                    mesh.PrimitivesList.Add(primitives);
                }
                _meshes.Add(mesh);
            }*/
            base.Initialize(); //memory!!!
        }

        protected override void LoadContent()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _effect = Content.Load<Effect>("VP");
            _testTexture2D = Content.Load<Texture2D>("wayfair2");

            //_model = GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\glTF\Sponza.gltf"); //memory!!!
            //_model.Scale = 0.5f;
            //_char = GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\original_anime_girls\scene.gltf"); //memory!!!
            //_char = GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\cradle_tower\scene.gltf"); //memory!!!
            _model = GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\duck\Duck.gltf"); //memory!!!
            _chars.Add(GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\aura__fortnite_item_shop_skin\scene.gltf")); //memory!!!
            _chars.Last().Scale = 0.25f;
            _chars.Add(GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\banana_plant\scene.gltf"));
            _chars.Last().Position.Y =  4;
            _chars.Add(GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\spirit_blossom_kindred\scene.gltf"));
            //_char = GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\banana_plant\scene.gltf"); //memory!!!
            
            //_char = GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\spirit_blossom_kindred\scene.gltf"); //memory!!!
            
            //_char = GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\lost_robot\scene.gltf"); //memory!!
            
            _sky = GLTFModel.LoadFromFile(GraphicsDevice, @"G:\Opera GX - Downloads\anime_starry_night\scene.gltf");
            _sky.Scale = 0.5f;
            _model.Scale = 0.25f;
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            _controlLight = Keyboard.GetState().IsKeyDown(Keys.R);
            
            _effect.Parameters["DeltaTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            _effect.Parameters["Unlit"].SetValue(Keyboard.GetState().IsKeyUp(Keys.X));
            _effect.Parameters["CameraPosition"].SetValue(_camera.Position);
            
            var direction = new Vector3();
            
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                direction += Vector3.Forward;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                direction += Vector3.Left;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                direction += Vector3.Right;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                direction += Vector3.Backward;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                direction -= Vector3.Down;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                direction -= Vector3.Up;
            }

            _camera.Move(-direction * (float)gameTime.ElapsedGameTime.TotalSeconds);
            
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _camera.RotateX(-1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _camera.RotateX(1);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _camera.RotateY(1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _camera.RotateY(-1);
            }
            
            _camera.Update(gameTime);

            world = Matrix.CreateTranslation(Vector3.Zero);// * Matrix.CreateScale();
            view = _camera.View;
            proj  = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000000f);
            proj = _camera.Projection;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Vector3 forward = Vector3.Transform(_camera.Forward, _camera.RotationMXY);
            //_sky.Position = _camera.Position + forward * 2;
            //_sky.Rotation = _camera.Rotation;

            _sky.Position = _camera.Position;
            _effect.Parameters["LightPosition"].SetValue(_model.Position);

            if (_controlLight)
            {
                _model.Position = _camera.Position;
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.SetRenderTarget(_dsScreen);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var c in _chars)
            {
                GLTFModel.Draw(GraphicsDevice, _effect, c, world, view, proj);
            }
            
            GLTFModel.Draw(GraphicsDevice, _effect, _model, world, view, proj);
            GLTFModel.Draw(GraphicsDevice, _effect, _sky, world, view, proj);
            
            /*foreach (var mesh in _meshes)
            {
                foreach (var prim in mesh.PrimitivesList)
                {
                    GraphicsDevice.SetVertexBuffer(prim.VertexBuffer);
                    foreach (var currentTechniquePass in _effect.CurrentTechnique.Passes)
                    {
                        _effect.Parameters["WorldViewProjection"].SetValue(world * view * proj);
                        _effect.Parameters["UseTexture"].SetValue(true);
                        _effect.Parameters["Texture"].SetValue(_loadedTextures[prim.Material.Uri]);
                        //_effect.Parameters["Texture"].SetValue(_testTexture2D);
                        currentTechniquePass.Apply();
                        GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, prim.VertexBuffer.VertexCount / 3);
                    }
                }
            }*/
            
            //GraphicsDevice.SetRenderTarget(null);
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            //_spriteBatch.Draw(_dsScreen, GraphicsDevice.Viewport.Bounds, Color.White);
            //_spriteBatch.End();
            base.Draw(gameTime);
        }
    }

}