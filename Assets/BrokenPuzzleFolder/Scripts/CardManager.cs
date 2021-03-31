using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public List<GameObject> cards;
    public GameObject cardHolder;

    private List<int> availableIndex = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    private List<int> usedIndex = new List<int>();    

    private void Awake()
    {
        Instance = this;
        InitializeCards();
    }

    public void InitializeCards()
    {
        int currentCards = 0;
        int currentIndex = 0;
        while(currentCards != 3)
        {
            currentIndex = availableIndex[Random.Range(0, availableIndex.Count - 1)];
            if (!usedIndex.Contains(currentIndex))
            {
                AddCardIndex(currentIndex);
                availableIndex.Remove(currentIndex);                
                currentCards++;
            }            
        }
    }

    public void AddCardIndex(int index)
    {
        GameObject newCard = Instantiate(cards[index], cardHolder.transform);
        //Debug.Log(newCard.name);
    }

    public void AddUnUsedCard()
    {
        int currentIndex = availableIndex[Random.Range(0, availableIndex.Count - 1)];
        //Debug.Log(currentIndex);
        AddCardIndex(currentIndex);
        availableIndex.Remove(currentIndex);
    }

    private void Update()
    {
        if(availableIndex.Count <= 2)
        {
            ReFillIndex();
        }
    }

    private void ReFillIndex()
    {
        Debug.Log("Refill available Index");
        availableIndex = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    }


}
