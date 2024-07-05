using BT.Process;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BT.Decorator
{
    public interface IDecorator : IProcess
    {

    }

    public class CheckBar_Decorator : IDecorator
    {
        float currentBarValue;
        float minBarValue;
        float distance;
        float barUsedPerDist;
        float totBareUsed;
        Node node;

        public List<IDecorator> decorators { get; set; }
        float startValue;
        bool cantReach = false;

        public delegate float GetFloat();
        event GetFloat getDistance;
        event GetFloat getCurrentBarValue;

        bool fistTime = true;

        public CheckBar_Decorator(GetFloat _GetCurrentValue, float _MinBarValue, GetFloat _GetDistance, float _BarUsedPerDist, Node _Node)
        {
            getCurrentBarValue = _GetCurrentValue;
            minBarValue = _MinBarValue;
            getDistance = _GetDistance;
            barUsedPerDist = _BarUsedPerDist;
            node = _Node;
        }



        public void AddDecorator(IDecorator _Decorator)
        { }

        public Node.Status Process()
        {
            if (cantReach)
                return NodeNullCheck();

            if (fistTime)
            {
                fistTime = false;
                startValue = (float)getCurrentBarValue?.Invoke();
                distance = (float)getDistance?.Invoke();
                totBareUsed = (distance + 1) * barUsedPerDist;
            }

            if (startValue <= totBareUsed)
            {
                cantReach = true;
                return Node.Status.Running;
            }
            //else 
            currentBarValue = (float)getCurrentBarValue?.Invoke();
            if (currentBarValue <= minBarValue)
            {
                cantReach = true;
                return Node.Status.Running;
            }
            //else 
            return Node.Status.Success;




        }

        /// <summary>
        /// node?.Process() else fail;
        /// </summary>
        /// <returns></returns>
        Node.Status NodeNullCheck()
        {
            if (node != null)
                return node.Process();

            return Node.Status.Failure;
        }

        public void Reset()
        {
            totBareUsed = 0;
            fistTime = true;
            cantReach = false;
        }
    }

    public class CalculateDistance_Decorator : IDecorator
    {
        public delegate void GiveDistance(float value);
        public delegate void GiveTarget(ITarget target);
        event GiveDistance giveDistance;
        event GiveTarget giveTarget;
        List<ITarget> trees = new();
        ITarget target = null;
        NavMeshAgent agent;


        public CalculateDistance_Decorator(GiveDistance _GiveDistance, GiveTarget _GiveTarget, List<ITarget> _Trees, NavMeshAgent _Agent)
        {
            giveDistance = _GiveDistance;
            giveTarget = _GiveTarget;

            trees = new();
            trees = _Trees;
            agent = _Agent;
        }

        public CalculateDistance_Decorator(GiveDistance _GiveDistance, GiveTarget _GiveTarget, ITarget _Target, NavMeshAgent _Agent)
        {
            giveDistance = _GiveDistance;
            giveTarget = _GiveTarget;
            trees = new() { _Target };
            agent = _Agent;
        }

        public List<IDecorator> decorators { get; set; }

        public void AddDecorator(IDecorator _Decorator)
        { }

        public Node.Status Process()
        {
            if (trees.Count <= 0)
                return Node.Status.Failure;

            if (target == null)
            {
                int rand = Random.Range(0, trees.Count);
                if (trees[rand].MeshRenderer.enabled == false)
                {
                    return Node.Status.Running;
                }

                target = trees[rand];
                //Debug.Log(target.MyGameObject.name, target.MyGameObject);
            }

            if (target == null)
            {
                return Node.Status.Failure;
            }

            giveTarget?.Invoke(target);

            agent.SetDestination(target.MyGameObject.transform.position);
            Vector3 agentPos = agent.transform.position - Vector3.up * agent.transform.position.y;
            Vector3 targetPos = target.MyGameObject.transform.position - Vector3.up * target.MyGameObject.transform.position.y;
            var dist = Vector3.Distance(agentPos, targetPos); ;
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
