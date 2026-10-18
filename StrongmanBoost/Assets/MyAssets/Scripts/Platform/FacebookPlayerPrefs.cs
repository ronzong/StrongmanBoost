#if FB_INSTANT && UNITY_WEBGL
using System.Globalization;
using System.Runtime.InteropServices;

// FB 版即时写入按玩家隔离的存档，避免依赖浏览器退出回调。
public static class FacebookPlayerPrefs
{
#if !UNITY_EDITOR
    [DllImport("__Internal")] private static extern int StrongmanBoost_PrefsHas(string key);
    [DllImport("__Internal")] private static extern string StrongmanBoost_PrefsGet(string key, string fallback);
    [DllImport("__Internal")] private static extern void StrongmanBoost_PrefsSet(string key, string value);
    [DllImport("__Internal")] private static extern void StrongmanBoost_PrefsDelete(string key);
    [DllImport("__Internal")] private static extern void StrongmanBoost_PrefsClear();
    public static bool HasKey(string key) => StrongmanBoost_PrefsHas(key) != 0;
    public static string GetString(string key, string fallback = "") => StrongmanBoost_PrefsGet(key, fallback);
    public static void SetString(string key, string value) => StrongmanBoost_PrefsSet(key, value);
    public static void DeleteKey(string key) => StrongmanBoost_PrefsDelete(key);
    public static void DeleteAll() => StrongmanBoost_PrefsClear();
    public static void Save() { }
#else
    public static bool HasKey(string key) => UnityEngine.PlayerPrefs.HasKey("strongmanboost:" + key);
    public static string GetString(string key, string fallback = "") => UnityEngine.PlayerPrefs.GetString("strongmanboost:" + key, fallback);
    public static void SetString(string key, string value) => UnityEngine.PlayerPrefs.SetString("strongmanboost:" + key, value);
    public static void DeleteKey(string key) => UnityEngine.PlayerPrefs.DeleteKey("strongmanboost:" + key);
    public static void DeleteAll() { }
    public static void Save() => UnityEngine.PlayerPrefs.Save();
#endif
    public static int GetInt(string key, int fallback = 0) => int.TryParse(GetString(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : fallback;
    public static float GetFloat(string key, float fallback = 0f) => float.TryParse(GetString(key), NumberStyles.Float, CultureInfo.InvariantCulture, out float value) ? value : fallback;
    public static void SetInt(string key, int value) => SetString(key, value.ToString(CultureInfo.InvariantCulture));
    public static void SetFloat(string key, float value) => SetString(key, value.ToString("R", CultureInfo.InvariantCulture));
}
#endif
