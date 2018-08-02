using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayerGameData2 : PlayerGameData2
{
    public new static MeleePlayerGameData2 Instance = null;

    public List<float> AbilitiesAttackRanges;

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
