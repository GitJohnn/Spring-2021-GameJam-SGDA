using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCardEffect : CardEffect
{
    public override void CardEffectMethod()
    {
        GridCombatSystem gridCombatSystem = GameObject.FindObjectOfType<GridCombatSystem>();
        gridCombatSystem.UsingCardType(GridCombatSystem.ElementCardUsed.Fire);
        Debug.Log("Use card effect");
        CardManager.Instance.LoopActiveCards(false);
    }
}
