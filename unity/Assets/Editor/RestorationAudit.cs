using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RestorationAudit
{
    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Track_Tutorial_1.3_Blockout.unity");

        foreach (var root in scene.GetRootGameObjects())
        foreach (var spline in root.GetComponentsInChildren<TrackSpline>(true))
        {
            typeof(TrackSpline).GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(spline, null);
            typeof(TrackSpline).GetMethod("InitSplineCache", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(spline, null);
            for (int i = 0; i < spline.nodes.Length - 1; i++)
            {
                var mesh = new Mesh();
                spline.GenerateTrackCollisionGeometry(i, i + 1, mesh);
                var go = new GameObject("Probe segment " + i);
                go.transform.SetParent(spline.transform, false);
                go.layer = 8;
                var col = go.AddComponent<MeshCollider>();
                col.sharedMesh = mesh;
                Debug.Log($"AUDIT_COLLIDER {i} vertices={mesh.vertexCount} bounds={col.bounds} normal={mesh.normals[mesh.vertexCount / 2]}");
            }
            Physics.SyncTransforms();
            foreach (var hit in Physics.RaycastAll(new Vector3(18.07f, 100, 14.16f), Vector3.down, 200, 1 << 8))
                Debug.Log($"AUDIT_RAY {hit.collider.name} point={hit.point} normal={hit.normal}");
            var oldMode = Physics.simulationMode;
            try
            {
                Physics.simulationMode = SimulationMode.Script;
                for (int kind = 0; kind < 5; kind++)
                {
                    var bodyObject = new GameObject("Vehicle probe");
                    bodyObject.transform.position = new Vector3(18.07f, 0, 14.16f);
                    bodyObject.transform.rotation = Quaternion.Euler(0,-97,0);
                    var body = bodyObject.AddComponent<Rigidbody>();
                    var capsuleObject = new GameObject("Track capsule");
                    capsuleObject.transform.SetParent(bodyObject.transform, false);
                    capsuleObject.layer = 17;
                    var capsule = capsuleObject.AddComponent<CapsuleCollider>();
                    capsule.center = new Vector3(0, -0.2f, 0);
                    capsule.radius = 0.6f;
                    capsule.height = 3.6f;
                    capsule.direction = 2;
                    if (kind > 0)
                    for (int wheel = 0; wheel < 4; wheel++)
                    {
                        var wheelObject = new GameObject("Wheel");
                        wheelObject.transform.SetParent(bodyObject.transform, false);
                        wheelObject.layer = 14;
                        var wc = wheelObject.AddComponent<WheelCollider>();
                        wc.suspensionDistance = 0.25f;
                        wc.center = new Vector3((wheel % 2 == 0 ? 1 : -1) * 0.8f, -0.4f, (wheel < 2 ? 1 : -1) * 1.2f);
                        wc.radius = 0.5f;
                        wc.mass = kind > 1 ? 0.02f : 0;
                        wc.brakeTorque = 0.05f;
                        var friction = new WheelFrictionCurve { extremumSlip = 2f, extremumValue = kind > 2 ? 1f : 1000f, asymptoteSlip = 4f, asymptoteValue = kind > 2 ? 0.5f : 500f, stiffness = 5f };
                        wc.forwardFriction = friction;
                        wc.sidewaysFriction = friction;
                        wc.suspensionSpring = new JointSpring { spring = kind > 3 ? 75 : 0.6f, damper = kind > 3 ? 5f : 0.2f, targetPosition = 0 };
                    }
                    Physics.SyncTransforms();
                    for (int step = 0; step < 60; step++) Physics.Simulate(1f / 30f);
                    Debug.Log($"AUDIT_VEHICLE_PROBE kind={kind} pos={body.position} velocity={body.linearVelocity} ignore={Physics.GetIgnoreLayerCollision(8,17)}");
                    Object.DestroyImmediate(bodyObject);
                }
            }
            finally { Physics.simulationMode = oldMode; }
            float t = spline.ClosestTTo(new Vector3(18.07f, 0, 14.16f));
            Debug.Log($"AUDIT_SPLINE t={t} point={spline.PointAtT(t)} height={spline.HeightAt(t, 0)}");
        }
    }
}
