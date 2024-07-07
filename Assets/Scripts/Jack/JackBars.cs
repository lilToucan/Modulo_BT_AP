using System;
using UnityEngine;
using UnityEngine.UI;

public class JackBars : MonoBehaviour
{
    [SerializeField] Image hungerBar;
    [SerializeField] Image thirstBar;

    [SerializeField] Image minHunger;
    [SerializeField] Image minThirst;

    JackTree jackTree;
    Jack jack;
    private void Awake()
    {
        jackTree = GetComponent<JackTree>();
        jack = GetComponent<Jack>();
    }

    private void OnEnable()
    {
        minHunger.fillAmount = jackTree.minHunger / jackTree.maxHunger;
        minThirst.fillAmount = jackTree.minThirst / jackTree.maxThirst;
        jack.jackSo.barChanged += UpdateBars;
    }
    private void OnDisable()
    {
        jack.jackSo.barChanged -= UpdateBars;
    }

    private void UpdateBars()
    {
        hungerBar.fillAmount = jackTree.currentHunger/jackTree.maxHunger;
        thirstBar.fillAmount = jackTree.currentThirst/jackTree.maxThirst;

#if UNITY_EDITOR
        minHunger.fillAmount = jackTree.minHunger / jackTree.maxHunger;
        minThirst.fillAmount = jackTree.minThirst / jackTree.maxThirst;
#endif

    }


}
