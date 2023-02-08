using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    public Transform target;
    public Canvas canvas;
    public Text text;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        text = canvas.GetComponentInChildren<Text>();
        text.text = "Hello World";
    }

    private void Update()
    {
        canvas.transform.position = target.position + new Vector3(0f, 2f, 0f);
    }
}
