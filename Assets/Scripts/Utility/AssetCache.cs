using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Utility
{
    public class AssetCache :SingletonGeneric<AssetCache>
    {
        private Dictionary<string, Object> objCache;

        private List<string> cacheInProgress;

        public AssetCache()
        {
            objCache = new Dictionary<string, Object>();
            cacheInProgress = new List<string>();
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
                if (cacheInProgress.Contains(addr))
                {//in progress
                    // return task;
                }
                var loadTask= LoadAssetFromAddressable<T>(addr);
                // cacheInProgress.Add(addr, loadTask);
                var result = await loadTask;
                // cacheInProgress.Remove(addr);
                if (!result)
                {
                    Debug.LogError($"cannot load asset for {addr}");
                    return null;
                }

                if (objCache.ContainsKey(addr))
                {//do not readd asset in cache
                    return objCache[addr] as T;
                }
                objCache.Add(addr, result);
                return result;
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