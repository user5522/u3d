using UnityEngine;

public class HighlightInSlowMode : MonoBehaviour
{
    private GameObject player;
    private Material slowModeMaterial;
    private Material defaultMaterial;
    private Renderer objRenderer;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        slowModeMaterial = player.GetComponent<SlowDownTime>().slowMotionMaterial;
        objRenderer = GetComponent<Renderer>();
        defaultMaterial = objRenderer.material;
    }

    void Update()
    {
        if (Time.timeScale < 1) objRenderer.material = slowModeMaterial;
        else objRenderer.material = defaultMaterial;
    }
}