using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCard : MonoBehaviour
{
    public enum General
    {
        Jandar,
        Ullar,
        Vydar,
        Einar,
        Aquilla,
        Utgar,
        Valkrill
    };
    public enum Planet
    {
        AlphaPrime,
        Earth,
        Eberron,
        Feylund,
        Grut,
        Icaria,
        Isadora,
        Marr,
        Planets,
        Toril,
        Valhalla
    };

    public enum Species
    {
        Arachnid,
        Doggin,
        Dragon,
        Dwarf,
        DzuTeh,
        Elf,
        Elementar,
        Fiantooth,
        Giant,
        Gryphillin,
        Human,
        Insect,
        Kyrie,
        Moltarn,
        Marro,
        Ogre,
        Orc,
        Primadon,
        Quasatch,
        Undead,
        Soulborg,
        Troll,
        Trolticor,
        Werewolf,
        Wulsinu,
        Viper
    };
    public enum Type
    {
        CommonHero,
        CommonSquad,
        UncommonHero,
        UniqueHero,
        UniqueSquad
    };
    public enum Class
    {
        Agent,
        Alphallon,
        Archer,
        Archkyrie,
        Archmage,
        Ashigaru,
        Beast,
        Brute,
        Champion,
        Cleric,
        Construct,
        Daimyo,
        Darklord,
        DeathKnight,
        Deathstalker,
        Deathwalker,
        Devourer,
        Divider,
        Drone,
        Duchess,
        Emperor,
        Fighter,
        Frostrager,
        Gladiator,
        Guard,
        Hive,
        Hunter,
        King,
        Knight,
        Lady,
        Lawman,
        Leader,
        Lord,
        Major,
        Minion,
        Monk,
        Mount,
        Ninja,
        Overlord,
        Predator,
        Prince,
        Protector,
        Pulverizer,
        Queen,
        Samurai,
        Savage,
        Scout,
        Sentinel,
        Sniper,
        Soldier,
        Stinger,
        Tribesman,
        Warden,
        Warlord,
        Warmonger,
        Warrior,
        Warwitch,
        Wizard,
        Wyrmling,
        Young
    };
    public enum Personality
    {
        Angry,
        Arrogant,
        Bold,
        Confident,
        Dauntless,
        Devout,
        Disciplined,
        Fearless,
        Fearsome,
        Ferocious,
        Inspiring,
        Loyal,
        Menacing,
        Merciful,
        Merciless,
        Militaristic,
        Mindless,
        Precise,
        Rebellious,
        Reckless,
        Relentless,
        Resolute,
        Skittish,
        Stoic,
        Terrifying,
        Tormented,
        Tormenting,
        Tricky,
        Valiant,
        Wild
    };
    public enum Size
    {
        Small,
        Medium,
        Large,
        Huge
    };
    public enum Base
    {
        Single,
        Double,
        LargeSingle
    };

    public General _general;
    public Planet _homeWorld;
    public Species _species;
    public Type _type;
    public Class _class;
    public Personality _personality;
    public Size _size;
    public Base _base;

    public string Name = "";
    
    public int Life;
    public int Move;
    public int Range;
    public int Attack;
    public int Defense;

    public int numberOfUnits;

    public int Height;
    public int Points;

    public int totalAmountDrafted = 0;

    public List<Power> Powers = new List<Power>();



}
