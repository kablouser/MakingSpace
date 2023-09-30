using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPlacer : MonoBehaviour
{


    private bool isPlacing = false;
    private Plant plantBeingPlaced;

    public Plant fuelPlant;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartPlacing(fuelPlant);
        }

        if(isPlacing && plantBeingPlaced != null)
        {
            //Make plant follow mouse
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            plantBeingPlaced.transform.position = Camera.main.ScreenToWorldPoint(mousePos);

            if(ValidPlacement())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    plantBeingPlaced.PlacedPlant();

                    isPlacing = false;
                    plantBeingPlaced = null;
                }
            }
            else
            {
                //Gray out
                print("Not valid");
            }
        }
    }

    public void StartPlacing(Plant PlantToPlace)
    {
        isPlacing = true;
        plantBeingPlaced = Object.Instantiate(PlantToPlace);
    }

    private bool ValidPlacement()
    {
        return true;
    }
}
