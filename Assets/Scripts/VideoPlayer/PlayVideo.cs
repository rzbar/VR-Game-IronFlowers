
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button beginButton;
    public Slider videoSlider;
    //public AudioSource audioSource;
    public TextMeshProUGUI totalTimeText;      // 用于显示总时长
    public TextMeshProUGUI currentTimeText;    // 用于显示当前时间
    public Toggle videoMute;
    public VideoClip[] videoClips;

    private float videoLength; // 视频总时长（秒）  

    void Start()
    {
        beginButton.onClick.AddListener(OnClickPlay);
        // 获取视频总时长  
        videoLength = (float)videoPlayer.length;
        // 设置总时长文本  
        totalTimeText.text = FormatTime(videoLength);

        // 监听视频播放事件以更新UI  
        videoPlayer.prepareCompleted += OnVideoReady;
        videoPlayer.frameReady += OnFrameReady;

        // 设置Slider监听视频时间  
        videoSlider.onValueChanged.AddListener(OnSliderValueChanged);
        videoMute.onValueChanged.AddListener(OnMute);
        //OnMute(false);
        
    }

    private void OnClickPlay()
    {
        if (videoPlayer.isPlaying)
        {
            PauseVideo();
        }
        else
        {
            OnPlayVideo();
        }
    }

    private void OnMute(bool value)
    {
        videoPlayer.SetDirectAudioMute(0, value);
    }

    private void OnVideoReady(VideoPlayer source)
    {
        // 视频准备完成后可以开始播放  
        videoPlayer.Play();
    }

    private void OnFrameReady(VideoPlayer source, long frameIdx)
    {
        // 更新当前时间文本  
        currentTimeText.text = FormatTime((float)source.time);

        // 更新Slider的Value以匹配视频的当前帧  
        videoSlider.value = (int)(source.frame * videoSlider.maxValue / videoPlayer.frameCount);
    }

    private void OnSliderValueChanged(float value)
    {
        // 当Slider的值改变时，设置视频到相应的帧  
        videoPlayer.frame = (long)(value * videoPlayer.frameCount / videoSlider.maxValue);
    }

    // 辅助方法用于格式化时间  
    public static string FormatTime(float seconds)
    {
        int minutes = (int)Mathf.Floor(seconds / 60);
        int remainingSeconds = (int)Mathf.Round(seconds % 60);
        string formatted = minutes.ToString("D2") + ":" + remainingSeconds.ToString("D2");
        return formatted;
    }

    // 其他方法，如播放、暂停等可以在此处添加  
    public void OnPlayVideo() { videoPlayer.Play(); }
    public void PauseVideo() { if (videoPlayer.isPlaying) videoPlayer.Pause(); } 

}
