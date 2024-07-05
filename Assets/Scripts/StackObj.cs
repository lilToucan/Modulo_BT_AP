using UnityEngine;

public class StackObj : MonoBehaviour, ITarget
{
    public GameObject MyGameObject { get => gameObject; }
    public MeshRenderer MeshRenderer { get => null; }
    public int StackAmount = 0;

    public void Interact()
    {
        StackAmount++;
    }
}
