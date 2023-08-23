using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Boxcast Property")]
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private LayerMask groundLayer;

    public bool IsSafeGround { get; private set; }
    public bool IsGrounded()
    {
        IsSafeGround = false;
        //바닥체크
        var colliders = Physics.OverlapBox(transform.position, boxSize, transform.rotation, groundLayer);
        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                psw_Demap map = collider.GetComponent<psw_Demap>();
                if (map != null)
                {
                    map.BreakGround();
                }

                if (collider.CompareTag("FallPlane"))
                {
                    //플레이어 피격처리
                    PlayerManager.Instance.PHealth.Hit(Vector3.zero, 2, false);
                }

                if (collider.CompareTag("SafeGround"))
                {
                    IsSafeGround = true;
                }
            }
            return true;
        }
        else
            return false;
    }

    float rayDistance = 0.5f;
    float maxSlopeAngle = 45f;
    RaycastHit slopeHit;
    public bool IsOnSlope()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.01f, Vector3.down);
        if (Physics.Raycast(ray, out slopeHit, rayDistance, groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return slopeHit.normal != Vector3.zero && angle < maxSlopeAngle;
        }
        return false;
    }

    public Vector3 AdjustDirectionToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }
}