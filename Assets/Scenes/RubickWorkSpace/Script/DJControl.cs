using UnityEngine;
using Mirror;

namespace ET
{
    public class DJControl : NetworkBehaviour
    {
        // Start is called before the first frame update
        public GameObject DJPuppy;

        //DanceFloorPivot_XGYD DanceFloorPivot_XGYD;

        void Start()
        {
            //DanceFloorPivot_XGYD=DanceFloorHelper_XGYD.GetGoFromScene("pivot").GetComponent<DanceFloorPivot_XGYD>();
        }

        [Command(requiresAuthority =false)]
        public void CmdSetDJ(NetworkIdentity DJId)
        {
            RpcSetDJ(DJId);
        }
        [ClientRpc]
        public void RpcSetDJ(NetworkIdentity DJId)
        {
            if (DJPuppy == null)
            {
                DJPuppy = NetworkClient.spawned[DJId.netId].gameObject;
                DJPuppy.transform.position = this.transform.position; 
                DJPuppy.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                DJPuppy.GetComponent<Puppy>().isMoving = false;
            }
            else
            {
                if (DJId == DJPuppy.GetComponent<NetworkIdentity>())
                {
                    //DJPuppy.transform.position += new Vector3(-5, 0, -5);
                    DJPuppy.GetComponent<Puppy>().MoveTo(new Vector2(-0.21f, -6.47f));
                    DJPuppy = null;
                }
                else
                {
                    //DJPuppy.transform.position += new Vector3(-5, 0, -5);
                    DJPuppy.GetComponent<Puppy>().MoveTo(new Vector2(-0.21f, -6.47f));
                    DJPuppy = null;
                    DJPuppy = NetworkClient.spawned[DJId.netId].gameObject;
                    DJPuppy.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    DJPuppy.GetComponent<Puppy>().isMoving = false;
                    DJPuppy.transform.position = this.transform.position;
                }
            }
            
        }

        public void OnDJButtonClicked()
        {
            CmdSetDJ(NetworkClient.localPlayer.GetComponent<NetworkIdentity>());
        }
    }
}
