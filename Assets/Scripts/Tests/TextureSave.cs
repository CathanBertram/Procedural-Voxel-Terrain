using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class TextureSave : MonoBehaviour
    {
        public void SaveTexture()
        {
            var sr = GetComponent<MeshRenderer>();
            //var texture = sr.sprite.texture;

            var texture = sr.sharedMaterial.mainTexture as Texture2D;
            
            byte[] bytes = texture.EncodeToPNG();
            var path = Application.dataPath + "/../Assets/SavedTextures/";
            
            
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var textureName = Directory.GetFiles(path).Length.ToString();

            File.WriteAllBytes($"{path}{textureName}.png", bytes);
        }
    }
}
