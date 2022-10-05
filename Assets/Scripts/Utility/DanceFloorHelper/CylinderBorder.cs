using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace ET
{
    public class CylinderBorder : MonoBehaviour
    {
        [OdinSerialize] public List<float> Scales;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public float ScaleTo(int id)
        {
            if (id < 0 || id >= Scales.Count)
            {
                Debug.LogError($"scale {id} does not exist");
                return -1f;
            }
            transform.localScale = Vector3.one * Scales[id];
            return Scales[id];
        }
    }
}
