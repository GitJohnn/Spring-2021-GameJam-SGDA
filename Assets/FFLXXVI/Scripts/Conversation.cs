using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Conversation : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;
    [SerializeField] string fileName = "Opening Convo";
    public List<DialogueMsgBox> boxes = new List<DialogueMsgBox>();
    public int pointer = 0;

    private void Start()
    {
        string path = "Assets/Resources/" + fileName + ".txt";
        StreamReader convoFileStream = new StreamReader(path);

        string lineToParse = convoFileStream.ReadLine();

        while (lineToParse != null)
        {
            Debug.Log(lineToParse);
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

            newBox.dialogue.text = tokens[1];

            newBox.gameObject.transform.parent = this.gameObject.transform;

            boxes.Add(newBox);
            newBox.convo = this;

            lineToParse = convoFileStream.ReadLine();
        }


        convoFileStream.Close();
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
    }

    public void EndConvo()
    {
        Debug.Log("Conversation ended");
        Destroy(this.gameObject);
    }
}
