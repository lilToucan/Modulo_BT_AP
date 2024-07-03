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

        public virtual Node.Status Process()
        {
            status = Node.Status.Success;
            if (decorators.Count > 0)
            {
                foreach (var _decorator in decorators)
                {
                    var _DecProcess = _decorator.Process();

                    if (_DecProcess != Node.Status.Success)
                    {
                        return _DecProcess;
                    }
                }
            } //else:
            return status;
        }

        public virtual void Reset()
        {
            if (decorators.Count <= 0 )
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
        GameObject target;
        float stopDist;
        public delegate void RemoveBar();
        event RemoveBar removeBar;
        float distance;

        // private variables:
        float lastDist;

        //public GoTo_Process(List<ITarget> _Targets, NavMeshAgent _Agent, IDecorator _Decorator, float _StopDist,RemoveBar _RemoveBar ) : base()
        //{
        //    stopDist = _StopDist;
        //    targets = _Targets;
        //    agent = _Agent;
        //    removeBar = _RemoveBar;
        //    decorators = new() { _Decorator };
        //    distance = distance
        //}

        public GoTo_Process(GameObject _Target, NavMeshAgent _Agent, float _StopDist, float _Distance ,RemoveBar _RemoveBar) : base()
        {
            stopDist = _StopDist;
            target = _Target;
            agent = _Agent;
            removeBar = _RemoveBar;
            decorators = new();
            distance = _Distance;
            lastDist = distance;
        }

        public override Node.Status Process()
        {
            if (target == null)
                return Node.Status.Failure;

            status = base.Process();
            if (status != Node.Status.Success)
                return status;

            agent.SetDestination(target.transform.position);

            distance = agent.remainingDistance;

            if(lastDist-distance >= 1)
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
        event Task task;

        float timer;

        public PerformingTask_Process(float _Duration) :base()
        {
            duration = _Duration;
            task = null;
        }

        public PerformingTask_Process(float _Duration, Task _Task) : base()
        {
            duration = _Duration;
            task = _Task;
        }

        public override Node.Status Process()
        {
            status = base.Process();
            if (status != Node.Status.Success)
                return status;

            if(timer < duration)
            {
                timer += Time.deltaTime;
                return Node.Status.Running;
            }

            task?.Invoke(); // ex: if jack eats he will restore to the full hunger bar 

            return Node.Status.Success;
        }

        public override void AddDecorator(IDecorator _Decorator)
        {
            base.AddDecorator(_Decorator);
        }
        public override void Reset()
        {
            base.Reset();
        }
    }
}
