using System.Collections.Generic;
using System.Threading.Tasks;
using ET;
using ShowGo;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Utility
{
    public enum EConfig : ushort
    {
        ShowGoPlayerCfg,
        XGAprcCfg,
        
    }
    
    public class AssetCache :SingletonGeneric<AssetCache>
    {
        private Dictionary<string, Object> objCache;

        private Dictionary<string, AsyncOperationHandle> cacheInProgress;

        private static Dictionary<EConfig, string> configDict = new Dictionary<EConfig, string>()
        {
            {EConfig.ShowGoPlayerCfg, "Cfg/ShowGoPlayerCfg"},
            {EConfig.XGAprcCfg, "Cfg/AprcCfg"},
        };

        public AssetCache()
        {
            objCache = new Dictionary<string, Object>();
            cacheInProgress = new Dictionary<string, AsyncOperationHandle>();

            foreach (var config in configDict)
            {
                CacheAssetFromAddressable(config.Value);
            }
        }


        public async Task<T> GetConfig<T>(EConfig configType) where T:Object
        {
            if (!configDict.TryGetValue(configType, out var path))
            {
                Debug.LogError($"no config for {configType} is found");
                return null;
            }
            return await GetAsset<Object>(path) as T;
            
        }
        
        
        /// <summary>
        /// use this to get asset from cache
        /// </summary>
        /// <param name="addr"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetAsset<T>(string addr) where T:Object
        {
            if (!objCache.TryGetValue(addr, out var obj))
            {
                if (cacheInProgress.TryGetValue(addr, out var handle))
                {//in progress
                    return await WaitForCache<T>(addr);
                    // if (objCache.TryGetValue(addr, out var justCached))
                    // {
                    //     return justCached as T;
                    // }
                }

                await CacheAssetFromAddressable(addr);
                // var loadTask= LoadAssetFromAddressable<T>(addr);
                // cacheInProgress.Add(addr, loadTask);
                // var result = await loadTask;
                // cacheInProgress.Remove(addr);
                // if (!result)
                // {
                //     Debug.LogError($"cannot load asset for {addr}");
                //     return null;
                // }
                if (objCache.TryGetValue(addr, out obj))
                {//do not readd asset in cache
                    return obj as T;
                }
                // objCache.Add(addr, result);
            }
            return obj as T;
        }


        // private async Task<T> LoadAssetFromAddressable<T>(string addr) where T: Object
        // {
        //     var operationHandle = Addressables.LoadAssetAsync<T>(addr);
        //     await operationHandle.Task;
        //     if (operationHandle.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         var result = operationHandle.Result;
        //         objCache.Add(addr, result);
        //         return result;
        //     }
        //     Debug.LogError($"load failed for asset{addr}");
        //     return null;
        // }

        protected Task<T> LoadAssetFromAddressable<T>(string addr) where T: Object
        {
            var tcs = new TaskCompletionSource<T>();
            var operationHandle = Addressables.LoadAssetAsync<T>(addr);
            operationHandle.Completed += handle =>
            {
                tcs.SetResult(handle.Result);
            };
            
            // Debug.LogError($"load failed for asset{addr}");
            return tcs.Task;
        }


        protected Task CacheAssetFromAddressable(string addr)
        {
            var tcs = new TaskCompletionSource<Object>();
            
            var operationHandle = Addressables.LoadAssetAsync<Object>(addr);
            cacheInProgress.Add(addr,operationHandle);
            operationHandle.Completed += handle =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    objCache.Add(addr, handle.Result);
                }
                cacheInProgress.Remove(addr);
                tcs.SetResult(handle.Result);
            };
            return tcs.Task;
        }

        protected async Task<T> WaitForCache<T>(string addr) where T:Object
        {
            if (cacheInProgress.TryGetValue(addr, out var handle))
            {
                await handle.Task;
                return handle.Result as T;
            }
            //not in progress, try get immediately
            if (objCache.TryGetValue(addr, out var obj))
            {
                return obj as T;
            }
            //not in anywhere
            Debug.LogError($"{addr} is not in cache or in progress");
            return null;
        }
        
        // public T GetAssetCoroutine<T>(string addr) where T : Object
        // {
        //     
        // }
        
        
        // private IEnumerator LoadAssetFromAddressableCoroutine<T>(string addr, Action callback) where T : Object
        // {
        //     var operationHandle = Addressables.LoadAssetAsync<T>(addr);
        //     operationHandle.Completed += callback;
        //     yield return operationHandle;
        //     if (operationHandle.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         var result = operationHandle.Result;
        //         objCache.Add(addr, result);
        //     }
        //     else
        //     {
        //         Debug.LogError($"load failed for asset{addr}");    
        //     }
        //     
        // }

    }

}