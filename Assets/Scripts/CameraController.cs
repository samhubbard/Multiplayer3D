using System;
using Cinemachine;
using UnityEngine;

namespace FarmGame3D
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera normalCamera;
        [SerializeField] private CinemachineVirtualCamera aimCamera;
        
        private void Awake()
        {
            FirstObjectNotifier.OnFirstObjectSpawned += FirstObjectNotifierOnOnFirstObjectSpawned;
        }

        private void OnDestroy()
        {
            FirstObjectNotifier.OnFirstObjectSpawned -= FirstObjectNotifierOnOnFirstObjectSpawned;
        }

        private void FirstObjectNotifierOnOnFirstObjectSpawned(Transform obj)
        {
            normalCamera.Follow = obj;
            normalCamera.LookAt = obj;
            aimCamera.Follow = obj;
            aimCamera.LookAt = obj;
        }
    }
}