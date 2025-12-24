using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinUI : MonoBehaviour
{
    public static WinUI I;

    Canvas canvas;
    TMP_Text text;
    Button restartBtn;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        RuntimeSingletons.Mark(gameObject);

        Build();
        Hide();
    }

    public void ShowWin(int day, int money)
    {
        Time.timeScale = 0f;

        bool newBest = HighscoreManager.TrySubmit(day, money);
        int score = HighscoreManager.ComputeScore(day, money);

        string msg =
            "YOU WIN!\n\n" +
            $"Day: {day}\n" +
            $"Money: {money}\n" +
            (newBest ? "NEW HIGHSCORE!\n\n" : "") +
            HighscoreManager.FormatBest();

        text.text = msg;
        canvas.gameObject.SetActive(true);
    }

    void Hide()
    {
        if (canvas) canvas.gameObject.SetActive(false);
    }
    void Restart()
    {
        Time.timeScale = 1f;
        RuntimeSingletons.DestroyAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }


    void Build()
    {
        var goCanvas = new GameObject("WinCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = goCanvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 3001;

        var bgGO = new GameObject("BG", typeof(Image));
        bgGO.transform.SetParent(goCanvas.transform, false);
        var bg = bgGO.GetComponent<Image>();
        bg.color = new Color(0, 0.2f, 0, 0.85f);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        var txtGO = new GameObject("Text", typeof(TextMeshProUGUI));
        txtGO.transform.SetParent(goCanvas.transform, false);
        text = txtGO.GetComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 34;
        text.color = Color.white;
        var txtRT = txtGO.GetComponent<RectTransform>();
        txtRT.anchorMin = new Vector2(0.08f, 0.35f);
        txtRT.anchorMax = new Vector2(0.92f, 0.9f);
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;

        var btnGO = new GameObject("RestartBtn", typeof(Image), typeof(Button));
        btnGO.transform.SetParent(goCanvas.transform, false);
        restartBtn = btnGO.GetComponent<Button>();
        var btnImg = btnGO.GetComponent<Image>();
        btnImg.color = new Color(0.2f, 0.4f, 0.2f, 1f);
        var btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.35f, 0.12f);
        btnRT.anchorMax = new Vector2(0.65f, 0.26f);
        btnRT.offsetMin = Vector2.zero;
        btnRT.offsetMax = Vector2.zero;

        var labelGO = new GameObject("Label", typeof(TextMeshProUGUI));
        labelGO.transform.SetParent(btnGO.transform, false);
        var label = labelGO.GetComponent<TextMeshProUGUI>();
        label.text = "RESTART";
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 30;
        label.color = Color.white;
        var labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero;
        labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = Vector2.zero;
        labelRT.offsetMax = Vector2.zero;

        restartBtn.onClick.AddListener(Restart);
    }
}
