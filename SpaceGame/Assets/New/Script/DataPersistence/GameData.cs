using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{

    public long lastUpdate;

    //[Header("Resources Data")]
    //[SerializeField] public int woodCount;
    //[SerializeField] public int metalCount;
    //[SerializeField] public int stoneCount;
    //[SerializeField] public int foodCount;
    //[SerializeField] public int population;

    //[Header("Coat Of Arms Data")]
    //[SerializeField] public string familyName;
    //[SerializeField] public string familyMotto;

    //[SerializeField] public int icon;
    //[SerializeField] public int backgroundMaterial;
    //[SerializeField] public int imageColour;



    // the values defined in the constructor are default values and will be loaded
    // when the game has no saved data ie. new game/first time playing
    public GameData()
    {
        // Resource Data
        //this.woodCount = 200;
        //this.foodCount = 200;
        //this.metalCount = 200;
        //this.stoneCount = 200;
        //this.population = 20;

        //// Coat of arms data
        //this.familyName = "Empty";
        //this.familyMotto = "Empty";
        //this.icon = 0;
        //this.backgroundMaterial = 0;
        //this.imageColour = 0;

    }
}
