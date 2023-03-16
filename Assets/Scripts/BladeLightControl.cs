using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeLightControl : MonoBehaviour
{
    private Material[] materials;
    private Color glow;
    private Color fade;

    // Start is called before the first frame update
    void Start()
    {
        materials = new Material[5];
        materials[0] = GameObject.Find(name + "/l1").GetComponent<Renderer>().material;
        materials[1] = GameObject.Find(name + "/l2").GetComponent<Renderer>().material;
        materials[2] = GameObject.Find(name + "/l3").GetComponent<Renderer>().material;
        materials[3] = GameObject.Find(name + "/l4").GetComponent<Renderer>().material;
        materials[4] = GameObject.Find(name + "/l5").GetComponent<Renderer>().material;

        glow = materials[0].GetColor("_EmissiveColor");
        fade = Color.black;
    }

    public void LightOff()
    {
        foreach (var material in materials)
        {
            material.SetColor("_EmissiveColor", fade);
        }
    }

    public void LightOn()
    {
        materials[0].SetColor("_EmissiveColor", glow);
        materials[1].SetColor("_EmissiveColor", glow);
        materials[2].SetColor("_EmissiveColor", glow);
        materials[3].SetColor("_EmissiveColor", fade);
        materials[4].SetColor("_EmissiveColor", fade);
    }

    public void BeenHit()
    {
        foreach (var material in materials)
        {
            material.SetColor("_EmissiveColor", glow);
        }
    }
}
