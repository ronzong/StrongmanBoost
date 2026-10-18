using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

#if UNITY_EDITOR

public class RemoveMissingScripts : Editor
    {
        [MenuItem("GameObject/Remove Missing Scripts")]

public static void Remove()
        {
            var objs = Resources.FindObjectsOfTypeAll<GameObject>();
            int count = objs.Sum(GameObjectUtility.RemoveMonoBehavioursWithMissingScript);
            Debug.Log($"Removed {count} missing scripts");
        }
    }
#endif


