using System.Collections;
using System.Linq;
using Ludiq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class FuTouPipelines
{
    [MenuItem("FuTouPipelines/BuildAndroidLib")]
    static void BuildAndroidLib()
    {
        // EditorCoroutineUtility.StartCoroutineOwnerless(DelayedBolt());
        var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
        // EditorUserBuildSettings.androidBuildType = AndroidBuildType.Debug;
        BuildPlayerOptions buildOptions = new BuildPlayerOptions()
        {
            locationPathName = "/Users/hyperbola/showgo/androidProject/unity",
            extraScriptingDefines = new []{"ANDROID_LIB"},
            options = BuildOptions.AcceptExternalModificationsToPlayer,
            scenes = EditorBuildSettings.scenes.Select(scene=>scene.path).ToArray(),
            target = BuildTarget.Android
        };
        
        addressableSettings.BuildAddressablesWithPlayerBuild =
            AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
        buildAddressableContent();
        BuildPipeline.BuildPlayer(buildPlayerOptions: buildOptions);
    }

    [MenuItem("FuTouPipelines/BuildAndroidWholeMock")]
    static void BuildAndroidWholeMock()
    {
        // EditorCoroutineUtility.StartCoroutineOwnerless(DelayedBolt());
        var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
        BuildPlayerOptions buildOptions = new BuildPlayerOptions()
        {
            locationPathName = "../../Build/AndroidWholeMock",
            extraScriptingDefines = new []{"ANDROID_WHOLE_MOCK"},
            scenes = EditorBuildSettings.scenes.Select(scene=>scene.path).ToArray(),
            target = BuildTarget.Android
        };

        addressableSettings.BuildAddressablesWithPlayerBuild =
            AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
        buildAddressableContent();
        BuildPipeline.BuildPlayer(buildPlayerOptions: buildOptions);
    }
    
    [MenuItem("FuTouPipelines/BuildIosLib")]
    static void BuildIosLib()
    {
        // EditorCoroutineUtility.StartCoroutineOwnerless(DelayedBolt());

        var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
        BuildPlayerOptions buildOptions = new BuildPlayerOptions()
        {
            locationPathName = "/Users/hyperbola/showgo/iOSLibProject/Project",
            extraScriptingDefines = new []{"IOS_LIB"},
            scenes = EditorBuildSettings.scenes.Select(scene=>scene.path).ToArray(),
            target = BuildTarget.iOS
        };

        addressableSettings.BuildAddressablesWithPlayerBuild =
            AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
        buildAddressableContent();
        BuildPipeline.BuildPlayer(buildPlayerOptions: buildOptions);
    }
    
    [MenuItem("FuTouPipelines/BoltBuild")]
    static void BoltBuild()
    {
        AotPreBuilder.PreCloudBuild();
    }
    
    [MenuItem("FuTouPipelines/BuildAddressable")]
    static void BuildAddressable()
    {
        var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
        buildAddressableContent();
    }
    
    static bool buildAddressableContent() {
        AddressableAssetSettings
            .BuildPlayerContent(out AddressablesPlayerBuildResult result);
        bool success = string.IsNullOrEmpty(result.Error);

        if (!success) {
            Debug.LogError("Addressables build error encountered: " + result.Error);
        }
        return success;
    }
    
    [MenuItem("FuTouPipelines/DelayedBoltBuild")]
    static IEnumerator DelayedBolt()
    {
        yield return new EditorWaitForSeconds(5.0f);
         BoltBuild();
         yield return null;
    }
}