using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarSprite : MonoBehaviour
{
    public PlayerEnergy energy;
    public Image targetImage;
    public List<Sprite> frames = new List<Sprite>();

    void Awake()
    {
        if (!targetImage) targetImage = GetComponent<Image>();
    }

    void LateUpdate()
    {
        if (!energy || !targetImage || frames == null || frames.Count == 0) return;

        int maxIndex = frames.Count - 1;

        float pct = energy.Energy / Mathf.Max(1f, (float)energy.maxEnergy);
        pct = Mathf.Clamp01(pct);

        int idx = Mathf.FloorToInt(pct * (maxIndex + 1));
        idx = Mathf.Clamp(idx, 0, maxIndex);

        if (energy.Energy <= 0) idx = 0;
        if (energy.Energy >= energy.maxEnergy) idx = maxIndex;

        targetImage.sprite = frames[idx];
    }
}
