using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameClearManager : MonoBehaviour
{
    public static GameClearManager Instance;

    [SerializeField] Image playerBossKillImage;

    [SerializeField] Image myBossKillImage;
    [SerializeField] RawImage myBossKillVideo;
    [SerializeField] VideoPlayer videoPlayer;

    public bool gameClear = false;
    bool videioFinish = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        myBossKillVideo.enabled = false;
        videioFinish = false;
        videoPlayer.Stop();
    }

    private void Update()
    {
        if (!gameClear) return;

        if (Input.GetKeyDown(KeyCode.F1))
            videoPlayer.playbackSpeed = 10;
        else if (Input.GetKeyUp(KeyCode.F1))
            videoPlayer.playbackSpeed = 1;

        if (videioFinish && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
            StartCoroutine(SceneChanger.Instance.ChangeSceneStart("StartScene"));
        }
    }

    public IEnumerator GameClear()
    {
        if (gameClear == true) yield break;
        gameClear = true;
        SoundManager.Instance.BGMVolume = 0;
        Time.timeScale = 0;
        PlayerManager.Instance.Anim.updateMode = AnimatorUpdateMode.Normal;

        playerBossKillImage.gameObject.SetActive(true);
        playerBossKillImage.color = new Color(1, 1, 1, 0.8f);
        playerBossKillImage.DOFade(0, 0.6f).SetUpdate(true).SetEase(Ease.InCubic);
        PlayerManager.Instance.FCamera.CameraShake(0.3f, 0.15f);
        yield return new WaitForSecondsRealtime(0.1f);
        PlayerManager.Instance.FCamera.CameraShake(0.2f, 0.15f);
        yield return new WaitForSecondsRealtime(0.1f);
        PlayerManager.Instance.FCamera.CameraShake(0.1f, 0.15f);
        yield return new WaitForSecondsRealtime(0.55f);
        videoPlayer.loopPointReached += VideoEnd;
        videoPlayer.targetTexture.Release();
        videoPlayer.Play();
        myBossKillVideo.enabled = true;
    }

    void VideoEnd(VideoPlayer vp)
    {
        videioFinish = true;
    }
}