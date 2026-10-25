using System;
using System.IO;
using System.Reflection;
using UnityEngine;

// Exercises the actual managed/native boundary used by restored legacy emitters.
public static class ParticleCompatibilityChecks
{
    public static void RunAndBuildAndroid()
    {
        Run();
        ModernBuild.Android();
    }

    public static void Run()
    {
        Check("last particle expires", new[] { 0.1f }, 0.2f, 0);
        Check("mixed lifetimes compact", new[] { 0.1f, 0.8f, 0.1f }, 0.2f, 1);
        Check("whole burst expires", new[] { 0.1f, 0.1f, 0.1f }, 0.2f, 0);
        Check("live burst remains", new[] { 0.8f, 0.8f }, 0.2f, 2);
        if ((int)ParticleRenderMode.Stretch != 3 || (int)ParticleRenderMode.HorizontalBillboard != 4 ||
            (int)ParticleRenderMode.VerticalBillboard != 5)
            throw new InvalidOperationException("Serialized Unity 3 particle modes changed");
        string root = Directory.GetParent(Application.dataPath).Parent.FullName;
        File.WriteAllText(Path.Combine(root, "work/qa/particle-compatibility.json"),
            "{\n  \"passed\": 5,\n  \"failed\": 0,\n  \"checks\": [\"last particle expires\", \"mixed lifetimes compact\", \"whole burst expires\", \"live burst remains\", \"Unity 3 serialized render modes\"]\n}\n");
        Debug.Log("PARTICLE_COMPATIBILITY_CHECKS passed=5 failed=0");
    }

    static void Check(string name, float[] lifetimes, float step, int expected)
    {
        var go = new GameObject("Particle compatibility check");
        try
        {
            var particles = go.AddComponent<ParticleSys>();
            particles.m_maxParticles = 8;
            foreach (float lifetime in lifetimes)
                particles.Emit(Vector3.zero, Vector3.right, 1, lifetime, Color.white, 0, 0);
            typeof(ParticleSys).GetMethod("LateUpdate_UpdateParticles", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(particles, new object[] { step });
            if (particles.LegacyParticleCount != expected)
                throw new InvalidOperationException(name + ": expected " + expected + ", got " + particles.LegacyParticleCount);
            foreach (var particle in particles.GetLegacyParticles())
                if (particle.energy <= 0 || !float.IsFinite(particle.position.x))
                    throw new InvalidOperationException(name + ": invalid active particle");
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
    }
}
