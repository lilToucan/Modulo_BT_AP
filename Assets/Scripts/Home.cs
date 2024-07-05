using UnityEngine;

internal class Home : MonoBehaviour,ITarget
{
    public GameObject MyGameObject { get=> gameObject; }
    public MeshRenderer MeshRenderer { get => null; }
    public void Interact()
    {
        //play sound?
    }
}