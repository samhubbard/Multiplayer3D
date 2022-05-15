using System;
using Cinemachine;
using UnityEngine;

namespace FarmGame3D
{
    public class CameraController : MonoBehaviour
    {
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
            CinemachineVirtualCamera virtualCamera = GetComponent<CinemachineVirtualCamera>();
            virtualCamera.Follow = obj;
            virtualCamera.LookAt = obj;
        }
    }
}