using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrivialHxGLTF;

namespace TestRendering
{
    public class GLTFModel
    {
        private GLTFFile _gltfFile;
        private readonly List<Mesh> _meshes = new List<Mesh>();
        private readonly Dictionary<string, Texture2D> _loadedTextures = new Dictionary<string, Texture2D>();
        public float Scale = 1f;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;

        private static readonly RasterizerState CullNone = new RasterizerState()
        {
            CullMode = CullMode.None
        };

        private static readonly RasterizerState CullClockWise = new RasterizerState()
        {
            CullMode = CullMode.CullClockwiseFace
        };

        public static void Draw(GraphicsDevice graphicsDevice, Effect effect, GLTFModel model, Matrix world, Matrix view, Matrix proj)
        {
            
            foreach (var mesh in model._meshes)
            {
                Matrix l_world = world;
                Matrix l_view = view;
                Matrix l_proj = proj;

                Matrix n_matrix = new Matrix(
                    new Vector4(mesh.Node.Matrix[0], mesh.Node.Matrix[4], mesh.Node.Matrix[8],mesh.Node.Matrix[12]),
                    new Vector4(mesh.Node.Matrix[1], mesh.Node.Matrix[5], mesh.Node.Matrix[9],mesh.Node.Matrix[13]),
                    new Vector4(mesh.Node.Matrix[2], mesh.Node.Matrix[6], mesh.Node.Matrix[10],mesh.Node.Matrix[14]),
                    new Vector4(mesh.Node.Matrix[3], mesh.Node.Matrix[7], mesh.Node.Matrix[11],mesh.Node.Matrix[15])
                );
                
                n_matrix = new Matrix(
                    new Vector4(mesh.Node.Matrix[0], mesh.Node.Matrix[1], mesh.Node.Matrix[2],mesh.Node.Matrix[3]),
                    new Vector4(mesh.Node.Matrix[4], mesh.Node.Matrix[5], mesh.Node.Matrix[6],mesh.Node.Matrix[7]),
                    new Vector4(mesh.Node.Matrix[8], mesh.Node.Matrix[9], mesh.Node.Matrix[10],mesh.Node.Matrix[11]),
                    new Vector4(mesh.Node.Matrix[12], mesh.Node.Matrix[13], mesh.Node.Matrix[14],mesh.Node.Matrix[15])
                );

                
                Matrix n_scale = Matrix.CreateScale(
                    new Vector3(mesh.Node.Scale[0], mesh.Node.Scale[1], mesh.Node.Scale[2])
                    );
                
                Matrix n_rotation = Matrix.CreateFromQuaternion(
                    new Quaternion(
                        new Vector4(mesh.Node.Rotation[0], mesh.Node.Rotation[1], mesh.Node.Rotation[2], mesh.Node.Rotation[3])
                        )
                    );
                
                Matrix n_translation = Matrix.CreateTranslation(
                    new Vector3(mesh.Node.Translation[0], mesh.Node.Translation[1], mesh.Node.Translation[2])
                    );
                
                l_world *= n_matrix;
                l_world *= n_translation * n_rotation *  n_scale;

                l_world *= Matrix.CreateScale(model.Scale) * (Matrix.CreateRotationX(model.Rotation.X) * Matrix.CreateRotationY(model.Rotation.Y) * Matrix.CreateRotationZ(model.Rotation.Z)) * Matrix.CreateTranslation(model.Position);

                var r = graphicsDevice.RasterizerState;

                foreach (var prim in mesh.PrimitivesList)
                {
                    graphicsDevice.RasterizerState = prim.Material.DoubleSided ? CullNone : CullClockWise;                    
                    graphicsDevice.SetVertexBuffer(prim.VertexBuffer);
                    foreach (var currentTechniquePass in effect.CurrentTechnique.Passes)
                    {
                        
                        effect.Parameters["WorldViewProjection"].SetValue(l_world * l_view * l_proj);
                        effect.Parameters["World"].SetValue(l_world);
                        effect.Parameters["View"].SetValue(l_view);
                        effect.Parameters["Projection"].SetValue(l_proj);
                        effect.Parameters["UseTexture"].SetValue(false);
                        effect.Parameters["Unlit"].SetValue(false);
                        effect.Parameters["Toon"].SetValue(false);
                        if (prim.Material.Extensions.Count >= 1)
                        {
                            if (prim.Material.Extensions.Contains("KHR_materials_unlit"))
                            {
                                effect.Parameters["Unlit"].SetValue(true);
                            }
                            if (prim.Material.Extensions.Contains("H073_materials_toon"))
                            {
                                effect.Parameters["Toon"].SetValue(true);
                            }
                        }
                        
                        effect.Parameters["BaseColorFactor"].SetValue(prim.Material.BaseColorFactor.ToVector4());
                        
                        if (!string.IsNullOrEmpty(prim.Material.Uri))
                        {
                            effect.Parameters["UseTexture"].SetValue(true);
                            effect.Parameters["Texture"].SetValue(model._loadedTextures[prim.Material.Uri]);
                        }
                        //_effect.Parameters["Texture"].SetValue(_testTexture2D);
                        currentTechniquePass.Apply();
                        graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, prim.VertexBuffer.VertexCount / 3);
                    }
                }
                graphicsDevice.RasterizerState = r;
            }
        }
        
        public static GLTFModel LoadFromFile(GraphicsDevice graphicsDevice, string path)
        {
            var model = new GLTFModel();
            model._gltfFile = GLTFLoader.Load(path);
            Console.WriteLine(model._gltfFile.Asset.Version);
            var scene = model._gltfFile.Scene;

            foreach (var image in model._gltfFile.Images)
            {
                if (Path.IsPathRooted(image.Uri))
                {
                    model._loadedTextures.Add(image.Uri, LoadFormFile(graphicsDevice, image.Uri));
                }
                else
                {
                    var combinedPath = Path.Combine(Path.GetDirectoryName(model._gltfFile.Path) ?? string.Empty, image.Uri);
                    model._loadedTextures.Add(image.Uri, LoadFormFile(graphicsDevice, combinedPath)); //memory!!!
                }
            }

            //foreach (var nodePointer in model._gltfFile.Scenes[scene].Nodes)
            //{
                //var i = model._gltfFile.Nodes[nodePointer].Mesh;
                //if (!i.HasValue)continue;
                
            foreach (var node in model._gltfFile.Nodes)
            {
                var children = node.Children;
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        var childNode = model._gltfFile.Nodes[child];
                        
                        if (node.Rotation != null)
                        {
                            Quaternion parentR = new Quaternion(node.Rotation[0], node.Rotation[1], node.Rotation[2],
                                node.Rotation[3]);
                            
                            Quaternion childR = new Quaternion(childNode.Rotation[0], childNode.Rotation[1], childNode.Rotation[2],
                                childNode.Rotation[3]);

                            childR *= parentR;

                            childNode.Rotation[0] = childR.X;
                            childNode.Rotation[1] = childR.Y;
                            childNode.Rotation[2] = childR.Z;
                            childNode.Rotation[3] = childR.W;
                        }
                        if (node.Translation != null)
                        {
                            for (var x = 0; x < node.Translation.Length; x++)
                            {
                                childNode.Translation[x] *= node.Translation[x];
                            }
                        }
                        if (node.Scale != null)
                        {
                            for (var x = 0; x < node.Scale.Length; x++)
                            {
                                childNode.Scale[x] *= node.Scale[x];
                            }
                        }
                        if (node.Matrix != null)
                        {
                            
                            /*Matrix parentM = new Matrix(
                                new Vector4(node.Matrix[0], node.Matrix[1], node.Matrix[2],node.Matrix[3]),
                                new Vector4(node.Matrix[4], node.Matrix[5], node.Matrix[6],node.Matrix[7]),
                                new Vector4(node.Matrix[8], node.Matrix[9], node.Matrix[10],node.Matrix[11]),
                                new Vector4(node.Matrix[12], node.Matrix[13], node.Matrix[14],node.Matrix[15])
                            );
                            
                            Matrix childM = new Matrix(
                                new Vector4(childNode.Matrix[0], childNode.Matrix[1], childNode.Matrix[2],childNode.Matrix[3]),
                                new Vector4(childNode.Matrix[4], childNode.Matrix[5], childNode.Matrix[6],childNode.Matrix[7]),
                                new Vector4(childNode.Matrix[8], childNode.Matrix[9], childNode.Matrix[10],childNode.Matrix[11]),
                                new Vector4(childNode.Matrix[12], childNode.Matrix[13], childNode.Matrix[14],childNode.Matrix[15])
                            );*/
                            
                            Matrix parentM = new Matrix(
                                new Vector4(node.Matrix[0], node.Matrix[4], node.Matrix[8],node.Matrix[12]),
                                new Vector4(node.Matrix[1], node.Matrix[5], node.Matrix[9],node.Matrix[13]),
                                new Vector4(node.Matrix[2], node.Matrix[6], node.Matrix[10],node.Matrix[14]),
                                new Vector4(node.Matrix[3], node.Matrix[7], node.Matrix[11],node.Matrix[15])
                            );
                            
                            Matrix childM = new Matrix(
                                new Vector4(childNode.Matrix[0], childNode.Matrix[4], childNode.Matrix[8],childNode.Matrix[12]),
                                new Vector4(childNode.Matrix[1], childNode.Matrix[5], childNode.Matrix[9],childNode.Matrix[13]),
                                new Vector4(childNode.Matrix[2], childNode.Matrix[6], childNode.Matrix[10],childNode.Matrix[14]),
                                new Vector4(childNode.Matrix[3], childNode.Matrix[7], childNode.Matrix[11],childNode.Matrix[15])
                            );

                            childM = Matrix.Multiply(parentM, childM);

                            var childS = Vector3.Zero;
                            var childT = Vector3.Zero;
                            var childR = Quaternion.Identity;
                            
                            childM.Decompose(out childS, out childR, out childT); 
                            
                            /*childNode.Rotation[0] = childR.X;
                            childNode.Rotation[1] = childR.Y;
                            childNode.Rotation[2] = childR.Z;
                            childNode.Rotation[3] = childR.W;
                            
                            childNode.Translation[0] = childT.X;
                            childNode.Translation[1] = childT.Y;
                            childNode.Translation[2] = childT.Z;
                            
                            childNode.Scale[0] = childS.X;
                            childNode.Scale[1] = childS.Y;
                            childNode.Scale[2] = childS.Z;*/
                            
                            childNode.Matrix[0]  = childM.M11;
                            childNode.Matrix[1]  = childM.M21;
                            childNode.Matrix[2]  = childM.M31;
                            childNode.Matrix[3]  = childM.M41;
                            
                            childNode.Matrix[4]  = childM.M12;
                            childNode.Matrix[5]  = childM.M22;
                            childNode.Matrix[6]  = childM.M32;
                            childNode.Matrix[7]  = childM.M42;
                            
                            childNode.Matrix[8]  = childM.M13;
                            childNode.Matrix[9]  = childM.M23;
                            childNode.Matrix[10] = childM.M33;
                            childNode.Matrix[11] = childM.M43;
                            
                            childNode.Matrix[12] = childM.M14;
                            childNode.Matrix[13] = childM.M24;
                            childNode.Matrix[14] = childM.M34;
                            childNode.Matrix[15] = childM.M44;
                            /////
                            /*childNode.Matrix[0]  = childM.M11;
                            childNode.Matrix[1]  = childM.M12;
                            childNode.Matrix[2]  = childM.M13;
                            childNode.Matrix[3]  = childM.M14;
                            
                            childNode.Matrix[4]  = childM.M21;
                            childNode.Matrix[5]  = childM.M22;
                            childNode.Matrix[6]  = childM.M23;
                            childNode.Matrix[7]  = childM.M24;
                            
                            childNode.Matrix[8]  = childM.M31;
                            childNode.Matrix[9]  = childM.M32;
                            childNode.Matrix[10] = childM.M33;
                            childNode.Matrix[11] = childM.M34;
                            
                            childNode.Matrix[12] = childM.M41;
                            childNode.Matrix[13] = childM.M42;
                            childNode.Matrix[14] = childM.M43;
                            childNode.Matrix[15] = childM.M44;*/
                            
                        }
                    }
                }
                var i = node.Mesh;
                if (!i.HasValue)continue;
                var mesh = new Mesh();
                foreach (var primitive in model._gltfFile.Meshes[i.Value].Primitives)
                {
                    var isIndexed = primitive.Indices.HasValue;
                    Accessor indexAccessor = null;
                    if (isIndexed)
                    {
                        indexAccessor = model._gltfFile.Accessors[primitive.Indices.Value];
                    }

                    var primitives = new Primitives();
                    foreach (var (key, value) in primitive.Attributes)
                    {
                        var dataAccessor = model._gltfFile.Accessors[value];

                        primitives.Dictionary.Add(key,
                            isIndexed
                                ? model._gltfFile.ReadAccessorIndexed(dataAccessor, indexAccessor)
                                : model._gltfFile.ReadAccessor(dataAccessor));
                    }

                    var matIndex = primitive.Material ?? -1;
                    if (matIndex != -1)
                    {
                        var material = new Material();
                        material.DoubleSided = model._gltfFile.Materials[matIndex].DoubleSided;

                        if (model._gltfFile.Materials[matIndex].Extensions != null)
                        {
                            var extensions = model._gltfFile.Materials[matIndex].Extensions;
                            foreach (var extension in extensions)
                            {
                                material.Extensions.Add(extension.Key);
                            }
                        }
                        
                        if (model._gltfFile.Materials[matIndex].PbrMetallicRoughness.BaseColorTexture != null)
                        {
                            var textureIndex = model._gltfFile.Materials[matIndex].PbrMetallicRoughness.BaseColorTexture.Index;
                            var imageIndex = model._gltfFile.Textures[textureIndex].Source;
                            material.Uri = model._gltfFile.Images[imageIndex].Uri;
                        }
                        if (model._gltfFile.Materials[matIndex].PbrMetallicRoughness.BaseColorFactor != null)
                        {
                            float[] c = model._gltfFile.Materials[matIndex].PbrMetallicRoughness.BaseColorFactor;
                            material.BaseColorFactor = new Color(c[0], c[1], c[2], c[3]);
                        }

                        primitives.Material = material;
                    }
                    
                    primitives.Init(graphicsDevice);
                    mesh.PrimitivesList.Add(primitives);
                }
                mesh.Node = node;
                model._meshes.Add(mesh);
            }

            return model;
        }
        
        protected static Texture2D LoadFormFile(GraphicsDevice graphicsDevice, string path)
        {
            var fileStream = new FileStream(path, FileMode.Open);
            var result = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();
            return result;
        }
        
    }
}