using UnityEngine;

public class ParticleAnimator : MonoBehaviour
{
    public bool doesAnimateColor, autodestruct, stopSimulation;
    public Color[] colorAnimation = { Color.white, Color.white, Color.white, Color.white, Color.clear };
    public Vector3 worldRotationAxis, localRotationAxis, rndForce, force;
    public float sizeGrow, damping = 1;
}
