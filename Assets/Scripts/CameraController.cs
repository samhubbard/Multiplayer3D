using System;
using Cinemachine;
using UnityEngine;

namespace FarmGame3D
{
    public class CameraController : MonoBehaviour
    {
        private CinemachineVirtualCamera virtualCamera;
        
        private void Awake()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            
            FirstObjectNotifier.OnFirstObjectSpawned += FirstObjectNotifierOnOnFirstObjectSpawned;
        }

        private void OnDestroy()
        {
            FirstObjectNotifier.OnFirstObjectSpawned -= FirstObjectNotifierOnOnFirstObjectSpawned;
        }

        private void FirstObjectNotifierOnOnFirstObjectSpawned(Transform obj)
        {
            virtualCamera.Follow = obj;
            virtualCamera.LookAt = obj;
        }
    }
}