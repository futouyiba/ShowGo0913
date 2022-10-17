using UnityEngine;
using UnityEngine.UI;

public class CircleProgress : MonoBehaviour
{
    private RectTransform circleprogress;

    public float attenuationspeed;
    // Start is called before the first frame update
    void Start()
    {
        attenuationspeed = 0.1f;
       circleprogress= this.gameObject.GetComponent<RectTransform>();
      
    }

    // Update is called once per frame
    void Update()
    {
        if (circleprogress.GetComponent<Image>().fillAmount > 0)
        {
            circleprogress.GetComponent<Image>().fillAmount -= Time.deltaTime * attenuationspeed;
        }

        if (circleprogress.GetComponent<Image>().fillAmount==0)
        {
            circleprogress.GetComponent<Image>().fillAmount= 1;
        }
    }
}
