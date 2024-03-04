using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get { return _instance; } }

    [Header("Singleton Settings")]
    /// <summary>
    /// Toggle whether to destroy this object when loading between scenes.
    /// </summary>
    [SerializeField] private bool _persistant;
    protected static T _instance;

    protected void Awake()
    {
        if(Instance != null) 
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            _instance = this as T; 
            if (_persistant)
                DontDestroyOnLoad(gameObject);
        }
    }

}
