using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour, ITarget
{
    public GameObject MyGameObject { get => gameObject; }
    MeshRenderer myRenderer;

    private void Awake()
    {
        myRenderer = GetComponent<MeshRenderer>();
    }
    public void Interact()
    {
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        myRenderer.enabled = false;
        yield return new WaitForSeconds(2);
        myRenderer.enabled = true;
    }
}


public interface ITarget
{
    GameObject MyGameObject { get; }
    void Interact();
}
