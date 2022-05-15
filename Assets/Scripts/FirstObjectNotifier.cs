using System;
using FishNet.Object;
using UnityEngine;

namespace FarmGame3D
{
    public class FirstObjectNotifier : NetworkBehaviour
    {
        public static event Action<Transform> OnFirstObjectSpawned;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                NetworkObject nob = LocalConnection.FirstObject;
                if (nob == NetworkObject)
                    OnFirstObjectSpawned?.Invoke(transform);
            }
        }
    }
}