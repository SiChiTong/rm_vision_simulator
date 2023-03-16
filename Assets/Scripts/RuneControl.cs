using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneControl : MonoBehaviour
{
    private BladeLightControl[] blcs;
    private bool isRunning;
    private float rotationSpeed = 60f;

    // Start is called before the first frame update
    void Start()
    {
        blcs = new BladeLightControl[5];

        blcs[0] = GameObject.Find("r1").GetComponent<BladeLightControl>();
        blcs[1] = GameObject.Find("r2").GetComponent<BladeLightControl>();
        blcs[2] = GameObject.Find("r3").GetComponent<BladeLightControl>();
        blcs[3] = GameObject.Find("r4").GetComponent<BladeLightControl>();
        blcs[4] = GameObject.Find("r5").GetComponent<BladeLightControl>();
    }

    void Update()
    {
        if (!isRunning)
        {
            foreach (var blc in blcs)
            {
                blc.LightOff();
            }
            StartCoroutine(RuneLoopCoroutine());
            isRunning = true;
        }

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    IEnumerator RuneLoopCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var blc in blcs)
        {
            blc.LightOn();
            yield return new WaitForSeconds(1.5f);
            blc.BeenHit();
            yield return new WaitForSeconds(0.01f);
        }

        // loop 3 times
        for (int i = 0; i < 3; i++)
        {
            foreach (var blc in blcs)
            {
                blc.LightOff();
            }
            yield return new WaitForSeconds(0.2f);

            foreach (var blc in blcs)
            {
                blc.BeenHit();
            }
            yield return new WaitForSeconds(0.2f);
        }

        isRunning = false;
    }
}
