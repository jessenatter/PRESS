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
        foreach (Upgrade upgrade in GameDataManager.savedUpgrades)
        {
            activeUpgrades.Add(upgrade);
            upgrade.ActivateUpgrade();
        }

        Stun stun = new Stun();
        Speed speed = new Speed();
        Magnet magnet = new Magnet();

        possibleUpgrades.AddRange(new Upgrade[] { magnet,stun,speed, });

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
                //int index = Random.Range(0, possibleUpgrades.Count);
                int index = i;

                Upgrade _offeredUpgrade = possibleUpgrades[index];

                for(int j = 0; j < offeredUpgrades.Count; j++)
                {
                    while(_offeredUpgrade == offeredUpgrades[j])
                        _offeredUpgrade = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
                }

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
        GameDataManager.savedUpgrades.Add(selectedUpgrade);
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
        manager.boxClass.boxBehaviour.stunTime += 15;
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
        manager.player.movingEntityBehaviour.moveSpeed *= 1.2f;
        manager.player.movingEntityBehaviour.dashSpeed *= 1.2f;
    }
}

public class Magnet : Upgrade
{
    public override void StartUpgrade(Manager _manager)
    {
        base.StartUpgrade(_manager);
        name = "Increase Magnet";
        //sprite = Object.Instantiate(Resources.Load<Sprite>("Sprites/Upgrades/Magnet"));
    }

    public override void ActivateUpgrade()
    {
        base.ActivateUpgrade();
        manager.boxClass.magnetBehavior.magnetSpeed += 0.3f;
    }
}