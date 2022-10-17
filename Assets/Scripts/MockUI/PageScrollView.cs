using System;
using System.Collections.Generic;
using System.IO;
using ShowGo;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PageScrollView : MonoBehaviour,IEndDragHandler
{
    private ScrollRect rect;
    public int pageCount;
    private RectTransform content;
    public Image headPic;
    public float[] pages;
  
    public float moveTime = 0.3f;
    private float timer = 0;
    private float startMovePos;
    private int currentPage;
    private bool isMoving = false;
    public GameObject optionPrefab; 
    public Transform OPtionGroup; 
    List<Sprite> imageList = new List<Sprite>();

    public List<uint> userId = new List<uint>();
    public RectTransform showgoname;

    private bool showScrollView;
    //考虑把scrollview中，content->image作为一个prefab，每增加一个小狗，读取他的头像作为image，并prefab.transform.SetParent(content,false)
    void Start()
    {
        countThePages();
        showScrollView = true;
    }

    // Update is called once per frame
    void Update()
    {
        ListenerMove();
        if (transform.gameObject.activeSelf&&showScrollView)
        {
            getShowgoInScene();
            showScrollView = false;
        }
        if (!transform.gameObject.activeSelf)
        {
            showScrollView = true;
        }
    }
    
    private static byte[] getImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }
    //监听移动
    public void ListenerMove()
    {
        if (isMoving)
        {
            timer += Time.deltaTime * (1 / moveTime);
            rect.horizontalNormalizedPosition = Mathf.Lerp(startMovePos,pages[currentPage],timer);
            if (timer >= 1)
            {
                isMoving = false;
            }
        }
    }
    
    public void ScrollToPage(int page)
    {
        isMoving = true;
        this.currentPage = page;
        timer = 0;
        startMovePos = rect.horizontalNormalizedPosition;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
          int minPage=0;
        //计算出离得最近的一页
        for (int i = 1; i < pages.Length; i++)
        {
            if (Mathf.Abs(pages[i] - rect.horizontalNormalizedPosition) <
                Mathf.Abs(pages[minPage] - rect.horizontalNormalizedPosition))
            {
                minPage = i;
            }
        }
        this.ScrollToPage(minPage);
       
        userId= SceneShowgoMgr.Instance.GetAllUserIds();
        showgoname.GetComponent<TextMeshProUGUI>().text = userId[minPage].ToString();
        VcamBehaviour.instance.LocalTargetPuppy = userId[minPage];
        VcamBehaviour.instance.LocalCameraChange(5);

    }
    
    //计算content的子物体有多少个，即有多少张图片，为”翻页“做准备
    public void countThePages()
    {
        rect = transform.GetComponent<ScrollRect>();
        if (rect == null)
        {
            throw new SystemException("未查询到ScrollRect");
        }

        content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        pageCount = content.childCount;

        pages = new float[pageCount];
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i] = i * (1.0f / (float)(pageCount - 1));
        }
        
        //这里，list对重复元素的控制需要debug
        userId= SceneShowgoMgr.Instance.GetAllUserIds();
        showgoname.GetComponent<TextMeshProUGUI>().text = userId[0].ToString();
        VcamBehaviour.instance.LocalTargetPuppy = userId[0];
        VcamBehaviour.instance.LocalCameraChange(5);
    }

    public void getShowgoInScene()
    {
        
            //点击“加好友”按钮，获取当前场景中所有小狗的头像，装载到content下，“我的名字”=当前头像对应小狗的名字
            //获取当前场景中所有小狗的头像
            string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG";
            string[] ImageType = imgtype.Split('|');
            for (int k = 0; k < ImageType.Length; k++)
            {
                //获取Application.dataPath文件夹下所有的图片路径  
                string[] dirs = Directory.GetFiles(Application.streamingAssetsPath, ImageType[k]);
                for (int j = 0; j < dirs.Length; j++)
                {
                    Texture2D tx = new Texture2D(300, 300);
                    tx.LoadImage(getImageByte(dirs[j]));
                    Sprite sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(-500, -500));
                    sprite.name= Path.GetFileNameWithoutExtension(dirs[j]);
                    //将获取的图片加到图片列表中
                    imageList.Add(sprite);
                }
            }
            //生成预制体
            for (int i = 0; i < userId.Count; i++)
            {
                GameObject go = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, OPtionGroup);
                go.name = i.ToString();
                int index = i;
                //当图片的数量小于生成的预制体的数量，让预制体的图片变成第一个
                if (index > imageList.Count - 1)
                {
                    index = 0;
                }
                //将图片赋值到预制体上
                go.transform.GetComponent<Image>().sprite = imageList[index];
                
            }
            countThePages();
           
           

    }


}
