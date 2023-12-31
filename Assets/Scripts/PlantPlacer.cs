using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlantPlacer : MonoBehaviour
{
    [SerializeField]
    private Main main;

    [SerializeField]
    private Color GrayOutColor = Color.gray;

    [SerializeField]
    private LayerMask LayersCanNotPlaceOn;

    private bool isPlacing = false;
    private Plant plantBeingPlaced;

    public Plant waterPlant;
    public Plant fuelPlant;
    public Plant oxygenPlant;

    public Transform player;
    public LayerMask placingRaycastMask;

    private Resources CurrentPlantType;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnWaterPlantButton();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnFuelPlantButton();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnOxygenPlantButton();
        }

        if(isPlacing && plantBeingPlaced != null)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(plantBeingPlaced.gameObject);
                plantBeingPlaced = null;
                isPlacing = false;
                return;
            }

            //Make plant follow mouse
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            plantBeingPlaced.transform.position = Camera.main.ScreenToWorldPoint(mousePos);

            if(ValidPlacement())
            {
                plantBeingPlaced.transform.position = new Vector3(plantBeingPlaced.transform.position.x, plantBeingPlaced.transform.position.y, 0.0f);
                plantBeingPlaced.SetGrayedOut(false);

                if(Input.GetMouseButtonDown(0))
                {
                    plantBeingPlaced.PlacedPlant();

                    switch(CurrentPlantType)
                    {
                        case Resources.Water:
                            main.waterSeeds--;
                            break;
                        case Resources.Fuel:
                            main.fuelSeeds--;
                            break;
                        case Resources.Oxygen:
                            main.oxygenSeeds--;
                            break;
                    }

                    isPlacing = false;
                    plantBeingPlaced = null;
                }
            }
            else
            {
                //Gray out
                plantBeingPlaced.SetGrayedOut(true);
            }
        }
    }

    public void StartPlacing(Plant PlantToPlace)
    {
        if(plantBeingPlaced != null)
        {
            Destroy(plantBeingPlaced.gameObject);
        }

        isPlacing = true;
        plantBeingPlaced = Object.Instantiate(PlantToPlace);
        plantBeingPlaced.main = main;
    }

    private bool ValidPlacement()
    {
        var plantPos = plantBeingPlaced.transform.position;
        if (Physics2D.OverlapBox(plantPos, 5f * plantBeingPlaced.spriteRenderer.transform.lossyScale * plantBeingPlaced.spriteRenderer.size, 0) != null)            
        {
            return false;
        }

        Vector3 vector = player.position - plantPos;
        float vectorDistance = vector.magnitude;
        if (Mathf.Epsilon < vectorDistance)
        {
            RaycastHit2D hit = Physics2D.Raycast(plantPos, vector / vectorDistance, vectorDistance - Mathf.Epsilon, placingRaycastMask);
            if (hit.collider == null)
            {
                // nothing in raycast path
                return true;
            }
            return false;
        }
        
        return true;
    }

    public void OnWaterPlantButton()
    {
        if(main.waterSeeds <= 0)
        {
            return;
        }
        CurrentPlantType = Resources.Water;
        StartPlacing(waterPlant);
    }

    public void OnFuelPlantButton()
    {
        if(main.fuelSeeds <= 0)
        {
            return;
        }
        CurrentPlantType = Resources.Fuel;
        StartPlacing(fuelPlant);
    }

    public void OnOxygenPlantButton()
    {
        if(main.oxygenSeeds <= 0)
        {
            return;
        }
        CurrentPlantType = Resources.Oxygen;
        StartPlacing(oxygenPlant);
    }
}
