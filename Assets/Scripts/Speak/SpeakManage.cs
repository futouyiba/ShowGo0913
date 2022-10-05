using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class SpeakManage : MonoBehaviour
    {
        public float interval = 3000f;
        [Tooltip("��󹷵�����")] [SerializeField] private int dog_Max;
        [Tooltip("��ͬʱ��ʾ��������")] [SerializeField] private int show_Max = 5;

        [Tooltip("��С��ʾʱ��")] [SerializeField] private int Min_duration;
        [Tooltip("�����ʾʱ��")] [SerializeField] private int Max_duration;
        [Tooltip("�����ʾ���쵯��")] [SerializeField] private int Max_Chat;

        [Tooltip("����")] public RectTransform uiCan;
        [Tooltip("����Ԥ����")] public GameObject uiSpeak;

        [Tooltip("����Ԥ����")] [SerializeField] private List<GameObject> Dog = new List<GameObject>();
        [Tooltip("��ǰʹ�õķ���")] private int roomSpeakID = 1;
        //[Tooltip("������ʽͼƬ")] [SerializeField] private List<GameObject> room_Sprite = new List<GameObject>();

        //�洢���������й�
        [SerializeField] private List<Speak> list_DogSpeak = new List<Speak>();

        //��Ҫ��ʾ������
        private List<Speak> show_Speak = new List<Speak>();
        //��Ҫ�ر�������
        private List<Speak> remove_Speak = new List<Speak>();

        void Start()
        {
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < dog_Max; i++)
            {
                Speak go = Instantiate(Dog[Random.Range(0, Dog.Count)], new Vector3(Random.Range(-6, 6), 0, Random.Range(0.5f, 5)), Quaternion.identity).GetComponent<Speak>();

                go.Init(uiCan, uiSpeak, i);
                list_DogSpeak.Add(go);

                if (show_Speak.Count < show_Max)
                {
                    Add_Show();
                }
            }

            StartCoroutine("NewMethod", interval);

        }


        private void Update()
        {
            foreach (Speak timer in show_Speak)
            {
                if (!timer.isRemoved)
                {
                    this.remove_Speak.Add(timer);
                    timer.Conceal();
                    continue;
                }
                timer.passedTime += Time.deltaTime;

                if (timer.passedTime >= timer.duration)
                {
                    this.remove_Speak.Add(timer);
                    timer.Conceal();
                }
            }

            for (int i = 0; i < remove_Speak.Count; i++)
            {
                show_Speak.Remove(remove_Speak[i]);
            }

            remove_Speak.Clear();

            Monitor();

        }

        private void NewMethod()
        {
            for (int i = 0; i < Max_Chat; i++)
            {
                int dog = Random.Range(0, list_DogSpeak.Count);
                string theWholeStr = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Duis at consectetur lorem donec massa sapien faucibus et molestie. Vestibulum mattis ullamcorper velit sed ullamcorper morbi tincidunt ornare massa. Quis imperdiet massa tincidunt nunc. Vulputate enim nulla aliquet porttitor lacus. Turpis egestas integer eget aliquet nibh. Proin nibh nisl condimentum id venenatis. Viverra adipiscing at in tellus integer feugiat scelerisque varius morbi. Mi bibendum neque egestas congue quisque. Dui sapien eget mi proin sed libero enim sed faucibus. Vulputate enim nulla aliquet porttitor lacus. Etiam tempor orci eu lobortis elementum. Facilisi morbi tempus iaculis urna id. Dictumst quisque sagittis purus sit amet volutpat consequat. Augue neque gravida in fermentum et sollicitudin ac orci phasellus.";
                int start = Random.Range(0, 30);
                int length = Random.Range(5, 10);
                str = theWholeStr.Substring(start, length);
                list_DogSpeak[dog].FaYan(str);
                //list_DogSpeak[dog].dm.PlayStopMove();
            }
        }

        private void LateUpdate()
        {
            //�ж���ʾ�б��Ƿ���Ա
            if (show_Speak.Count < show_Max)
                Add_Show();
        }

        private void Monitor()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                roomSpeakID = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                roomSpeakID = 4;
            }
            //else if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    roomSpeakID = 4;
            //}
            //else if (Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    roomSpeakID = 5;
            //}
            //else if (Input.GetKeyDown(KeyCode.Alpha5))
            //{
            //    roomSpeakID = 6;
            //}
        }

        int i = 0;
        //����ʾ�б���Ӷ���
        private void Add_Show() 
        {
            if (list_DogSpeak.Count < 3) return;

            if (!list_DogSpeak[i].isRemoved && !list_DogSpeak[i].isFaYan && list_DogSpeak[i].isCanRemoved) 
            {
                show_Speak.Add(list_DogSpeak[i]);

                list_DogSpeak[i].isRemoved = true;
                if (Random.Range(0, 3) > 0) 
                {
                    list_DogSpeak[i].Show(roomSpeakID);
                    //list_DogSpeak[i].GengHuanSpeak(room_Sprite[roomSpeakID]);
                }
                else
                {
                    list_DogSpeak[i].Show(2);
                }
                list_DogSpeak[i].duration = Random.Range(Min_duration, Max_duration);

            }
            i++;
            if (i >= dog_Max)
                i = 0;            
        }



        #region ���Է��Թ���
        string str = "";

        private void OnGUI()
        {
            str = GUI.TextField(new Rect(50, 300, 300, 100), str);
            if (GUI.Button(new Rect(50, 400, 300, 60), "����"))
            {
                for (int i = 0; i < Max_Chat; i++)
                {
                    int dog = Random.Range(0, list_DogSpeak.Count);
                    list_DogSpeak[dog].FaYan(str);
                    //list_DogSpeak[dog].dm.PlayStopMove();
                }
            }
        }


        #endregion

    }
}