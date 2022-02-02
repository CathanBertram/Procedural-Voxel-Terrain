using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EasingType
{
    Linear,
    EaseInQuadratic,
    EaseOutQuadratic,
    EaseInOutQuadratic,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine, 
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
    EaseInBack,
    EaseOutBack,
    EaseInOutBack, 
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce
}

public static class Easing
{
    public static float Linear(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
    
    public static float EaseInQuadratic(float a, float b, float t)
    {
        return a + (b - a) * (t * t);
    }
    
    public static float EaseOutQuadratic(float a, float b, float t)
    {
        return a + (b - a) * (t * (2 - t));
    }
    public static float EaseInOutQuadratic(float a, float b, float t)
    {
        t = t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        return a + (b - a) * (t * (2 - t));
    }
    
    public static float EaseInCubic(float a, float b, float t)
    {
        return a + (b - a) * (t * t * t);
    }

    public static float EaseOutCubic(float a, float b, float t) 
    { 
        return a + (b - a) * ((--t) * t * t + 1);
    }

    public static float EaseInOutCubic(float a, float b, float t)
    {
        t = t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
        return a + (b - a) * t;
    }

    public static float EaseInQuart(float a, float b, float t)
    {
        return a + (b - a) * (t * t * t * t);
    }

    public static float EaseOutQuart(float a, float b, float t)
    {
        return a + (b - a) * (1 - (--t) * t * t * t);
    }

    public static float EaseInOutQuart(float a, float b, float t)
    {
        t = t < .5 ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
        return a + (b - a) * t;
    }

    public static float EaseInQuint(float a, float b, float t)
    {
        return a + (b - a) * (t*t*t*t*t);
    }

    public static float EaseOutQuint(float a, float b, float t)
    {
        return a + (b - a) * (1+(--t)*t*t*t*t);
    }

    public static float EaseInOutQuint(float a, float b, float t)
    {
        t = t < 0.5f ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;
        return a + (b - a) * t;
    }

    public static float EaseInSine(float a, float b, float t)
    {
        return a + (b - a) * (1 - Mathf.Cos((t * Mathf.PI) * 0.5f));
    }
    public static float EaseOutSine(float a, float b, float t)
    {
        return a + (b - a) * (Mathf.Sin((t * Mathf.PI) * 0.5f));
    }
    public static float EaseInOutSine(float a, float b, float t)
    {
        return a + (b - a) * (-(Mathf.Cos(Mathf.PI * t) - 1) * 0.5f);
    }

    public static float EaseInExpo(float a, float b, float t)
    {
        t = t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
        return a + (b - a) * t;
    }

    public static float EaseOutExpo(float a, float b, float t)
    {
        t = t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
        return a + (b - a) * t;
    }

    public static float EaseInOutExpo(float a, float b, float t)
    {
        t = t == 0 ? 0 :
            t == 1 ? 1 :
            t < 0.5f ? Mathf.Pow(2, 20 * t - 10) * 0.5f : 
            (2 - Mathf.Pow(2, -20 * t + 10)) * 0.5f;
        return a + (b - a) * t;
    }

    public static float EaseInCirc(float a, float b, float t)
    {
        return a + (b - a) * (1 - Mathf.Sqrt(1 - t * t));
    }

    public static float EaseOutCirc(float a, float b, float t)
    {
        return a + (b - a) * (Mathf.Sqrt(1 - (t - 1) * (t - 1)));
    }

    public static float EaseInOutCirc(float a, float b, float t)
    {
        t = t < 0.5f
            ? (1 - Mathf.Sqrt(1 - (2 * t) * (2 * t))) * 0.5f
            : (Mathf.Sqrt(1 - (-2 * t + 2) * (-2 * t + 2)) + 1) * 0.5f;

        return a + (b - a) * t;
    }

    public static float EaseInBack(float a, float b, float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;

        return a + (b - a) * (c3 * t * t * t - c1 * t * t);
    }

    public static float EaseOutBack(float a, float b, float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;

        return a + (b - a) * (1 + c3 * ((t - 1) * (t - 1) * (t - 1)) + c1 * ((t - 1) * (t - 1)));
    }

    public static float EaseInOutBack(float a, float b, float t)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;

        t = t < 0.5f
            ? ((2 * t) * (2 * t) * ((c2 + 1) * 2 * t - c2)) * 0.5f
            : ((2 * t - 2) * (2 * t - 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) * 0.5f;

        return a + (b - a) * t;
    }

    public static float EaseInBounce(float a, float b, float t)
    {
        return a + (b - a) * (1 - EaseOutBounce(1 - t));
    }

    public static float EaseOutBounce(float a, float b, float t)
    {
        return a + (b - a) * EaseOutBounce(t);
    }
    
    public static float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1 / d1) {
            return n1 * t * t;
        } else if (t < 2 / d1) {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        } else if (t < 2.5 / d1) {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        } else {
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }

    public static float EaseInOutBounce(float a, float b, float t)
    {
        t = t < 0.5f
            ? (1 - EaseOutBounce(1 - 2 * t)) * 0.5f
            : (1 + EaseOutBounce(2 * t - 1)) * 0.5f;

        return a + (b - a) * t;
    }
}
