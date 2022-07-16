using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    private static float fov = 60f, volume = 0f, sensitivity = 1f;

    public static void setFov(float f)
    {
        fov = f;
    }
    public static void setVolume(float v)
    {
        volume = v;
    }
    public static void setSensitivity(float s)
    {
        sensitivity = s;
    }

    public static float getFov() {
        return fov;
    }
    public static float getVolume() {
        return volume;
    }
    public static float getSensitivity() {
        return sensitivity;
    }
}
