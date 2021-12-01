using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestRendering
{
    public class Material
    {
        public string Uri;
        public bool DoubleSided = false;
        public Color BaseColorFactor = Color.White;
        public List<string> Extensions = new List<string>();
    }
}