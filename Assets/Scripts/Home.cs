using UnityEngine;

internal class Home : MonoBehaviour,ITarget
{
    public GameObject MyGameObject { get=> gameObject; }

    public void Interact()
    {
        //play sound?
    }
}