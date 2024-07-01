using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BT.Decoration;

namespace BT.Process
{
    public interface IProcess
    {
        IDecoration decoration { get; set; }
        Node.Status Process();
        void Reset();
    }


    public class BaseProcess : IProcess
    {
        Node.Status status;

        public IDecoration decoration { get; set; }

        public virtual Node.Status Process()
        {
            return status;
        }

        public virtual void Reset()
        {
            
        }
    }


    public class GoToTree : IProcess
    {
        Node.Status status;
        List<GameObject> trees;
        NavMeshAgent agent;

        public IDecoration decoration { get; set; }

        public GoToTree(List<GameObject> _Trees, NavMeshAgent _Agent, IDecoration _Decoration)
        {
            trees = _Trees;
            agent = _Agent;
            decoration = _Decoration;
        }
        public GoToTree(List<GameObject> _Trees, NavMeshAgent _Agent)
        {
            trees = _Trees;
            agent = _Agent;
            decoration = null;
        }

        public Node.Status Process()
        {
            if (decoration != null)
            {
                var decorStatus = decoration.Process();
                if (decorStatus == Node.Status.Success)
                {

                }
            }

            return status;
        }

        public void Reset()
        {

        }
    }
}
