using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BT.Process;

namespace BT
{
    public class Node
    {
        public enum Status { Success, Failure, Running }
        public readonly string Name;

        public readonly List<Node> Children = new();
        public int CurrentChild;

        public Node(string _Name)
        {
            Name = _Name;
        }
        public virtual void AddChild(Node _ChildNode)
        {
            Children.Add(_ChildNode);
        }

        public virtual Status Process()
        {
            return Children[CurrentChild].Process();
        }

        public virtual void Reset()
        {
            CurrentChild = 0;
            foreach (var _child in Children)
            {
                _child.Reset();
            }
        }
    }

    public class BehaviourTree : Node
    {
        public BehaviourTree(string _Name) : base(_Name)
        { }

        public override Status Process()
        {
            if (CurrentChild < Children.Count)
            {
                var status = Children[CurrentChild].Process();
                if(status != Status.Success)
                {
                    return status;
                }

                // else 
                CurrentChild++;
            }

            return Status.Success;
        }

        public override void Reset()
        {
            base.Reset();
        }
        public override void AddChild(Node _ChildNode)
        {
            base.AddChild(_ChildNode);
        }
    }

    public class Sequence : Node
    {
        public Sequence(string _Name) : base(_Name)
        {

        }

        public override Status Process()
        {
            if(CurrentChild >= Children.Count)
            {
                Reset();
                return Status.Success;
            }
            var childStatus = Children[CurrentChild].Process();
            if (childStatus == Status.Success)
            {
                CurrentChild++;
                return Status.Running;
            }
            else if(childStatus == Status.Running)
            {
                return Status.Running;
            }
            else
            {
                Reset();
                return Status.Failure;
            }
        }
        public override void AddChild(Node _ChildNode)
        {
            base.AddChild(_ChildNode);
        }

        public override void Reset()
        {
            base.Reset();
        }
    }

    public class Leaf : Node
    {
        public IProcess MyProcess;

        public Leaf(string _Name, IProcess _Process) : base(_Name)
        {
            MyProcess = _Process;
        }

        public override Status Process()
        {
            ; return MyProcess.Process();
        }

        public override void Reset()
        {
            MyProcess.Reset();
        }

        public override void AddChild(Node _ChildNode)
        {
            base.AddChild(_ChildNode);
        }
    }
   
    
}
