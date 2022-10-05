using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserConfig
{
    public uint appID;
    public string userID;
    public string appSign;

    public UserConfig()
    {
        appID = 0;
        userID = "";
        appSign = "";
    }
}

public class UserConfigMgr : MonoBehaviour
{
    private static UserConfig userConfig = new UserConfig();
    private static bool isUpdateUserConfigByUI = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void UpdateUserConfigByUI(UserConfig config)
    {
        isUpdateUserConfigByUI = true;
        userConfig.appID = config.appID;
        userConfig.userID = config.userID;
        userConfig.appSign = config.appSign;
    }

    public static bool IsUpdateUserConfigByUI()
    {
        return isUpdateUserConfigByUI;
    }

    public static uint AppID()
    {
        return userConfig.appID;
    }

    public static string UserID()
    {
        return userConfig.userID;
    }

    public static string AppSign()
    {
        return userConfig.appSign;
    }
}