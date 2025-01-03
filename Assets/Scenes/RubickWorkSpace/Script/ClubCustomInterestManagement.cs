using System.Collections.Generic;
using UnityEngine;
using Mirror;

// NOTE: Attach this component to the same object as your Network Manager.
namespace ET
{
    public class ClubCustomInterestManagement : InterestManagement
    {
        //[Tooltip("The maximum range that objects will be visible at. Add DistanceInterestManagementCustomRange onto NetworkIdentities for custom ranges.")]
        public int visRange = 10;

        //[Tooltip("Rebuild all every 'rebuildInterval' seconds.")]
        public float rebuildInterval = 1;
        double lastRebuildTime;

        // helper function to get vis range for a given object, or default.
        int GetVisRange(NetworkIdentity identity)
        {
            return identity.TryGetComponent(out DistanceInterestManagementCustomRange custom) ? custom.visRange : visRange;
        }

        [ServerCallback]
        public override void Reset()
        {
            lastRebuildTime = 0D;
        }

        public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
        {
            if (identity.gameObject.GetComponent<Puppy>() == null)
            {
                return true;
            }
            else
            {
                int range = GetVisRange(identity);
                return Vector3.Distance(identity.transform.position, newObserver.identity.transform.position) < range;
            }
            
        }

        public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
        {
            // cache range and .transform because both call GetComponent.
            int range = GetVisRange(identity);
            Vector3 position = identity.transform.position;

            // brute force distance check
            // -> only player connections can be observers, so it's enough if we
            //    go through all connections instead of all spawned identities.
            // -> compared to UNET's sphere cast checking, this one is orders of
            //    magnitude faster. if we have 10k monsters and run a sphere
            //    cast 10k times, we will see a noticeable lag even with physics
            //    layers. but checking to every connection is fast.
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                // authenticated and joined world with a player?
                if (conn != null && conn.isAuthenticated && conn.identity != null)
                {
                    // check distance
                    if (conn.identity.gameObject.GetComponent<Puppy>()!=null && Vector3.Distance(conn.identity.transform.position, position) < range)
                    {
                        newObservers.Add(conn);
                    }
                }
            }
        }

        // internal so we can update from tests
        [ServerCallback]
        internal void Update()
        {
            // rebuild all spawned NetworkIdentity's observers every interval
            if (NetworkTime.localTime >= lastRebuildTime + rebuildInterval)
            {
                foreach (NetworkIdentity identity in NetworkServer.spawned.Values)
                {
                    if(identity.gameObject.GetComponent<Puppy>() != null) NetworkServer.RebuildObservers(identity, false);
                }
                lastRebuildTime = NetworkTime.localTime;
            }
        }
    }

}
