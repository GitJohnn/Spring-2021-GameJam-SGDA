using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFL
{
    class Player : Character
    {
        public ElementCard elementCard;
        public WeaponCard weaponCard;
        public ArmorCard armorCard;

        public void AddCard(ElementCard card)
        {
            elementCard = card;
        }

        public void AddCard(WeaponCard card)
        {
            weaponCard = card;
        }

        public void AddCard(ArmorCard card)
        {
            armorCard = card;
        }

        public override void Attack(Character target)
        {
            elementCard.ApplyEffect(target);
            weaponCard.ApplyEffect(this);
            armorCard.ApplyEffect(this);

            int dmg = atk - target.def;
            if (dmg <= 0) dmg = 0;
            target.TakeDmg(dmg);

            if (target.hasCounterStatus)
            {
                TakeDmg(1);
                target.hasCounterStatus = false;
            }


            target.Attack(this);
        }
    }
}