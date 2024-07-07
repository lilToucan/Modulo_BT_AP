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
            {
                var nodeStatus = NodeNullCheck();
                if (nodeStatus != Node.Status.Success)
                {
                    return nodeStatus;
                }
                // if success: 
                cantReach = false;
                return Node.Status.Success;
            }

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
            Reset();
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
            node?.Reset();
        }
    }

    public class CalculateDistance_Decorator : IDecorator
    {
        public delegate void GiveDistance(float value);
        public delegate void GiveTarget(ITarget target);
        event GiveDistance giveDistance;
        event GiveTarget giveTarget;
        List<ITarget> targets = new();
        ITarget target = null;
        NavMeshAgent agent;
        public List<IDecorator> decorators { get; set; }


        public CalculateDistance_Decorator(GiveDistance _GiveDistance, GiveTarget _GiveTarget, List<ITarget> _Targets, NavMeshAgent _Agent)
        {
            giveDistance = _GiveDistance;
            giveTarget = _GiveTarget;

            targets = new();
            targets = _Targets;
            agent = _Agent;
        }

        public CalculateDistance_Decorator(GiveDistance _GiveDistance, GiveTarget _GiveTarget, ITarget _Target, NavMeshAgent _Agent)
        {
            giveDistance = _GiveDistance;
            giveTarget = _GiveTarget;
            targets = new() { _Target };
            agent = _Agent;
        }


        public void AddDecorator(IDecorator _Decorator)
        { }

        public Node.Status Process()
        {
            if (targets.Count <= 0)
                return Node.Status.Failure;

            if (target == null)
            {
                int rand = Random.Range(0, targets.Count);
                if (targets[rand].MeshRenderer != null)
                {
                    if (targets[rand].MeshRenderer.enabled == false)
                    {
                        return Node.Status.Running;
                    }

                }

                target = targets[rand];

                if (target == null)
                {
                    return Node.Status.Failure;
                }
            }


            giveTarget?.Invoke(target);

            Vector3 agentPos = agent.transform.position;
            agentPos.y = 0;
            Vector3 targetPos = target.MyGameObject.transform.position;
            targetPos.y = 0;

            var dist = Vector3.Distance(agentPos, targetPos); ;

            giveDistance?.Invoke(dist);

            return Node.Status.Success;
        }

        public void Reset()
        {
            target = null;
        }
    }

    public class CalculateShortestDistance_Decorator : IDecorator
    {
        public delegate void GiveDistance(float value);
        public delegate void GiveTarget(ITarget target);
        event GiveDistance giveDistance;
        event GiveTarget giveTarget;
        List<ITarget> targets = new();
        ITarget target = null;
        NavMeshAgent agent;
        public List<IDecorator> decorators { get; set; }


        public CalculateShortestDistance_Decorator(GiveDistance _GiveDistance, GiveTarget _GiveTarget, List<ITarget> _Targets, NavMeshAgent _Agent)
        {
            giveDistance = _GiveDistance;
            giveTarget = _GiveTarget;

            targets = new();
            targets = _Targets;
            agent = _Agent;
        }

        public CalculateShortestDistance_Decorator(GiveDistance _GiveDistance, GiveTarget _GiveTarget, ITarget _Target, NavMeshAgent _Agent)
        {
            giveDistance = _GiveDistance;
            giveTarget = _GiveTarget;
            targets = new() { _Target };
            agent = _Agent;
        }


        public void AddDecorator(IDecorator _Decorator)
        { }

        public Node.Status Process()
        {
            if (targets.Count <= 0)
                return Node.Status.Failure;

            float minDist = 999;
            
            if (target == null)
            {
                foreach (ITarget _Target in targets)
                {
                    if (_Target.MeshRenderer != null)
                        if (_Target.MeshRenderer.enabled == false)
                            continue;

                    var _TargPos = _Target.MyGameObject.transform.position;
                    var _AgentPos = agent.transform.position;
                    _TargPos.y = 0;
                    _AgentPos.y = 0;

                    var dist = Vector3.Distance(_AgentPos, _TargPos);
                    if (dist < minDist)
                    {
                        target = _Target;
                        minDist = dist;
                    }
                }

                if (target == null)
                {
                    Debug.Log(minDist);
                    return Node.Status.Failure;
                }
            }


            giveTarget?.Invoke(target);
            giveDistance?.Invoke(minDist);

            return Node.Status.Success;
        }

        public void Reset()
        {
            target = null;
        }
    }

    public class Check : IDecorator
    {
        public List<IDecorator> decorators { get; set; }
        public delegate bool GetBool();
        event GetBool getBool;


        public Check(GetBool _GetBool)
        {
            getBool = _GetBool;
        }

        public void AddDecorator(IDecorator _Decorator)
        {

        }

        public Node.Status Process()
        {
            if ((bool)getBool?.Invoke())
                return Node.Status.Success;
            else
                return Node.Status.Failure;
        }

        public void Reset()
        {

        }
    }
}
