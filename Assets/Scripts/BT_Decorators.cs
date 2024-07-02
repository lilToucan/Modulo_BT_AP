using BT.Process;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BT.Decorator
{
    public interface IDecorator : IProcess
    {

    }

    public class CheckBarDecorator : IDecorator
    {
        float currentBarValue;
        float minBarValue;
        float distance;
        float barUsedPerDist;
        float totBareUsed;
        Node node;
        public delegate float GetValue();
        event GetValue getCurrentValue;
        public List<IDecorator> decorators { get; set; }

        public CheckBarDecorator(GetValue _GetCurrentValue, float _MinBarValue, float _Distance, float _BarUsedPerDist, Node _Node)
        {
            getCurrentValue = _GetCurrentValue;

            minBarValue = _MinBarValue;
            distance = _Distance;
            barUsedPerDist = _BarUsedPerDist;
            node = _Node;

            totBareUsed = (distance + 1) * barUsedPerDist;
        }



        public void AddDecorator(IDecorator _Decorator)
        { }

        public Node.Status Process()
        {
            currentBarValue = (float) getCurrentValue?.Invoke();
            if (currentBarValue <= minBarValue || currentBarValue <= totBareUsed)
            {
                return node.Process();
            }

            return Node.Status.Success;
        }

        public void Reset()
        {
            totBareUsed = 0;
            node = null;
        }
    }

    public class CalculateTarget_Decorator : IDecorator
    {
        public delegate void GiveDistance(float value);
        public delegate void GiveTarget(GameObject target);
        event GiveDistance giveDistance;
        event GiveTarget giveTarget;
        event GiveTarget getTarget;
        List<ITarget> trees = new();
        GameObject target = null;
        NavMeshAgent agent;


        public CalculateTarget_Decorator(GiveDistance _GiveDistance, GiveTarget _GiveTarget,List<ITarget> _Trees,NavMeshAgent _Agent)
        {
            giveDistance = _GiveDistance;
            giveTarget = _GiveTarget;
            trees = _Trees;
            agent = _Agent;
        }

        public CalculateTarget_Decorator(GiveDistance _GiveDistance, GiveTarget _GiveTarget, ITarget _Target, NavMeshAgent _Agent)
        { 
            giveDistance = _GiveDistance;
            giveTarget = _GiveTarget;
            trees.Add(_Target);
            agent = _Agent;
        }

        public List<IDecorator> decorators { get; set; }

        public void AddDecorator(IDecorator _Decorator)
        {}

        public Node.Status Process()
        {
            if (trees.Count <= 0)
                return Node.Status.Failure;

            int rand = Random.Range(0, trees.Count);
            if (trees[rand].MyGameObject.activeInHierarchy == false)
            {
                return Node.Status.Running;
            }

            target = trees[rand].MyGameObject;

            if (target == null)
                return Node.Status.Failure;

            giveTarget?.Invoke(target);

            agent.SetDestination(target.transform.position);
            var dist = agent.remainingDistance;
            agent.SetDestination(agent.transform.position);

            giveDistance?.Invoke(dist);

            return Node.Status.Success;
        }

        public void Reset()
        {
            target = null;
        }
    }
}
