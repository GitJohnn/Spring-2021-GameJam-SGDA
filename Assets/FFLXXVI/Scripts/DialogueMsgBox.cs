using UnityEngine;
using UnityEngine.UI;

public class DialogueMsgBox : MonoBehaviour
{
    public Text speaker;
    public Text dialogue;
    public GameObject namePanel;
    public GameObject textPanel;
    public Conversation convo;

    public void Advance()
    {
        convo.pointer++;
        convo.boxes[convo.pointer - 1] = null;


        if (convo.boxes[convo.pointer] == null)
        {
            convo.EndConvo();
        }


        convo.boxes[convo.pointer].gameObject.SetActive(true);


        Destroy(this.gameObject);
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (speaker == null)
        {
            textPanel.SetActive(false);
            namePanel.SetActive(false);
        }
        else
        {
            textPanel.SetActive(true);
            namePanel.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Advance();
        }
    }
}
