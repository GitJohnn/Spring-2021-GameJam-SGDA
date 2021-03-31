using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightArmorCardEffect : CardEffect
{
    public override void CardEffectMethod()
    {
        GridCombatSystem gridCombatSystem = GameObject.FindObjectOfType<GridCombatSystem>();
        UnitGridCombat playerUnit = gridCombatSystem.GetNextActiveUnit(UnitGridCombat.Team.Player);
        playerUnit.IncreaseDefence(1);
    }
}
