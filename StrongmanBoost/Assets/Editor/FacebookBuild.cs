using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class FacebookBuild
{
    [Serializable] public class ExternalConfig
    {
        public string appId, interstitialPlacementId, rewardedPlacementId;
    }
    [Serializable] private class PackageInfo
    {
        public string appId, interstitialPlacementId, rewardedPlacementId, unityVersion, builtAtUtc, zipPath, zipSha256;
        public string sdkVersion = "8.0", pluginCommit = "ec32ba76dc3716b3a08e4ebe15e5b5725488e005";
        public bool bannerEnabled, iapEnabled, uploaded, publishedPreview;
        public long zipBytes;
        public string[] localValidation = new string[0];
        public string realFacebookValidation = "Pending manual Facebook platform testing";
    }
    private static string Repo => Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;

    // 只设置 WebGL 参数；Android/iOS 的宏和广告路径保持独立。
    [MenuItem("Build/StrongmanBoost/Facebook Instant Games")]
    public static void Build()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, "FB_INSTANT");
        PlayerSettings.WebGL.template = "PROJECT:FB";
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        // SDK 用异常返回取消/无填充；禁用异常会让 WebAssembly 直接 abort。
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
        PlayerSettings.WebGL.dataCaching = false;
        PlayerSettings.runInBackground = true;
        PlayerSettings.defaultWebScreenWidth = 540;
        PlayerSettings.defaultWebScreenHeight = 960;
        PlayerSettings.stripEngineCode = true;
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.Low);
        string output = Path.Combine(Repo, "Builds/Facebook/web");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { "Assets/MyAssets/Scene/Game.unity" },
            locationPathName = output, target = BuildTarget.WebGL, options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Facebook build failed: " + report.summary.result);
        Repackage();
    }

    // 外部配置打包时复制到网页旁；换 ID 仅重新压缩，不需要重新编译 Unity。
    [MenuItem("Build/StrongmanBoost/Repackage With External Config")]
    public static void Repackage()
    {
        string json = File.ReadAllText(Path.Combine(Repo, "Tools/facebook-config.json"));
        var config = JsonUtility.FromJson<ExternalConfig>(json);
        if (config == null || !Regex.IsMatch(config.appId ?? "", @"^\d+$") ||
            !Regex.IsMatch(config.interstitialPlacementId ?? "", "^" + config.appId + @"_\d+$") ||
            !Regex.IsMatch(config.rewardedPlacementId ?? "", "^" + config.appId + @"_\d+$"))
            throw new Exception("Invalid external Facebook configuration");
        string output = Path.Combine(Repo, "Builds/Facebook/web");
        if (!File.Exists(Path.Combine(output, "index.html"))) throw new Exception("Build the game first");
        File.WriteAllText(Path.Combine(output, "facebook-config.json"), json, new UTF8Encoding(false));
        string zip = Path.Combine(Repo, "Builds/Facebook/StrongmanBoost-Facebook-" + config.appId + ".zip");
        if (File.Exists(zip)) File.Delete(zip);
        using (var archive = ZipFile.Open(zip, ZipArchiveMode.Create))
        foreach (string file in Directory.GetFiles(output, "*", SearchOption.AllDirectories))
        {
            string relative = file.Substring(output.Length + 1).Replace('\\', '/');
            if (relative.StartsWith("ButDontShipItWithYourGame") || relative.EndsWith(".symbols.json")) continue;
            archive.CreateEntryFromFile(file, relative, System.IO.Compression.CompressionLevel.Optimal);
        }
        var info = new PackageInfo {
            appId = config.appId, interstitialPlacementId = config.interstitialPlacementId,
            rewardedPlacementId = config.rewardedPlacementId, unityVersion = Application.unityVersion,
            builtAtUtc = DateTime.UtcNow.ToString("o"), zipPath = "Builds/Facebook/" + Path.GetFileName(zip), zipBytes = new FileInfo(zip).Length
        };
        using (var sha = SHA256.Create()) using (var input = File.OpenRead(zip))
            info.zipSha256 = BitConverter.ToString(sha.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
        File.WriteAllText(Path.Combine(Repo, "Tools/facebook-build-info.json"), JsonUtility.ToJson(info, true), new UTF8Encoding(false));
        FacebookSiteExport.Export(output);
        Debug.Log("Facebook package: " + zip);
    }
}
