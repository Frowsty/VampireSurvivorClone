using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] GameObject back_button;
    [SerializeField] GameObject fps_controller;
    [SerializeField] Toggle fps_toggle;
    [SerializeField] Toggle fullscreen;
    [SerializeField] Slider max_fps;

    private Updater updater;
    
    MenuController.MenuState prev_menu_state;
    private MenuController menu_controller;

    private bool show_fps = false;
    
    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        updater = gameObject.GetComponent<Updater>();
        menu_controller = gameObject.GetComponent<MenuController>();
        Application.targetFrameRate = 300;

        prev_menu_state = menu_controller.getMenuState();
        
        show_fps = fps_toggle.isOn;
        fps_controller.SetActive(show_fps);
        
        updater.setShowFps(show_fps);
        
        fps_toggle.onValueChanged.AddListener(delegate
        {
            show_fps = fps_toggle.isOn;
            updater.setShowFps(show_fps);
            toggleFPS();
        });
        
        fullscreen.onValueChanged.AddListener(delegate
        {
            Screen.fullScreen = fullscreen.isOn;
        });
        
        max_fps.onValueChanged.AddListener(delegate
        {
            Application.targetFrameRate = (int)max_fps.value;
        });
    }

    /*
     * PUBLIC FUNCTIONS
     */
    public void UpdateMenu()
    {
        
    }
    
    public void setPrevMenuState(MenuController.MenuState state) => prev_menu_state = state;

    public void onBackButton() => menu_controller.setMenuState(prev_menu_state);
    
    public void toggleFPS() => fps_controller.SetActive(show_fps);
}
