using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour, ITarget
{
    public GameObject MyGameObject { get => gameObject; }

    public void Interact()
    {
        //play sound?
    }
}
