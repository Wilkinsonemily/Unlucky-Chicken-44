using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastUI : MonoBehaviour
{
    public static ToastUI I;

    public float showDuration = 2f;
    public float fadeDuration = 0.15f;

    public int maxQueue = 3;
    public float sameMessageCooldown = 0.2f;
    public bool replaceWhileShowing = true;

    Canvas canvas;
    CanvasGroup cg;
    RectTransform panel;
    TMP_Text tmpText;

    readonly Queue<string> queue = new Queue<string>();
    Coroutine runner;

    string lastMessage = "";
    float lastMessageTime = -999f;

    bool showing;
    Coroutine fadeCo;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        RuntimeSingletons.Mark(gameObject);

        BuildUI();
    }

    public static void Say(string msg)
    {
        if (I == null || string.IsNullOrWhiteSpace(msg)) return;
        I.Push(msg);
    }

    void Push(string msg)
    {
        if (msg == lastMessage && (Time.unscaledTime - lastMessageTime) < sameMessageCooldown)
            return;

        lastMessage = msg;
        lastMessageTime = Time.unscaledTime;

        if (replaceWhileShowing && showing)
        {
            tmpText.text = msg;
            if (runner != null) StopCoroutine(runner);
            runner = StartCoroutine(ShowSingleThenContinue());
            return;
        }

        while (queue.Count >= maxQueue) queue.Dequeue();
        queue.Enqueue(msg);

        if (runner == null)
            runner = StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        while (queue.Count > 0)
        {
            yield return ShowMessage(queue.Dequeue());
        }
        runner = null;
    }

    IEnumerator ShowSingleThenContinue()
    {
        yield return ShowMessage(tmpText.text);

        while (queue.Count > 0)
            yield return ShowMessage(queue.Dequeue());

        runner = null;
    }

    IEnumerator ShowMessage(string msg)
    {
        tmpText.text = msg;

        showing = true;
        if (!panel.gameObject.activeSelf) panel.gameObject.SetActive(true);

        yield return Fade(1f);

        yield return new WaitForSecondsRealtime(showDuration);

        yield return Fade(0f);

        panel.gameObject.SetActive(false);
        showing = false;
    }

    IEnumerator Fade(float target)
    {
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeRoutine(target));
        yield return fadeCo;
        fadeCo = null;
    }

    IEnumerator FadeRoutine(float target)
    {
        float start = cg.alpha;
        float t = 0f;

        if (fadeDuration <= 0f)
        {
            cg.alpha = target;
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        cg.alpha = target;
    }

    void BuildUI()
    {
        var goCanvas = new GameObject("ToastCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = goCanvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 10000;

        var scaler = goCanvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 1f;

        var goPanel = new GameObject("ToastPanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        panel = goPanel.GetComponent<RectTransform>();
        panel.SetParent(goCanvas.transform, false);

        panel.anchorMin = new Vector2(0.1f, 0f);
        panel.anchorMax = new Vector2(0.9f, 0f);
        panel.pivot = new Vector2(0.5f, 0f);
        panel.anchoredPosition = new Vector2(0, 24);
        panel.sizeDelta = new Vector2(0, 120);

        var img = goPanel.GetComponent<Image>();
        img.color = new Color(0, 0, 0, 0.85f);
        img.raycastTarget = false;

        cg = goPanel.GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        panel.gameObject.SetActive(false);

        var goText = new GameObject("ToastText", typeof(RectTransform), typeof(TextMeshProUGUI));
        goText.transform.SetParent(panel, false);

        var rt = goText.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(20, 20);
        rt.offsetMax = new Vector2(-20, -20);

        tmpText = goText.GetComponent<TextMeshProUGUI>();
        tmpText.fontSize = 32;
        tmpText.alignment = TextAlignmentOptions.MidlineLeft;
        tmpText.color = Color.white;
        tmpText.outlineWidth = 0.25f;
        tmpText.enableWordWrapping = true;
        tmpText.raycastTarget = false;
    }
}
