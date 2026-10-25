using UnityEditor.SceneManagement;
using UnityEngine;

public static class PhysicsMigrationProbe
{
    public static void Run()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var previousMode = Physics.simulationMode;
        try
        {
        Physics.simulationMode = SimulationMode.Script;
        for (int kind = 0; kind < 8; kind++)
        {
            var root = new GameObject("Root");
            root.transform.position = new Vector3(-600, kind * 20, -7);
            var anchor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            anchor.transform.SetParent(root.transform, false);
            anchor.transform.localPosition = new Vector3(1.3f, 3, 0.7f);
            var rb = anchor.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.isKinematic = false;
            rb.interpolation = (kind & 1) != 0 ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
            var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.transform.SetParent(root.transform, false);
            var platformBody = platform.AddComponent<Rigidbody>();
            var joint = anchor.AddComponent<CharacterJoint>();
            joint.connectedBody = platformBody;
            Physics.SyncTransforms();
            Physics.Simulate(1f / 30f);
            var target = rb.position + Vector3.left * 9;
            for (int frame = 0; frame < 10; frame++)
            {
                switch (kind / 2)
                {
                    case 0: rb.transform.position = target; Physics.SyncTransforms(); break;
                    case 1: rb.constraints = RigidbodyConstraints.None; rb.position = target; rb.constraints = RigidbodyConstraints.FreezePosition; break;
                    case 2: rb.constraints = RigidbodyConstraints.None; rb.transform.position = target; Physics.SyncTransforms(); rb.constraints = RigidbodyConstraints.FreezePosition; break;
                    case 3: rb.isKinematic = true; rb.constraints = RigidbodyConstraints.None; rb.position = target; rb.isKinematic = false; rb.constraints = RigidbodyConstraints.FreezePosition; break;
                }
                Physics.Simulate(1f / 30f);
            }
            Debug.Log($"PHYSICS_PROBE kind={kind} target={target:F3} actual={rb.position:F3} transform={rb.transform.position:F3} error={Vector3.Distance(target, rb.position)} platform={platformBody.position:F3}");
            Object.DestroyImmediate(root);
        }
        }
        finally { Physics.simulationMode = previousMode; }
    }
}
