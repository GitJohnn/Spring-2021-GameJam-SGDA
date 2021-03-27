using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;

        attackText.text  =  card.AttackBoost.ToString();
        speedText.text   =  card.SpeedBoost.ToString();
        defenceText.text =  card.DefenseBoost.ToString();

    }

}
