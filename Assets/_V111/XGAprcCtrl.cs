using System.Collections.Generic;
using System.Threading.Tasks;
using ShowGo;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Utility;
using ZEGO;

namespace ShowGo
{
    public class XGAprcCtrl :SerializedMonoBehaviour
    {
        public Transform bodies;
        public Transform decors;

        [OdinSerialize] protected Dictionary<string, Transform> decorDict;

        protected Transform enabledBody;

        public async void LvChanged(int roomLv)
        {
            var cfg = await AssetCache.Instance.GetConfig<XGAprcCfg>(EConfig.XGAprcCfg);
            var baseInfo = cfg.GetBaseInfo(roomLv);
            if (baseInfo == null)
            {
                return;
            }
            
            BodiesOff();
            DecorsOff();
            EnableBody(baseInfo.bodyName);
            if (!string.IsNullOrWhiteSpace(baseInfo.decorName))
            {
                EnableDecor(baseInfo.decorName);
            }
        }
        
        
        /// <summary>
        /// enable body by its name
        /// disable all other bodies
        /// </summary>
        /// <param name="id"></param>
        public void EnableBody(string id)
        {
            foreach (Transform bodyTransform in bodies)
            {
                PartOnOff(bodyTransform, bodyTransform.name == id);
                if (bodyTransform.name == id) enabledBody = bodyTransform;
            }
        }
        
        /// <summary>
        /// enable decor part by its name
        /// disable all others in the same group
        /// </summary>
        /// <param name="id"></param>
        public async void EnableDecor(string id)
        {
            if (!decorDict.TryGetValue(id, out Transform decorTransform))
            {
                Debug.LogError($"decor named {id} does not exist");
                return;
            }
            
            //validate decor
            if (!await ValidateDecor(id))
            {
                Debug.LogWarning($"{id} decor is not valid for current body");
                return;
            } 
            
            foreach (Transform decorInGroup in decorTransform.parent)
            {
                if (decorInGroup != decorTransform)
                {
                    PartOnOff(decorInGroup, false);
                }
                else
                {
                    PartOnOff(decorInGroup, true);
                }
            }
        }

        private void PartOnOff(Transform partTransform, bool isOn)
        {
            var meshRenderer = partTransform.GetComponent<MeshRenderer>();
            meshRenderer.enabled = isOn;
        }

        /// <summary>
        /// see if the decor is valid for this body
        /// </summary>
        /// <param name="decorName"></param>
        private async Task<bool> ValidateDecor(string decorName)
        {
            if (enabledBody)
            {
                var cfg = await AssetCache.Instance.GetConfig<XGAprcCfg>(EConfig.XGAprcCfg);
                if (!cfg.allowed_decors.TryGetValue(enabledBody.name, out var result))
                {
                    Debug.LogError($"no validation info found for {decorName}");
                    return false;
                }
                return result.Contains(decorName);
            }
            return false;
        }

        protected void BodiesOff()
        {
            foreach (Transform body in bodies)
            {
                PartOnOff(body, false);
            }

            enabledBody = null;

        }

        protected void DecorsOff()
        {
            foreach (var decor in decorDict)
            {
                PartOnOff(decor.Value, false);
            }
        }
        
        
        
        [Button("RefreshAprcStruct")]
        public void UpdateConfig()
        {
            decorDict ??= new Dictionary<string, Transform>();
            foreach (Transform decorGroup in decors)
            {
                foreach (Transform decor in decorGroup)
                {
                    if (decorDict.TryGetValue(decor.name, out var result))
                    {
                        Debug.LogError($"decor named {decor.name} already exists");
                        continue;
                    }
                    decorDict.Add(decor.name, decor);
                }
            }
        }

    }
}