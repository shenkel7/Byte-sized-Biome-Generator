using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{

    public static float amplitude;
    public static float precipitation;
    public static float temperature;

    float MAX_AMPLITUDE = 30.0f;
    float MAX_PRECIPITATION = 100.0f;
    float MAX_TEMP = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        amplitude = 0;
        precipitation = 0;
        temperature = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // amp range [0, 1]
    public void setAmplitude(float amp)
    {
        amplitude = amp * MAX_AMPLITUDE;
    }

    // range [0, 1]
    public void setPrecipitation(float precip)
    {
        precipitation = precip * MAX_PRECIPITATION;
    }

    // range [0, 1]
    public void setTemperature(float temp)
    {
        temperature = temp * MAX_TEMP;
    }
}
