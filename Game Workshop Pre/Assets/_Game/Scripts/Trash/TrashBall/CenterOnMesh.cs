using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOnMesh : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    private void LateUpdate()
    {
        transform.position = _meshRenderer.bounds.center;
    }
}
