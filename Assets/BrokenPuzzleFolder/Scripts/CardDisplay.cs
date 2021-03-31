using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour
{
    //set up a field to be filled with a card object
    public Card card;

    public Text nameText;
    public Text descriptionText;

    public Image artworkImage;

    public Text attackText;
    public Text speedText;
    public Text defenceText;    

    private EventTrigger eventTrigger;
    private Selectable selectable;

    private int cardIndex = -1;

    // Start is called before the first frame update
    void Awake()
    {
        //OnSelectCard.AddListener()
        //set Selectable
        eventTrigger = GetComponent<EventTrigger>();
        selectable = GetComponent<Selectable>();
        //Set Card properties
        nameText.text = card.name;
        descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;

        attackText.text  =  card.AttackBoost.ToString();
        speedText.text   =  card.SpeedBoost.ToString();
        defenceText.text =  card.DefenseBoost.ToString();
    }

    public void DeActivateCard()
    {        
        CardManager.Instance.AddUnUsedCard(cardIndex);
        GameHandler_GridCombatSystem.Instance.CardPanelActivation(false);
        this.gameObject.SetActive(false);
    }

    public void MakeCardUsable(bool value)
    {
        selectable.interactable = value;
        eventTrigger.enabled = value;
    }

    public void SetCardIndex(int value)
    {
        cardIndex = value;
    }

}
