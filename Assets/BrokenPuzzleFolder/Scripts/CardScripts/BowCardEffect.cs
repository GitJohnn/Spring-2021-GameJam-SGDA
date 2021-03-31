using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCardEffect : CardEffect
{
    public override void CardEffectMethod()
    {
        GridCombatSystem gridCombatSystem = GameObject.FindObjectOfType<GridCombatSystem>();
        UnitGridCombat playerUnit = gridCombatSystem.GetNextActiveUnit(UnitGridCombat.Team.Player);
        playerUnit.IncreaseAttack(1 , "Bow");
        playerUnit.IncreaseDistance(3);
        CardManager.Instance.LoopActiveCards(false);
    }
}
