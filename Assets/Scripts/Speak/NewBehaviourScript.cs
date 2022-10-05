using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]private List<GameObject> listA = new List<GameObject>();
    [SerializeField]private List<GameObject> listB = new List<GameObject>();
    [SerializeField]private List<GameObject> listC = new List<GameObject>();


    [SerializeField] private Transform cat001;
    [SerializeField] private Transform goA, goB, goC;

    int A, B, C = 0;

    private void Start()
    {
        Addressables.InstantiateAsync("Characters/2D/cat001.prefab", cat001);
        Addressables.LoadAssetsAsync<GameObject>(new List<string> { "XG_A" }, null, Addressables.MergeMode.Intersection).Completed += XG_A;
        Addressables.LoadAssetsAsync<GameObject>(new List<string> { "XG_B" }, null, Addressables.MergeMode.Intersection).Completed += XG_B;
        Addressables.LoadAssetsAsync<GameObject>(new List<string> { "XG_C" }, null, Addressables.MergeMode.Intersection).Completed += XG_C;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(300, 100, 200, 200), "ÏÂÒ»×é"))
        {
            if (A > listA.Count - 1) A = 0;
            if (B > listB.Count - 1) B = 0;
            if (C > listC.Count - 1) C = 0;

            Instantiate(listA[A], goA);
            Instantiate(listB[B], goB);
            Instantiate(listC[C], goC);

            A++;
            B++;
            C++;
        }
    }

    private void XG_A(AsyncOperationHandle<IList<GameObject>> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded) 
        {
            foreach (var item in obj.Result)
            {
                listA.Add(item);
            }
        }
    }
    
    private void XG_B(AsyncOperationHandle<IList<GameObject>> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded) 
        {
            foreach (var item in obj.Result)
            {
                listB.Add(item);
            }
        }
    }
    
    private void XG_C(AsyncOperationHandle<IList<GameObject>> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded) 
        {
            foreach (var item in obj.Result)
            {
                listC.Add(item);
            }
        }
    }
}
