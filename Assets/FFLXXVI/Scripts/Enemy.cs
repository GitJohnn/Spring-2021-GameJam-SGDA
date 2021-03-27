using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Enemy : Character
{
    public override void Attack(Character target)
    {
        int dmg = this.atk - target.def;
        if (dmg <= 0) dmg = 0;
        target.TakeDmg(dmg);

        if (target.hasCounterStatus)
        {
            this.TakeDmg(1);
            target.hasCounterStatus = false;
        }
    }
}
