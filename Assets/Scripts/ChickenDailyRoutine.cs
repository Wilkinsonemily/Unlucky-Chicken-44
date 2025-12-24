using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChickenDailyRoutine : MonoBehaviour
{
    [Header("Points")]
    public Transform insidePoint;
    public Transform outsidePoint;

    [Header("Move")]
    public float moveSpeed = 3f;

    [Header("State")]
    public bool isOutsideNow = false;

    Rigidbody2D rb;
    Collider2D col;
    ChickenWander wander;

    bool moving;
    Vector3 target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        wander = GetComponent<ChickenWander>();
    }

    void Update()
    {
        if (!moving) return;

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if ((transform.position - target).sqrMagnitude < 0.0001f)
        {
            moving = false;
            SetGhost(false);

            if (isOutsideNow)
            {
                if (wander)
                {
                    wander.SetAreaCenter(outsidePoint.position);
                    wander.ResumeWander();
                }
            }
            else
            {
                if (wander) wander.PauseWander();
            }
        }
    }

    public void GoInside()
    {
        if (insidePoint == null) return;
        isOutsideNow = false;
        StartMove(insidePoint.position);
    }

    public void GoOutside()
    {
        if (outsidePoint == null) return;
        isOutsideNow = true;
        StartMove(outsidePoint.position);
    }

    void StartMove(Vector3 pos)
    {
        target = pos;
        moving = true;

        if (wander) wander.PauseWander();
        SetGhost(true);
    }

    void SetGhost(bool ghost)
    {
        if (rb) rb.isKinematic = ghost;
        if (col) col.isTrigger = ghost;
    }
}
