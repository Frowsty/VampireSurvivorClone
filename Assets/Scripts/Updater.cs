using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Updater : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI fps;

    private bool show_fps = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (show_fps)
        {   int temp = (int)(1 / Time.unscaledDeltaTime);
            fps.SetText(temp.ToString());
        }
    }
    
    public void setShowFps(bool show) => show_fps = show;
}
