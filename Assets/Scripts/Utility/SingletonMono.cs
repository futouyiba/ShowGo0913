using System;
using System.Security.Cryptography;
using Mirror;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// if T is monobehaviour or NetworkBehaviour, dont use this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonGeneric<T> where T : class, new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance==null)
                {
                    _instance = new T();
                }
                return _instance;
            }

            private set
            {
            }
        }
    }
    
    
    public abstract class SingletonMono<T>:MonoBehaviour where T:MonoBehaviour 
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    Debug.LogError($"instance does not exists, config in the scene");
                }
                return _instance;
            }
            
            private set{}
        }

        protected virtual void Awake()
        {
            if (!_instance) _instance = this as T;
            else
            {
                Debug.LogError($"instance already exists for {(this as T).gameObject.name}");
                GameObject.Destroy((this as T).gameObject);
            }
        }
    }

    public abstract class SingletonNetworkBehaviour<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    Debug.LogError($"instance does not exists, config in the scene");
                }
                return _instance;
            }
            
            private set{}
        }

        void Awake()
        {
            if (!_instance) _instance = this as T;
            else
            {
                Debug.LogError($"instance already exists for {(this as T).gameObject.name}");
                GameObject.Destroy((this as T).gameObject);
            }
        }

        protected virtual void Update()
        {
            
        }
    }
}

