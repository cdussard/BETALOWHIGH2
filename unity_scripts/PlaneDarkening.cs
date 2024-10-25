using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneDarkening : MonoBehaviour
{
    [Range(0f, 1f)]
    public float darknessLevel = 0f; // Paramètre de contrôle de l'obscurité
    private Color baseColor = new Color(0.15f, 0.14f, 0.14f, 0f); // Couleur de base (transparente)
    private Color maxColor = new Color(0.15f, 0.14f, 0.14f, 1f); // Couleur maximale "#272323"

    private Material planeMaterial;

    void Start()
    {
        planeMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Interpoler entre transparent et maxColor en fonction de darknessLevel
        Color currentColor = Color.Lerp(baseColor, maxColor, darknessLevel);
        planeMaterial.color = currentColor;
    }
}