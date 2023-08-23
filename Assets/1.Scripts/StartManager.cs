using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartManager : MonoBehaviour
{
    [SerializeField] RawImage videoImage;
    [SerializeField] Button startButton;

    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] VideoClip[] videoClips;

    bool isStart = false;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM("BGM2");
        SoundManager.Instance.BGMVolume = 1;
        if (SceneChanger.Instance.prevSceneName == "")
        {
            SceneChanger.Instance.gameObject.SetActive(false);
            startButton.interactable = false;
            videoPlayer.clip = videoClips[0];
            videoPlayer.loopPointReached += VideoEnd;
            videoPlayer.targetTexture.Release();
            videoPlayer.Play();
        }
        else
        {
            StartCoroutine(SceneChanger.Instance.ChangeSceneEnd());
            startButton.interactable = true;
            videoPlayer.clip = videoClips[1];
            videoPlayer.isLooping = true;
            videoPlayer.Play();
        }
    }

    void VideoEnd(VideoPlayer vp)
    {
        startButton.interactable = true;
        videoPlayer.clip = videoClips[1];
        videoPlayer.loopPointReached -= VideoEnd;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }

    void StartButtonEvent()
    {
        if (isStart == true) return;
        isStart = true;
        SoundManager.Instance.PlaySFX("Start");

        videoPlayer.clip = videoClips[2];
        videoPlayer.loopPointReached += LastVideoEnd;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }

    void LastVideoEnd(VideoPlayer vp)
    {
        SceneChanger.Instance.gameObject.SetActive(false);
        videoImage.DOFade(0, 0.3f).OnComplete(() => { UnityEngine.SceneManagement.SceneManager.LoadScene("Stage"); });
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (startButton != null && startButton.onClick.GetPersistentEventCount() == 0)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(startButton.onClick, StartButtonEvent);
        }
    }
#endif
}
