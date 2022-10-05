using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;

public class CameraStateBehaviour : StateMachineBehaviour
{
    [SerializeField]
    protected Animator _animator;

    [SerializeField]
    protected AnimatorStateInfo _stateInfo;

    public void Init(Animator animator, AnimatorStateInfo info)
    {
        this._animator = animator;
        this._stateInfo = info;
    }
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        // Debug.Log($"animator named {animator.name}, state hash {stateInfo.fullPathHash}");
        
    }
}
