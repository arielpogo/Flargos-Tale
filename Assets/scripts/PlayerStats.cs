using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object which stores player data.
/// </summary>
[System.Serializable]
public class PlayerStats {
    /// <summary>
    /// 0 = valid |
    /// 1 = file not found |
    /// 2 = file couldnt be parsed
    /// </summary>
    public int status = 0;
    public System.DateTime date;
    public int saveNum = 0;
    public int strength = 1;
    public int stealth = 1;
    public int dollars { get; private set; } = 1; //make my life easier, might change later
    public int cents { get; private set; } = 10;
    public int debt = 0;
    public int TylerValue = 0;
    public string sceneName = "mainMenu";
    public Vector2 savedPos;
    public List<Item> inventory = new List<Item>();


    //****************************//
    //                            //
    //   PLAYER DATA INTERFACE    //
    //                            //
    //****************************//

    /// <summary>
    /// Please don't use negative parameters.
    /// </summary>
    /// <param name="dollars"></param>
    /// <param name="cents"></param>
    public void AddMoney(int dollars, int cents) {
        this.dollars += dollars;
        this.cents += cents;
        if (this.cents >= 100) {
            this.dollars += this.cents / 100;
            this.cents %= 100;
        }
    }

    /// <summary>
    /// Please don't use negative parameters.
    /// </summary>
    /// <param name="dollars"></param>
    /// <param name="cents"></param>
    /// <returns>Whether the transaction can go through (i.e. no negative money)</returns>
    public bool RemoveMoney(int dollars, int cents) {
        int tempDollars = this.dollars;
        int tempCents = this.cents;
        tempDollars -= dollars;
        tempCents -= cents;
        if (tempCents < 0) {
            int dollarsDown = tempCents / -100 + 1;
            tempCents += dollarsDown * 100;
            tempDollars -= dollarsDown;
        }
        if (tempDollars < 0) return false;
        else {
            this.dollars = tempDollars;
            this.cents = tempCents;
            return true;
        }
    }
}