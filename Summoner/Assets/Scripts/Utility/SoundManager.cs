using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Common;
using Res;

public class SoundManager : MonoBehaviour
{
    public enum SoundBGType
    {
        SoundBGType_None,//无
        SoundBGType_Add,//从小到大
        SoundBGType_Reduce,//从大到小
    }
    #region 变量参数
    public const float MAX_VOLUME = 0.2f;
    [Range(0, 1f)]
    private float _volume = MAX_VOLUME;
    private bool _OpenBgMusic = true;
    private bool _OpenSound = true;
    private string _BgMusicName = string.Empty;
    private readonly float _DelayChange = 1.0f;
    private float _fEndTime = -1.0f;
    private AudioClip _ChangeBGObject = null;
    private AudioSource _bgMusic = null;
    private List<AudioSource> _list = new List<AudioSource>();
    private SoundBGType _eSoundBGType = SoundBGType.SoundBGType_None;
    static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("[SoundManager]");
                _instance = go.AddComponent<SoundManager>();
                _instance.Initalize();
            }
            return _instance;
        }
    }

    public static AudioSource BgMusic
    {
        get
        {
            return Instance._bgMusic;
        }
    }

    public static float Volume
    {
        get
        {
            return Instance._volume;
        }
        set
        {
            Instance._volume = value;
            for (int i = 0; i < Instance._list.Count; ++i)
            {
                Instance._list[i].volume = value;
            }

            if(Instance._bgMusic != null)
            {
                Instance._bgMusic.volume = value;
            }

            if(value <= 0)
            {
                PlayerPrefs.SetInt("volume", -1);
            }
            else
            {
                PlayerPrefs.SetInt("volume", 1);
            }
        }
    }

    public static bool OpenBgMusic
    {
        get
        {
            return Instance._OpenBgMusic;
        }
        set
        {
            if(value != Instance._OpenBgMusic)
            {
                if (value)
                {
                    Instance.ResumeAudio(BgMusic);
                }
                else
                {
                    Instance.StopAudio(BgMusic);
                }
            }
                 Instance._OpenBgMusic = value;
            if (!value)
            {
                PlayerPrefs.SetInt("OpenBgMusic", -1);
            }
            else
            {
                PlayerPrefs.SetInt("OpenBgMusic", 1);
            }
            PlayerPrefs.Save();
        }
    }

    public static bool OpenSound
    {
        get
        {
            return Instance._OpenSound;
        }
        set
        {
            if (!value)
            {
                Instance.ClearSound();
            }
            Instance._OpenSound = value;
            if (!value)
            {
                PlayerPrefs.SetInt("OpenSound", -1);
            }
            else
            {
                PlayerPrefs.SetInt("OpenSound", 1);
            }
            PlayerPrefs.Save();
        }
    }
    #endregion
    #region 内在函数
    protected void Initalize()
    {
        _list.Clear();
        Volume = PlayerPrefs.GetInt("volume") >= 0 ? MAX_VOLUME : 0;
        OpenBgMusic = (PlayerPrefs.GetInt("OpenBgMusic") >= 0);
        OpenSound = (PlayerPrefs.GetInt("OpenSound") >= 0);
        _bgMusic = gameObject.AddComponent<AudioSource>();
        this.transform.SetParent(Common.Root.root);
    }

    public void Deinitialization()
    {
        ClearSound();
        _BgMusicName = string.Empty;
        _bgMusic.clip = null;
    }

    private void StopAudio(AudioSource source)
    {
        if(source != null)
        {
            source.Stop();
        }
    }

    private void ResumeAudio(AudioSource source)
    {
        if (source != null)
        {
            source.Play();
        }
    }

    private void PlayAudio(AudioClip clip, bool loop, float volume, float delay = 0.0f,AudioSource source = null)
    {
        if(clip == null)
        {
            return;
        }

        if (source == null)
        {
            GameObject go = new GameObject(clip.name);
            source = go.AddComponent<AudioSource>();
            go.transform.SetParent(transform);
            _list.Add(source);
        }
        source.loop = loop;
        source.clip = clip;
        source.volume = volume;
        if (delay > 0)
        {
            source.PlayDelayed(delay);
        }
        else
        {
            source.Play();
        }
    }

    private void Update()
    {
        int count = _list.Count - 1;
        for(int i = count; i > -1; --i)
        {
            if(!_list[i].isPlaying)
            {
                Destroy(_list[i].gameObject);
                _list.RemoveAt(i);
            }
        }
        UpdateBG();
    }

    private void UpdateBG()
    {
        if(_eSoundBGType == SoundBGType.SoundBGType_None)
        {
            return;
        }

        if(_eSoundBGType == SoundBGType.SoundBGType_Reduce)
        {
            if (_fEndTime > 0.0f)
            {
                float endTime = _fEndTime - Time.time;
                float percent = (endTime / _DelayChange);
                Instance._bgMusic.volume = Volume * percent;
                if (percent <= 0)
                {
                    if (_ChangeBGObject != null)
                    {
                        _instance.PlayAudio(_ChangeBGObject, true, Volume, 0.0f, BgMusic);
                        Instance._bgMusic.volume = Volume;
                    }
                    _fEndTime = Time.time + _DelayChange;
                    _eSoundBGType = SoundBGType.SoundBGType_Add;
                    _ChangeBGObject = null;
                }
            }
        }
        else if(_eSoundBGType == SoundBGType.SoundBGType_Add)
        {
            if (_fEndTime > 0.0f)
            {
                float endTime = _fEndTime - Time.time;
                float percent = 1 - (endTime / _DelayChange);
                Instance._bgMusic.volume = Volume * percent;
                if (percent >= 1.0f)
                {
                    _eSoundBGType = SoundBGType.SoundBGType_None;
                    _fEndTime = -1;
                }
            }
        }
    }

    private void ClearSound()
    {
        for (int i = 0; i < _list.Count; ++i)
        {
            _list[i].Stop();
            Destroy(_list[i].gameObject);
        }
        _list.Clear();
    }


    #endregion
    #region 公共函数
    public static void PlayBGMusic(string path)
    {
        if(!OpenBgMusic)
        {
            return;
        }

        if (Instance._BgMusicName == path)
        {
            return;
        }
        Instance._BgMusicName = path;
        Res.ResourcesManager.Instance.AsyncLoadResource<AudioClip>(path, Res.ResourceType.Audio, OnBGMusicComplete);
    }

    private static void OnBGMusicComplete(string path, Object go)
    {
        Instance._fEndTime = Time.time + Instance._DelayChange;
        Instance._ChangeBGObject = go as AudioClip;
        Instance._eSoundBGType = SoundBGType.SoundBGType_Reduce;
    }

    public static void PlaySound(string path ,bool isLoop = false,float delay = 0)
    {
        if(!OpenSound)
        {
            return;
        }
        AudioClip clip = Res.ResourcesManager.Instance.SyncGetResource<AudioClip>(path, Res.ResourceType.Audio);
        _instance.PlayAudio(clip, isLoop, Volume);
    }
    #endregion
}
