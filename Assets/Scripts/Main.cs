using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    [Header("Victory Condition")]
    public float distanceTravelled;
    public float destinationDistance;
    public float speedPerEngine;

    [System.Serializable]
    public struct Engine
    {
        public uint health;
    }
    public Engine[] engines = new Engine[2];
    public uint GetActiveEngines()
    {
        uint count = 0;
        foreach (Engine engine in engines)
        {
            if (0 < engine.health)
                count++;            
        }
        return count;
    }

    public Slider destinationDistanceSlider;
    public GameObject victoryScreen;


    public void Start()
    {
        
    }

    public void Update()
    {
        if (distanceTravelled < destinationDistance)
        {
            uint activeEngines = GetActiveEngines();
            if (0 < activeEngines)
            {
                distanceTravelled += Time.deltaTime * activeEngines * speedPerEngine;
                destinationDistanceSlider.value = distanceTravelled / destinationDistance;
            }
            victoryScreen.SetActive(false);
        }
        else
        {
            // win
            victoryScreen.SetActive(true);
            destinationDistanceSlider.value = 1f;
        }
    }
}
