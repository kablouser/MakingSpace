using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    [Header("Victory Condition")]
    public float distanceTravelled;
    public float destinationDistance;    

    [Header("Engines")]
    public float speedPerEngine;
    public AudioSource engine0Audio, engine1Audio;
    public AudioClip engineOnClip, engineOffClip, engineActiveClip;
    public float engineOnClipDuration;
    public float engine0OnClipDurationRemain, engine1OnClipDurationRemain;

    [System.Serializable]
    public struct Engine
    {
        public uint health;
        public GameObject afterburner;
    }
    public Engine[] engines = new Engine[2];

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
        public bool prevFireFXActive;

        // also handles randomising audio pitch
        public void SetActive(bool isActive)
        {
            fireFX.SetActive(isActive);
            if (isActive)
                fireFX.GetComponent<AudioSource>().pitch = Random.Range(0.95f, 1.05f);
        }
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

    public int engine0FireNode, engine1FireNode, generator0FireNode, generator1FireNode;

    [Header("Asteroid System")]
    // there is only max 1 asteroid at any time
    public Asteroid asteroid;
    public float asteroidIntervals;
    public float nextAsteroidTimeRemain;
    public float asteroidSpawnNegX;
    public Transform asteroidTarget;
    public int asteroidTargetFireNode;
    public float asteroidSpeed;
    public float asteroidExplodeFXDuration;
    public float asteroidExplodeTimeRemain;
    public float asteroidRotationSpeed;
    public GameObject asteroidImpactAudio;

    [Header("Resources")]
    public uint water;
    public uint fuel, oxygen;

    public List<Plant> plants = new List<Plant>();
    public int waterSeeds = 0;
    public int fuelSeeds = 0;
    public int oxygenSeeds = 0;

    [Header("Generators")]
    public Light2D generator0Light, generator1Light;
    public float generatorActiveIntensityBase;
    public float generatorActiveIntensityVary;
    public float generatorActiveLightPulseRate;
    public Light2D[] shipLights;
    public float shipLightIntensityFull;
    public Transform shipSweepLight0, shipSweepLight1;
    public float shipSweepLightAngleMax = 10f;
    public float shipSweepLightPulseRate = 2f;
    public AudioSource generator0Audio, generator1Audio;
    public AudioClip
        generatorActiveClip,
        generatorOnClip,
        generatorOffClip;
    public float generatorOnClipDuration;
    public float generator0PlayDurationRemain, generator1PlayDurationRemain;

    public void Start()
    {
        asteroid.gameObject.SetActive(false);
        foreach (FireNode node in fireNodes)
        {
            node.fireFX.SetActive(false);
        }
    }

    public void Update()
    {
        #region victory
        if (distanceTravelled < destinationDistance)
        {
            // Ship engine fire damage
            UpdateEngine(engine0FireNode, 0, engine0Audio, ref engine0OnClipDurationRemain);
            UpdateEngine(engine1FireNode, 1, engine1Audio, ref engine1OnClipDurationRemain);

            uint activeEngines = 0;
            foreach (Engine engine in engines)
            {
                if (0 < engine.health)
                {
                    activeEngines++;
                    engine.afterburner.SetActive(true);
                }
                else
                    engine.afterburner.SetActive(false);
            }

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
        #endregion

        #region fire spread
        if (0f < nextFireSpreadTimeRemain)
        {
            nextFireSpreadTimeRemain -= Time.deltaTime;
        }
        else
        {
            nextFireSpreadTimeRemain = fireSpreadTime;

            HashSet<int> spreadToFireNodes = new HashSet<int>(fireNodes.Length);
            for (int i = 0; i < fireNodes.Length; i++)
            {
                if (fireNodes[i].fireFX.activeSelf &&
                    fireNodes[i].prevFireFXActive) // ignore first frame of on-fire. easier reaction?
                    spreadToFireNodes.AddRange(fireNodes[i].neighbours);

                fireNodes[i].prevFireFXActive = fireNodes[i].fireFX.activeSelf;
            }
            foreach (int i in spreadToFireNodes)
                fireNodes[i].fireFX.SetActive(true);
        }
        #endregion

        #region asteroid
        if (Mathf.Epsilon < asteroidExplodeTimeRemain)
        {
            // exploding animation
            asteroidExplodeTimeRemain -= Time.deltaTime;

            if (asteroidExplodeTimeRemain <= 0f)
            {
                // animation end
                asteroidExplodeTimeRemain = 0f;
                asteroid.gameObject.SetActive(false);
            }
        }
        else if (asteroid.gameObject.activeSelf && asteroidTarget != null)
        {
            // asteroid hasn't hit ship yet
            var asteroidTransform = asteroid.transform;

            float distanceMove = asteroidSpeed * Time.deltaTime;
            if (Vector3.SqrMagnitude(asteroidTransform.position - asteroidTarget.position) < distanceMove * distanceMove)
            {
                // hit
                SetAsteroidExploded(true);
                asteroidExplodeTimeRemain = asteroidExplodeFXDuration;
                // set on fire!
                fireNodes[asteroidTargetFireNode].fireFX.SetActive(true);
                asteroidImpactAudio.transform.position = asteroidTarget.position;
                asteroidImpactAudio.GetComponent<AudioSource>().Play();
            }
            else
            {
                // still moving
                asteroidTransform.position = Vector3.MoveTowards(asteroidTransform.position, asteroidTarget.position, distanceMove);
                asteroid.rotator.Rotate(0, 0, Time.deltaTime * asteroidRotationSpeed);
            }
        }
        else
        {
            // waiting for next asteroid to start
            if (0f < nextAsteroidTimeRemain)
            {
                nextAsteroidTimeRemain -= Time.deltaTime;
            }
            else
            {
                nextAsteroidTimeRemain = asteroidIntervals;

                // spawn asteroid
                asteroid.gameObject.SetActive(true);
                SetAsteroidExploded(false);
                asteroidTarget = null;

                // pick an impact target
                const int MAX_TRIES = 16;
                for (int i = 0; i < MAX_TRIES; i++)
                {
                    asteroidTargetFireNode = Random.Range(0, fireNodes.Length);
                    if (asteroidTargetFireNode < fireNodes.Length &&
                        !fireNodes[asteroidTargetFireNode].fireFX.activeSelf)
                    {
                        asteroidTarget = fireNodes[asteroidTargetFireNode].fireFX.transform;
                        break;
                    }
                }

                if (asteroidTarget == null)
                {
                    // pick one, but its probably on fire already!
                    asteroidTargetFireNode = Random.Range(0, fireNodes.Length);
                    asteroidTarget = fireNodes[asteroidTargetFireNode].fireFX.transform;
                }
            }
            asteroid.transform.position = asteroidTarget.position - Vector3.left * asteroidSpawnNegX;
        }
        #endregion

        #region Generators
        {
            int generatorsActive = 2;
            float plantGrowthMultiplier = 1.0f;
            float generatorSin = Mathf.Sin(Time.time * generatorActiveLightPulseRate);
            UpdateGenerator(generator0FireNode, generatorSin, generator0Light, generator0Audio, ref plantGrowthMultiplier, ref generatorsActive, ref generator0PlayDurationRemain);
            UpdateGenerator(generator1FireNode, generatorSin, generator1Light, generator1Audio, ref plantGrowthMultiplier, ref generatorsActive, ref generator1PlayDurationRemain);

            foreach (Plant plant in plants)
            {
                if (plant != null)
                {
                    plant.gowthSpeedMultiplier = plantGrowthMultiplier;
                }
            }

            float shipLightIntensity;
            switch (generatorsActive)
            {
                case 0:
                    shipLightIntensity = 0f;
                    break;
                case 1:
                    shipLightIntensity = shipLightIntensityFull / 4f;
                    break;
                default:
                    shipLightIntensity = shipLightIntensityFull;
                    break;
            }

            foreach (Light2D light in shipLights)
            {
                light.intensity = shipLightIntensity;
            }

            if (shipLightIntensity == 0f)
            {
                shipSweepLight0.gameObject.SetActive(false);
                shipSweepLight1.gameObject.SetActive(false);
            }
            else
            {
                shipSweepLight0.gameObject.SetActive(true);
                shipSweepLight1.gameObject.SetActive(true);
                float shipSweepSin = Mathf.Sin(Time.time * shipSweepLightPulseRate);
                shipSweepLight0.rotation = Quaternion.Euler(0, 0, 180 + shipSweepSin * shipSweepLightAngleMax);
                shipSweepLight1.rotation = Quaternion.Euler(0, 0, 180 - shipSweepSin * shipSweepLightAngleMax);
            }
        }
        #endregion
    }

    public void SetAsteroidExploded(bool isExploded)
    {
        asteroid.asteroidBase.SetActive(!isExploded);
        asteroid.explodeFX.SetActive(isExploded);
        asteroid.afterburnerFX.SetActive(!isExploded);
    }

    // Does audio switching
    public void UpdateGenerator(
        in int generatorFireNode, in float generatorSin,
        Light2D light,
        AudioSource audioSource,
        ref float plantGrowthMultiplier, ref int generatorsActive, ref float playDurationRemain)
    {
        if (fireNodes[generatorFireNode].fireFX.activeSelf)
        {
            plantGrowthMultiplier -= 0.4f;
            generatorsActive--;
            if (light.enabled)
            {
                light.enabled = false;
                StartOffClip(audioSource, generatorOffClip);
            }
        }
        else
        {
            if (!light.enabled)
            {
                light.enabled = true;
                StartOnClip(audioSource, generatorOnClip, generatorOnClipDuration, ref playDurationRemain);
            }
            light.intensity = generatorActiveIntensityBase + generatorSin * generatorActiveIntensityVary;

            UpdateOnClipIntoLooping(audioSource, generatorActiveClip, ref playDurationRemain);
        }
    }

    public void StartOffClip(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.loop = false;
        source.Play();
    }

    public void StartOnClip(AudioSource source, AudioClip clip, float clipDuration, ref float durationRemain)
    {
        StartOffClip(source, clip);
        durationRemain = clipDuration;
    }

    public void UpdateOnClipIntoLooping(AudioSource source, AudioClip clip, ref float durationRemain)
    {
        if (0f < durationRemain)
        {
            durationRemain -= Time.deltaTime;
        }
        else if (source.clip != clip)
        {
            source.clip = clip;
            source.loop = true;
            source.Play();
        }
    }

    public void UpdateEngine(in int fireNode, in int engineIndex, AudioSource engineAudio, ref float remain)
    {
        if (fireNodes[fireNode].fireFX.activeSelf)
        {
            if (engines[engineIndex].health != 0)
            {
                engines[engineIndex].health = 0;
                StartOffClip(engineAudio, engineOffClip);
            }
        }
        else
        {
            // auto repairs. EASY MODE
            if (engines[engineIndex].health == 0)
            {
                engines[engineIndex].health = 1;
                StartOnClip(engineAudio, engineOnClip, engineOnClipDuration, ref remain);
            }
            UpdateOnClipIntoLooping(engineAudio, engineActiveClip, ref remain);
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

    [Header("[EDITOR] ConvertSpriteRendererMaterialsTo")]
    public Material convertSpriteRendererMaterial;
    [ContextMenu("/ConvertSpriteRendererMaterialsTo")]
    public void ConvertSpriteRendererMaterialsTo()
    {
        SpriteRenderer[] found = FindObjectsOfType<SpriteRenderer>(true);
        Undo.RecordObjects(found, "ConvertSpriteRendererMaterialsTo");

        foreach (SpriteRenderer spriteRenderer in found)
        {
            spriteRenderer.material = convertSpriteRendererMaterial;
        }
    }
#endif
}
