using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCardEffect : CardEffect
{
    public override void CardEffectMethod()
    {
        GridCombatSystem gridCombatSystem = GameObject.FindObjectOfType<GridCombatSystem>();
        gridCombatSystem.UsingCardType(GridCombatSystem.ElementCardUsed.Ice);
        Debug.Log("Using ice card type");
    }
}
