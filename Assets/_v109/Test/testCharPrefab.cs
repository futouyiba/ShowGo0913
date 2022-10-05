using System.Collections;
using System.Collections.Generic;
using Bolt;
using ShowGo;
using Sirenix.OdinInspector;
using UnityEngine;

public class testCharPrefab : MonoBehaviour
{
    private ShowGoPlayer playerScript;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<ShowGoPlayer>();
    }


    
    [Button("aprctest")]
    public void testChangeAprc(int id)
    {
        playerScript.OnAprcIdChanged(-1, id);
    }

    [Button("MoveEnd")]
    public void MoveEnd()
    {
        var fsm = playerScript.GetComponent<StateMachine>();
        fsm.TriggerUnityEvent("MoveEnd");
    }

    [Button("TrainStateChange")]
    public void TrainStateChange()
    {
        playerScript.SvrTrainStateChange("");
    }
}
