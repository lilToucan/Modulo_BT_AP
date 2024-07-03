using BT;
using BT.Decorator;
using BT.Process;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Jack : MonoBehaviour
{
    [SerializeField] ITarget homePoint;  // sleep
    [SerializeField] ITarget foodPoint;  // eat
    [SerializeField] ITarget waterPoint; // drink

    List<ITarget> trees = new();

    [SerializeField] float minHunger, maxHunger;
    [SerializeField] float minThirst, maxThirst;
    [SerializeField] float hungerUse, thirstUse;

    [SerializeField] float stopDist;

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
        //sequences:
        treeDay = new("day tree (Jack: i won't cut this one) ");
        treeNight = new("night tree (Jack: i felling eppy)");
        Sequence foodSequence = new("Food Sequence (Jack: i got to eat man)");
        Sequence drinkSequence = new("Drink sequence (Jack: i'm thirsty *coff* *coff*)");

        //distance calulators:
        CalculateDistance_Decorator distanceFromTree = new CalculateDistance_Decorator(GetDistance, GetTarget, trees, agent);
        CalculateDistance_Decorator distanceFromFood = new(GetDistance, GetTarget, foodPoint, agent);
        CalculateDistance_Decorator distanceFromWater = new(GetDistance, GetTarget, waterPoint, agent);


        //Bars check:
        CheckBarDecorator hungerCheck = new(() => currentHunger, minHunger, distance, hungerUse, foodSequence);
        CheckBarDecorator thirstCheck = new(() => currentThirst, minThirst, distance, hungerUse, foodSequence);

        //GoTo:
        GoTo_Process goToTree = new(target, agent, stopDist, distance, DecreaseBars);
            goToTree.AddDecorator(hungerCheck); goToTree.AddDecorator(thirstCheck);
        GoTo_Process goToFood = new(foodPoint.MyGameObject, agent, stopDist, distance, DecreaseBars);
        GoTo_Process goToWater = new(waterPoint.MyGameObject, agent, stopDist, distance, DecreaseBars);



        //Leaf chopTree = new Leaf();
    }

    private void DecreaseBars()
    {
        currentHunger -= hungerUse;
        currentThirst -= thirstUse;
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
