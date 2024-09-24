using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    [SerializeField] public GameObject play_menu;
    [SerializeField] public GameObject death_menu;
    [SerializeField] GameObject upgrade_menu;
    [SerializeField] GameObject settings_menu;
    [SerializeField] GameObject menu_light;
    
    private PlayMenu play_menu_controller;
    private DeathMenu death_menu_controller;
    private UpgradeMenu upgrade_menu_controller;
    private SettingsMenu settings_menu_controller;
    private Player player;
    
    public bool allow_bind = false;
    public bool show_menu = true;
    public enum MenuState
    {
        PlayMenu = 0,
        DeathMenu = 1,
        UpgradeMenu = 2,
        SettingsMenu = 3
    }

    private MenuState menu_state = MenuState.PlayMenu;
    private MenuState prev_menu_state = MenuState.PlayMenu;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        play_menu_controller = gameObject.GetComponent<PlayMenu>();
        death_menu_controller = gameObject.GetComponent<DeathMenu>();
        upgrade_menu_controller = gameObject.GetComponent<UpgradeMenu>();
        settings_menu_controller = gameObject.GetComponent<SettingsMenu>();
        
        menu_state = MenuState.PlayMenu;
        prev_menu_state = menu_state;

        play_menu.SetActive(true);
        death_menu.SetActive(false);
        upgrade_menu.SetActive(false);
        settings_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!show_menu || player.game_started)
        {
            play_menu.SetActive(false);
            death_menu.SetActive(false);
            upgrade_menu.SetActive(false);
            settings_menu.SetActive(false);
            menu_light.SetActive(false);
            
            play_menu_controller.resetPositions();
            death_menu_controller.resetPositions();
            
            player.enableLights();

            return;
        }
        
        player.disableLights();
        
        resetActiveState();
        
        switch (menu_state)
        {
            case MenuState.PlayMenu:
                if (!play_menu.activeSelf)
                    play_menu.SetActive(true);
                play_menu_controller.UpdateMenu();
                break;
            case MenuState.DeathMenu:
                if (!death_menu.activeSelf)
                    death_menu.SetActive(true);
                death_menu_controller.UpdateMenu();
                break;
            case MenuState.UpgradeMenu:
                if (!upgrade_menu.activeSelf)
                    upgrade_menu.SetActive(true);
                upgrade_menu_controller.UpdateMenu();
                break;
            case MenuState.SettingsMenu:
                if (!settings_menu.activeSelf)
                    settings_menu.SetActive(true);
                settings_menu_controller.setPrevMenuState(prev_menu_state);
                settings_menu_controller.UpdateMenu();
                break;
        }
    }

    private void resetActiveState()
    {
        play_menu.SetActive(getMenuState() == MenuState.PlayMenu);
        death_menu.SetActive(getMenuState() == MenuState.DeathMenu);
        settings_menu.SetActive(getMenuState() == MenuState.SettingsMenu);
        upgrade_menu.SetActive(getMenuState() == MenuState.UpgradeMenu);
        
        menu_light.SetActive(true);
    }

    public void setMenuState(MenuState state)
    {
        prev_menu_state = menu_state;
        menu_state = state;
    }

    public MenuState getMenuState() => menu_state;

    public void toggleMenu(bool state) => show_menu = state;
}
