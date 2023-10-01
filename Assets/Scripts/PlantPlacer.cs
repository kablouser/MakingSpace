using System.Collections;
using System.Collections.Generic;
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
                plantBeingPlaced.transform.position = new Vector3(plantBeingPlaced.transform.position.x, plantBeingPlaced.transform.position.y, 0.0f);
                plantBeingPlaced.SetGrayedOut(false);

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
                plantBeingPlaced.SetGrayedOut(true);
            }
        }
    }

    public void StartPlacing(Plant PlantToPlace)
    {
        isPlacing = true;
        plantBeingPlaced = Object.Instantiate(PlantToPlace);
        plantBeingPlaced.main = main;
    }

    private bool ValidPlacement()
    {
        if(Physics2D.OverlapBox(plantBeingPlaced.transform.position, plantBeingPlaced.spriteRenderer.size, 0) != null)
        {
            return false;
        }

        return true;
    }
}
