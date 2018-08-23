using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class MagePlayerGameData2 : PlayerGameData2
{
    public new static MagePlayerGameData2 Instance = null;

    public List<string> ProjectilesTags;
    public List<int> ProjectilesDmgs;

    public List<MageAbilityInstantiateType> AbilitiesTypes;
    public List<string> AbilitiesInstatiateTrTags;

    protected override void Start()
    {
        base.Start();
    }
}
