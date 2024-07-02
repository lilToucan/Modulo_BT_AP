using BT;
using BT.Decorator;
using BT.Process;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Jack : MonoBehaviour
{
    [SerializeField] ITarget homePoint;  // sleep
    [SerializeField] ITarget foodPoint;  // eat
    [SerializeField] ITarget waterPoint; // drink

    List<ITarget> trees = new ();
    
    [SerializeField] float minHunger,maxHunger;
    [SerializeField] float minThirst,maxThirst;
    [SerializeField] float hungerUse, thirstUse;

    float currentHunger;
    float currentThirst;

    float distance;
    GameObject target;

    BehaviourTree treeDay;
    BehaviourTree treeNight;
    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        trees.AddRange(FindObjectsOfType<Tree>());
    }

    private void Start()
    {
        treeDay = new("day tree (Jack: i won't cut this one) ");
        treeNight = new("night tree (Jack: i felling eppy)");
        CalculateTarget_Decorator calculateTarget = new CalculateTarget_Decorator(GetDistance, GetTarget, trees, agent);

        Sequence foodSequence = new("Food Sequence (jack: i got to eat man)");

        CheckBarDecorator hungerCheck = new(() => currentHunger, minHunger, distance, hungerUse, foodSequence);

        //Leaf checkHunger = new Leaf();
    }

    private void GetTarget(GameObject _Target)
    {
        target = _Target;
    }

    private void GetDistance(float value)
    {
        distance = value;
    }
}
