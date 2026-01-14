using UnityEngine;


[BehaviourNode(2, "Actions")]
public class SpotTargetNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private float _searchDistance = 5f;
    [SerializeField] private float _fieldOfViewAngle = 90f;
    [SerializeField] private TargetType _typeToTarget = TargetType.Player;
    [SerializeField] private LayerMask _targetMask =  (1 << 9) | (1 << 10); // 9 and 10 are intended to be Player and Enemy
    [SerializeField] private LayerMask _obstacleMask = 1 << 14; //14 is intended to be Wall
    [SerializeField] private ITargetable _currentTarget;
    [SerializeField] private bool _lastKnownPositionMode;

    // stored here for debug purposes
    private float _storedRotation = 0f;

    // Behaviour tree
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

    public override BTNodeState Evaluate()
    {
        _isActive = true;

        Blackboard.TryGet("targetPosition", out Vector2? targetPosition); // Get last known target position

        if (_currentTarget == null)
        {
            if (FindVisibleTarget())
            {
                return BTNodeState.Success;
            } else
            {
                if (targetPosition.HasValue && _lastKnownPositionMode) return BTNodeState.Success;
                return BTNodeState.Failure;
            }
        }
        else
        {
            Vector2 targetPos = TargetPos(_currentTarget);
            bool stillVisible = IsTargetVisible(_currentTarget, targetPos);
            if (!stillVisible)
            {
                _currentTarget = null;
                Blackboard.Set<ITargetable>("target", null);
                if (targetPosition.HasValue && _lastKnownPositionMode) return BTNodeState.Success;
                return BTNodeState.Failure;
            } else
            {
                Blackboard.Set<ITargetable>("target", _currentTarget);
                Blackboard.Set<Vector2?>("targetPosition", targetPos);
                return BTNodeState.Success;
            }
        }

    }

    private bool FindVisibleTarget()
    {
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(Self.transform.position, _searchDistance, _targetMask);
        foreach (Collider2D potentialTargetCollider in targetsInRange)
        {
            if (potentialTargetCollider.TryGetComponent(out ITargetable potentialTarget) && potentialTarget.GetTargetType() == _typeToTarget)
            {
                Vector2 targetPos = TargetPos(potentialTarget);

                if (IsTargetVisible(potentialTarget, targetPos))
                {
                    _currentTarget = potentialTarget;
                    Blackboard.Set<ITargetable>("target", _currentTarget);
                    Blackboard.Set<Vector2?>("targetPosition", targetPos);
                    return true;
                }
            }
        }
        return false;
    }
    
    private Vector2 TargetPos(ITargetable potentialTarget)
    {
        MonoBehaviour potentialTargetMono = potentialTarget as MonoBehaviour;
        if (potentialTargetMono == null)
        {
            Debug.LogError("ITargetable found on non-monobehaviour. What happened?");
            return Vector2.zero;
        }
        return potentialTargetMono.transform.position;
    }

    private bool IsTargetVisible(ITargetable potentialTarget, Vector2 targetPos)
    {
        float rotation = Self.FacingRotation;
        float radians = rotation * Mathf.Deg2Rad;
        Vector2 facingDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        _storedRotation = rotation;

        Vector2 potentialTargetDirection = (targetPos - (Vector2) Self.transform.position).normalized;
        if (Vector2.Angle(facingDirection, potentialTargetDirection) < _fieldOfViewAngle / 2)
        {
            float potentialTargetDistance = Vector2.Distance(Self.transform.position, targetPos);
            // Wall raycast check
            RaycastHit2D hit = Physics2D.Raycast(Self.transform.position, potentialTargetDirection, potentialTargetDistance, _obstacleMask);
            if (!hit && potentialTargetDistance <= _searchDistance)
            {
                return true;
            }
        }
        return false;
    }



    protected override void DrawDebug()
    {
        if (Self == null) return;
        

        Vector3 rightBoundary = DirectionFromAngle(_fieldOfViewAngle / 2);
        Vector3 leftBoundary = DirectionFromAngle(-_fieldOfViewAngle / 2);

        Gizmos.color = Color.red;
        if (_currentTarget != null) Gizmos.color = Color.green;
        Gizmos.DrawLine(Self.transform.position, Self.transform.position + rightBoundary * _searchDistance);
        Gizmos.DrawLine(Self.transform.position, Self.transform.position + leftBoundary * _searchDistance);
    }
    
    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        float rad = (angleInDegrees + _storedRotation) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

}
