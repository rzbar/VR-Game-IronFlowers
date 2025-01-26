using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CanShowDetails : MonoBehaviour
{
    private ShowDetailsManager DM;
    public Vector3 offset=new Vector3(1,0.5f,0);
    public string details;
    private void Awake()
    {
        DM = GameObject.Find("ShowDetailsManager").GetComponent<ShowDetailsManager>();
    }
    public void OnHover()
    {
        GameObject obj = DM.ShowCanvas(details);
        obj.transform.position = transform.position + offset;
        obj.transform.LookAt(Camera.main.transform);
        obj.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - obj.transform.position); 
        print(this.name+"被选中");
    }
    public void OnDisHover()
    {
        DM.DisCanvas();
        print(this.name + "被取消选中");
    }
}
