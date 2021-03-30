using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dummy : MonoBehaviour
{
    public Conversation convo;
    public GameObject text;
    public string level1;

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(level1);
    }
}
