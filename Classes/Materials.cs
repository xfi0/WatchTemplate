using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WatchTemplate.Classes
{
    public class Materials
    {
        public static Material StandardMaterial;
        public static Material SpriteMaterial;
        public static Material Standard()
        {
            if (StandardMaterial == null)
            {
                StandardMaterial = new Material(Shader.Find("Standard"));
            }
            return StandardMaterial;
        }
        public static Material SpriteDefault()
        {
            if (SpriteMaterial == null)
            {
                SpriteMaterial = new Material(Shader.Find("Sprites/Default"));
            }
            return SpriteMaterial;
        }
    }
}
