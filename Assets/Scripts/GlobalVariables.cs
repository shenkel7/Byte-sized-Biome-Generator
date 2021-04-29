using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{

    public static float amplitude;
    public static float precipitation;
    public static float temperature;

    // Start is called before the first frame update
    void Start()
    {
        amplitude = 2.0f;
        precipitation = 1;
        temperature = 90;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // amp range [0, 1]
    public void setAmplitude(float amp)
    {
        amplitude = amp * 40.0f;
    }

    // range [0, 1]
    public void setPrecipitation(float precip)
    {
        precipitation = precip * 100.0f;
    }

    // range [0, 1]
    public void setTemperature(float temp)
    {
        temperature = temp * 100.0f;
    }
}
