using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public AudioClip thunderClip;

    void Start()
    {
        Time.timeScale = 0f;

        IntroUI.Show(thunderClip, () =>
        {
            Time.timeScale = 1f;
        });
    }
}