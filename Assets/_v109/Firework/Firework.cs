using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    public ParticleSystem FireworkParticle;
    private ParticleSystem.EmissionModule EmissionModule;
    private ParticleSystem.MainModule MainModule;

    private float MaxRate = 200f;//每秒钟正常产生的粒子数量，逐渐增加它，而不是修改duration
    private float MinRate = 0f;

    // 达到MaxRate所需时间  
    public float SpeedUpTime = 3f;
    // 开始呲花的时间  
    private float FireworkStartTime;


    // Start is called before the first frame update
    void Start()
    {
        FireworkParticle.Stop();
        EmissionModule = FireworkParticle.emission;
        EmissionModule.rateOverTime = MinRate;
        MainModule = FireworkParticle.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (FireworkParticle.isPlaying)
        {
            if (Time.time - FireworkStartTime < SpeedUpTime)
            {
                EmissionModule.rateOverTime = MinRate + (MaxRate - MinRate) * ((Time.time - FireworkStartTime) / SpeedUpTime);
            }
            else
            {
                //mainModule.duration = MinDuration;
            }
        }
    }

    public void StartFirework()
    {
        //this.transform.localPosition = new Vector3(0, 0, 0);
        //FireworkParticle.Clear();改了延迟消失
        float r = Random.Range(0.8f, 1.6f);
        MainModule.startSpeed = new ParticleSystem.MinMaxCurve(7*r, 10*r);
        EmissionModule.rateOverTime = MinRate;
        FireworkParticle.Play();
        //this.transform.localScale = new Vector3(1, 1, Random.Range(0.8f,1.6f));//不知道为什么这个会变，这两句必加上不然形变

        FireworkStartTime = Time.time;
    }

    public void EndFirework()
    {
        FireworkParticle.Stop();
        //this.transform.localScale = new Vector3(1, 1, 1);//不知道为什么这个会变，这两句必加上不然形变

    }

}
