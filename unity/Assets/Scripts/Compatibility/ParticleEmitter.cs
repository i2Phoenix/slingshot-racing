using UnityEngine;

// Serialized Unity 3 particle settings, driven by the game's recovered simulator.
// The conversion script preserves component file IDs and every referenced asset.
public class ParticleEmitter : MonoBehaviour
{
    public bool emit, useWorldSpace = true, rndRotation;
    public float minSize = 1, maxSize = 1, minEnergy = 1, maxEnergy = 1;
    public float minEmission = 10, maxEmission = 10, emitterVelocityScale;
    public float angularVelocity, rndAngularVelocity;
    public Vector3 worldVelocity, localVelocity, rndVelocity, tangentVelocity, ellipsoid;
    public bool oneShot;
    public float minEmitterRange;
    private ParticleSys backend;

    private ParticleSys Backend
    {
        get
        {
            if (backend == null)
            {
                backend = GetComponent<ParticleSys>();
                if (backend == null)
                {
                    backend = gameObject.AddComponent<ParticleSys>();
                    backend.m_maxParticles = Mathf.Max(512, Mathf.CeilToInt(maxEnergy * maxEmission) + 1);
                    backend.m_combineColours = true;
                    backend.m_ellipsoid = ellipsoid;
                    backend.m_angularVelocity = angularVelocity;
                }
            }
            backend.Initialize();
            return backend;
        }
    }

    private void Start()
    {
        var simulator = Backend;
        if (oneShot && emit)
        {
            Emit(Mathf.RoundToInt(Random.Range(minEmission, maxEmission)));
            emit = false;
        }
    }

    public int particleCount { get { return Backend.LegacyParticleCount; } }
    public Particle[] particles { get { return Backend.GetLegacyParticles(); } set { Backend.SetLegacyParticles(value); } }
    public void ClearParticles() { Backend.ClearLegacyParticles(); }
    public void Emit() { Emit(Mathf.RoundToInt(Random.Range(minEmission, maxEmission))); }
    public void Emit(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 position = Vector3.Scale(Random.insideUnitSphere, ellipsoid);
            Vector3 velocity = localVelocity + Vector3.Scale(Random.insideUnitSphere, rndVelocity);
            if (useWorldSpace) { position = transform.TransformPoint(position); velocity = transform.TransformDirection(velocity) + worldVelocity; }
            else velocity += transform.InverseTransformDirection(worldVelocity);
            Emit(position, velocity, Random.Range(minSize, maxSize), Random.Range(minEnergy, maxEnergy), Color.white,
                rndRotation ? Random.Range(0, 360f) : 0, angularVelocity + Random.Range(-rndAngularVelocity, rndAngularVelocity));
        }
    }
    public void Emit(Vector3 position, Vector3 velocity, float size, float energy, Color colour, float rotation = 0, float spin = 0)
    {
        if (!useWorldSpace) { position = transform.TransformPoint(position); velocity = transform.TransformDirection(velocity); }
        Backend.Emit(position, velocity, size, energy, colour, rotation, spin);
    }
}

public struct Particle
{
    public Vector3 position, velocity;
    public float energy, startEnergy, size, rotation, angularVelocity;
    public Color color;
}
