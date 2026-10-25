using UnityEngine;

[AddComponentMenu("Graphics/Shader Warmer")]
public class ShaderWarmer : MonoBehaviour
{
	public Shader[] m_includedShaders;

	private void Start()
	{
		Shader.WarmupAllShaders();
	}
}
