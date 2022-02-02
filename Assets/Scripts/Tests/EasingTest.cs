using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class EasingTest : MonoBehaviour
    {
        private Texture2D texture;
        [SerializeField] private Renderer r;
        [SerializeField] private int size;
        [SerializeField] private int lineWidth;
        [SerializeField] private EasingType easeType;
        
        private Color[] colors;
        private void Start()
        {
            r = GetComponent<Renderer>();
            
            texture = new Texture2D(128, 128);
            texture.filterMode = FilterMode.Point;
            r.sharedMaterial.mainTexture = texture;

           
        }

        private void SetBackground()
        {
            colors = new Color[size * size];
            
            foreach (var item in colors)
            {
                for (int i = 0; i < size * size; i++)
                {
                    colors[i] = Color.white;
                }
            }
            
            texture.SetPixels(colors);

        }
        
        public void UpdateColours()
        {
            r = GetComponent<Renderer>();
            
            texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            r.sharedMaterial.mainTexture = texture;
            
            SetBackground();

            for (int x = 0; x < size; x++)
            {
                int y = 0;
                switch (easeType)
                {
                    case EasingType.Linear:
                        y = Mathf.RoundToInt(Easing.Linear(0, size, x / (float)size));
                        break;
                    case EasingType.EaseInQuadratic:
                        y = Mathf.RoundToInt(Easing.EaseInQuadratic(0, size, x / (float)size));
                        break;
                    case EasingType.EaseOutQuadratic:
                        y = Mathf.RoundToInt(Easing.EaseOutQuadratic(0, size, x / (float)size));
                        break;
                    case EasingType.EaseInOutQuadratic:
                        y = Mathf.RoundToInt(Easing.EaseInOutQuadratic(0, size, x / (float)size));
                        break;
                    case EasingType.EaseInCubic:
                        y = Mathf.RoundToInt(Easing.EaseInCubic(0, size, x / (float)size));
                        break;
                    case EasingType.EaseOutCubic:
                        y = Mathf.RoundToInt(Easing.EaseOutCubic(0, size, x / (float)size));
                        break;
                    case EasingType.EaseInOutCubic:
                        y = Mathf.RoundToInt(Easing.EaseInOutCubic(0, size, x / (float)size));
                        break;
                    case EasingType.EaseInQuart:
                        y = Mathf.RoundToInt(Easing.EaseInQuart(0, size, x / (float)size));
                        break;
                    case EasingType.EaseOutQuart:
                        y = Mathf.RoundToInt(Easing.EaseOutQuart(0, size, x / (float)size));
                        break;
                    case EasingType.EaseInOutQuart:
                        y = Mathf.RoundToInt(Easing.EaseInOutQuart(0, size, x / (float)size));
                        break;
                    case EasingType.EaseInQuint:
                        y = Mathf.RoundToInt(Easing.EaseInQuint(0, size, x / (float)size));
                        break;
                    case EasingType.EaseOutQuint:
                        y = Mathf.RoundToInt(Easing.EaseOutQuint(0, size, x / (float)size));
                        break;
                    case EasingType.EaseInOutQuint:
                        y = Mathf.RoundToInt(Easing.EaseInOutQuint(0, size, x / (float)size));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                SetColours(x, y);
            }
            
            texture.Apply();
        }
        private void SetColours(int x, int y)
        {
            texture.SetPixel(x, y, Color.black);

            for (int x1 = x - lineWidth; x1 < x + lineWidth; x1++)
            {
                for (int y1 = y - lineWidth; y1 < y + lineWidth; y1++)
                {
                    texture.SetPixel(x1, y1, Color.black);
                }
            }
        }
    }
}
