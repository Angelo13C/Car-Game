using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TransformSystemGroup))]
public partial class CameraTargetSystem : SystemBase
{
    private Transform _cameraTarget;
    
    protected override void OnUpdate()
    {
        if(_cameraTarget == null)
        {
            _cameraTarget = new GameObject("Camera target").transform;
            var cinemachineCamera = Object.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            cinemachineCamera.LookAt = _cameraTarget;
            cinemachineCamera.Follow = _cameraTarget;
        }
        
        foreach(var (cameraTarget, transform) in SystemAPI.Query<CameraTarget, LocalToWorld>())
        {            
            _cameraTarget.position = transform.Position;
            _cameraTarget.rotation = transform.Rotation;
        }
    }
}