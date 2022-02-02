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
                int y = easeType switch
                {
                    EasingType.Linear => Mathf.RoundToInt(Easing.Linear(0, size, x / (float) size)),
                    EasingType.EaseInQuadratic => Mathf.RoundToInt(Easing.EaseInQuadratic(0, size, x / (float) size)),
                    EasingType.EaseOutQuadratic => Mathf.RoundToInt(Easing.EaseOutQuadratic(0, size, x / (float) size)),
                    EasingType.EaseInOutQuadratic => Mathf.RoundToInt(Easing.EaseInOutQuadratic(0, size, x / (float) size)),
                    EasingType.EaseInCubic => Mathf.RoundToInt(Easing.EaseInCubic(0, size, x / (float) size)),
                    EasingType.EaseOutCubic => Mathf.RoundToInt(Easing.EaseOutCubic(0, size, x / (float) size)),
                    EasingType.EaseInOutCubic => Mathf.RoundToInt(Easing.EaseInOutCubic(0, size, x / (float) size)),
                    EasingType.EaseInQuart => Mathf.RoundToInt(Easing.EaseInQuart(0, size, x / (float) size)),
                    EasingType.EaseOutQuart => Mathf.RoundToInt(Easing.EaseOutQuart(0, size, x / (float) size)),
                    EasingType.EaseInOutQuart => Mathf.RoundToInt(Easing.EaseInOutQuart(0, size, x / (float) size)),
                    EasingType.EaseInQuint => Mathf.RoundToInt(Easing.EaseInQuint(0, size, x / (float) size)),
                    EasingType.EaseOutQuint => Mathf.RoundToInt(Easing.EaseOutQuint(0, size, x / (float) size)),
                    EasingType.EaseInOutQuint => Mathf.RoundToInt(Easing.EaseInOutQuint(0, size, x / (float) size)),
                    EasingType.EaseInSine => Mathf.RoundToInt(Easing.EaseInSine(0, size, x / (float) size)),
                    EasingType.EaseOutSine => Mathf.RoundToInt(Easing.EaseOutSine(0, size, x / (float) size)),
                    EasingType.EaseInOutSine => Mathf.RoundToInt(Easing.EaseInOutSine(0, size, x / (float) size)),
                    EasingType.EaseInExpo => Mathf.RoundToInt(Easing.EaseInExpo(0, size, x / (float) size)),
                    EasingType.EaseOutExpo => Mathf.RoundToInt(Easing.EaseOutExpo(0, size, x / (float) size)),
                    EasingType.EaseInOutExpo => Mathf.RoundToInt(Easing.EaseInOutExpo(0, size, x / (float) size)),
                    EasingType.EaseInCirc => Mathf.RoundToInt(Easing.EaseInCirc(0, size, x / (float) size)),
                    EasingType.EaseOutCirc => Mathf.RoundToInt(Easing.EaseOutCirc(0, size, x / (float) size)),
                    EasingType.EaseInOutCirc => Mathf.RoundToInt(Easing.EaseInOutCirc(0, size, x / (float) size)),
                    EasingType.EaseInBack => Mathf.RoundToInt(Easing.EaseInBack(0, size, x / (float) size)),
                    EasingType.EaseOutBack => Mathf.RoundToInt(Easing.EaseOutBack(0, size, x / (float) size)),
                    EasingType.EaseInOutBack => Mathf.RoundToInt(Easing.EaseInOutBack(0, size, x / (float) size)),
                    EasingType.EaseInBounce => Mathf.RoundToInt(Easing.EaseInBounce(0, size, x / (float) size)),
                    EasingType.EaseOutBounce => Mathf.RoundToInt(Easing.EaseOutBounce(0, size, x / (float) size)),
                    EasingType.EaseInOutBounce => Mathf.RoundToInt(Easing.EaseInOutBounce(0, size, x / (float) size)),
                    _ => throw new ArgumentOutOfRangeException()
                };

                // if (y < size * 0.5f)
                // {
                //     y += 20;
                // }else if (y > size * 0.5f)
                // {
                //     y -= 20;
                // }

                SetColours(x, y, Color.black);
            }
            
            SetColours(0,0, Color.red);

            texture.Apply();
        }
        private void SetColours(int x, int y, Color color)
        {
            texture.SetPixel(x, y, color);

            for (int x1 = x - lineWidth; x1 < x + lineWidth; x1++)
            {
                for (int y1 = y - lineWidth; y1 < y + lineWidth; y1++)
                {
                    texture.SetPixel(x1, y1, color);
                }
            }
        }
    }
}
