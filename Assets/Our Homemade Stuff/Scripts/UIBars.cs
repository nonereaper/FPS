using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.UI;

public class UIBars : MonoBehaviour
{
    [SerializeField] private GameObject front;
    private double percent;
    private float length;
    
    // Start is called before the first frame update
    void Start()
    {
        length = GetComponent<RectTransform>().rect.width;
        
    }
    public void setPercent(double p) {
        percent = p;

    }
    public void changeText(string s) {
        transform.GetChild(1).GetComponent<TMP_Text>().text = s;
    }
    // Update is called once per frame
    void Update()
    {
        float l = (float)(percent*length);
        front.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, l);
        front.GetComponent<RectTransform>().anchoredPosition = new Vector2((l-length)/2, front.GetComponent<RectTransform>().anchoredPosition.y);
    }
}
