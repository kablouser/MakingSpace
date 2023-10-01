using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
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

#if UNITY_EDITOR
    [Header("Fire System [EDITOR]")]
    public float fireNodeNeighbourDistance;
    public bool showFireNodeNeighboursGizmos = true;
    [Header("Put your fire FX here first then click ... -> GenerateFireNodeNeighbours")]
    public GameObject[] fireNodeFireFXInit;
#endif
    [System.Serializable]
    public struct FireNode
    {
        public GameObject fireFX;
        public List<int> neighbours;
    }
    /*
     * if you want to know if a place is on fire, use a index var
     * e.g.
     * int engine0FireIndex = 5;
     * int engine1FireIndex = 6;
     * if (fireNodes[engine0FireIndex].fireFX.activeSelf)
     *     // engine0 is on fire!
     */
    [Header("Fire System [GAME]")]    
    public FireNode[] fireNodes;
    public float fireSpreadTime;
    public float nextFireSpreadTimeRemain;

    [Header("Resources")]
    public uint
        water,
        fuel,
        oxygen;

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
            return;
        }

        if (0f < nextFireSpreadTimeRemain)
        {
            nextFireSpreadTimeRemain -= Time.deltaTime;
        }
        else
        {
            nextFireSpreadTimeRemain = fireSpreadTime;

            HashSet<int> spreadToFireNodes = new HashSet<int>(fireNodes.Length);
            for (int i = 0; i < fireNodes.Length; i++)
                if (fireNodes[i].fireFX.activeSelf)
                    spreadToFireNodes.AddRange(fireNodes[i].neighbours);
            foreach (int i in spreadToFireNodes)
                fireNodes[i].fireFX.SetActive(true);
        }
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (showFireNodeNeighboursGizmos && fireNodes != null)
        {
            for (int i = 0; i < fireNodes.Length; i++)
            {
                if (fireNodes[i].fireFX == null)
                    continue;

                Vector3 positionI = fireNodes[i].fireFX.transform.position;

                foreach (int j in fireNodes[i].neighbours)
                {
                    Vector3 positionJ = fireNodes[j].fireFX.transform.position;

                    bool inRange = Vector3.Distance(positionJ, positionI) <= fireNodeNeighbourDistance;
                    Gizmos.color = inRange ? Color.green : Color.red;
                    Gizmos.DrawLine(positionJ, positionI);
                }
            }
        }
    }

    [ContextMenu("/GenerateFireNodeNeighbours")]
    public void GenerateFireNodeNeighbours()
    {
        if (fireNodeFireFXInit == null)
            return;

        Undo.RecordObject(this, "GenerateFireNodeNeighbours");

        fireNodes = new FireNode[fireNodeFireFXInit.Length];

        for (int i = 0; i < fireNodes.Length; i++)
        {
            fireNodes[i].fireFX = fireNodeFireFXInit[i];
        }

        for (int i = 0; i < fireNodes.Length; i++)
        {
            if (fireNodes[i].neighbours == null)
                fireNodes[i].neighbours = new List<int>();
            else
                fireNodes[i].neighbours.Clear();

            Vector3 positionI = fireNodes[i].fireFX.transform.position;

            for (int j = 0; j < fireNodes.Length; j++)
            {
                if (i == j)
                    continue;

                Vector3 positionJ = fireNodes[j].fireFX.transform.position;
                if (Vector3.Distance(positionJ, positionI) <= fireNodeNeighbourDistance)
                {
                    fireNodes[i].neighbours.Add(j);
                }
            }
        }
    }
#endif
}
