using UnityEngine;

public class XGBprefabControl : MonoBehaviour
{
 
    public MeshRenderer[] partsmeshrender;
  
    public enum EXGBVariant
    {
        Xiugou,
        XiugouBall,
        XiugouSkirt
    }

    public void SetBase(EXGBVariant variant)
    {
        ShutAll();
        switch (variant)
        {
            case EXGBVariant.Xiugou:
                partsmeshrender[9].enabled = true;
                break;
            case EXGBVariant.XiugouBall:
                partsmeshrender[9].enabled = true;
                showtbasketball();
                break;
            case EXGBVariant.XiugouSkirt:
                partsmeshrender[9].enabled = true;
                showskirt();
                break;
        }
    }
    
    public void ShutAll()
    {
        foreach (var meshRenderer in partsmeshrender)
        {
            meshRenderer.enabled = false;
        }
    }
    
    
    
    
    void Start()
    {
        
        int a = 0;
        //遍历prefab所有子物体
        
        partsmeshrender = new MeshRenderer[transform.childCount];
        foreach (Transform t in transform)
        {
           // parts[a++] = t;
            partsmeshrender[a++] = t.gameObject.GetComponent<MeshRenderer>();
        }
        
        
        //显示xiugou的基础形象
        partsmeshrender[9].enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
       
            
            if (Input.GetKeyDown(KeyCode.A))
            {
                showtbasketball();
            }
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                bowknot();
            }
          
            if (Input.GetKeyDown(KeyCode.C))
            {
                showXGBglass();
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                showhair();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                showbottlecoke();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                showphone();
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                showstick();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                showships();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                showskirt();
            }
        }
    

    public void showtbasketball()
    {
        //显示篮球
        partsmeshrender[0].enabled = true;
    }

    public void bowknot()
    {
        //显示蝴蝶结发饰
        partsmeshrender[1].enabled = true;
        partsmeshrender[3].enabled = false;
    }

    public void showXGBglass()
    {
        //显示墨镜
        partsmeshrender[2].enabled = true;
    }

    public void showhair()
    {
        //显示kunkun头发
        partsmeshrender[1].enabled = false;
        partsmeshrender[3].enabled = true;
    }

    public void showbottlecoke()
    {
        //显示可乐瓶
        partsmeshrender[4].enabled = true;
        partsmeshrender[5].enabled = false;
        partsmeshrender[8].enabled = false;
    }

    public void showphone()
    {
        //显示手机
        partsmeshrender[4].enabled = false;
        partsmeshrender[5].enabled = true;
        partsmeshrender[8].enabled = false;
    }

    public void showstick()
    {
        //显示荧光棒
        partsmeshrender[4].enabled = false;
        partsmeshrender[5].enabled = false;
        partsmeshrender[8].enabled = true;
    }

    public void showships()
    {
        //显示薯条
        partsmeshrender[6].enabled = true;
    }

    public void showskirt()
    {
        //显示裙子
        partsmeshrender[7].enabled = true;
    }

}
