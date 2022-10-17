using UnityEngine;
using Random = UnityEngine.Random;

public class XGAprefabControl : MonoBehaviour
{
 
    public MeshRenderer[] partsmeshrender;

    public enum EXGAVariant
    {
        SausageDog,
        StandCat1,
        StandCat2,
        Xiugou1
    }

    public void SetBase(EXGAVariant variant)
    {
        ShutAll();
        switch (variant)
        {
            case EXGAVariant.SausageDog:
                partsmeshrender[5].enabled = true;
                break;
            case EXGAVariant.StandCat1:
                partsmeshrender[6].enabled = true;
                break;
            case EXGAVariant.StandCat2:
                partsmeshrender[7].enabled = true;
                break;
            case EXGAVariant.Xiugou1:
                partsmeshrender[8].enabled = true;
                break;
                
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
        
        
        //随机显示基础prefab，即没有任何装饰物的prefab
     
        partsmeshrender[Random.Range(5, partsmeshrender.Length)].enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (partsmeshrender[8].enabled==true)
        {
            
            if (Input.GetKeyDown(KeyCode.A))
            {
                showturkeyhat();
            }
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                showDJhat();
            }
          
            if (Input.GetKeyDown(KeyCode.C))
            {
                showstrawhat();
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                showglass();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                showcoke();
            }
        }
    }

    public void showturkeyhat()
    {
        //显示火鸡帽子,关闭DJ帽子，关闭草帽
        partsmeshrender[0].enabled = true;
        partsmeshrender[1].enabled = false;
        partsmeshrender[2].enabled = false;
    }

    public void showDJhat()
    {
        //关闭火鸡帽子，显示DJ帽子，关闭草帽
        partsmeshrender[0].enabled = false;
        partsmeshrender[1].enabled = true;
        partsmeshrender[2].enabled = false;
    }

    public void showstrawhat()
    {
        //关闭火鸡帽子，关闭DJ帽子，显示草帽
        partsmeshrender[0].enabled = false;
        partsmeshrender[1].enabled = false;
        partsmeshrender[2].enabled = true;
    }

    public void showglass()
    {
        //显示墨镜
        partsmeshrender[3].enabled = true;
    }

    public void showcoke()
    {
        //显示可乐
        partsmeshrender[4].enabled = true;
    }

    public void ShutAll()
    {
        foreach (var meshRenderer in partsmeshrender)
        {
            meshRenderer.enabled = false;
        }
    }

}
