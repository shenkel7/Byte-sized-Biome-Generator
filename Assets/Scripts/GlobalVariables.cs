using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{

    public static float amplitude;
    public static float precipitation;
    public static float temperature;
    public static float xSeed;
    public static float ySeed;

    float MAX_AMPLITUDE = 20.0f;
    float MAX_PRECIPITATION = 320.0f;
    float MAX_TEMP = 80.0f;

    // Start is called before the first frame update
    void Start()
    {
        amplitude = 0;
        precipitation = 0;
        temperature = 0;
        xSeed = .3f;
        ySeed = .3f;
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
        precipitation = precip;
    }

    // range [0, 1]
    public void setTemperature(float temp)
    {
        temperature = temp;
    }

    public void setXSeed(string x)
    {
        if(x.Length == 0)
        {
            xSeed = 0;
        } else
        {
            xSeed = float.Parse(x);
        }
    }

    public void setYSeed(string y)
    {
        if(y.Length == 0)
        {
            ySeed = 0;
        } else
        {
            ySeed = float.Parse(y);
        }
    }
}
