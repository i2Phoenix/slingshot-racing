using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class ModernBuild
{
    public static void Configure()
    {
        PlayerSettings.companyName = "Snowbolt Interactive";
        PlayerSettings.productName = "Slingshot Racing Remastered";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.crescentmoongames.slingshotracing.modern");
        PlayerSettings.bundleVersion = "1.4.0.0";
        PlayerSettings.Android.bundleVersionCode = 15;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)36;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Android, ApiCompatibilityLevel.NET_4_6);
        PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.Android, ManagedStrippingLevel.Minimal);
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.colorSpace = ColorSpace.Gamma;
        var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AppIcon.png");
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { icon });
        PlayerSettings.Android.useCustomKeystore = false;
        PlayerSettings.runInBackground = false;
        PlayerSettings.resizableWindow = false;
        PlayerSettings.Android.resizeableActivity = true;
        PlayerSettings.resetResolutionOnWindowResize = true;
        EditorUserBuildSettings.buildAppBundle = false;
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Slingshot/Build modern Android APK")]
    public static void Android()
    {
        BuildAndroid(false);
    }

    [MenuItem("Slingshot/Build development Android APK")]
    public static void AndroidDevelopment()
    {
        BuildAndroid(true);
    }

    static void BuildAndroid(bool development)
    {
        FoldCompatibilityChecks.Run();
        Configure();
        string filename = development ? "SlingshotRacing-modern-arm64-development.apk" : "SlingshotRacing-modern-arm64.apk";
        string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../builds/" + filename));
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = output, target = BuildTarget.Android, options = development ? BuildOptions.Development : BuildOptions.None
        });
        Debug.Log("SLINGSHOT_BUILD_RESULT " + report.summary.result + " " + report.summary.totalErrors + " errors " + output);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Android build failed");
    }

    [MenuItem("Slingshot/Play restored game")]
    public static void Play()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/_Boot.unity");
        EditorApplication.isPlaying = true;
    }
}
