using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class Card
{
    public string name;
    string description;
}

class CardCollection
{
    ElementCard fireCard = new ElementCard(ElementCard.Element.FIRE);
    ElementCard iceCard = new ElementCard(ElementCard.Element.ICE);
    ElementCard thunderCard = new ElementCard(ElementCard.Element.THUNDER);

    WeaponCard swordCard = new WeaponCard(WeaponCard.Weapon.SWORD);
    WeaponCard lanceCard = new WeaponCard(WeaponCard.Weapon.LANCE);
    WeaponCard bowCard = new WeaponCard(WeaponCard.Weapon.BOW);

    ArmorCard heavyCard = new ArmorCard(ArmorCard.Armor.HEAVY);
    ArmorCard lightCard = new ArmorCard(ArmorCard.Armor.LIGHT);
    ArmorCard thornCard = new ArmorCard(ArmorCard.Armor.THORN);

    
}

class ElementCard : Card
{
    public enum Element
    {
        FIRE,
        ICE,
        THUNDER
    }

    public Element elementType;

    public ElementCard(Element element)
    {
        elementType = element;
        switch(element)
        {
            case (Element)0:
                name = "Fire Card";
                break;
            case (Element)1:
                name = "Ice Card";
                break;
            case (Element)2:
                name = "Thunder Card";
                break;
        }
    }

    public void ApplyEffect(Character target)
    {
        System.Random random = new System.Random();
        int generated = random.Next(1, 100);

        if (generated > 50)
        {
            switch (elementType)
            {
                case Element.FIRE:
                    target.ApplyBurn();
                    break;
                case Element.ICE:
                    target.ApplyFrozen();
                    break;
                case Element.THUNDER:
                    target.ApplyShocked();
                    break;
            }
        }
    }
}

class WeaponCard : Card
{
    public enum Weapon
    {
        SWORD,
        LANCE,
        BOW
    }

    public Weapon weaponType;

    public WeaponCard(Weapon weapon)
    {
        weaponType = weapon;

        switch (weapon)
        {
            case (Weapon)0:
                name = "Sword Card";
                break;
            case (Weapon)1:
                name = "Lance Card";
                break;
            case (Weapon)2:
                name = "Bow Card";
                break;
        }
    }

    public void ApplyEffect(Character owner)
    {
        //effects affect attack range
    }
}


class ArmorCard : Card
{
    public enum Armor
    {
        HEAVY,
        LIGHT,
        THORN
    }

    public Armor armorType;

    public ArmorCard(Armor armor)
    {
        armorType = armor;

        switch (armor)
        {
            case (Armor)0:
                name = "Heavy Card";
                break;
            case (Armor)1:
                name = "Light Card";
                break;
            case (Armor)2:
                name = "Thorn Card";
                break;
        }
    }

    public void ApplyEffect(Character owner)
    {
        switch (armorType)
        {
            case Armor.HEAVY:
                owner.def += 2;
                owner.modifiers.ApplyBuff(Character.Modifiers.Stat.MOVE, -1);
                break;
            case Armor.LIGHT:
                owner.def += 1;
                break;
            case Armor.THORN:
                owner.hasCounterStatus = true;
                break;
        }
    }

    public void RemoveEffect(Character owner)
    {
        switch (armorType)
        {
            case Armor.HEAVY:
                owner.def -= 2;
                break;
            case Armor.LIGHT:
                owner.def -= 1;
                break;
        }
    }
}