using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] GameObject main_menu_controller;
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI highscore_text;
    
    private MenuController menu_controller;

    private Player player;

    private float original_position;
    
    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        menu_controller = gameObject.GetComponent<MenuController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        original_position = panel.transform.position.y;
        
        resetPositions();
    }

    private float verticalMove(float current_y, float original_y)
    {
        if (original_y - current_y > 1)
            current_y += (10f * Mathf.Abs(original_y - current_y)) * Time.deltaTime;
        else
            return original_y;

        return current_y;
    }
    
    /*
     * PUBLIC FUNCTIONS
     */
    public void UpdateMenu()
    {
        panel.transform.position = new Vector3(panel.transform.position.x, verticalMove(panel.transform.position.y, original_position), panel.transform.position.z);
        highscore_text.SetText(PlayerPrefs.GetInt("highscore").ToString());
    }

    public void startGame()
    {
        player.enableLights();
        menu_controller.allow_bind = true;
        //main_menu_controller.SetActive(false);
        player.game_started = true;
        menu_controller.show_menu = false;
    }

    public void resetPositions()
    {
        panel.transform.position = new Vector3(panel.transform.position.x, original_position * 2 * -1, panel.transform.position.z);
    }

    public void enterSettings() => menu_controller.setMenuState(MenuController.MenuState.SettingsMenu);

    public void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}