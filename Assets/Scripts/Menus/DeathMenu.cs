using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu: MonoBehaviour
{
    [SerializeField] GameObject death_menu_controller;
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI score_text;
    [SerializeField] TextMeshProUGUI highscore_text;

    private Player player;
    private MenuController menu_controller;

    private float original_position;
    
    
    // Start is called before the first frame update
    void Start()
    {
        menu_controller = gameObject.GetComponent<MenuController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
  
        original_position = panel.transform.position.y;
    }

    // Update is called once per frame
    public void UpdateMenu()
    {
        // y position is being animated
        panel.transform.position = new Vector3(panel.transform.position.x, verticalMove(panel.transform.position.y, original_position), panel.transform.position.z);

        score_text.SetText(player.getScore().ToString());
        highscore_text.SetText(PlayerPrefs.GetInt("highscore").ToString());
    }

    private float verticalMove(float current_y, float original_y)
    {
        if (original_y - current_y > 1)
            current_y += (10f * Mathf.Abs(original_y - current_y)) * Time.deltaTime;
        else
            return original_y;

        return current_y;
    }

    public void mainMenu() => SceneManager.LoadScene(0);

    public void enterSettings() => menu_controller.setMenuState(MenuController.MenuState.SettingsMenu);

    public void resetPositions()
    {
        panel.transform.position = new Vector3(panel.transform.position.x, original_position * 2 * -1, panel.transform.position.z);
    }

    public void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
