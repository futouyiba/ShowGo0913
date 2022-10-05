using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

namespace ET
{
    /// <summary>
    /// 角色跑火车
    /// 这一部分在角色身上，纯粹的客户端行为
    /// 经过修改依然使用
    /// </summary>
    public class TrainState : MonoBehaviour
    {
        #region 变量
        private int index = -1;//在列表中的位置
        private Vector3 target;//移动到的目标点
        private float speed = 5;//移动速度
        private float rotateSpeed = 70;//旋转速度
        private float space = 0.3f;//角色间距
        private Vector3 InitPos;//角色的初始位置
        private bool isExit = false;//是否退出跑圈

        public Transform chasingTarget = null;
        // public List<Transform> runCharacterList;//跑圈角色列表 
        #endregion

        #region 属性
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        public bool IsExit
        {
            get { return isExit; }
            private set { }
        } 
        #endregion

        private void Start()
        {
            // runCharacterList = TrainSystem.instance.runCharacterList;
        }

        private void Update()
        {
            if (isExit)
            {
                ExitTrain();
            }
        }

        #region Train状态用到的3个方法

        public void JoinTrainInit(int index, Transform chasingTarget)
        {
            if (chasingTarget == null && index != 0)
            {
                Debug.LogError($"adding index= {index} with chasing target=null");
                return;
            }
            
            isExit = false;
            InitPos = transform.position;//记录run前的最后位置
            this.index = index;//设置所在跑酷list的index
            this.chasingTarget = chasingTarget;
            var fsm = GetComponent<StateMachine>();
            if(fsm.enabled) fsm.TriggerUnityEvent("TrainStart");
            // runCharacterList.Add(this.transform);//添加到跑酷队列
        }
        // public void OnStart()
        // {
        //     isExit = false;
        //     InitPos = transform.position;//记录run前的最后位置
        //     index = runCharacterList.Count;//设置所在跑酷list的index
        //     runCharacterList.Add(this.transform);//添加到跑酷队列
        // }
        // public void OnUpdate()
        // {
        //     // TrainPosUpdate(TrainSystem.instance.transform, TrainSystem.instance.Radius);
        // }
        public void OnExit()
        {
            Reset();
            var fsm = GetComponent<StateMachine>();
            if(fsm.enabled) fsm.TriggerUnityEvent("TrainEnd");
        }
        #endregion


        #region 加入/退出  跑圈
        /// <summary>
        /// 加入火车
        /// </summary>
        /// <param name="center"></param>
        /// <param name="r"></param>
        /// <param name="runCharacterList"></param>
        public void TrainPosUpdate(Transform center, float r)
        {
            if (index == 0)
            {
                target = center.position + (transform.position - center.position).normalized * r;
                target.y = transform.position.y;
                Vector3 dir = (target - transform.position).normalized;
                if (Vector3.Distance(transform.position, target) > 0.02f)
                {
                    transform.Translate(dir * speed * Time.deltaTime, Space.World);
                }
                else 
                {
                    transform.RotateAround(center.position, Vector3.up, rotateSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.LookRotation(Vector3.forward);
                }
            }
            else if (index > 0)
            {
                if (Vector3.Distance(transform.position, chasingTarget.position) > space)
                {
                    transform.Translate((chasingTarget.position - transform.position).normalized * speed * Time.deltaTime, Space.World);
                }
                else
                {
                    Vector3 target = center.position + (transform.position - center.position).normalized * r;
                    target.y = transform.position.y;
                    Vector3 dir = (target - transform.position).normalized;
                    if (Vector3.Distance(transform.position, target) > 0.1f)
                    {
                        transform.Translate(dir * speed * Time.deltaTime, Space.World);
                    }
                }

            }
        }

        /// <summary>
        /// 退出火车
        /// </summary>
        private void ExitTrain()
        {

            Vector3 dir = (InitPos - transform.position).normalized;
            if (Vector3.Distance(transform.position, InitPos) > 0.02f)
            {
                transform.Translate(dir * speed * Time.deltaTime, Space.World);
            }
            else
            {
                isExit = false;
            }
        } 
        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            index = -1;
            isExit = true;
        }
    }
}
