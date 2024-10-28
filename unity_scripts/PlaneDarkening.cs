using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlaneDarkening : MonoBehaviour
{
    [Range(0f, 1f)]
    public float darknessLevel = 0f; // Paramètre de contrôle de l'obscurité
    //private Color baseColor = new Color(0.0f, 0.14f, 0.14f, 0f); // Couleur de base (transparente)
    private Color maxColor = new Color(0.15f, 0.14f, 0.14f, 1f); // Couleur maximale "#272323"
    public Color currentColor;
    public neurofeedbackBETALOWHIGH2 script;
    public GameObject controller;
    float slope_parasite;
    float intercept_parasite;
    float opaciteMax;

    private Material planeMaterial;

    void Start()
{       Debug.Log("start plane");
        planeMaterial = GetComponent<Renderer>().material;
        script = controller.GetComponent<neurofeedbackBETALOWHIGH2>();
        slope_parasite = script.slope_parasite;
        intercept_parasite = script.intercept_parasite;
        opaciteMax = script.opaciteMax;
        Debug.Log(string.Format("slope para : {0} intercept para : {1}",slope_parasite.ToString(),intercept_parasite));
    }

    void Update()
    {
        if (script.currentState == "main")
        {
            darknessLevel = script.currentParasiteBetaValue * slope_parasite + intercept_parasite;
            Debug.Log(script.currentParasiteBetaValue);
            if (darknessLevel < 0)//plus bas que le meilleur seuil
            {
                darknessLevel = 0.0f;
            }
            else if (darknessLevel>opaciteMax)
            {
                darknessLevel = opaciteMax;
            }
            else 
            {

            }
            Debug.Log(string.Format("darkness {0}", darknessLevel.ToString()));
            // Interpoler entre transparent et maxColor en fonction de darknessLevel
            currentColor = maxColor;
            currentColor.a = darknessLevel; // Modifier seulement l'opacité
            //currentColor = Color.Lerp(baseColor, maxColor, darknessLevel);
            planeMaterial.color = currentColor;
        }

        else if (script.currentState == "endTrial")
        {
            currentColor.a = 0f; // Rendre complètement transparent
            //currentColor = Color.Lerp(baseColor, maxColor, 0f);
            planeMaterial.color = currentColor;
        }



    }
}