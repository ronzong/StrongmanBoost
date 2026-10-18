using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class FacebookSiteExport
{
    [Serializable] private class SiteFile { public string path; public string sha256; }
    [Serializable] private class SiteManifest
    {
        public string game;
        public string appId;
        public string unityVersion;
        public string builtAtUtc;
        public string sdkMode = "preview";
        public string sdkVersion = "8.0";
        public string pluginCommit = "ec32ba76dc3716b3a08e4ebe15e5b5725488e005";
        public List<SiteFile> files = new List<SiteFile>();
    }

    [MenuItem("Build/StrongmanBoost/Sync Existing Build To Site")]
    public static void ExportExistingBuild()
    {
        string project = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
        Export(Path.Combine(project, "Builds/Facebook/web"));
    }

    public static void Export(string source)
    {
        string project = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
        string siteRoot = Path.Combine(Directory.GetParent(project).FullName, "site");
        string destination = Path.GetFullPath(Path.Combine(siteRoot, "StrongmanBoost"));
        string index = File.ReadAllText(Path.Combine(source, "index.html"));
        string mock = File.ReadAllText(Path.Combine(project, "Tools/local-fb-sdk.js"));
        const string sdk = "https://connect.facebook.net/en_US/fbinstant.8.0.js";
        if (!index.Contains(sdk)) throw new Exception("FB SDK script was not found; preview export aborted.");
        // 使用相对路径，兼容 GitHub Pages 的仓库名和游戏名两层目录。
        index = index.Replace(sdk, "./local-fb-sdk.js").Replace("<title>", "<title>[Preview] ");
        string manifestPath = Path.Combine(destination, "build-info.json");
        SiteManifest previous = File.Exists(manifestPath)
            ? JsonUtility.FromJson<SiteManifest>(File.ReadAllText(manifestPath)) : null;
        var manifest = new SiteManifest {
            game = "StrongmanBoost", appId = JsonUtility.FromJson<FacebookBuild.ExternalConfig>(File.ReadAllText(Path.Combine(source, "facebook-config.json"))).appId, unityVersion = Application.unityVersion,
            builtAtUtc = DateTime.UtcNow.ToString("o")
        };
        Directory.CreateDirectory(destination);
        foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories).OrderBy(x => x))
        {
            string relative = file.Substring(source.Length + 1).Replace('\\', '/');
            if (relative.StartsWith("ButDontShipItWithYourGame") || relative.EndsWith(".symbols.json")) continue;
            byte[] contents = relative == "index.html" ? Encoding.UTF8.GetBytes(index) : File.ReadAllBytes(file);
            Write(destination, relative, contents, manifest);
        }
        Write(destination, "local-fb-sdk.js", Encoding.UTF8.GetBytes(mock), manifest);
        // 只清理上一次导出记录的过期文件，保留其他游戏和仓库元数据。
        var currentFiles = new HashSet<string>(manifest.files.Select(file => file.path));
        if (previous != null && previous.game == manifest.game && previous.files != null)
        {
            foreach (SiteFile file in previous.files)
            {
                if (currentFiles.Contains(file.path)) continue;
                string stale = Resolve(destination, file.path);
                if (File.Exists(stale)) File.Delete(stale);
            }
        }
        File.WriteAllText(manifestPath, JsonUtility.ToJson(manifest, true), new UTF8Encoding(false));
        string noJekyll = Path.Combine(siteRoot, ".nojekyll");
        if (!File.Exists(noJekyll)) File.WriteAllText(noJekyll, "");
        Debug.Log("Static preview synced: " + destination);
    }

    private static string Resolve(string root, string relative)
    {
        string target = Path.GetFullPath(Path.Combine(root, relative));
        if (!target.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
            relative.Split('/', '\\').Any(part => part.StartsWith(".")))
            throw new Exception("Invalid preview output path: " + relative);
        return target;
    }

    private static void Write(string root, string relative, byte[] contents, SiteManifest manifest)
    {
        string target = Resolve(root, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(target));
        File.WriteAllBytes(target, contents);
        using (var hash = SHA256.Create())
            manifest.files.Add(new SiteFile {
                path = relative, sha256 = BitConverter.ToString(hash.ComputeHash(contents)).Replace("-", "").ToLowerInvariant()
            });
    }
}
