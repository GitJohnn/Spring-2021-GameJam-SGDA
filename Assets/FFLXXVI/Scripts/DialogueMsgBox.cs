using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueMsgBox : MonoBehaviour
{
    public Text speaker;
    public Text dialogue;
    public GameObject namePanel;
    public GameObject textPanel;
    public Conversation convo;
    public AudioSource typeSoundSource;

    public void Advance()
    {
        convo.pointer++;
        convo.boxes[convo.pointer - 1] = null;


        

        try
        {
            if (convo.boxes[convo.pointer] == null)
            {
                convo.EndConvo();
            }
        }
        catch(System.ArgumentOutOfRangeException)
        {
            convo.EndConvo();
        }


        convo.boxes[convo.pointer].gameObject.SetActive(true);


        Destroy(this.gameObject);
    }

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(speaker == null)
        StartCoroutine("PlayText");
    }

    IEnumerator PlayText()
    {
        string d = dialogue.text;
        dialogue.text = "";
        foreach (char c in d)
        {
            dialogue.text += c;
            typeSoundSource.Play();

            if (Character.Equals(c, ','))
                yield return new WaitForSecondsRealtime(0.8f);
            else
                yield return new WaitForSecondsRealtime(0.1f);
        }
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Advance();
        }
    }
}
