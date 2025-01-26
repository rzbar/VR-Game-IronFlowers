using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PPTplayer : MonoBehaviour
{
    public List<Sprite> sprites;
    private int index=0;
    public Image image;
    public Button left;
    public Button right;

    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
            image.sprite = sprites[index];
        }
    }
    private void Start()
    {
        left.onClick.AddListener(() =>
        {
            Index = (Index - 1 + sprites.Count) % sprites.Count;
        });
        right.onClick.AddListener(() =>
        {
            Index = (Index + 1) % sprites.Count;

        });
    }
}
