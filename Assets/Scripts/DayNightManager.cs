using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DayNightManager : MonoBehaviour
{
    public static DayNightManager I;

    [Range(0f, 1f)] public float time01 = 0.25f;
    public float dayLengthSeconds = 300f;

    [Range(0f, 1f)] public float nightStart = 0.75f;
    [Range(0f, 1f)] public float nightEnd = 0.20f;

    public bool IsNight { get; private set; }
    public bool IsSleeping { get; private set; }

    public UnityEvent OnNightStarted;
    public UnityEvent OnDayStarted;

    public AnimationCurve darknessCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Range(0f, 1f)] public float maxDarkness = 0.65f;

    public float fadeDuration = 1f;

    Canvas overlayCanvas;
    Image darknessOverlay;
    Image fadeImage;

    bool lastIsNight;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        RuntimeSingletons.Mark(gameObject);


        EnsureOverlayBuilt();

        lastIsNight = IsWithinNight(time01);
        IsNight = lastIsNight;
    }

    void Update()
    {
        if (IsSleeping) return;

        EnsureOverlayBuilt();

        time01 += Time.deltaTime / Mathf.Max(0.01f, dayLengthSeconds);
        if (time01 > 1f) time01 -= 1f;

        IsNight = IsWithinNight(time01);

        if (IsNight != lastIsNight)
        {
            lastIsNight = IsNight;
            if (IsNight) OnNightStarted?.Invoke();
            else OnDayStarted?.Invoke();
        }

        float d = DarknessAmount(time01);
        if (darknessOverlay)
        {
            var c = darknessOverlay.color;
            c.a = d * maxDarkness;
            darknessOverlay.color = c;
        }
    }

    bool IsWithinNight(float t)
    {
        if (nightStart < nightEnd) return t >= nightStart && t < nightEnd;
        return t >= nightStart || t < nightEnd;
    }

    float DarknessAmount(float t)
    {
        return IsWithinNight(t) ? darknessCurve.Evaluate(1f) : darknessCurve.Evaluate(0f);
    }

    public void SleepToMorning()
    {
        EnsureOverlayBuilt();

        time01 = 0.25f;
        IsNight = false;
        lastIsNight = false;

        if (darknessOverlay)
        {
            var c = darknessOverlay.color;
            c.a = DarknessAmount(time01) * maxDarkness;
            darknessOverlay.color = c;
        }

        ToastUI.Say("A new day begins.");
        OnDayStarted?.Invoke();
    }

    public IEnumerator SleepWithFade()
    {
        IsSleeping = true;

        yield return FadeToBlackOnly();
        SleepToMorning();
        yield return new WaitForSecondsRealtime(0.2f);
        yield return FadeFromBlackOnly();

        IsSleeping = false;
    }

    public IEnumerator FadeToBlackOnly()
    {
        EnsureOverlayBuilt();
        if (!fadeImage) yield break;

        float t = 0f;
        while (t < fadeDuration)
        {
            if (!fadeImage) yield break;
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(0f, 1f, t / Mathf.Max(0.01f, fadeDuration));
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        if (fadeImage) fadeImage.color = Color.black;
    }

    public IEnumerator FadeFromBlackOnly()
    {
        EnsureOverlayBuilt();
        if (!fadeImage) yield break;

        float t = 0f;
        while (t < fadeDuration)
        {
            if (!fadeImage) yield break;
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(1f, 0f, t / Mathf.Max(0.01f, fadeDuration));
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        if (fadeImage) fadeImage.color = new Color(0, 0, 0, 0);
    }

    void EnsureOverlayBuilt()
    {
        if (overlayCanvas && darknessOverlay && fadeImage) return;

        if (overlayCanvas == null)
        {
            var goCanvas = new GameObject("DayNight_OverlayCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            goCanvas.transform.SetParent(transform, false);
            overlayCanvas = goCanvas.GetComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 500;

            var scaler = goCanvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 1f;
        }

        if (darknessOverlay == null)
        {
            var goOverlay = new GameObject("DarknessOverlay", typeof(Image));
            goOverlay.transform.SetParent(overlayCanvas.transform, false);
            darknessOverlay = goOverlay.GetComponent<Image>();
            darknessOverlay.color = new Color(0, 0, 0, 0);

            var rt = darknessOverlay.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            darknessOverlay.raycastTarget = false;
        }

        if (fadeImage == null)
        {
            var goFade = new GameObject("FadeScreen", typeof(Image));
            goFade.transform.SetParent(overlayCanvas.transform, false);
            fadeImage = goFade.GetComponent<Image>();
            fadeImage.color = new Color(0, 0, 0, 0);

            var rt = fadeImage.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            fadeImage.raycastTarget = false;
        }
    }
    public void LoadFromSave(float t01)
    {
        time01 = Mathf.Repeat(t01, 1f);
    }
}
