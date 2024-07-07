using UnityEngine;

public class Food : MonoBehaviour, ITarget
{
    MeshRenderer myRenderer;
    public GameObject MyGameObject { get => gameObject; }
    public MeshRenderer MeshRenderer { get => myRenderer; }

    private void Awake()
    {
        myRenderer = GetComponent<MeshRenderer>();
    }
    public void Interact()
    {
        myRenderer.enabled = false;
    }
}
