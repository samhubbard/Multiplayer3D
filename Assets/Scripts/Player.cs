using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

namespace FarmGame3D
{
    public class Player : NetworkBehaviour
    {
        #region Data Types

        private struct MoveData
        {
            public float Horizontal;
            public float Vertical;

            public MoveData(float horizontal, float vertical)
            {
                Horizontal = horizontal;
                Vertical = vertical;
            }
        }

        private struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;

            public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
            }
        }
        #endregion

        public float MoveRate = 10f;
        private Rigidbody rb;
        private bool subscribed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void SubscribeToTimeManager(bool subscribe)
        {
            if (TimeManager == null) return;
            if (subscribe == subscribed) return;

            subscribed = subscribe;

            if (subscribe)
            {
                TimeManager.OnTick += TimeManagerOnOnTick;
                TimeManager.OnPostTick += TimeManagerOnOnPostTick;
            }
            else
            {
                TimeManager.OnTick -= TimeManagerOnOnTick;
                TimeManager.OnPostTick -= TimeManagerOnOnPostTick;
            }
        }

        private void OnDestroy()
        {
            SubscribeToTimeManager(false);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            SubscribeToTimeManager(true);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            SubscribeToTimeManager(true);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            
            SubscribeToTimeManager(false);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            
            SubscribeToTimeManager(false);
        }

        private void TimeManagerOnOnTick()
        {
            if (IsOwner)
            {
                Reconciliation(default, false);

                MoveData data;
                GatherInputs(out data);
                
                Move(data, false);
            }

            if (IsServer)
            {
                Move(default, true);
            }
        }
        
        private void TimeManagerOnOnPostTick()
        {
            ReconcileData data = new ReconcileData
            (
                transform.position, 
                transform.rotation, 
                rb.velocity,
                rb.angularVelocity
            );
            
            Reconciliation(data, true);
        }

        private void GatherInputs(out MoveData data)
        {
            data = default;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal == 0f && vertical == 0f) return;

            data = new MoveData(horizontal, vertical);
        }

        [Replicate]
        private void Move(MoveData data, bool asServer, bool replaying = false)
        {
            Vector3 movement = new Vector3(data.Horizontal, 0f, data.Vertical).normalized * MoveRate;
            rb.AddForce(movement);
        }

        [Reconcile]
        private void Reconciliation(ReconcileData data, bool asServer)
        {
            transform.position = data.Position;
            transform.rotation = data.Rotation;
            rb.velocity = data.Velocity;
            rb.angularVelocity = data.AngularVelocity;
        }
    }
}