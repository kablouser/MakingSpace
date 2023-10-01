using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    [SerializeField]
    private Main main;

    [SerializeField]
    private TextMeshProUGUI waterSeedsText;
    [SerializeField]
    private TextMeshProUGUI fuelSeedsText;
    [SerializeField]
    private TextMeshProUGUI oxygenSeedsText;

    private void Update()
    {
        waterSeedsText.SetText(main.waterSeeds.ToString());
        fuelSeedsText.SetText(main.fuelSeeds.ToString());
        oxygenSeedsText.SetText(main.oxygenSeeds.ToString());
    }
}
