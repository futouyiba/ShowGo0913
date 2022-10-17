using System;
using Unity.Mathematics;
// using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ET
{
    /// <summary>
    /// store static methods for dance pool
    /// </summary>
    public class DanceFloorHelper_XGYD
    {
        public static GameObject GetGoFromScene(string tag)
        {
            var goFind = GameObject.FindGameObjectWithTag(tag);
            if (goFind == null)
            {
                Debug.LogError($"no such gameobject with tag: {tag}");
                return null;
            }

            return goFind;
        }

        public static float GetPivotY()
        {
            var pivot = GetGoFromScene("pivot").GetComponent<DanceFloorPivot_XGYD>();
            return pivot.transform.position.y;
        }

        public static Vector3 BuildWorldPosition(Vector2 scenePos)
        {
            Vector3 result=new Vector3(scenePos.x,Single.NegativeInfinity, scenePos.y);
            var pivot = GetGoFromScene("pivot").GetComponent<DanceFloorPivot_XGYD>();
            Vector3 rayOrigin = new Vector3(scenePos.x, pivot.high.position.y, scenePos.y);
            var hits = Physics.RaycastAll(rayOrigin, Vector3.down, Single.MaxValue);
            foreach (var hit in hits)
            {
                if(!hit.transform.CompareTag("Ground")) continue;
                else
                {
                    result.y = hit.point.y;
                    // Debug.Log($"hitting position is {result}, ray is hitting{hit.collider.gameObject.name},ray origin is {rayOrigin}, scene pos is {scenePos}");
                    break;
                }
            }

            
            if (result.y < -1000f)
            {
                Debug.LogWarning($"not hitting any ground");
                return Vector3.negativeInfinity;
            }
            
            return result;
        }

        public static Vector2 PosUnified2Scene(Vector2 unifiedPos)
        {
            // Debug.LogWarning($"uni pos is {unifiedPos}");
            var pivot = GetGoFromScene("pivot").GetComponent<DanceFloorPivot_XGYD>();
            var pos0 = pivot.small.position;
            var pos1 = pivot.big.position;
            var targetX = math.lerp(pos0.x, pos1.x, unifiedPos.x);
            var targetZ = math.lerp(pos0.z, pos1.z, unifiedPos.y);
            var targetPos = new Vector2(targetX, targetZ);
            return targetPos;
        }

        public static Vector3 PosUnifiedPolar2Scene(Vector2 unifiedPos)
        {
            var pivot = GetGoFromScene("pivot").GetComponent<DanceFloorPivot_XGYD>();
            float randomR = unifiedPos.x * pivot.cur_Radius;
            float randomA = unifiedPos.y * 360f;
            var randomDir = Quaternion.AngleAxis(randomA, Vector3.up) * Vector3.right;
            var offsetVec = randomDir * randomR;
            return pivot.center.position + offsetVec;
        }

        public static Vector2 PosScene2Unified(Vector2 scenePos)
        {
            var pivot = GetGoFromScene("pivot").GetComponent<DanceFloorPivot_XGYD>();
            var pos0 = pivot.small.position;
            var pos1 = pivot.big.position;

            var resultX = math.unlerp(pos0.x, pos1.x, scenePos.x);
            var resultY = math.unlerp(pos0.z, pos1.z, scenePos.y);
            Vector2 result = new Vector2(resultX, resultY);
            return result;
        }

        public static Vector2 GetRandomDanceFloorPos()
        {
            float randomX = Random.Range(0f, 1f);
            float randomY = Random.Range(0f, 1f);
            // Debug.Log($"randomed pos is {randomX},{randomY}");
            return new Vector2(randomX, randomY);
        }

        // public Vector3 GetUpgradableRandomPos()
        // {
        //     var floor = GameObject.FindWithTag("Ground");
        //     var bounds = floor.GetComponent<MeshFilter>().mesh.bounds;
        //     Bounds smaller = bounds;
        //     smaller.extents = bounds.extents * 0.6f;
        //     var x = Mathf.Lerp(bounds.min.x, bounds.max.x, Random.Range(0, 1f));
        //     var z = Mathf.Lerp(bounds.min.z, bounds.max.z, Random.Range(0, 1f));
        //     Ray r = new Ray()
        //     {
        //         direction = Vector3.down,
        //         origin = new Vector3(x, 500f, z)
        //     };
        //     if (RaycastHit())
        //     {
        //         
        //     }
        // }

        // public static Vector2 GetRandomDanceFloorPosRadius()
        // {
        //     
        // }
        
    }
}
