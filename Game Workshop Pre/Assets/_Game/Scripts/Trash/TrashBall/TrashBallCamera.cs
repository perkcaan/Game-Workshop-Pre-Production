using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TrashBallCamera : StaticInstance<TrashBallCamera>
{
    private Camera _cameraComponent;
    private List<TrashBallRenderer> _trashBalls = new List<TrashBallRenderer>();

    protected override void Awake()
    {
        base.Awake();
        _cameraComponent = GetComponent<Camera>();
        _cameraComponent.eventMask = 0;
        _cameraComponent.orthographic = true;
        _cameraComponent.clearFlags = CameraClearFlags.SolidColor;
        _cameraComponent.backgroundColor = new Color(0, 0, 0, 0);
    }

    private void LateUpdate()
    {
        foreach (TrashBallRenderer trashball in _trashBalls)
        {
            trashball.ModelRenderer.enabled = true;
            Bounds bounds = trashball.ModelRenderer.bounds;

            // Center camera on model
            _cameraComponent.transform.position = bounds.center + new Vector3(0f, 0f, -10f);

            // Orthographic size matches fixed RT in world units
            float worldSize = (trashball.RenderTexture.width + 2 * 2) / 16f; // texture + padding ÷ PPU
            _cameraComponent.orthographicSize = worldSize * 0.5f;

            // Render into the fixed RenderTexture
            _cameraComponent.targetTexture = trashball.RenderTexture;
            _cameraComponent.Render();
            trashball.ModelRenderer.enabled = false;
        }

        _cameraComponent.targetTexture = null;
    }

    public void RegisterTrashBall(TrashBallRenderer tbRenderer) {
        _trashBalls.Add(tbRenderer);
        tbRenderer.ModelRenderer.enabled = false;
    }
    public void UnregisterTrashBall(TrashBallRenderer tbRenderer) => _trashBalls.Remove(tbRenderer);
}
