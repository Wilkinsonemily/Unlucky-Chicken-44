using UnityEngine;

public class ChickenLockedPaidAndFeed : MonoBehaviour, IInteractable2D
{
    public bool inFarm = true;
    public bool isReleased = false;
    public bool captured = false;

    public Transform insideFarmPoint;
    public float moveSpeed = 3f;

    Rigidbody2D rb;
    Collider2D col;

    bool moving;
    Vector3 target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        if (DayNightManager.I != null)
            DayNightManager.I.OnNightStarted.AddListener(GoBackInsideAtNight);
    }

    void OnDisable()
    {
        if (DayNightManager.I != null)
            DayNightManager.I.OnNightStarted.RemoveListener(GoBackInsideAtNight);
    }

    void Update()
    {
        if (!moving) return;

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if ((transform.position - target).sqrMagnitude < 0.0001f)
        {
            moving = false;
            SetGhost(false);
            isReleased = !inFarm;
        }
    }

    public void ReleaseTo(Vector3 worldPos)
    {
        if (captured) return;
        inFarm = false;
        isReleased = false;
        StartMove(worldPos);
    }

    void GoBackInsideAtNight()
    {
        if (captured) return;
        if (!inFarm && insideFarmPoint != null)
        {
            inFarm = true;
            isReleased = false;
            StartMove(insideFarmPoint.position);
        }
    }

    void StartMove(Vector3 pos)
    {
        target = pos;
        moving = true;
        SetGhost(true);
    }

    void SetGhost(bool ghost)
    {
        if (rb) rb.isKinematic = ghost;
        if (col) col.isTrigger = ghost;
    }

    public void Interact(GameObject interactor)
    {
        if (captured) return;

        if (inFarm || !isReleased)
        {
            ToastUI.Say("This chicken is inside the other farm. Pay the farmer to release one.");
            return;
        }

        if (!PlayerInventory.I.UseOneFeed())
        {
            ToastUI.Say("You need 1 Feed to capture this chicken.");
            return;
        }

        if (HomePenManager.I == null)
        {
            ToastUI.Say("Home pen not found.");
            return;
        }

        if (!HomePenManager.I.HasFreeSlot())
        {
            ToastUI.Say("Your chicken pen is full.");
            return;
        }

        captured = true;

        var pe = interactor ? interactor.GetComponent<PlayerEnergy>() : null;
        if (pe) pe.DrainForCapture();

        if (col) col.enabled = false;

        HomePenManager.I.StoreChicken(gameObject);
        ToastUI.Say("Chicken captured.");
        HUD.I?.RefreshAll();
    }
}
