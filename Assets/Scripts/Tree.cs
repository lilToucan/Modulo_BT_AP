using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, ITarget
{
    public GameObject MyGameObject { get; }
}

public interface ITarget
{
    GameObject MyGameObject { get; }
}
