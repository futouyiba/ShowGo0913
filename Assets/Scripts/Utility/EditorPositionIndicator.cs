using UnityEngine;

namespace ET
{
    [ExecuteInEditMode]
    public class EditorPositionIndicator : MonoBehaviour
    {
        [SerializeField] private Color color;
        [SerializeField] private float length;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            var center = transform.position;
            Vector3 xStart = center;
            xStart.x -= length / 2;
            Vector3 xEnd = center;
            xEnd.x += length / 2;
            Gizmos.DrawLine(xStart, xEnd);
            Vector3 zStart = center;
            zStart.z -= length / 2;
            Vector3 zEnd = center;
            zEnd.z += length / 2;
            Gizmos.DrawLine(zStart, zEnd);
        }
    }
}
