using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleBehaviorTree
{
    public class BehaviorTreeBuilder 
    {
        private readonly RootNode _tree;
        private readonly List<INode> _pointers = new List<INode>();

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public BehaviorTreeBuilder()
        {
            _tree = new RootNode();
            _pointers.Add(_tree);
        }

        public BehaviorTreeBuilder Name(string name)
        {
            _tree.Name = name;
            return this;
        }

        public BehaviorTreeBuilder AddNodeWithPointer(INode task)
        {
            AddNode(task);
            _pointers.Add(task);

            return this;
        }

        public BehaviorTreeBuilder ParentTask<P>(string name) where P : INode, new()
        {
            var parent = new P { Name = name };
            return AddNodeWithPointer(parent);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public BehaviorTreeBuilder Parallel(string name = "Parallel", int numAllowedToFail = 0, int numRequiredToSucceed = -1)
        {
            Parallel parent = new Parallel { Name = name, NumAllowedToFail = numAllowedToFail, NumRequiredToSucceed = numRequiredToSucceed };
            return AddNodeWithPointer(parent);
        }

        public BehaviorTreeBuilder RandomSelector(string name = "RandomSelector")
        {
            return ParentTask<RandomSelector>(name);
        }

        public BehaviorTreeBuilder RandomSequence(string name = "RandomSequence")
        {
            return ParentTask<RandomSequence>(name);
        }

        public BehaviorTreeBuilder Selector(string name = "Selector")
        {
            return ParentTask<Selector>(name);
        }

        public BehaviorTreeBuilder Sequence(string name = "Sequence")
        {
            return ParentTask<Sequence>(name);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public BehaviorTreeBuilder Inverter(string name = "Inverter")
        {
            return ParentTask<Inverter>(name);
        }

        public BehaviorTreeBuilder RepeatForever(string name = "RepeatForever")
        {
            return ParentTask<RepeatForever>(name);
        }
        
        public BehaviorTreeBuilder RepeatUntilFailure(string name = "RepeatUntilFailure")
        {
            return ParentTask<RepeatUntilFailure>(name);
        }

        public BehaviorTreeBuilder RepeatUntilSuccess(string name = "RepeatUntilSuccess")
        {
            return ParentTask<RepeatUntilSuccess>(name);
        }

        public BehaviorTreeBuilder ReturnFailure(string name = "ReturnFailure")
        {
            return ParentTask<ReturnFailure>(name);
        }

        public BehaviorTreeBuilder ReturnSuccess(string name = "ReturnSuccess")
        {
            return ParentTask<ReturnSuccess>(name);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public BehaviorTreeBuilder Do(string name, Func<Blackboard, NodeState> action)
        {
            return AddNode(new Action(action)
            {
                Name = name,
            });
        }

        public BehaviorTreeBuilder Do(Func<Blackboard, NodeState> action)
        {
            return Do("Action", action);
        }
        
        public BehaviorTreeBuilder Condition(string name, Func<Blackboard, bool> action)
        {
            return AddNode(new Condition(action) { Name = name });
        }

        public BehaviorTreeBuilder Condition(Func<Blackboard, bool> action)
        {
            return Condition("Condition", action);
        }

        public BehaviorTreeBuilder WaitForSeconds(string name, float waitTime)
        {
            return AddNode(new WaitForSeconds(waitTime) { Name = name });
        }

        public BehaviorTreeBuilder WaitForSeconds(float waitTime)
        {
            return WaitForSeconds("WaitForSeconds", waitTime);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private INode PointerCurrent
        {
            get
            {
                if (_pointers.Count == 0) return null;
                return _pointers[_pointers.Count - 1];
            }
        }

        public BehaviorTreeBuilder AddNode(INode node)
        {
            PointerCurrent.AddChild(node);
            return this;
        }

        public BehaviorTreeBuilder End()
        {
            _pointers.RemoveAt(_pointers.Count - 1);
            return this;
        }

        public RootNode Build()
        {
            return _tree;
        }
    }
}
