using UnityEngine;
using UnityEngine.EventSystems;

namespace ShowGo
{
    public class GiftButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        // ��ť�Ƿ��ǰ���״̬  
        private bool isDown = false;

        // ����ť�����º�ϵͳ�Զ����ô˷���  
        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("����");
            //if (PlayerControl.OnGiftButtonDown())
            {
                isDown = true;
                FireworkBehaviour.instance.OnGiftButtonDown();
            }

        }

        // ����ţ̌���ʱ���Զ����ô˷���  
        public void OnPointerUp(PointerEventData eventData)
        {
            if (isDown)
            {
                //Debug.Log("̧��");
                isDown = false;
                //Firework.EndFirework();
                FireworkBehaviour.instance.OnGiftButtonUp();
            }

        }

        // �����Ӱ�ť���뿪��ʱ���Զ����ô˷���  
        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDown)
            {
                //Debug.Log("̧��");
                isDown = false;
                //Firework.EndFirework();
                FireworkBehaviour.instance.OnGiftButtonUp();
            }
        }
    }
}