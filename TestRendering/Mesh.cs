using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrivialHxGLTF;

namespace TestRendering
{
    public class Mesh
    {
        public Node Node;
        public List<Primitives> PrimitivesList = new List<Primitives>();
    }
    
    public class Primitives
    {
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer
        public bool IsIndex = false;
        public Material Material;
        public Dictionary<string, float[]> Dictionary = new Dictionary<string, float[]>();

        public void Init(GraphicsDevice graphicsDevice)
        {
            var positions = new List<Vector3>();
            var uvs = new List<Vector2>();
            var normals = new List<Vector3>();
            
            foreach (var (key, value) in Dictionary)
            {
                switch (key)
                {
                    case "POSITION":
                    {
                        for (var x = 0; x < value.Length; x += 3)
                        {
                            positions.Add(new Vector3(value[x], value[x + 1], value[x + 2]));
                            //Console.WriteLine(positions[x / 3]);
                        }

                        break;
                    }
                    case "NORMAL":
                    {
                        for (var x = 0; x < value.Length; x += 3)
                        {
                            normals.Add(new Vector3(value[x], value[x + 1], value[x + 2]));
                        }

                        break;
                    }
                    case "TEXCOORD_0":
                    {
                        for (var x = 0; x < value.Length; x += 2)
                        {
                            uvs.Add(new Vector2(value[x], value[x + 1]));
                        }

                        break;
                    }
                }
            }
            var count = positions.Count;
            var vertexData = new VertexPositionNormalTexture[count];
            
            for (var x = 0; x < count; x++)
            {
                vertexData[x] = new VertexPositionNormalTexture(positions[x], normals[x], uvs.Count >= count ? uvs[x] : Vector2.One);
            }  
            VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, count, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertexData);
        }
    }
    
    public class Attribute
    {
        
    }
}