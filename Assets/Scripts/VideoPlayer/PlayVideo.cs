using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public GameObject StartMove;//启动动画
    private void Awake()
    {
        StartPlay();
    }

    public void StartPlay()
    {
        if (StartMove != null)//播放启动动画
        {
            VideoPlayer videoPlayer = StartMove.GetComponent<VideoPlayer>();
            StartMove.SetActive(true);//默认对象关闭
            //videoPlayer.frame = 100;//跳过前100帧
            videoPlayer.loopPointReached += VideoPlayer_loopPointReached;//添加播放结束事件
            StartMove.transform.SetAsLastSibling();
            videoPlayer.Play();//播放视频
        }
        else
            Debug.LogError("启动动画 StartMove=null");
    }

    /// <summary>
    /// 播放结束或播放到循环的点时被执行
    /// </summary>
    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        if (StartMove != null)
        {
            StartMove.SetActive(false);
            //GameObject.Destroy(StartMove);
            //StartMove = null;
        }
        else
            Debug.LogError("启动动画 播放结束 StartMove=null");
    }

}
