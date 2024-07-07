using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BT.Decorator;

namespace BT.Process
{
    public interface IProcess
    {
        List<IDecorator> decorators { get; set; }
        void AddDecorator(IDecorator _Decorator);
        Node.Status Process();
        void Reset();
    }

    public class BaseProcess : IProcess
    {
        protected Node.Status status;

        public List<IDecorator> decorators { get; set; }

        public BaseProcess()
        {
            decorators = new();
            status = Node.Status.Running;
        }

        public BaseProcess(List<IDecorator> _Decorators)
        {
            decorators = _Decorators;
            status = Node.Status.Running;
        }

        /// <summary>
        /// all decorations processes
        /// </summary>
        /// <returns></returns>
        public virtual Node.Status Process()
        {
            status = Node.Status.Success;
            if (decorators.Count > 0)
            {
                foreach (var _Decorator in decorators)
                {
                    var _DecProcess = _Decorator.Process();

                    if (_DecProcess != Node.Status.Success)
                    {
                        return _DecProcess;
                    }
                    _Decorator.Reset();
                }

            } //else:

            return status;
        }

        public virtual void Reset()
        {
            if (decorators.Count <= 0)
                return;

            foreach (var _decorator in decorators)
            {
                _decorator.Reset();
            }
        }

        public virtual void AddDecorator(IDecorator _Decorator)
        {
            decorators ??= new();
            decorators.Add(_Decorator);
        }
    }

    public class GoTo_Process : BaseProcess
    {
        // given variables:
        NavMeshAgent agent;

        public delegate ITarget GetITarget();
        GetITarget getTarget;

        public delegate float GetFloat();
        event GetFloat getDistance;

        ITarget target;
        float stopDist;
        public delegate void RemoveBar();
        event RemoveBar removeBar;
        float distance;

        // private variables:
        float lastDist;

        bool failed;

        public delegate void GiveITarget(ITarget value);
        event GiveITarget giveOldTarget;

        //public GoTo_Process(List<ITarget> _Targets, NavMeshAgent _Agent, IDecorator _Decorator, float _StopDist,RemoveBar _RemoveBar ) : base()
        //{
        //    stopDist = _StopDist;
        //    targets = _Targets;
        //    agent = _Agent;
        //    removeBar = _RemoveBar;
        //    decorators = new() { _Decorator };
        //    distance = distance
        //}

        public GoTo_Process(GetITarget _GetTarget, NavMeshAgent _Agent, float _StopDist, GetFloat _GetDistance, RemoveBar _RemoveBar, GiveITarget _GiveOldTarget) : base()
        {
            stopDist = _StopDist;
            getTarget = _GetTarget;
            agent = _Agent;
            removeBar = _RemoveBar;
            decorators = new();
            getDistance = _GetDistance;
            giveOldTarget = _GiveOldTarget;
        }

        /// <summary>
        /// goes to target
        /// </summary>
        /// <returns></returns>
        public override Node.Status Process()
        {
            if (target == null)
            {
                target = getTarget?.Invoke();
                distance = (float)getDistance?.Invoke();
                lastDist = distance;
                agent.SetDestination(target.MyGameObject.transform.position);
            }

            // all decorations processes

            if (decorators.Count > 0)
            {
                Node.Status _DecProcess = Node.Status.Failure;
                foreach (var _Decorator in decorators)
                {
                    _DecProcess = _Decorator.Process();

                    if (_DecProcess != Node.Status.Success)
                    {
                        return _DecProcess;
                    }
                }
                giveOldTarget?.Invoke(target);
                agent.SetDestination(target.MyGameObject.transform.position);
                
            }



            //if (status != Node.Status.Success)
            //{
            //    failed = true;
            //    return status;
            // -
            //}
            //else if (failed)
            //    return Node.Status.Success;
            // ----
            //else
            //{
            //    target = getTarget?.Invoke();
            //    Debug.Log("Successsssssss");
            //}



            Vector3 agentPos = agent.transform.position;
            agentPos.y = 0;
            Vector3 targetPos = target.MyGameObject.transform.position;
            targetPos.y = 0;

            distance = Vector3.Distance(agentPos, targetPos);

            //Debug.LogError(Vector3.Distance(target.MyGameObject.transform.position, agent.transform.position) + "\n" + distance);
            //if (distance <= 0)
            //{
            //    return Node.Status.Running;
            //}

            if (lastDist - distance >= 1)
            {
                lastDist = distance;
                removeBar?.Invoke();
            }

            if (distance <= stopDist)
                status = Node.Status.Success;
            else
                status = Node.Status.Running;

            return status;
        }

        public override void Reset()
        {
            target = null;
            distance = 0;
            failed = false;

            base.Reset();
        }

        public override void AddDecorator(IDecorator _Decorator)
        {
            base.AddDecorator(_Decorator);
        }
    }

    public class PerformingTask_Process : BaseProcess
    {
        float duration;
        public delegate void Task();
        event Task taskOver;

        float timer;

        public PerformingTask_Process(float _Duration) : base()
        {
            duration = _Duration;
            taskOver = null;
        }

        public PerformingTask_Process(float _Duration, Task _TaskOver) : base()
        {
            duration = _Duration;
            taskOver = _TaskOver;
        }

        public override Node.Status Process()
        {
            status = base.Process();
            if (status != Node.Status.Success)
                return status;

            if (timer < duration)
            {
                timer += Time.deltaTime;
                return Node.Status.Running;
            }

            taskOver?.Invoke(); // ex: if jack eats he will restore to the full hunger bar 

            return Node.Status.Success;
        }

        public override void AddDecorator(IDecorator _Decorator)
        {
            base.AddDecorator(_Decorator);
        }
        public override void Reset()
        {
            timer = 0;
            base.Reset();
        }
    }

    public class Idle : BaseProcess
    {
        public override Node.Status Process()
        {
            return Node.Status.Running;

        }
        public override void AddDecorator(IDecorator _Decorator)
        {
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
