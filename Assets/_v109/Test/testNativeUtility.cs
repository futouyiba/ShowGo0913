using Sirenix.OdinInspector;
using UnityEngine;
using ShowGo;

public class testNativeUtility : MonoBehaviour
{
    [ButtonGroup("NewMsgs")][Button("MyInfo")]
    public void testMyInfo()
    {
        var msg = ContinentalMessenger.MockMyInfo(123);
        Debug.Log(msg);
    }

    [ButtonGroup("NewMsgs")][Button("testInitFinish")]
    public void testInitFinish()
    {
        var msg = ContinentalMessenger.MockInitFinish();
        Debug.Log(msg);
    }
    
    [ButtonGroup("NewMsgs")][Button("testUserMsg")]
    public void testUserMsg()
    {
        var msg = ContinentalMessenger.MockUserMsg();
        Debug.Log(msg);
    }

    
}
