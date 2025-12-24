using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class SeedPickup : MonoBehaviour
{
    public int seedGain = 1;
    public float triggerRadius = 0.25f;

    bool canPick;
    bool picked;

    void Reset()
    {
        EnsureCollider();
    }

    void Awake()
    {
        EnsureCollider();
    }

    void OnEnable()
    {
        picked = false;
        canPick = false;
        StartCoroutine(EnablePick());
    }

    IEnumerator EnablePick()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        canPick = true;
    }

    void EnsureCollider()
    {
        var c = GetComponent<Collider2D>();
        if (c == null) c = gameObject.AddComponent<CircleCollider2D>();

        c.isTrigger = true;

        var circle = c as CircleCollider2D;
        if (circle != null)
        {
            if (circle.radius <= 0.001f) circle.radius = triggerRadius;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canPick) return;
        if (picked) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        picked = true;

        inv.AddSeeds(seedGain);

        var pe = other.GetComponent<PlayerEnergy>();
        if (pe != null) pe.DrainForSeedCollect();

        ToastUI.Say($"+{seedGain} Seed");
        Destroy(gameObject);
    }
}
