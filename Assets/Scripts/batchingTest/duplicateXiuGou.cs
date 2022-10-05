using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class duplicateXiuGou : MonoBehaviour
    {

        public GameObject xiugou;

        public int width;

        public int length;
        // Start is called before the first frame update
        void Start()
        {
            for(var i = 0; i < width*2; i+=2)
            {
                for(var j = 0; j < length*2; j+=2)
                {
                    Instantiate(xiugou, new Vector3(i, 0, j), Quaternion.identity);
                    
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
