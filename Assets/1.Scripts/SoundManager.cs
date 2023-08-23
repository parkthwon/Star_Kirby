using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundData
{
    public string key;
    public AudioClip audioClip;
    public AudioMixerGroup audioMixerGroup;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    bool bgmVolumeChange = false;
    float bgmFadeTime = 0;
    float bgmFadeDelay = 0.5f;
    float lastBgmVolume = 0;
    float bgmVolume = 0;
    public float BGMVolume
    {
        set
        {
            if (bgmVolume == value) return;
            bgmVolumeChange = true;
            bgmFadeTime = 0;
            lastSfxVolume = bgmVolume;
            bgmVolume = value;
        }
    }

    bool sfxVolumeChange = false;
    float sfxFadeTime = 0;
    float sfxFadeDelay = 0.5f;
    float lastSfxVolume = 0;
    float sfxVolume = 1;
    public float SFXVolume
    {
        set
        {
            if (sfxVolume == value) return;
            sfxVolumeChange = true;
            sfxFadeTime = 0;
            lastSfxVolume = sfxVolume;
            sfxVolume = value;
        }
    }

    [SerializeField] AudioMixer audioMixer = null;

    [SerializeField] List<SoundData> loadingBGMSoundInfos = new List<SoundData>();
    [SerializeField] List<SoundData> loadingSFXSoundInfos = new List<SoundData>();

    Dictionary<string, SoundData> bgmContainer = new Dictionary<string, SoundData>();
    Dictionary<string, SoundData> sfxContainer = new Dictionary<string, SoundData>();

    GameObject bgmObj = null;   // 백그라운드 오브젝트
    AudioSource bgmSrc = null;  // 백그라운드 AudioSource 컴포넌트

    int sfxMaxCount = 10;
    int sfxCurCount = 0;
    List<GameObject> sfxObjList = new List<GameObject>(); //ArrayList m_sndObjList = new ArrayList();          // 효과음 오브젝트
    AudioSource[] sfxSrcList;

    SoundData soundData = null;

    // Start is called before the first frame update

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSound();
        LoadChildGameObj();
    }

    private void Update()
    {
        if (bgmVolumeChange)
        {
            bgmFadeTime += Time.unscaledDeltaTime;
            if (bgmFadeTime > bgmFadeDelay)
            {
                bgmVolumeChange = false;
                audioMixer.SetFloat("BGM", bgmVolume);
            }
            else
                audioMixer.SetFloat("BGM", Mathf.Lerp(lastBgmVolume, bgmVolume, bgmFadeTime / bgmFadeDelay));
        }

        if (sfxVolumeChange)
        {
            sfxFadeTime += Time.unscaledDeltaTime;
            if (sfxFadeTime > sfxFadeDelay)
            {
                sfxVolumeChange = false;
                audioMixer.SetFloat("SFX", sfxVolume);
            }
            else
                audioMixer.SetFloat("SFX", Mathf.Lerp(lastSfxVolume, sfxVolume, sfxFadeTime / sfxFadeDelay));
        }
    }

    void LoadSound()
    {
        //bgm
        for (int i = 0; i < loadingBGMSoundInfos.Count; i++)
        {
            bgmContainer.Add(loadingBGMSoundInfos[i].key, loadingBGMSoundInfos[i]);
        }

        //sfx
        for (int i = 0; i < loadingSFXSoundInfos.Count; i++)
        {
            sfxContainer.Add(loadingSFXSoundInfos[i].key, loadingSFXSoundInfos[i]);
        }

        sfxSrcList = new AudioSource[sfxMaxCount];
    }

    public void LoadChildGameObj()
    {
        //m_bgmObj == null 이면 PlayBGM()하게 되면 다시 로딩하게 된다. 
        if (bgmObj == null)
        {
            bgmObj = new GameObject();
            bgmObj.transform.SetParent(this.transform);
            bgmObj.transform.position = Vector3.zero;
            bgmSrc = bgmObj.AddComponent<AudioSource>();
            bgmSrc.playOnAwake = false;
            bgmObj.name = "BGMObj";
        }

        for (int a_ii = 0; a_ii < sfxMaxCount; a_ii++)
        {
            // 최대 4개까지 재생되게 제어 렉방지(Androud: 4개, PC: 무제한)  
            if (sfxObjList.Count < sfxMaxCount)
            {
                GameObject newSoundOBJ = new GameObject();
                newSoundOBJ.transform.SetParent(this.transform);
                newSoundOBJ.transform.localPosition = Vector3.zero;
                AudioSource a_AudioSrc = newSoundOBJ.AddComponent<AudioSource>();
                a_AudioSrc.playOnAwake = false;
                a_AudioSrc.loop = false;
                newSoundOBJ.name = "SFXObj";

                sfxSrcList[sfxObjList.Count] = a_AudioSrc;
                sfxObjList.Add(newSoundOBJ);
            }
        }//for (int a_ii = 0; a_ii < m_EffSdCount; a_ii++)
    }

    #region BGM
    public void PlayBGM(string key)
    {
        soundData = bgmContainer[key];

        //Scene이 넘어가면 GameObject는 지워지고, m_bgmObj == null 이면 
        //PlayBGM()하게 되면 다시 로딩하게 된다. 
        if (bgmObj == null)
        {
            bgmObj = new GameObject();
            bgmObj.transform.SetParent(this.transform);
            bgmObj.transform.position = Vector3.zero;
            bgmSrc = bgmObj.AddComponent<AudioSource>();
            bgmSrc.playOnAwake = false;
            bgmObj.name = "BGMObj";
        }

        if (soundData != null && bgmSrc != null)
        {
            if (bgmSrc.clip == soundData.audioClip)
                return;

            bgmSrc.clip = soundData.audioClip;
            bgmSrc.outputAudioMixerGroup = soundData.audioMixerGroup;
            //bgmSrc.volume = bgmVolume;
            bgmSrc.loop = true;
            bgmSrc.spatialBlend = 0f;
            bgmSrc.Play(0);
        }
    }

    public void StopBGM()
    {
        if (bgmSrc != null)
        {
            bgmSrc.Stop();
            bgmSrc.clip = null;
        }
    }
    #endregion

    #region SFX
    //효과음 플레이 함수
    public void PlaySFX(string key, float delay = 0, bool bLoop = false)
    {
        if (sfxContainer.ContainsKey(key) == false)
            return;

        soundData = sfxContainer[key];

        // 최대 4개까지 재생
        if (sfxObjList.Count < sfxMaxCount)
        {
            GameObject newSoundOBJ = new GameObject();
            newSoundOBJ.transform.SetParent(this.transform);
            newSoundOBJ.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundOBJ.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            newSoundOBJ.name = "SFXObj";

            sfxSrcList[sfxObjList.Count] = a_AudioSrc;
            sfxObjList.Add(newSoundOBJ);
        }

        if (soundData != null && sfxSrcList[sfxCurCount] != null)
        {
            sfxSrcList[sfxCurCount].clip = soundData.audioClip;
            sfxSrcList[sfxCurCount].outputAudioMixerGroup = soundData.audioMixerGroup;
            sfxSrcList[sfxCurCount].spatialBlend = 0f;
            //sfxSrcList[sfxCurCount].volume = sfxVolume;
            sfxSrcList[sfxCurCount].loop = bLoop;
            sfxSrcList[sfxCurCount].PlayDelayed(delay);

            sfxCurCount++;
            if (sfxMaxCount <= sfxCurCount)
                sfxCurCount = 0;
        }
    }

    //동일한 효과음 한번만 호출
    public void PlaySFXOnce(string key, float delay = 0, bool bLoop = false)
    {
        if (sfxContainer.ContainsKey(key) == false)
            return;

        soundData = sfxContainer[key];

        foreach (var sfxSrc in sfxSrcList)
        {
            if (sfxSrc.clip == soundData.audioClip)
            {
                if (!sfxSrc.isPlaying)
                {
                    //sfxSrc.volume = sfxVolume;
                    sfxSrc.loop = bLoop;
                    sfxSrc.PlayDelayed(delay);
                }
                return;
            }
        }

        // 최대 10개까지 재생
        if (sfxObjList.Count < sfxMaxCount)
        {
            GameObject newSoundOBJ = new GameObject();
            newSoundOBJ.transform.SetParent(this.transform);
            newSoundOBJ.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundOBJ.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            newSoundOBJ.name = "SFXObj";

            sfxSrcList[sfxObjList.Count] = a_AudioSrc;
            sfxObjList.Add(newSoundOBJ);
        }

        if (soundData != null && sfxSrcList[sfxCurCount] != null)
        {
            sfxSrcList[sfxCurCount].clip = soundData.audioClip;
            sfxSrcList[sfxCurCount].outputAudioMixerGroup = soundData.audioMixerGroup;
            sfxSrcList[sfxCurCount].spatialBlend = 0f;
            //sfxSrcList[sfxCurCount].volume = sfxVolume;
            sfxSrcList[sfxCurCount].loop = bLoop;
            sfxSrcList[sfxCurCount].PlayDelayed(delay);

            sfxCurCount++;
            if (sfxMaxCount <= sfxCurCount)
                sfxCurCount = 0;
        }
    }

    public void PlaySFXFromObject(Vector3 soundPosition, string key, float delay = 0, bool bLoop = false)
    {
        if (sfxContainer.ContainsKey(key) == false)
            return;

        soundData = sfxContainer[key];

        // 최대 4개까지 재생
        if (sfxObjList.Count < sfxMaxCount)
        {
            GameObject newSoundOBJ = new GameObject();
            newSoundOBJ.transform.SetParent(this.transform);
            newSoundOBJ.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundOBJ.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            newSoundOBJ.name = "SFXObj";

            sfxSrcList[sfxObjList.Count] = a_AudioSrc;
            sfxObjList.Add(newSoundOBJ);
        }

        if (soundData != null && sfxSrcList[sfxCurCount] != null)
        {
            sfxSrcList[sfxCurCount].transform.position = soundPosition;

            sfxSrcList[sfxCurCount].clip = soundData.audioClip;
            sfxSrcList[sfxCurCount].outputAudioMixerGroup = soundData.audioMixerGroup;
            sfxSrcList[sfxCurCount].spatialBlend = 1f;
            //sfxSrcList[sfxCurCount].volume = sfxVolume;
            sfxSrcList[sfxCurCount].loop = bLoop;
            sfxSrcList[sfxCurCount].PlayDelayed(delay);

            sfxCurCount++;
            if (sfxMaxCount <= sfxCurCount)
                sfxCurCount = 0;
        }
    }

    public void ClearAllSFX()
    {
        foreach (var src in sfxSrcList)
            src.Stop();
    }

    public void ChangeSFXPitch(float pitch)
    {
        foreach (var src in sfxSrcList)
        {
            if (src.clip == null)
                src.pitch = pitch;
            else if (src.clip.name == "super" || src.clip.name == "hot") src.pitch = 1;
            else src.pitch = pitch;
        }
    }
    #endregion
}