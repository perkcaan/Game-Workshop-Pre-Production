using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBallRenderer : MonoBehaviour
{
    public MeshRenderer QuadRenderer { get; private set; }
    [SerializeField] private MeshRenderer _modelRenderer;
    public MeshRenderer ModelRenderer { get { return _modelRenderer; } }

    [SerializeField] private int _textureSize = 64;  // fixed RT size
    private const float PixelsPerUnit = 16f;         // PPU
    private const int PaddingPixels = 2;             // extra buffer around the model

    [HideInInspector] public RenderTexture RenderTexture;

    private void Start()
    {
        QuadRenderer = GetComponent<MeshRenderer>();

        // Fixed RenderTexture
        RenderTexture = new RenderTexture(_textureSize, _textureSize, 16, RenderTextureFormat.ARGB32);
        RenderTexture.filterMode = FilterMode.Point;
        RenderTexture.useMipMap = false;
        RenderTexture.wrapMode = TextureWrapMode.Clamp;
        RenderTexture.Create();

        // Assign a copy of the material so it has its own texture
        QuadRenderer.material = new Material(QuadRenderer.material);
        QuadRenderer.material.mainTexture = RenderTexture;

        TrashBallCamera.Instance.RegisterTrashBall(this);

        // Set initial quad size (fixed for pixel-perfectness)
        float worldSize = (_textureSize + PaddingPixels * 2) / PixelsPerUnit;
        QuadRenderer.transform.localScale = new Vector3(worldSize, worldSize, 1f);
    }

    void OnDestroy()
    {
        if (TrashBallCamera.Instance != null) TrashBallCamera.Instance.UnregisterTrashBall(this);
        RenderTexture.Release();
        Destroy(RenderTexture);
    }
}
