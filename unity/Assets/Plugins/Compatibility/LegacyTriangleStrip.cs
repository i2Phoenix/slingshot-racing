using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Unity 3 exposed strips on Mesh. Modern Mesh requires explicit triangles.
internal static class LegacyTriangleStrip
{
    private static readonly ConditionalWeakTable<Mesh, Dictionary<int, int[]>> strips = new ConditionalWeakTable<Mesh, Dictionary<int, int[]>>();

    public static int[] Get(Mesh mesh, int submesh)
    {
        int[] strip;
        return strips.GetOrCreateValue(mesh).TryGetValue(submesh, out strip) ? (int[])strip.Clone() : new int[0];
    }

    public static void Set(Mesh mesh, int[] strip, int submesh)
    {
        strips.GetOrCreateValue(mesh)[submesh] = (int[])strip.Clone();
        var triangles = new List<int>(Math.Max(0, strip.Length - 2) * 3);
        for (int i = 2; i < strip.Length; i++)
        {
            int a = strip[i - 2], b = strip[i - 1], c = strip[i];
            if (a == b || a == c || b == c) continue;
            triangles.Add((i & 1) == 0 ? a : b);
            triangles.Add((i & 1) == 0 ? b : a);
            triangles.Add(c);
        }
        mesh.SetTriangles(triangles, submesh);
    }
}
