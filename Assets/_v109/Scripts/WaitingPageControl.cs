using UnityEngine;
using UnityEngine.UI;
namespace ShowGo
{
    public class WaitingPageControl : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] float LastLoginTime = 0f;
        [SerializeField] float SecurityShowUpTime = 5f;
        [SerializeField] float ErrorPageShowUpTime = 60f;//�������Timeout����KcpTransport���ã���������

        [SerializeField] ClubNetworkManager ClubNetworkManager;
        [SerializeField] GameObject LoadingIcon;
        [SerializeField] GameObject SecurityIcon;
        [SerializeField] GameObject ErrorPage;
        [SerializeField] Button ErrorPageButton;

        [SerializeField] Text Text;
        public static WaitingPageControl instance;
        public bool isConnecting = false;

        void Start()
        {
            instance = this;
            ErrorPageButton.onClick.AddListener(() => OnErrorPageButtonClicked());
        }

        // Update is called once per frame
        void Update()
        {
            if (isConnecting && Time.time - LastLoginTime > SecurityShowUpTime)
            {
                if (LoadingIcon.activeInHierarchy) { LoadingIcon.SetActive(false);  }
                if (!SecurityIcon.activeInHierarchy) { SecurityIcon.SetActive(true); Text.text = "�� �� �� ��  . . ."; }

            }
            else
            {
                if (!LoadingIcon.activeInHierarchy) { LoadingIcon.SetActive(true); Text.text = "�� ȡ ��  . . ."; }
                if (SecurityIcon.activeInHierarchy) { SecurityIcon.SetActive(false); }
            }

            if(isConnecting && Time.time - LastLoginTime > ErrorPageShowUpTime && !ErrorPage.activeInHierarchy)
            {
                isConnecting = false;
                ErrorPage.SetActive(true);
            }
        }

        private void OnErrorPageButtonClicked()
        {
            ErrorPage.SetActive(false);
            ClubNetworkManager.StartClient();
        }

        public void OnStartClient()
        {
            LastLoginTime = Time.time;
            isConnecting = true;
        }

    }

}

