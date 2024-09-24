using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : MonoBehaviour
{
    [SerializeField] GameObject main_menu_controller;
    [SerializeField] GameObject play_button;
    [SerializeField] GameObject exit_button;
    [SerializeField] GameObject setting_button;
    
    private MenuController menu_controller;

    private Player player;

    private Vector3 play_original_pos;
    private Vector3 exit_original_pos;
    private Vector3 setting_original_pos;
    
    // Start is called before the first frame update
    void Start()
    {
        menu_controller = gameObject.GetComponent<MenuController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        play_original_pos = play_button.transform.position;
        exit_original_pos = exit_button.transform.position;
        setting_original_pos = setting_button.transform.position;
        
        resetPositions();
    }

    // Update is called once per frame
    public void UpdateMenu()
    {
        // y position is being animated
        play_button.transform.position = new Vector3(play_original_pos.x, verticalMove(play_button.transform.position.y, play_original_pos.y), play_original_pos.z);
        setting_button.transform.position = new Vector3(setting_original_pos.x, verticalMove(setting_button.transform.position.y, setting_original_pos.y), setting_original_pos.z);
        exit_button.transform.position = new Vector3(exit_original_pos.x, verticalMove(exit_button.transform.position.y, exit_original_pos.y), exit_original_pos.z);
    }

    private float verticalMove(float current_y, float original_y)
    {
        if (current_y < original_y)
            current_y += (10f * Mathf.Abs(original_y - current_y)) * Time.deltaTime;
        else
            return original_y;

        return current_y;
    }

    public void startGame()
    {
        player.enableLights();
        menu_controller.allow_bind = true;
        main_menu_controller.SetActive(false);
        player.game_started = true;
        menu_controller.show_menu = false;
    }

    public void resetPositions()
    {
        play_button.transform.position = new Vector3(play_original_pos.x, -100, play_original_pos.z);
        setting_button.transform.position = new Vector3(setting_original_pos.x, -200, setting_original_pos.z);
        exit_button.transform.position = new Vector3(exit_original_pos.x, -300, exit_original_pos.z);
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
