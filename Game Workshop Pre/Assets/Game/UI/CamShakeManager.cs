using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamShakeManager : MonoBehaviour
{
    public static CamShakeManager instance;

    [SerializeField] private float _shakeForce = 1.8f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(_shakeForce);

    }
}
