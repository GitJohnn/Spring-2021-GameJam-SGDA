using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummy : MonoBehaviour
{
    public Conversation convo;
    public GameObject text;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            convo.Activate();
            Destroy(text);
        }
    }
}
