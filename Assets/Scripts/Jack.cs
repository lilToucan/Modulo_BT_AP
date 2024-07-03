using BT;
using BT.Decorator;
using BT.Process;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Jack : MonoBehaviour
{
    [Header("static points")]
    ITarget homePoint;  // sleep
    ITarget foodPoint;  // eat
    ITarget waterPoint; // drink
    ITarget stackPoint; // stack
    [SerializeField] GameObject log;

    List<ITarget> trees = new();

    [Header("Hunger Settings:")]
    [SerializeField] float minHunger;
    [SerializeField] float maxHunger;
    [SerializeField] float hungerUse;

    [Header("Thirst Settings:")]
    [SerializeField] float minThirst;
    [SerializeField] float maxThirst;
    [SerializeField] float thirstUse;

    [Header("Process Settings:")]
    [SerializeField] float stopDist;
    [SerializeField] float eatDur;
    [SerializeField] float drinkDur;
    [SerializeField] float treeCutDur;
    float currentHunger;
    float currentThirst;

    float distance;
    ITarget target;

    BehaviourTree treeDay;
    BehaviourTree treeNight;
    NavMeshAgent agent;
    bool hasLog = false;
    Node.Status treeStatus = Node.Status.Running;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        trees.AddRange(FindObjectsOfType<Tree>());
        stackPoint = FindObjectOfType<StackObj>();
        waterPoint = FindObjectOfType<Water>();
        foodPoint = FindObjectOfType<Food>();
        homePoint = FindObjectOfType<Home>();
    }

    private void Start()
    {
        InizializeBT();
    }

    private void Update()
    {
        if (treeStatus != Node.Status.Running)
        {
            treeDay.Reset();
        }
        treeStatus = treeDay.Process();
    }

    private void InizializeBT()
    {
        //sequences:
        treeDay = new("day tree (Jack: i won't cut this one) ");
        treeNight = new("night tree (Jack: i felling eppy)");

        Sequence daySequence = new("day sequence");
        Sequence nightSequence = new("night sequence");
        Sequence foodSequence = new("Food Sequence (Jack: i got to eat man)");
        Sequence drinkSequence = new("Drink sequence (Jack: i'm thirsty *coff* *coff*)");
        #region distance calulations:

        //distance calulators:
        CalculateDistance_Decorator distanceToTree = new(GetDistance, GetTarget, trees, agent);
        CalculateDistance_Decorator distanceFromFood = new(GetDistance, GetTarget, foodPoint, agent);
        CalculateDistance_Decorator distanceFromWater = new(GetDistance, GetTarget, waterPoint, agent);

        //Distance Leafs:
        Leaf leaf_DistanceToTree = new("Calculate Distance", distanceToTree);
        Leaf leaf_DistanceToFood = new("Calculate Distance", distanceFromFood);
        Leaf leaf_DistanceToWater = new("Calculate Distance", distanceFromWater);

        #endregion

        //Bars check:
        CheckBar_Decorator hungerCheck = new(() => currentHunger, minHunger, distance, hungerUse, foodSequence);
        CheckBar_Decorator thirstCheck = new(() => currentThirst, minThirst, distance, hungerUse, foodSequence);


        #region GoTo:
        //GoTo:
        GoTo_Process goToTree = new(target, agent, stopDist, distance, DecreaseBars);
        goToTree.AddDecorator(hungerCheck); goToTree.AddDecorator(thirstCheck);
        GoTo_Process goToFood = new(foodPoint, agent, stopDist, distance, DecreaseBars);
        GoTo_Process goToWater = new(waterPoint, agent, stopDist, distance, DecreaseBars);
        GoTo_Process goToStack = new(stackPoint, agent, stopDist, distance, DecreaseBars);
        GoTo_Process goToHome = new(homePoint, agent, stopDist, distance, DecreaseBars);
        goToHome.AddDecorator(hungerCheck); goToHome.AddDecorator(thirstCheck);

        //GoTo leafs:
        Leaf leaf_GoToTree = new("Going to tree", goToTree);
        Leaf leaf_GoToFood = new("Going to eat", goToFood);
        Leaf leaf_GoToWater = new("Going to drink", goToWater);
        Leaf leaf_GoToHome = new("Going home", goToHome);
        Leaf leaf_GoToStack = new("Go to stack", goToStack);
        #endregion

        #region PerformTask:
        //PerformTask:
        PerformingTask_Process cutTree = new(treeCutDur, GetLog);
        PerformingTask_Process eat = new(eatDur, OnEating);
        PerformingTask_Process drink = new(drinkDur, OnDrink);
        PerformingTask_Process dropLog = new(0.1f, OnDropLog);

        //Performing task leafs:
        Leaf leaf_CutTree = new("cutting tree", cutTree);
        Leaf leaf_Eat = new("Eating", eat);
        Leaf leaf_Drink = new("Drinking", drink);
        #endregion

        //Day sequence:
        treeDay.AddChild(daySequence);
        daySequence.AddChild(leaf_DistanceToTree);
        daySequence.AddChild(leaf_GoToTree);
        daySequence.AddChild(leaf_CutTree);
        daySequence.AddChild(leaf_GoToStack);

        //Food sequence:
        foodSequence.AddChild(leaf_DistanceToFood);
        foodSequence.AddChild(leaf_Eat);

        //Drink sequence:
        drinkSequence.AddChild(leaf_DistanceToWater);
        drinkSequence.AddChild(leaf_Drink);
    }

    private void OnDropLog()
    {
        if (hasLog)
        {
            hasLog = false;
            log.SetActive(false);
            stackPoint.Interact();
        }
    }

    private void OnDrink()
    {
        currentThirst = maxThirst;
    }

    private void OnEating()
    {
        currentHunger = maxHunger;
    }

    private void GetLog()
    {
        hasLog = true;
        log.SetActive(true);

    }

    private void DecreaseBars()
    {
        currentHunger -= hungerUse;
        currentThirst -= thirstUse;
    }

    private void GetTarget(ITarget _Target)
    {
        target = _Target;
        //Debug.LogWarning(target.MyGameObject.name, target.MyGameObject);
    }

    private void GetDistance(float value)
    {
        distance = value;
    }
}
