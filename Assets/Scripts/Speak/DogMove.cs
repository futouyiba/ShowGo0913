using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ET
{
    public class DogMove : MonoBehaviour
    {
        [Tooltip("ËÙ¶È")][SerializeField]private float speed;

        private bool isMove = true;
        private Tween tw;

        private void Move(Vector3 pos, float distance) 
        {
            tw = transform.DOLocalMove(pos, distance / speed).SetEase(Ease.Linear);
        }

        private void RandomPos() 
        {
            Vector3 pos = new Vector3(Random.Range(-6, 6), 0, Random.Range(0.5f, 5));
            float distance = Vector3.Distance(transform.position, pos);
            if ( distance < 3)
            {
                 RandomPos();
            }
            else
            {
                Move(pos, distance) ;
            }

        }

        IEnumerator Start() 
        {
            while (true)
            {
                if (isMove)
                {
                    yield return new WaitForSeconds(Random.Range(10, 20));

                    RandomPos();
                }
                yield return null;
            }
        }


        // ÔÝÍ£ÒÆ¶¯
        //public void PlayStopMove()
        //{
        //    tw.Kill();
        //    isMove = !isMove;
        //}
    }
}
