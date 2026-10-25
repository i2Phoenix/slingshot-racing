using UnityEngine;

[AddComponentMenu("Graphics/Shader Warmer")]
public class ShaderWarmer : MonoBehaviour
{
	public Shader[] m_includedShaders;

	// Keep the serialized shader references. Global warmup is unsafe with
	// Unity 6/GLES internal variants; used variants compile on first render.
}
