using UnityEngine;
using UnityEngine.EventSystems;

namespace ShowGo
{
    public class GiftButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        // 按钮是否是按下状态  
        private bool isDown = false;

        // 当按钮被按下后系统自动调用此方法  
        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("按下");
            //if (PlayerControl.OnGiftButtonDown())
            {
                isDown = true;
                FireworkBehaviour.instance.OnGiftButtonDown();
            }

        }

        // 当按钮抬起的时候自动调用此方法  
        public void OnPointerUp(PointerEventData eventData)
        {
            if (isDown)
            {
                //Debug.Log("抬起");
                isDown = false;
                //Firework.EndFirework();
                FireworkBehaviour.instance.OnGiftButtonUp();
            }

        }

        // 当鼠标从按钮上离开的时候自动调用此方法  
        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDown)
            {
                //Debug.Log("抬起");
                isDown = false;
                //Firework.EndFirework();
                FireworkBehaviour.instance.OnGiftButtonUp();
            }
        }
    }
}