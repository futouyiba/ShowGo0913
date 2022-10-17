using UnityEngine;

public class Firework : MonoBehaviour
{
    public ParticleSystem FireworkParticle;
    private ParticleSystem.EmissionModule EmissionModule;
    private ParticleSystem.MainModule MainModule;

    private float MaxRate = 200f;//ÿ�����������������������������������������޸�duration
    private float MinRate = 0f;

    // �ﵽMaxRate����ʱ��  
    public float SpeedUpTime = 3f;
    // ��ʼ�ڻ���ʱ��  
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
        //FireworkParticle.Clear();�����ӳ���ʧ
        float r = Random.Range(0.8f, 1.6f);
        MainModule.startSpeed = new ParticleSystem.MinMaxCurve(7*r, 10*r);
        EmissionModule.rateOverTime = MinRate;
        FireworkParticle.Play();
        //this.transform.localScale = new Vector3(1, 1, Random.Range(0.8f,1.6f));//��֪��Ϊʲô�����䣬������ؼ��ϲ�Ȼ�α�

        FireworkStartTime = Time.time;
    }

    public void EndFirework()
    {
        FireworkParticle.Stop();
        //this.transform.localScale = new Vector3(1, 1, 1);//��֪��Ϊʲô�����䣬������ؼ��ϲ�Ȼ�α�

    }

}
