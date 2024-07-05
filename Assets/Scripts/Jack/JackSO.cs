using System;
using UnityEngine;

[CreateAssetMenu(fileName ="jack", menuName = "jack")]
public class JackSO : ScriptableObject
{
    public Action barChanged;
    public Action ohIMDie;
}
