using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyArmorCardEffect : CardEffect
{
    public override void CardEffectMethod()
    {
        GridCombatSystem gridCombatSystem = GameObject.FindObjectOfType<GridCombatSystem>();
        UnitGridCombat playerUnit = gridCombatSystem.GetNextActiveUnit(UnitGridCombat.Team.Player);
        playerUnit.IncreaseDefence(2);
        CardManager.Instance.LoopActiveCards(false);
    }
}
