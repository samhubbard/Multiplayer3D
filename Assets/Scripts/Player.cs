using System;
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
            public Quaternion Rotation;

            public MoveData(float horizontal, float vertical, Quaternion rotation)
            {
                Horizontal = horizontal;
                Vertical = vertical;
                Rotation = rotation;
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
        private Animator anim;
        private float horizontal;
        private float vertical;
        private bool isOnGround;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            float inputMagnitude = Mathf.Clamp01(rb.velocity.magnitude);
            
            anim.SetFloat("Speed", inputMagnitude);

            var localVelocity = transform.InverseTransformDirection(rb.velocity);

            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit result;
            isOnGround = Physics.Raycast(ray, out result, .5f);

            if (isOnGround)
            {
                anim.gameObject.transform.rotation = Quaternion.LookRotation(localVelocity);
            }

            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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

            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if (horizontal == 0f && vertical == 0f) return;

            data = new MoveData(horizontal, vertical, transform.rotation);
        }

        [Replicate]
        private void Move(MoveData data, bool asServer, bool replaying = false)
        {
            Vector3 movement = new Vector3(data.Horizontal, 0f, data.Vertical) * MoveRate;
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
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