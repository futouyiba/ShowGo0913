using Mirror;
using ShowGo;
using Sirenix.OdinInspector;

namespace _v109.Test
{

    public class testPartyBehaviour : SerializedMonoBehaviour
    {

        [Button("trainStuff")]
        public void OnTrainUsersChanged(SyncList<uint>.Operation op, int itemIndex, uint oldUserId, uint newItem)
        {
            PartyBehaviour.Instance.OnTrainUsersChanged(op, itemIndex, oldUserId, newItem);
        }

    }

}