using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour, ITarget
{
    public GameObject MyGameObject { get => gameObject; }
    public MeshRenderer MeshRenderer { get => myRenderer; }

    [SerializeField] float respawnTime;
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
        yield return new WaitForSeconds(respawnTime);
        myRenderer.enabled = true;
    }
}


public interface ITarget
{
    GameObject MyGameObject { get; }
    MeshRenderer MeshRenderer { get; }
    void Interact();
}
