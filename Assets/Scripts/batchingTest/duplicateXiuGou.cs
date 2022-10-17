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
            for(int i = 0; i < width; i+=1)
            {
                for(int j = 0; j < length; j+=1)
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
