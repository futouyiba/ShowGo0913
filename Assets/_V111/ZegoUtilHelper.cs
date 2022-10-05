using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class ZegoUtilHelper{
    static Scrollbar logScroll;
    static Text logText;
    public static string DeviceName()
    {
        string device = "";

#if UNITY_ANDROID
        device = "Android";
#elif UNITY_IPHONE
        device = "iPhone";
#elif UNITY_STANDALONE_WIN
        device = "Windows";
#elif UNITY_STANDALONE_LINUX
        device = "Linux";
#elif UNITY_STANDALONE_OSX
        device = "macOS";
#else
        device = "Unknown";
#endif
        return device;
    }

    public string GetRandomString(int min = 0, int max = 99999)
    {
        return UnityEngine.Random.Range(0,99999).ToString();
    }

    public static void InitLogView()
    {
        logScroll = GameObject.Find("ScrollVew_Log").GetComponent<Scrollbar>();
        logText = GameObject.Find("Text_Log").GetComponent<Text>();
    }

    public static void PrintLogToView(string logInfo)
    {
        // console log
        Debug.Log(logInfo);

        if(logText)
        {
            logText.fontSize = 30;

            if(logInfo.Length + logText.text.Length >= 15000)
            {
                logText.text = "";
            }
            if(logInfo.Length >= 15000)
            {
                logInfo = logInfo.Substring(0, 14997) + "...";
            }
                
            string time = string.Format("[ {0} ] ", DateTime.Now.ToString("HH:mm:ss.fff"));
            if(logText.text == "")
            {
                logText.text = time + logInfo;
            }
            else
            {
                logText.text = logText.text + Environment.NewLine + time + logInfo;
            }
        }
    }

    public static uint AppID()
    {
        var is_set_by_ui = UserConfigMgr.IsUpdateUserConfigByUI();

        if(is_set_by_ui)
        {
            return UserConfigMgr.AppID();
        }
        else
        {
            return KeyCenter.appID;
        }
    }

    public static string UserID()
    {
        var is_set_by_ui = UserConfigMgr.IsUpdateUserConfigByUI();

        if(is_set_by_ui)
        {
            return UserConfigMgr.UserID();
        }
        else
        {
            return DeviceName() + "_" + System.Environment.UserName + "_" + UnityEngine.Random.Range(0,99999).ToString();
        }
    }

    public static string AppSign()
    {
        var is_set_by_ui = UserConfigMgr.IsUpdateUserConfigByUI();

        if(is_set_by_ui)
        {
            return UserConfigMgr.AppSign();
        }
        else
        {
            return KeyCenter.appSign;
        }
    }

    // Convert default text string encoding type to dest encoding type
    public static string ConvertDefaultTextEncoding(string src, Encoding encoding)
    {
        byte[] default_bytes = Encoding.Default.GetBytes(src);
        byte[] dest_bytes = Encoding.Convert(Encoding.Default, encoding, default_bytes);

        return encoding.GetString(dest_bytes);
    }
}


public class KeyCenter
{   
    // Developers can get appID from admin console.
    // https://console.zego.im/dashboard
    // for example: 123456789;
    public const uint appID = 1666228805;

    // AppSign only meets simple authentication requirements.
    // If you need to upgrade to a more secure authentication method,
    // please refer to [Guide for upgrading the authentication mode from using the AppSign to Token](https://docs.zegocloud.com/faq/token_upgrade)
    // Developers can get AppSign from admin [console](https://console.zego.im/dashboard)
    // for example: "abcdefghijklmnopqrstuvwxyz0123456789abcdegfhijklmnopqrstuvwxyz01";
    public const string appSign = "cb6616491e3edc0e407ebc804f6f6dc124ea200ed4953ff6e90121f500ba0acc";

    public const string CDNStreamerAddr = "rtmp://streamer.zanyule.cn/live/simple";
}
