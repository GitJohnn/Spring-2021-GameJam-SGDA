using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFL
{
    class Enemy : Character
    {
        public override void Attack(Character target)
        {
            int dmg = atk - target.def;
            if (dmg <= 0) dmg = 0;
            target.TakeDmg(dmg);

            if (target.hasCounterStatus)
            {
                TakeDmg(1);
                target.hasCounterStatus = false;
            }
        }
    }
}