using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricCardEffect : CardEffect
{
    public override void CardEffectMethod()
    {
        GridCombatSystem gridCombatSystem = GameObject.FindObjectOfType<GridCombatSystem>();
        gridCombatSystem.UsingCardType(GridCombatSystem.ElementCardUsed.Thunder);
        Debug.Log("Using Electric Card");
        CardManager.Instance.LoopActiveCards(false);
    }
}
