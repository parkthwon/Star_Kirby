using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //인풋매니저
    InputManager input = new InputManager();
    public static InputManager Input { get { return Instance.input; } }

    //카메라
    public Camera mainCamera;
    [SerializeField] Camera changeCamera;
    [SerializeField] FollowCamera.CameraState startCameraState;
    [SerializeField] Transform boss;
    public Transform bossGround;

    //포스트 프로세싱
    [SerializeField] PostProcessProfile postProcessProfile;
    ColorGrading colorGrading;

    //시작위치
    public bool isStartMotion;
    public Vector3 startPos;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Color Grading
        postProcessProfile.TryGetSettings(out colorGrading);
        colorGrading.colorFilter.value = Color.white;
        if (startCameraState == FollowCamera.CameraState.BossBasic)
        {
            PlayerManager.Instance.FCamera.SetBossBasic(boss);
            SoundManager.Instance.PlayBGM("BGM1");
        }
        else
        {
            PlayerManager.Instance.FCamera.State = startCameraState;
            SoundManager.Instance.PlayBGM("BGM2");
        }

        SoundManager.Instance.BGMVolume = 1;
        SceneChanger.Instance.gameObject.SetActive(true);
        StartCoroutine(SceneChanger.Instance.ChangeSceneEnd());
    }

    // Update is called once per frame
    void Update()
    {
        input.OnUpdate();

        if (UnityEngine.Input.GetKeyDown(KeyCode.J))
        {
            if (PlayerManager.Instance.FCamera.State == FollowCamera.CameraState.BossBasic)
                PlayerManager.Instance.FCamera.SetBossTopView(boss, bossGround);
            else
                PlayerManager.Instance.FCamera.SetBossBasic(boss);
        }
    }

    public void PlayerChangeStart()
    {
        changeCamera.enabled = true;
        StartCoroutine(PlayerChangeStartCoroutine());
    }

    IEnumerator PlayerChangeStartCoroutine()
    {
        Color postColor = colorGrading.colorFilter.value;
        while (postColor.r > 0.5f)
        {
            postColor = new Color(postColor.r - Time.unscaledDeltaTime, postColor.g - Time.unscaledDeltaTime, postColor.b - Time.unscaledDeltaTime);
            colorGrading.colorFilter.value = postColor;
            yield return null;
        }
        postColor = new Color(0.5f, 0.5f, 0.5f);
        colorGrading.colorFilter.value = postColor;
    }

    public void PlayerChangeEnd()
    {
        StartCoroutine(PlayerChangeEndCoroutine());
    }

    IEnumerator PlayerChangeEndCoroutine()
    {
        Color postColor = colorGrading.colorFilter.value;
        while (postColor.r < 1f)
        {
            postColor = new Color(postColor.r + Time.unscaledDeltaTime, postColor.g + Time.unscaledDeltaTime, postColor.b + Time.unscaledDeltaTime); ;
            colorGrading.colorFilter.value = postColor;
            yield return null;
        }
        postColor = new Color(1f, 1f, 1f);
        colorGrading.colorFilter.value = postColor;
        changeCamera.enabled = false;
    }

    #region Die
    public void PlayerDie()
    {
        changeCamera.enabled = true;
        StartCoroutine(PlayerDieCoroutine());
    }

    IEnumerator PlayerDieCoroutine()
    {
        Color postColor = colorGrading.colorFilter.value;
        while (postColor.r > 0.0f)
        {
            postColor = new Color(postColor.r - Time.unscaledDeltaTime, postColor.g - Time.unscaledDeltaTime, postColor.b - Time.unscaledDeltaTime);
            colorGrading.colorFilter.value = postColor;
            yield return null;
        }
        postColor = Color.black;
        colorGrading.colorFilter.value = postColor;
    }
    #endregion
}
