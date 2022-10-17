using UnityEngine;

public class Top3DJSwitch : MonoBehaviour
{
    public RectTransform DJ1;
    public RectTransform DJ2;
    public RectTransform DJ3;
    public RectTransform DJ1name;
    private Vector3 tempPostion;
    private float attenuationTime;
    

   
    // Start is called before the first frame update
    void Start()
    {
        attenuationTime = 1f;

    }

    // Update is called once per frame
    void Update()
    {
       
        // Debug.Log(attenuationTime);
        if (attenuationTime>0)
        {
            attenuationTime -= Time.deltaTime * 0.1f;
        }
        if (attenuationTime<0)
        {
            switchDJHeadPic();
            attenuationTime = 1;
        }
     
        
    }

    public void switchDJHeadPic()
    {
        tempPostion=  DJ3.transform.position ;
        DJ3.transform.position = DJ2.transform.position;
        DJ2.transform.position = DJ1.transform.position;
        DJ1.transform.position = tempPostion;
    }
}
