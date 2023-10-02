using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Main main;

    [SerializeField]
    private TextMeshProUGUI waterSeedsText;
    [SerializeField]
    private TextMeshProUGUI fuelSeedsText;
    [SerializeField]
    private TextMeshProUGUI oxygenSeedsText;
    [SerializeField]
    private TextMeshProUGUI oxygenText;
    [SerializeField]
    private TextMeshProUGUI waterText;
    [SerializeField]
    private TextMeshProUGUI fuelText;


    private void Update()
    {
        waterSeedsText.SetText(main.waterSeeds.ToString());
        fuelSeedsText.SetText(main.fuelSeeds.ToString());
        oxygenSeedsText.SetText(main.oxygenSeeds.ToString());
        oxygenText.SetText($"O2: {Mathf.Ceil(main.oxygen)}s");
        waterText.SetText("H2O: " + main.water);
        fuelText.SetText($"Fuel: {Mathf.Ceil(main.fuel)}s");
    }
}
