using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ShowGo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PuppyCardBut : MonoBehaviour
{
    private Button puppyCardBut;
    private GameObject puppyCardBut_heartbeat;
    [Tooltip("��Ƭ�Ƿ���Ե��")]private bool isState = true;
    [SerializeField] private TextMeshProUGUI Nickname;
    [SerializeField] private TextMeshProUGUI Detail;
    [SerializeField] private TextMeshProUGUI Desc;
    [SerializeField] private Image GenderImg;
    [SerializeField] private Sprite FemaleSpr;
    [SerializeField] private Sprite MaleSpr;

    [SerializeField] private Image HeadPic;
    
    
    private void Awake()
    {
        puppyCardBut = transform.Find("puppyCardBut").GetComponent<Button>();
        puppyCardBut_heartbeat = transform.Find("puppyCardBut/heartbeat").gameObject;
    }

    /// <summary>
    /// �޸�״̬
    /// </summary>
    public void alterStart(bool bo) 
    {
        isState = bo;
        OnRoomCardBut();
    }

    void Start()
    {
        // roomCardBut.onClick.AddListener(OnRoomCardBut);
        
    }

    public void AddBtnFunc(Action action)
    {
        if (action == null) return;
        puppyCardBut.onClick.AddListener(action.Invoke);
    }
    

    //�޸ķ��俨Ƭ״̬
    private void OnRoomCardBut()
    {
        puppyCardBut.interactable = isState;
        puppyCardBut_heartbeat.SetActive(isState);
    }


    public void SetNickname(string name)
    {
        Nickname.SetText(name);
    }

    public void SetDetail(bool isMale, int age, string constellation)
    {
        GenderImg.sprite = isMale ? MaleSpr : FemaleSpr;
        string detailText = $"{age},{constellation}";
        // Detail.SetText(detailText);
    }

    public void SetDesc(string desc)
    {
        Desc.SetText(desc);
    }

    public void SetHeadPic(string url)
    {
        if(url!="")url = "http://pic.zanyule.cn/" + url;
        else url = "http://pic.zanyule.cn/" + "user/0b7e7b7ba498455bbd31ba23944e09c6.png";//这是个默认图片吧属于是
        StartCoroutine(FetchHeadPic(url));
    }

    IEnumerator FetchHeadPic(string url)
    {
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texD1 = new DownloadHandlerTexture(true);
        wr.downloadHandler = texD1;
        yield return wr.SendWebRequest();
        int width = 100;
        int high = 100;
        if (wr.result==UnityWebRequest.Result.Success)
        {
            Texture2D tex = new Texture2D(width, high);
            tex = texD1.texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            HeadPic.sprite = sprite;
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll(transform);
    }
}
