
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
    public TextMeshProUGUI totalTimeText;      // ������ʾ��ʱ��
    public TextMeshProUGUI currentTimeText;    // ������ʾ��ǰʱ��
    public Toggle videoMute;
    public VideoClip[] videoClips;

    private float videoLength; // ��Ƶ��ʱ�����룩  

    void Start()
    {
        beginButton.onClick.AddListener(OnClickPlay);
        // ��ȡ��Ƶ��ʱ��  
        videoLength = (float)videoPlayer.length;
        // ������ʱ���ı�  
        totalTimeText.text = FormatTime(videoLength);

        // ������Ƶ�����¼��Ը���UI  
        videoPlayer.prepareCompleted += OnVideoReady;
        videoPlayer.frameReady += OnFrameReady;

        // ����Slider������Ƶʱ��  
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
        // ��Ƶ׼����ɺ���Կ�ʼ����  
        videoPlayer.Play();
    }

    private void OnFrameReady(VideoPlayer source, long frameIdx)
    {
        // ���µ�ǰʱ���ı�  
        currentTimeText.text = FormatTime((float)source.time);

        // ����Slider��Value��ƥ����Ƶ�ĵ�ǰ֡  
        videoSlider.value = (int)(source.frame * videoSlider.maxValue / videoPlayer.frameCount);
    }

    private void OnSliderValueChanged(float value)
    {
        // ��Slider��ֵ�ı�ʱ��������Ƶ����Ӧ��֡  
        videoPlayer.frame = (long)(value * videoPlayer.frameCount / videoSlider.maxValue);
    }

    // �����������ڸ�ʽ��ʱ��  
    public static string FormatTime(float seconds)
    {
        int minutes = (int)Mathf.Floor(seconds / 60);
        int remainingSeconds = (int)Mathf.Round(seconds % 60);
        string formatted = minutes.ToString("D2") + ":" + remainingSeconds.ToString("D2");
        return formatted;
    }

    // �����������粥�š���ͣ�ȿ����ڴ˴����  
    public void OnPlayVideo() { videoPlayer.Play(); }
    public void PauseVideo() { if (videoPlayer.isPlaying) videoPlayer.Pause(); } 

}
