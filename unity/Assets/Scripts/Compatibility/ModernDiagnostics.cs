#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Development-only observations of real input, cameras and scene objects.
public sealed class ModernDiagnostics : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        var go = new GameObject("Modern diagnostics");
        DontDestroyOnLoad(go);
        go.AddComponent<ModernDiagnostics>();
    }

    void Awake()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        SceneManager.sceneLoaded += Loaded;
        StartCoroutine(WatchRace());
    }

    IEnumerator WatchRace()
    {
        var observed = new HashSet<Vehicle>();
        var managers = new HashSet<RaceManager>();
        while (true)
        {
            yield return new WaitForSecondsRealtime(2);
            if (DynamicCamera.Get != null)
                foreach (var camera in Camera.allCameras)
                    Debug.Log($"QA_RACE_CAMERA {camera.name} pos={camera.transform.position:F3} rot={camera.transform.eulerAngles:F3} fov={camera.fieldOfView} near={camera.nearClipPlane} far={camera.farClipPlane} clear={camera.clearFlags} background={camera.backgroundColor} target={camera.targetTexture} mask={camera.cullingMask}");
            foreach (var manager in FindObjectsByType<RaceManager>(FindObjectsSortMode.None))
                if (managers.Add(manager))
                    manager.OnFinalisedRaceStats += stats => Debug.Log($"QA_FINAL_STATS position={stats.m_finishPosition} award={stats.m_award} laps={stats.m_numLapsCompleted} remaining={stats.m_numLapsRemaining} time={stats.m_raceTime:F3}");
            foreach (var grade in FindObjectsByType<PipeGUI_Results_GradePlate>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var sprite = grade.m_position;
                if (sprite == null || !sprite.gameObject.activeInHierarchy) continue;
                Debug.Log($"QA_FINISH_EFFECTS streamers={FindObjectsByType<Streamers>(FindObjectsSortMode.None).Length}");
                var uv = sprite.GetComponent<MeshFilter>().sharedMesh.uv;
                Debug.Log($"QA_GRADE_SPRITE default={sprite.defaultAnim} current={sprite.CurAnimIndex} started={sprite.Started} animFrames={sprite.GetCurAnim()?.GetFrameCount()} uv={uv[0]} path={Path(sprite.transform)}");
            }
            foreach (var v in FindObjectsByType<Vehicle>(FindObjectsSortMode.None))
            {
                if (observed.Add(v))
                {
                    foreach (var hit in Physics.RaycastAll(new Vector3(v.Position.x, 100, v.Position.z), Vector3.down, 200, 1 << LayerMask.NameToLayer("Track")))
                    {
                        Debug.Log($"QA_GROUND {hit.collider.name} point={hit.point:F4} normal={hit.normal}");
                        foreach (var collider in v.GetComponentsInChildren<Collider>())
                            Debug.Log($"QA_COLLISION {collider.name} layer={collider.gameObject.layer} enabled={collider.enabled} active={collider.gameObject.activeInHierarchy} ignoredLayer={Physics.GetIgnoreLayerCollision(collider.gameObject.layer, hit.collider.gameObject.layer)} ignoredPair={Physics.GetIgnoreCollision(collider, hit.collider)} bounds={collider.bounds}");
                    }
                    v.OnLapEvent += (vehicle, time) => Debug.Log($"QA_LAP player={vehicle.PlayerIndex} laps={vehicle.NumLapsDone} seconds={time:F3}");
                    v.OnGrappleEvent += point => Debug.Log($"QA_GRAPPLE player={v.PlayerIndex} point={point.name}");
                    v.OnGrappleReleaseEvent += point => Debug.Log($"QA_RELEASE player={v.PlayerIndex}");
                    v.OnDeathEvent += vehicle => Debug.Log($"QA_DEATH player={v.PlayerIndex}");
                }
                Debug.Log($"QA_VEHICLE player={v.PlayerIndex} human={v.IsLocalHuman} pos={v.Position:F2} speed={v.Velocity.magnitude:F2} laps={v.NumLapsDone} dead={v.IsDead} grapple={v.GrapplePoint != null} timeScale={Time.timeScale}");
                if (v.PlayerIndex == 0)
                    Debug.Log($"QA_TRACK spline={v.CurrentTrackSpline} hub={v.CurrentTrackHub} t={v.CurrentT:F4} tangent={v.TrackTangent:F3} velocity={v.Velocity:F3}");
            }
            foreach (var pursuer in FindObjectsByType<FleePursuer>(FindObjectsSortMode.None))
                Debug.Log($"QA_PURSUER pos={pursuer.Position:F2} speed={pursuer.TrackForwardSpeed:F2}");
        }
    }
    void OnDestroy() { SceneManager.sceneLoaded -= Loaded; }
    void Loaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(Describe(scene.name));
        if (scene.name == "TitleScreen") StartCoroutine(DescribeSuspension());
    }

    IEnumerator DescribeSuspension()
    {
        for (int sample = 0; sample < 120; sample++)
        {
            yield return new WaitForSecondsRealtime(2);
            foreach (var brace in FindObjectsByType<FrontendBrace>(FindObjectsSortMode.None))
            {
                if (brace.VehicleIndex != Settings.VehicleStyleIndex || brace.Hoist == null) continue;
                var body = brace.Hoist.GetComponent<Rigidbody>();
                Debug.Log($"QA_HOIST index={brace.VehicleIndex} t={Time.time:F3} root={brace.transform.position:F4} item={brace.ListItem.transform.position:F4} body={body.position:F4} rotation={body.rotation.eulerAngles:F3} gravity={Physics.gravity} dt={Time.fixedDeltaTime}");
                foreach (var joint in brace.GetComponentsInChildren<Joint>())
                {
                    var point = joint.transform.TransformPoint(joint.anchor);
                    var connected = body.transform.TransformPoint(joint.connectedAnchor);
                    var anchorBody = joint.GetComponent<Rigidbody>();
                    Debug.Log($"QA_JOINT {joint.name} transform={joint.transform.position:F4} body={anchorBody.position:F4} auto={joint.autoConfigureConnectedAnchor} connectedLocal={joint.connectedAnchor:F4} error={Vector3.Distance(point, connected):F6} frozen={anchorBody.constraints} kinematic={anchorBody.isKinematic}");
                }
                var link = brace.Hoist.GetComponent<BoneLink>();
                var transforms = brace.GetComponentsInChildren<Transform>();
                for (int i = 0; i < link.m_srcName.Length; i++)
                {
                    Transform source = null, destination = null;
                    foreach (var t in transforms)
                    {
                        if (t.name == link.m_srcName[i]) source = t;
                        if (t.name == link.m_dstName[i]) destination = t;
                    }
                    if (source != null && destination != null)
                        Debug.Log($"QA_ROPE {source.name}->{destination.name} src={source.position:F4} dst={destination.position:F4} error={Vector3.Distance(source.position + link.m_offset, destination.position):F6}");
                }

            }
        }
    }
    IEnumerator Describe(string scene)
    {
        yield return new WaitForSecondsRealtime(4);
        Debug.Log($"QA_LIGHTMAPS count={LightmapSettings.lightmaps.Length}");
        Debug.Log($"QA_SCENE {scene} screen={Screen.width}x{Screen.height} safe={Screen.safeArea} timeScale={Time.timeScale} triggers={Physics.queriesHitTriggers}");
        foreach (var c in Camera.allCameras)
            Debug.Log($"QA_CAMERA {c.name} rect={c.pixelRect} ortho={c.orthographicSize} fov={c.fieldOfView} pos={c.transform.position} rot={c.transform.eulerAngles}");
        foreach (var light in FindObjectsByType<Light>(FindObjectsSortMode.None))
            Debug.Log($"QA_LIGHT {light.name} intensity={light.intensity} color={light.color}");
        if (!UIManager.Exists()) yield break;
        var ui = UIManager.instance;
        Debug.Log($"QA_UI type={ui.pointerType} blocked={ui.blockInput} enabled={ui.enabled}");
        foreach (var c in ui.uiCameras)
            Debug.Log($"QA_UI_CAMERA {c.camera} depth={c.rayDepth} mask={c.mask.value}");
        foreach (var b in FindObjectsByType<Collider>(FindObjectsSortMode.None))
        {
            if (b.GetComponent<IUIObject>() == null) continue;
            var c = ui.uiCameras[0].camera;
            Debug.Log($"QA_CONTROL {Path(b.transform)} enabled={b.enabled} trigger={b.isTrigger} layer={b.gameObject.layer} bounds={b.bounds} screen={c.WorldToScreenPoint(b.bounds.center)} stringType={b.GetComponent("IUIObject")} genericType={b.GetComponent<IUIObject>()}");
        }
    }

    static string Path(Transform t) { return t.parent == null ? t.name : Path(t.parent) + "/" + t.name; }

    string lastDisplayState;
    string lastAndroidState;
    float nextWindowSample;

    void OnApplicationPause(bool paused) { Debug.Log($"QA_LIFECYCLE pause={paused}"); }
    void OnApplicationFocus(bool focused) { Debug.Log($"QA_LIFECYCLE focus={focused}"); }

    void DescribeDisplay()
    {
        string state = $"screen={Screen.width}x{Screen.height} safe={Screen.safeArea} orientation={Screen.orientation} display={Display.main.systemWidth}x{Display.main.systemHeight} render={Display.main.renderingWidth}x{Display.main.renderingHeight} dpi={Screen.dpi} focused={Application.isFocused} touchEnabled={Input.multiTouchEnabled}";
        if (lastDisplayState != state)
        {
            lastDisplayState = state;
            Debug.Log("QA_DISPLAY " + state);
            foreach (var camera in Camera.allCameras)
                Debug.Log($"QA_DISPLAY_CAMERA {camera.name} aspect={camera.aspect} rect={camera.pixelRect} ortho={camera.orthographicSize} pos={camera.transform.position}");
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Time.realtimeSinceStartup < nextWindowSample) return;
        nextWindowSample = Time.realtimeSinceStartup + 1;
        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var window = activity.Call<AndroidJavaObject>("getWindow"))
        using (var decor = window.Call<AndroidJavaObject>("getDecorView"))
        using (var resources = activity.Call<AndroidJavaObject>("getResources"))
        using (var config = resources.Call<AndroidJavaObject>("getConfiguration"))
        {
            string android = $"decor={decor.Call<int>("getWidth")}x{decor.Call<int>("getHeight")} configDp={config.Get<int>("screenWidthDp")}x{config.Get<int>("screenHeightDp")} density={config.Get<int>("densityDpi")} windowFocus={decor.Call<bool>("hasWindowFocus")}";
            if (android != lastAndroidState)
            {
                lastAndroidState = android;
                Debug.Log("QA_ANDROID_WINDOW " + android);
            }
        }
#endif
    }

    void Update()
    {
        DescribeDisplay();
        foreach (var t in Input.touches)
        {
            if (t.phase != TouchPhase.Began && t.phase != TouchPhase.Ended) continue;
            Debug.Log($"QA_TOUCH id={t.fingerId} {t.phase} {t.position} screen={Screen.width}x{Screen.height}");
            if (!UIManager.Exists()) continue;
            foreach (var c in UIManager.instance.uiCameras)
            {
                var ray = c.camera.ScreenPointToRay(t.position);
                var hits = Physics.RaycastAll(ray, c.rayDepth, c.mask, QueryTriggerInteraction.Collide);
                foreach (var h in hits)
                    Debug.Log($"QA_HIT {c.camera.name} {Path(h.transform)} distance={h.distance} stringType={h.collider.GetComponent("IUIObject")} genericType={h.collider.GetComponent<IUIObject>()}");
            }
        }
    }
}
#endif
