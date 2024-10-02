using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Updater : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] TMPro.TextMeshProUGUI fps;

    private bool show_fps = false;
    
    /*
     * PRIVATE FUNCTIONS
     */
    void Update()
    {
        if (show_fps)
        {   int temp = (int)(1 / Time.unscaledDeltaTime);
            fps.SetText(temp.ToString());
        }
    }
    
    /*
     * PUBLIC FUNCTIONS
     */
    public void setShowFps(bool show) => show_fps = show;
}
