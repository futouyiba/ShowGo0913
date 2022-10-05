using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ET
{
    public class Speak : MonoBehaviour
    {
        private RectTransform uiCan;

        private GameObject uiSpeak;
        private Image uiChat;
        private Transform uiRoom;
        private TextMeshProUGUI uiText;
        
        public float duration;      //需要持续的时间
        public float passedTime;    //这个speak 已经持续的时间 

        public bool isCanRemoved;   //是否可以显示房间号
        public bool isRemoved;      //是否显示房间号
        public bool isFaYan;     //是否在发言
        private Vector2 SpeakPos;

        //public DogMove dm;

        public void Init(RectTransform uiCan,GameObject uiSpeak,int ID) 
        {
            this.uiCan = uiCan;
            this.uiSpeak = uiSpeak;
            isCanRemoved = true;
            //dm = GetComponent<DogMove>();
        }

        private void InitPrefab()
        {
            uiSpeak = Instantiate(uiSpeak, UIPos(), Quaternion.identity);
            uiSpeak.transform.SetParent(uiCan, false);

            uiChat = uiSpeak.transform.Find("Chat").GetComponent<Image>();
            uiRoom = uiSpeak.transform.GetChild(1);
            uiText = uiChat.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        }

        Vector3 ve3;
        //世界坐标转UI坐标
        private Vector2 UIPos() 
        {
            ve3 = Camera.main.WorldToScreenPoint(transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCan, ve3, null, out Vector2 ve2);
            ve2 += new Vector2(0, 240);
            return ve2;
        }

        //public void GengHuanSpeak(int ID)
        //{
        //    uiRoom = uiSpeak.transform.GetChild(ID);
        //    Debug.Log(uiRoom.name);
        //    //uiRoom.sprite = spr;
        //    //uiRoom.SetNativeSize();
        //}

        Tween tew;
        //显示
        public void Show(int ID)
        {
            uiRoom = uiSpeak.transform.GetChild(ID);
            

            if (uiText == null) 
            {
                InitPrefab();
            }
            else
            {
                uiSpeak.gameObject.SetActive(true);
            }
            uiRoom.gameObject.SetActive(true);
            ShowTween();
        }

        

        //隐藏
        public void Conceal()
        {
            isRemoved = false;
            duration = 0;
            passedTime = 0;
            if (isFaYan) return;
            ConcealTween();
        }

        private void ShowTween()
        {
            uiSpeak.transform.DOScale(Vector3.one, 1f).OnComplete(() =>
            {
                if (!isFaYan)
                {
                    InvokeRepeating("PlayShowTween", 0,3);
                   
                }
                else
                {
                    StopShowTween();
                }
            });
        }

        private void PlayShowTween() 
        {
            //tew = (uiSpeak.transform.DOPunchRotation(new Vector3(0, 0, 5), 0.1f)).SetLoops(2, LoopType.Yoyo);

        }

        private void StopShowTween() 
        {
            CancelInvoke("PlayShowTween");
            tew.Kill();
        }

        private void ConcealTween() 
        {
            uiSpeak.transform.DOScale(new Vector3(0, 0, 0), 0.5f).OnComplete(() =>
            {
                StopShowTween();
                uiSpeak.transform.localRotation = Quaternion.Euler(Vector3.zero);

                uiChat.gameObject.SetActive(false);
                uiRoom.gameObject.SetActive(false);
                uiSpeak.gameObject.SetActive(false);

                isCanRemoved = true;//设为可以显示房间
                //dm.PlayStopMove();
            });
        }

        private void Update()
        {
            if (SpeakPos != UIPos()) 
            {
                SpeakPos = UIPos();
                uiSpeak.transform.localPosition = SpeakPos;
            }
            

            //发言的计时器
            if (isFaYan)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    isFaYan = false;

                    Conceal();
                }
            }
        }

        #region 发言
        private float timer;
        public void FaYan(string str)
        {
            isFaYan = true;
            isCanRemoved = false;

            if (uiText == null)
            {
                InitPrefab();
            }
            else if (isRemoved)
            {
                duration = 0;
                passedTime = 0;
                isRemoved = false;
                uiRoom.gameObject.SetActive(false);

            }
            else
            {
                uiSpeak.gameObject.SetActive(true);
            }
            uiChat.gameObject.SetActive(true);
            ShowTween();

            uiText.text = str;
            timer = 5;
        }
        #endregion

        

    }
}
