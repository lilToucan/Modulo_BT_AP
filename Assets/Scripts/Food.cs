using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour, ITarget
{
    public GameObject MyGameObject { get => gameObject; }
    public MeshRenderer MeshRenderer { get => null; }
    public void Interact()
    {
        //play sound?
    }
}
