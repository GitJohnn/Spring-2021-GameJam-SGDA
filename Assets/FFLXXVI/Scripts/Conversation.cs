using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class Conversation : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;
    [SerializeField] string fileName = "Opening Convo";
    public UnityEvent onConvoEnd;
    public List<DialogueMsgBox> boxes = new List<DialogueMsgBox>();
    public int pointer = 0;
    public bool activateOnStart = false;

    private void Start()
    {
        string path = "Assets/Resources/" + fileName + ".txt";
        StreamReader convoFileStream = new StreamReader(path);

        string lineToParse = convoFileStream.ReadLine();

        while (lineToParse != null)
        {
            //Debug.Log(lineToParse);
            DialogueMsgBox newBox = Instantiate(dialogueBox).GetComponent<DialogueMsgBox>();

            string[] tokens = lineToParse.Split(':');

            if (!string.Equals(tokens[0], "null"))
            {
                newBox.speaker.text = tokens[0];
            }
            else
            {
                newBox.speaker = null;
            }

            try
            {
                newBox.dialogue.text = tokens[1];
            }
            catch(System.IndexOutOfRangeException)
            {
                break;
            }

            newBox.gameObject.transform.parent = this.gameObject.transform;

            boxes.Add(newBox);
            newBox.convo = this;

            lineToParse = convoFileStream.ReadLine();
        }


        convoFileStream.Close();

        if (activateOnStart)
        {
            Activate();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndConvo();
        }
    }

    public void Activate()
    {
        boxes[pointer].gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void EndConvo()
    {
        onConvoEnd.Invoke();
        Time.timeScale = 1;
        Destroy(this.gameObject);
    }
}
