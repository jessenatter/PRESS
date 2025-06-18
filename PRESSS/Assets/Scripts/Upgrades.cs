using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public static class Upgrades
{
    public static List<Upgrade> activeUpgrades = new List<Upgrade>();
    static List<Upgrade> possibleUpgrades = new List<Upgrade>();

    public static List<Upgrade> offeredUpgrades = new List<Upgrade>();

    public static bool upgradeSelected = false,upgradeOffered = false;
    static List<Image> upgradeImages = new List<Image>();
    static List<TextMeshProUGUI> upgradeTexts = new List<TextMeshProUGUI>();
    static public GameObject upgradeMenu;
    public static Manager manager;

    public static void OfferNewUpgrade()
    {
        if (!upgradeOffered)
        {
            upgradeMenu.SetActive(true);
            upgradeSelected = false;

            offeredUpgrades.Clear();
            for (int i = 0; i < 3; i++)
            {
                int index = Random.Range(0, possibleUpgrades.Count);
                Upgrade _offeredUpgrade = possibleUpgrades[index];
                offeredUpgrades.Add(_offeredUpgrade);
                upgradeImages[i] = _offeredUpgrade.image;
                upgradeTexts[i].text = _offeredUpgrade.name;
            }

            upgradeOffered = true;
        }
    }

    public static void SelectUpgrade(Upgrade selectedUpgrade)
    {
        activeUpgrades.Add(selectedUpgrade);
        selectedUpgrade.StartUpgrade(manager);
        upgradeSelected = true;
        upgradeMenu.SetActive(false);
        upgradeOffered = false;
    }
}

public class Upgrade
{
    public string name;
    public Image image;
    public Manager manager;

    public virtual void StartUpgrade(Manager _manager)
    {
        manager = _manager;
    }

    public virtual void UpdateUpgrade()
    {

    }
}

public class IncreaseBoxSize : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Size";
        float newScale = manager.boxClass.gameObject.transform.localScale.x * 1.5f;
        manager.boxClass.gameObject.transform.localScale = new Vector2(newScale, newScale);
    }
}

public class Spikes : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Spikes";
    }
}

public class Stun : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Stun";
    }
}

public class Bounce : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Bounce";
    }
}

public class Speed : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Speed";
    }
}