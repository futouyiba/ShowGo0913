using ShowGo;
using UnityEngine;

public class CPButton : MonoBehaviour
{
    public RectTransform scrollViewRect;
    private bool show;
    private void Start()
    { 
        show = false;
    }

   public void clickCPButton()
    {
        if (!show)
        {
            show = true;
            scrollViewRect.gameObject.SetActive(show);
        }
        else
        {
            show = false;
            scrollViewRect.gameObject.SetActive(show);
            //关闭处CP面板，镜头执行场内迅游
            VcamBehaviour.instance.LocalCameraChange(0);
        }
    }
}
