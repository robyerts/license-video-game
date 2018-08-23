using UnityEngine;
using System.Collections;

public abstract class PlayerCharInfoStorage : MonoBehaviour
{
    public abstract bool SaveChar(PlayerCharInfo charInfo);
    public abstract PlayerCharInfo LoadChar(int index);
    public abstract bool DeleteChar(int index);
}
