using UnityEngine;

public class GPUSkinningMaterial
{
    public Material material = null;

    public GPUSkinningExecuteOncePerFrame executeOncePerFrame = new GPUSkinningExecuteOncePerFrame();

    public void Destroy()
    {
        if(material != null)
        {
            Object.Destroy(material);
            material = null;
        }
    }
}
