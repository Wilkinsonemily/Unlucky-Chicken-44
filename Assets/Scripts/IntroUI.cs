using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroUI : MonoBehaviour
{
    static IntroUI inst;

    Canvas canvas;
    TMP_Text text;
    Button nextBtn;

    int page;
    System.Action onDone;

    AudioClip thunderClip;
    float originalMusicVol;
    bool musicDucked;

    [SerializeField] float duckMult = 0.35f;
    [SerializeField] float thunderVol = 1.0f;

    readonly string[] pages = new string[]
    {
        "Thunderstorm Night...\n\nWelcome.\n\n",
        "Please collect all 44 chickens as fast as possible\nand make lots of money!\n\n",
        "Move: WASD / Arrow\nInteract: SPACE\n\n"
    };

    public static void Show(AudioClip thunder, System.Action done)
    {
        Ensure();
        inst.onDone = done;
        inst.thunderClip = thunder;
        inst.page = 0;
        inst.canvas.gameObject.SetActive(true);
        inst.musicDucked = false;
        inst.Render();
        inst.PlayThunderAndDuckMusic();
    }

    static void Ensure()
    {
        if (inst != null) return;
        var go = new GameObject("IntroUI");
        DontDestroyOnLoad(go);
        inst = go.AddComponent<IntroUI>();
        inst.Build();
        inst.canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!canvas || !canvas.gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Next();

        if (Input.GetKeyDown(KeyCode.Escape))
            Finish();
    }

    void Next()
    {
        page++;
        if (page >= pages.Length) { Finish(); return; }
        Render();
    }

    void Finish()
    {
        canvas.gameObject.SetActive(false);

        if (musicDucked && AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(originalMusicVol);
            musicDucked = false;
        }

        var cb = onDone;
        onDone = null;
        cb?.Invoke();
    }

    void Render()
    {
        text.text = pages[Mathf.Clamp(page, 0, pages.Length - 1)];
    }

    void PlayThunderAndDuckMusic()
    {
        var am = AudioManager.Instance;
        if (am == null) return;

        originalMusicVol = am.GetVolume();
        am.SetVolume(originalMusicVol * Mathf.Clamp01(duckMult));
        musicDucked = true;

        if (thunderClip != null)
            am.PlaySFXOneShot(thunderClip, Mathf.Clamp01(thunderVol));
    }

    void Build()
    {
        canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
        canvas.transform.SetParent(transform, false);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 6000;

        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 1f;

        var bg = new GameObject("BG", typeof(Image)).GetComponent<Image>();
        bg.transform.SetParent(canvas.transform, false);
        bg.color = new Color(0, 0, 0, 0.70f);
        var bgRT = bg.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;

        var panel = new GameObject("Panel", typeof(Image)).GetComponent<Image>();
        panel.transform.SetParent(canvas.transform, false);
        panel.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);
        var prt = panel.rectTransform;
        prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0.5f);
        prt.sizeDelta = new Vector2(680, 300);
        prt.anchoredPosition = Vector2.zero;

        text = new GameObject("Text", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        text.transform.SetParent(panel.transform, false);
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 28;
        var trt = text.rectTransform;
        trt.anchorMin = new Vector2(0.06f, 0.25f);
        trt.anchorMax = new Vector2(0.94f, 0.92f);
        trt.offsetMin = trt.offsetMax = Vector2.zero;

        nextBtn = MakeBtn(panel.transform, "NEXT (ENTER)", new Vector2(0, -108));
        nextBtn.onClick.AddListener(Next);
    }

    Button MakeBtn(Transform parent, string label, Vector2 anchoredPos)
    {
        var go = new GameObject("NextButton", typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var img = go.GetComponent<Image>();
        img.color = new Color(0.22f, 0.22f, 0.22f, 1f);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.12f);
        rt.sizeDelta = new Vector2(220, 52);
        rt.anchoredPosition = anchoredPos;

        var t = new GameObject("Text", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        t.transform.SetParent(go.transform, false);
        t.text = label;
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = 20;
        var trt = t.rectTransform;
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }
}