using UnityEngine;

// Numeric values are serialized in the Unity 3 scenes and prefabs.
public enum ParticleRenderMode { Billboard = 0, Stretch = 3, SortedBillboard = 2, HorizontalBillboard = 4, VerticalBillboard = 5 }

public class ParticleRenderer : MonoBehaviour
{
    public Material sharedMaterial;
    public float cameraVelocityScale, lengthScale = 2, velocityScale, maxParticleSize = .25f;
    public ParticleRenderMode particleRenderMode;
    public int uvAnimationXTile = 1, uvAnimationYTile = 1;
    public float uvAnimationCycles = 1;
}
