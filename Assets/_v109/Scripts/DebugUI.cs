using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShowGo;
using DG.Tweening;
using Mirror;
using ET.Utility;
using System;


public class DebugUI : MonoBehaviour 
{
    [SerializeField] private RectTransform chitchatInputField;
    private TMP_InputField chitchat_InputField;
    private Button chitchat_But;
    private Button packUp;

    [SerializeField]private bool isChitchat = false;

    //[Tooltip("持续显示时间")][SerializeField]private float showTime;

    #region 单例
    public static DebugUI Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //防止转场时销毁单例对象
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            //销毁多余的单例
            Destroy(this.gameObject);
            return;
        }
    }
    #endregion


    void Start()
    {
        chitchat_InputField = chitchatInputField.Find("InputField").GetComponent<TMP_InputField>();
        chitchat_But = chitchatInputField.Find("Button").GetComponent<Button>();
        packUp = chitchatInputField.Find("PackUp").GetComponent<Button>();

        packUp.onClick.AddListener(() =>
        {
            isChitchat = false;
            ShowChitchatInputField();
        });

        OnChitchat_But(NetworkClient.localPlayer.GetComponent<ShowGoPlayer>().UserId);

        MeDJ.onClick.AddListener(() => SetMeDJ());
        RandDJ.onClick.AddListener(() => SetRandDJ());
        KickOffDJ.onClick.AddListener(() => DoKickOffDJ());

    }


    float time;
    int i = 0;
    void Update()
    {
        //连续点击弹出
        if (!isChitchat)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (i == 0)
                {
                    time = Time.time;
                }

                i++;
                if (Time.time - time > 1f)
                {
                    i = 0;
                }
                time = Time.time;

                if (i >= 5)
                {
                    isChitchat = true;
                    ShowChitchatInputField();
                    time = 0;
                    i = 0;
                }
            }
        }
        //else
        //{
        //    time += Time.deltaTime;
        //    if (time > showTime)
        //    {
        //        isChitchat = false;
        //        ShowChitchatInputField();
        //        time = 0;
        //    }
        //}
    }


    public void OnChitchat_But(uint userId)
    {
        chitchat_But.onClick.AddListener(() =>
        {
            SceneShowgoMgr.Instance.GetUser(userId).GetComponent<ShowGoPlayer>().SendChatMsg(userId, chitchat_InputField.text);
            chitchat_InputField.text = "";
        });
    }

    private void ShowChitchatInputField() 
    {
        if (isChitchat)
        {
            StartDOTween(chitchatInputField, -540, 0.3f);
        }
        else
        {
            StartDOTween(chitchatInputField, -1290, 0.3f);
        }
    }

    private void StartDOTween(RectTransform rtf, float pos,float DTTime) 
    {
        rtf.DOLocalMoveX(pos, DTTime).SetEase(Ease.Linear) ;
    }

    public Button MeDJ;
    public Button RandDJ;
    public Button KickOffDJ;
    private void SetMeDJ()
    {
        DJChanged changed = new DJChanged()
        {
            oldDJ = SceneShowgoMgr.Instance.currentDJ,
            newDJ = (int)SceneShowgoMgr.Instance.Me.UserId,
            ts = DateTime.UtcNow.Millisecond
        };
        var msg = NativeUtility.MakeOp(changed);
        ContinentalMessenger.Instance.Native2UnityMsg(msg);
    }
    private void SetRandDJ()
    {
        DJChanged changed = new DJChanged()
        {
            oldDJ = SceneShowgoMgr.Instance.currentDJ,
            newDJ = (int)SceneShowgoMgr.Instance.GetRandomUser().UserId,
            ts = DateTime.UtcNow.Millisecond
        };
        var msg = NativeUtility.MakeOp(changed);
        ContinentalMessenger.Instance.Native2UnityMsg(msg);
    }

    private void DoKickOffDJ()
    {
        DJChanged changed = new DJChanged()
        {
            oldDJ = SceneShowgoMgr.Instance.currentDJ,
            newDJ = -1,
            ts = DateTime.UtcNow.Millisecond
        };
        var msg = NativeUtility.MakeOp(changed);
        ContinentalMessenger.Instance.Native2UnityMsg(msg);
    }
}
