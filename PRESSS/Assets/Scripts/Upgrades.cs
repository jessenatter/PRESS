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

    public static void Start()
    {
        IncreaseBoxSize increaseBoxSize = new IncreaseBoxSize();
        Spikes spikes = new Spikes();
        Stun stun = new Stun();
        Bounce bounce = new Bounce();
        Speed speed = new Speed();

        possibleUpgrades.AddRange(new Upgrade[] { increaseBoxSize,spikes,stun,bounce,speed, });

        foreach (Upgrade upgrade in possibleUpgrades)
            upgrade.StartUpgrade(manager);

        for(int i = 0; i < 3; i++)
        {
            upgradeImages.Add(upgradeMenu.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>());
            upgradeTexts.Add(upgradeMenu.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        }
    }


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
                upgradeImages[i].sprite = _offeredUpgrade.sprite;
                upgradeTexts[i].text = _offeredUpgrade.name;
            }

            upgradeOffered = true;
        }
    }

    public static void SelectUpgrade(Upgrade selectedUpgrade)
    {
        activeUpgrades.Add(selectedUpgrade);
        selectedUpgrade.ActivateUpgrade();
        upgradeSelected = true;
        upgradeMenu.SetActive(false);
    }
}

public class Upgrade
{
    public string name;
    public Sprite sprite;
    public Manager manager;

    public virtual void StartUpgrade(Manager _manager)
    {
        manager = _manager;
    }

    public virtual void UpdateUpgrade()
    {

    }

    public virtual void ActivateUpgrade()
    {
        GameObject newSpriteLayer = new GameObject("SpriteLayer");
        newSpriteLayer.transform.position = manager.boxClass.gameObject.transform.position;
        newSpriteLayer.transform.SetParent(manager.boxClass.gameObject.transform);
        SpriteRenderer sr = newSpriteLayer.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = manager.boxClass.lastSortingLayer + 1;
        manager.boxClass.lastSortingLayer += 1;
    }
}

public class IncreaseBoxSize : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Size";
        sprite = Object.Instantiate(Resources.Load<Sprite>("Sprites/Upgrades/Scale"));
    }

    public override void ActivateUpgrade()
    {
        base.ActivateUpgrade();
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
        sprite = Object.Instantiate(Resources.Load<Sprite>("Sprites/Upgrades/Spikes"));
    }

    public override void ActivateUpgrade()
    {
        base.ActivateUpgrade();
    }
}

public class Stun : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Stun";
        sprite = Object.Instantiate(Resources.Load<Sprite>("Sprites/Upgrades/Stun"));
    }

    public override void ActivateUpgrade()
    {
        base.ActivateUpgrade();
    }
}

public class Bounce : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Bounce";
        sprite = Object.Instantiate(Resources.Load<Sprite>("Sprites/Upgrades/Bounce"));
    }

    public override void ActivateUpgrade()
    {
        base.ActivateUpgrade();
        //manager.boxClass.rb.sharedMaterial.bounciness += 0.33f;
    }
}

public class Speed : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Speed";
        sprite = Object.Instantiate(Resources.Load<Sprite>("Sprites/Upgrades/Speed"));
    }

    public override void ActivateUpgrade()
    {
        base.ActivateUpgrade();
        manager.player.movingEntityBehaviour.moveSpeed *= 1.5f;
        manager.player.movingEntityBehaviour.dashSpeed *= 1.5f;
    }
}