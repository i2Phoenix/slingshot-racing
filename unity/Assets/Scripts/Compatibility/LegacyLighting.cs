using UnityEngine;
using UnityEngine.SceneManagement;

public static class LegacyLighting
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Install()
    {
        SceneManager.sceneLoaded += RestoreIntensity;
    }

    static void RestoreIntensity(Scene scene, LoadSceneMode mode)
    {
        // Unity's pre-5 scene upgrader doubles light intensity. These shaders
        // preserve the Unity 3 lighting equations, including pixel and SH
        // lighting. Restore the source intensity before either is calculated.
        foreach (var root in scene.GetRootGameObjects())
            foreach (var light in root.GetComponentsInChildren<Light>(true))
                light.intensity *= 0.5f;
    }
}
