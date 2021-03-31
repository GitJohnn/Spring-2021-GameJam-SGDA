using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCreditsScene : MonoBehaviour
{
    public void CloseApp()
    {
        Debug.Log("Closing App");
        Application.Quit();
    }
}
