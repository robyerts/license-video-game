using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayerGameData2 : PlayerGameData2
{
    public new static MeleePlayerGameData2 Instance = null;

    public List<float> AbilitiesAttackRanges;

    protected override void Start()
    {
    //    base.Start();
    //    Instance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Instance = this;
    }
}
