using UnityEngine;
using TMPro;

namespace ShowGo
{
    public class SongAuthorTitle : MonoBehaviour
    {
        public bool isShowing = false;
        public TMP_Text AuthorTitle;
        float Starttime;
        void Start()
        {
            Starttime = Time.time;
        }

        void Update()
        {
            if (Time.time - Starttime > 0.1f)
            {
                if (AudioVisualizer.AudioSampler.instance.GetVolume() <= 0)//��ֹ�����û��spawn
                {
                    if (isShowing) { HideAuthorTitle(); }
                }
                else
                {
                    if (VcamBehaviour.instance.PreviousVC == VcamBehaviour.instance.vCams[0] && !isShowing) { ShowAuthorTitle(); }
                    if (VcamBehaviour.instance.PreviousVC != VcamBehaviour.instance.vCams[0] && isShowing) { HideAuthorTitle(); }
                }
            }
            
        }

        public void ShowAuthorTitle()
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            isShowing = true;
        }
        public void HideAuthorTitle()
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            isShowing = false;
        }

        public void SetAuthorTitle(string author,string title)
        {
            //if (author == "") { author = "Unknown"; }
            //if (author == "") { author = "Unknown"; }
            AuthorTitle.text = author + " - " + title;
        }
    }
}
