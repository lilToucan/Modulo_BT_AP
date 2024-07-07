using BT;
using BT.Decorator;
using BT.Process;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JackTree : MonoBehaviour
{
    ITarget homePoint;  // sleep
    List<ITarget> foodPoints = new();  // eat
    ITarget waterPoint; // drink
    ITarget stackPoint; // stack
    List<ITarget> trees = new();

    [Header("Hunger Settings:")]
    [SerializeField] public float minHunger;
    [SerializeField] public float maxHunger;
    [SerializeField] public float hungerUse;

    [Header("Thirst Settings:")]
    [SerializeField] public float minThirst;
    [SerializeField] public float maxThirst;
    [SerializeField] public float thirstUse;

    [Header("Process Settings:")]
    [SerializeField] GameObject log;
    [SerializeField] float stopDist;
    [SerializeField] float eatDur;
    [SerializeField] float drinkDur;
    [SerializeField] float treeCutDur;
    public float currentHunger;
    public float currentThirst;

    float distance;
    ITarget target;

    BehaviourTree treeDay;
    BehaviourTree treeNight;
    BehaviourTree currentTree;
    NavMeshAgent agent;
    bool hasLog = false;
    Node.Status treeStatus = Node.Status.Running;

    Jack jack;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        trees.AddRange(FindObjectsOfType<Tree>());
        foodPoints.AddRange(FindObjectsOfType<Food>());
        stackPoint = FindObjectOfType<StackObj>();
        waterPoint = FindObjectOfType<Water>();
        homePoint = FindObjectOfType<Home>();
        jack = GetComponent<Jack>();
    }

    private void OnEnable()
    {
        Clock.OnTimeChange += TimeChanged;
    }

    private void OnDisable()
    {
        Clock.OnTimeChange -= TimeChanged;
    }

    private void TimeChanged()
    {
        currentTree.Reset();

        if (Clock.IsDay)
            currentTree = treeDay;
        else
            currentTree = treeNight;

        currentTree.Reset();
    }

    private void Start()
    {
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        InizializeBT();
    }

    private void Update()
    {
        if (treeStatus != Node.Status.Running)
        {
            //Debug.Log(treeStatus);
            currentTree.Reset();
        }
        treeStatus = currentTree.Process();
    }

    private void InizializeBT()
    {
        // gl mr venice no 30 (;﹏;)

        //sequences:
        treeDay = new("day tree (Jack: i won't cut this one) ");
        treeNight = new("night tree (Jack: i felling eppy)");

        //Selectors:
        Selector hasToDropLog = new("has to drop log?");

        //Sequences: 
        Sequence daySequence = new("day sequence");
        Sequence nightSequence = new("night sequence");
        Sequence eatSequence = new("Food Sequence (Jack: i got to eat man)");
        Sequence drinkSequence = new("Drink sequence (Jack: i'm thirsty *coff* *coff*)");
        Sequence dropLogSequence = new("Dropping log");
        Sequence dontDropLogSequence = new("skip log");

        #region distance calulations:

        //distance calulators:
        CalculateDistance_Decorator distanceToTree = new(GetDistance, GetTarget, trees, agent);
        CalculateShortestDistance_Decorator distanceFromFood = new(GetDistance, GetTarget, foodPoints, agent);
        CalculateDistance_Decorator distanceFromWater = new(GetDistance, GetTarget, waterPoint, agent);

        //Distance Leafs:
        Leaf leaf_DistanceToTree = new("Calculate Distance", distanceToTree);
        Leaf leaf_ShortestDistanceToFood = new("Calculate Distance", distanceFromFood);
        Leaf leaf_DistanceToWater = new("Calculate Distance", distanceFromWater);

        #endregion

        //Bars check:
        CheckBar_Decorator hungerCheck = new(() => currentHunger, minHunger, () => distance, hungerUse, eatSequence);
        CheckBar_Decorator thirstCheck = new(() => currentThirst, minThirst, () => distance, hungerUse, drinkSequence);


        #region GoTo:
        //GoTo:
        GoTo_Process goToTree = new(() => target, agent, stopDist, () => distance, DecreaseBars, GetTarget);
        goToTree.AddDecorator(hungerCheck); goToTree.AddDecorator(thirstCheck);
        GoTo_Process goToFood = new(() => target, agent, stopDist, () => distance, DecreaseBars, GetTarget);
        GoTo_Process goToWater = new(() => waterPoint, agent, stopDist, () => distance, DecreaseBars, GetTarget);
        GoTo_Process goToStack = new(() => stackPoint, agent, stopDist, () => distance, DecreaseBars, GetTarget);
        GoTo_Process goToSatckDecorated = new(() => stackPoint, agent, stopDist, () => distance, DecreaseBars, GetTarget);
        goToSatckDecorated.AddDecorator(new Check (()=> hasLog));
        GoTo_Process goToHome = new(() => homePoint, agent, stopDist, () => distance, DecreaseBars, GetTarget);
        goToHome.AddDecorator(hungerCheck); goToHome.AddDecorator(thirstCheck);

        //GoTo leafs:
        Leaf leaf_GoToTree = new("Going to tree", goToTree);
        Leaf leaf_GoToFood = new("Going to eat", goToFood);
        Leaf leaf_GoToWater = new("Going to drink", goToWater);
        Leaf leaf_GoToHome = new("Going home", goToHome);
        Leaf leaf_GoToStack = new("Go to stack", goToStack);
        Leaf leaf_GoToStackDecorated = new("go to stack if has log", goToSatckDecorated);
        #endregion

        #region PerformTask:
        //PerformTask:
        PerformingTask_Process cutTree = new(treeCutDur, GetLog);
        PerformingTask_Process eat = new(eatDur, OnEating);
        PerformingTask_Process drink = new(drinkDur, OnDrink);
        PerformingTask_Process dropLog = new(0.1f, OnDropLog);
        PerformingTask_Process sleep = new(0.1f, Sleep);

        //Performing taskOver leafs:
        Leaf leaf_CutTree = new("cutting tree", cutTree);
        Leaf leaf_DropLog = new("drop log", dropLog);
        Leaf leaf_Eat = new("Eating", eat);
        Leaf leaf_Drink = new("Drinking", drink);
        Leaf leaf_Sleep = new("Sleeping", sleep);
        #endregion

        //Day sequence:
        treeDay.AddChild(daySequence);
        daySequence.AddChild(leaf_DistanceToTree);
        daySequence.AddChild(leaf_GoToTree);
        daySequence.AddChild(leaf_CutTree);
        daySequence.AddChild(leaf_GoToStack);
        daySequence.AddChild(leaf_DropLog);



        //Food sequence:
        eatSequence.AddChild(leaf_ShortestDistanceToFood);
        eatSequence.AddChild(leaf_GoToFood);
        eatSequence.AddChild(leaf_Eat);
        eatSequence.AddChild(leaf_DistanceToTree);



        //Drink sequence:
        drinkSequence.AddChild(leaf_DistanceToWater);
        drinkSequence.AddChild(leaf_GoToWater);
        drinkSequence.AddChild(leaf_Drink);
        drinkSequence.AddChild(leaf_DistanceToTree);


        // night sequence:
        Leaf leaf_Nothing = new("nothing", new Idle());

        dropLogSequence.AddChild(leaf_GoToStackDecorated);
        dropLogSequence.AddChild(leaf_DropLog);
        dropLogSequence.AddChild(nightSequence);

        treeNight.AddChild(hasToDropLog);

        hasToDropLog.AddChild(dropLogSequence);
        hasToDropLog.AddChild(nightSequence);


        nightSequence.AddChild(leaf_GoToHome);
        nightSequence.AddChild(leaf_Sleep);
        nightSequence.AddChild(leaf_Nothing);


        currentTree = treeDay;
    }

    private void Sleep()
    {

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
        jack.jackSo.barChanged?.Invoke();
    }

    private void OnEating()
    {
        target.Interact();
        currentHunger = maxHunger;
        jack.jackSo.barChanged?.Invoke();
    }

    private void GetLog()
    {
        hasLog = true;
        log.SetActive(true);
        target.Interact();
    }

    private void DecreaseBars()
    {
        currentHunger -= hungerUse;
        currentThirst -= thirstUse;
        jack.jackSo.barChanged?.Invoke();
        if (currentHunger <= 0 || currentThirst <= 0)
        {
            jack.jackSo.ohIMDie?.Invoke();
        }
    }

    private void GetTarget(ITarget _Target)
    {
        //add anim?
        //add sound? 
        //add my will to live? 
        target = _Target;
    }

    private void GetDistance(float value)
    {
        distance = value;
    }
}
