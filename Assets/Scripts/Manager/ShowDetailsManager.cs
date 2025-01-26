using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowDetailsManager : MonoBehaviour
{
    public GameObject canvas;
    private void Start()
    {
        if (canvas == null) return;
        canvas.SetActive(false);
    }
    public GameObject ShowCanvas(string str)
    {
        if (canvas == null) return null;
        canvas.SetActive(true);
        TMP_Text text = canvas.transform.Find("Text").GetComponent<TMP_Text>();
        text.text = str;
        return canvas;
    }
    public void DisCanvas()
    {
        if (canvas == null) return;
        canvas.SetActive(false);
    }
}
