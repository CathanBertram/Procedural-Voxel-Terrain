using System.Collections;
using System.Collections.Generic;
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
    EaseInOutQuint
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
}
