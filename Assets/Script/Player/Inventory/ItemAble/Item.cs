using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public string NameItem;
    public enum Type
    {
        Weapond,
        Clothes_Head,
        Clothes_Body,
        Clothes_Pant,
        Clothes_Feet,
        Veil,
        Potion,
        Other
    }
    public enum Type_Weapon
    {
        Axe,
        SwordAndShield,
        Stuff,
        Bow
    }

    public int Veil_Skill;
    public float Veil_Dash_Cooldown;
    public Type type;
    public Type_Weapon type_Weapon;
    public Sprite ItemPic;
    public int HP, Armor, Damage, CriRate, CriDamage;
}
