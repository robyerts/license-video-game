using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings2 : MonoBehaviour {

    public static GameSettings2 Instance = null;
    public static int MaxNrAbilitiesInBattle = 4; // UI is not updated with this number; used further in Mage/Melee Players to loop through abilities

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }
}
