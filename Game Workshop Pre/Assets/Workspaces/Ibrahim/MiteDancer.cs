using UnityEngine;
using DG.Tweening;

public class MiteDancer : MonoBehaviour
{
    public Vector3 normalScale = Vector3.one;
    public Vector3 squishedScale = new Vector3(1.5f, 0.1f, 1.5f);

    public float startDelay = 1f;
    public float unsquishDuration = 0.4f;
    public float initialJumpHeight = 2f;
    public float initialJumpDuration = 0.5f;
    public float hopHeight = 0.5f;
    public float hopDuration = 0.3f;
    public float swayAngle = 15f;
    public float swayDuration = 0.3f;

    public void Dance()
    {
        transform.DOKill();
        
        transform.rotation = Quaternion.identity;
        transform.localScale = squishedScale;

        Sequence entranceSequence = DOTween.Sequence();
        entranceSequence.AppendInterval(startDelay);
        entranceSequence.Append(transform.DOScale(normalScale, unsquishDuration).SetEase(Ease.OutBack));
        entranceSequence.Append(transform.DOJump(transform.position, initialJumpHeight, 1, initialJumpDuration).SetEase(Ease.OutQuad));
        entranceSequence.OnComplete(StartHoppingAndSwaying); 
    }

    private void StartHoppingAndSwaying()
    {
        ParticleManager.Instance.Play("Confetti", transform.position);
        transform.DOJump(transform.position, hopHeight, 1, hopDuration)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.InOutQuad);

        Sequence swaySequence = DOTween.Sequence();
        swaySequence.Append(transform.DORotate(new Vector3(0, 0, swayAngle), swayDuration / 2f).SetEase(Ease.InOutSine));
        swaySequence.Append(transform.DORotate(new Vector3(0, 0, -swayAngle), swayDuration).SetEase(Ease.InOutSine));     
        swaySequence.Append(transform.DORotate(Vector3.zero, swayDuration / 2f).SetEase(Ease.InOutSine));                 
        swaySequence.SetLoops(-1, LoopType.Restart);
    }

    public void StopDance()
    {
        transform.DOKill();
        transform.DORotate(Vector3.zero, 0.2f);
        transform.DOScale(normalScale, 0.2f);
    }
}