using System;
using System.Collections.Generic;
using UnityEngine;

// Unity 6 drops the Unity 3 LightmapSettings table and renderer atlas bindings
// during scene import. These references retain the baked pixels from the APK.
[DefaultExecutionOrder(-32000)]
public sealed class LegacyLightmaps : MonoBehaviour
{
    [Serializable]
    public struct Binding
    {
        public Renderer renderer;
        public int index;
        public Vector4 scaleOffset;
    }

    public Texture2D[] textures;
    public Binding[] bindings;
    public TrackLighting[] trackLighting;

    void Awake()
    {
        var maps = new List<LightmapData>(LightmapSettings.lightmaps);
        int offset = maps.Count;
        foreach (var texture in textures)
            maps.Add(new LightmapData { lightmapColor = texture });
        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        LightmapSettings.lightmaps = maps.ToArray();
        foreach (var binding in bindings)
        {
            binding.renderer.lightmapIndex = offset + binding.index;
            binding.renderer.lightmapScaleOffset = binding.scaleOffset;
        }
        foreach (var lighting in trackLighting)
            lighting.lightmapIndex += offset;
    }
}
