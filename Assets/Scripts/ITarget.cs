using UnityEngine;

public interface ITarget
{
    GameObject MyGameObject { get; }
    MeshRenderer MeshRenderer { get; }
    void Interact();
}