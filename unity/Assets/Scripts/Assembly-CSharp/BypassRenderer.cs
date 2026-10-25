using UnityEngine;

public class BypassRenderer : BypassRendererBase
{
    private void OnPostRender()
    {
        for (int i = 0; i < m_numActiveMeshes; i++)
        {
            var state = m_activeMeshes[i];
            if (state.material == null || state.material.m_material == null) continue;
            if (state.material.m_material.SetPass(0))
                state.mesh.DrawRange(state.matrix, state.startIndex, state.indexCount);
        }
    }
}
