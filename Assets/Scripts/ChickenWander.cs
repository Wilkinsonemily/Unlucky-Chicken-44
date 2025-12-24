
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChickenWander : MonoBehaviour
{
    [Header("Wander Area")]
    public float areaLimit = 6f;
    public Vector2? customCenter = null;

    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float moveTime = 2f;
    public float idleTime = 2f;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer sr;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 moveDir;
    private bool isMoving;
    private bool paused;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        startPos = customCenter.HasValue ? customCenter.Value : (Vector2)transform.position;
        StartCoroutine(WanderLoop());
    }

    void FixedUpdate()
    {
        if (paused || !isMoving) return;

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(rb.position, startPos) > areaLimit)
        {
            moveDir = (startPos - rb.position).normalized;
            UpdateAnimation(moveDir);
        }
    }

    IEnumerator WanderLoop()
    {
        while (true)
        {
            isMoving = false;
            UpdateAnimation(Vector2.zero);

            yield return new WaitForSeconds(Random.Range(idleTime * 0.5f, idleTime * 1.5f));

            isMoving = true;
            moveDir = Random.insideUnitCircle.normalized;
            UpdateAnimation(moveDir);

            float t = Random.Range(moveTime * 0.7f, moveTime * 1.2f);
            while (t > 0)
            {
                if (!paused) t -= Time.deltaTime;
                yield return null;
            }
        }
    }

    void UpdateAnimation(Vector2 dir)
    {
        if (!animator) return;

        if (dir == Vector2.zero)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isSide", false);
            animator.SetFloat("horizontal", 0);
            animator.SetFloat("vertical", 0);
            return;
        }

        animator.SetBool("isMoving", true);

        float absX = Mathf.Abs(dir.x);
        float absY = Mathf.Abs(dir.y);

        if (absY >= absX)
        {
            animator.SetBool("isSide", false);
            animator.SetFloat("vertical", dir.y > 0 ? 1 : -1);
            animator.SetFloat("horizontal", 0);
        }
        else
        {
            animator.SetBool("isSide", true);
            animator.SetFloat("vertical", 0);
            animator.SetFloat("horizontal", 1);

            if (sr)
                sr.flipX = dir.x < 0;
        }
    }


    public void PauseWander()
    {
        paused = true;
        isMoving = false;
        UpdateAnimation(Vector2.zero);
    }

    public void ResumeWander()
    {
        paused = false;
    }

    public void SetAreaCenter(Vector2 newCenter)
    {
        startPos = newCenter;
        customCenter = newCenter;
    }

    private void OnTriggerEnter2D(Collider2D col) { }
}