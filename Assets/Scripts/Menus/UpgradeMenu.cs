using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] GameObject upgrade_menu_controller;
    [SerializeField] TMPro.TextMeshProUGUI points_text;

    private Player player;
    private int attraction = 0;
    private int fire_rate = 0;
    private int damage = 0;
    private int health_regen = 0;
    
    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    /*
     * PUBLIC FUNCTIONS
     */
    public void UpdateMenu()
    {
        player.setAttractionPoints(attraction);
        player.setFireRatePoints(fire_rate);
        player.setDamagePoints(damage);
        player.setHealthRegen(health_regen);
        points_text.SetText(player.getUpgradePoints().ToString());
    }

    public void closeUpgradeMenu()
    {
        upgrade_menu_controller.SetActive(false);
        player.game_started = true;
    }

    public void incrementHealthRegen(TMPro.TextMeshProUGUI value)
    {
        if (player.getUpgradePoints() == 0 || health_regen >= 20)
            return;
        health_regen++;
        value.SetText(health_regen.ToString());
        
        player.decrementUpgradePoints();
    }
    
    public void decrementHealthRegen(TMPro.TextMeshProUGUI value)
    {
        if (health_regen == 0)
            return;
        health_regen--;
        value.SetText(health_regen.ToString());
        
        player.incrementUpgradePoints();
    }

    public void incrementAttraction(TMPro.TextMeshProUGUI value)
    {
        if (player.getUpgradePoints() == 0 || attraction >= 25)
            return;
        attraction++;
        value.SetText(attraction.ToString());
        
        player.decrementUpgradePoints();
    }

    public void decrementAttraction(TMPro.TextMeshProUGUI value)
    {
        if (attraction == 0)
            return;
        attraction--;
        value.SetText(attraction.ToString());
        
        player.incrementUpgradePoints();
    }
    
    public void incrementFireRate(TMPro.TextMeshProUGUI value)
    {
        if (player.getUpgradePoints() == 0 || fire_rate >= 10)
            return;
        fire_rate++;
        value.SetText(fire_rate.ToString());

        player.updateFireRate();
        player.decrementUpgradePoints();
    }

    public void decrementFireRate(TMPro.TextMeshProUGUI value)
    {
        if (fire_rate == 0)
            return;
        fire_rate--;
        value.SetText(fire_rate.ToString());
        
        player.updateFireRate();
        player.incrementUpgradePoints();
    }
    
    public void incrementDamage(TMPro.TextMeshProUGUI value)
    {
        if (player.getUpgradePoints() == 0 || damage >= 25)
            return;
        damage++;
        value.SetText(damage.ToString());
        
        player.decrementUpgradePoints();
    }

    public void decrementDamage(TMPro.TextMeshProUGUI value)
    {
        if (damage == 0)
            return;
        damage--;
        value.SetText(damage.ToString());
        
        player.incrementUpgradePoints();
    }
}
