using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ludiq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenSesame : MonoBehaviour
{
    public List<string>  RubickUIpassword;
    public List<string>  minhangUIpassword;

    public List<string> inputPassword;

    public GameObject wheel;

    public GameObject Top3DJ;

    public GameObject DebugCanvas;

    public Button buttons;
    // Start is called before the first frame update
    void Start()
    {
        RubickUIpassword = new List<string>(){"1","2","3","4","5","6"};
        minhangUIpassword = new List<string>(){"1","2","3","6","5","4"};
        inputPassword = new List<string>();
        for (int i = 0; i < transform.childCount; i++)
        {
            buttons = transform.GetChild(i).GetComponent<Button>();
            buttons.onClick.AddListener(onButtonClick);
        }
    }
    

    public void onButtonClick()
    {   
        if (EventSystem.current.currentSelectedGameObject.name == "1")
        {
            inputPassword.Add("1");
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "2")
        {
            inputPassword.Add("2");
        }
        else if  (EventSystem.current.currentSelectedGameObject.name == "3")
        {
            inputPassword.Add("3");
        }
        else if  (EventSystem.current.currentSelectedGameObject.name == "4")
        {
            inputPassword.Add("4");
        }
        else if  (EventSystem.current.currentSelectedGameObject.name == "5")
        {
            inputPassword.Add("5");
        }
        else if  (EventSystem.current.currentSelectedGameObject.name == "6")
        {
            inputPassword.Add("6");
        }
        else if  (EventSystem.current.currentSelectedGameObject== null)
        {
           Debug.Log("nothing");
        }
        if (EventSystem.current.currentSelectedGameObject.name == "confirm")
        {
            //取当前输入密码的后六位，与预设的密码进行比对，控制UI的显隐
            if (inputPassword.GetRange(inputPassword.Count-6,6).SequenceEqual(RubickUIpassword))
            {
                DebugCanvas.SetActive(true);
            }
            else if (inputPassword.GetRange(inputPassword.Count-6,6).SequenceEqual(minhangUIpassword))
            {
                Top3DJ.SetActive(true);
                wheel.SetActive(true);
            }
            else
            {
                DebugCanvas.SetActive(false);
                Top3DJ.SetActive(false);
                wheel.SetActive(false);
            }
        }
    }
}
