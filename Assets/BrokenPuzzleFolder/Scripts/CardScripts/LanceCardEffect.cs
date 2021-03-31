using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanceCardEffect : CardEffect
{
    public override void CardEffectMethod()
    {
        GridCombatSystem gridCombatSystem = GameObject.FindObjectOfType<GridCombatSystem>();
        UnitGridCombat playerUnit = gridCombatSystem.GetNextActiveUnit(UnitGridCombat.Team.Player);
        playerUnit.IncreaseAttack(2, "Lance");
        playerUnit.IncreaseDistance(1);

    }
}
