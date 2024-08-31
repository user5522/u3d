using System.Linq;
using UnityEngine;

public class HighlightInSlowMode : MonoBehaviour
{
    private GameObject player;
    private Material slowModeMaterial;
    private Material defaultMaterial;
    private Renderer objRenderer;
    private SlowDownTime sdt;

    void Awake()
    {
        // this sucks ass
        player = GameObject.FindGameObjectsWithTag("Player").SkipWhile(e => e.name != "Player").First();
        sdt = player.GetComponent<SlowDownTime>();
        slowModeMaterial = sdt.slowMotionMaterial;
        objRenderer = GetComponent<Renderer>();
        defaultMaterial = objRenderer.material;
    }

    void Update()
    {
        if (sdt.isSlowMotion) objRenderer.material = slowModeMaterial;
        else objRenderer.material = defaultMaterial;
    }
}