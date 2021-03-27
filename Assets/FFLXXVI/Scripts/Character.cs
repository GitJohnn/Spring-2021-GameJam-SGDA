using System.Collections;
using System.Collections.Generic;
using UnityEngine;


abstract class Character : MonoBehaviour
{
    protected int atk;
    public int def;
    public int health;
    public int maxHealth;
    protected int movement;
    public bool isDead;


    public bool isBurned;
    int burnCount = 0;
    public bool isFrozen;
    int freezeCount = 0;
    public bool isShocked;
    int shockCount = 0;

    public void TakeDmg(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
        }
    }

    public void TurnStart()
    {

        if (isBurned)
        {
            TakeDmg(1);
            burnCount--;

            if (burnCount <= 0)
            {
                burnCount = 0;
                isBurned = false;
            }
        }
        else if (isFrozen)
        {
            modifiers.moveMod -= 1;

            freezeCount--;
            if (freezeCount <= 0)
            {
                freezeCount = 0;
                isFrozen = false;
            }
        }


        if (isShocked)
        {
            movement = 0;

            shockCount--;
            if (shockCount <= 0)
            {
                shockCount = 0;
                isShocked = false;
            }
        }
    }

    //movement + modifiers.moveMod used when moving on map

    public void TurnEnd()
    {
        modifiers.Reset();
    }

    public void ApplyBurn()
    {
        isBurned = true;
        burnCount = 3;
    }

    public void ApplyFrozen()
    {
        isFrozen = true;
        freezeCount = 3;
    }

    public void ApplyShocked()
    {
        isShocked = true;
        shockCount = 1;
    }


    public abstract void Attack(Character target);

    // statuses
    public bool hasCounterStatus = false;


    public class Modifiers
    {
        public enum Stat
        {
            MOVE
        }
        Stat stat;

        public int moveMod = 0;

        public void ApplyBuff(Stat statToBuff, int modifyAmount)
        {
            switch (statToBuff)
            {
                case Stat.MOVE:
                    moveMod += modifyAmount;
                    break;
            }
        }

        public void Reset()
        {
            moveMod = 0;
        }
    }

    public Modifiers modifiers = new Modifiers();
}
