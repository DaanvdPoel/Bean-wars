using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is basicaly the same script as HunterBrainBTSimple but it is breaking about all the rules to produce really nice, readable and maintainable code.
/// Hopefully you find the HunterBrainBTSimple script much more understandable and informative.
/// PLEASE study HunterBrainBTSimple.cs and recognize all the effort put into a consistent coding style and what this brings to the reader.
/// </summary>
namespace SimpleBehaviorTree.Examples
{
    class HunterBlackboardSimpleUgly : Blackboard {
        public float m_distanceToTarget = 0.0f;
    }

    public class HunterBrainBTSimpleUgly : MonoBehaviour
    {
        [SerializeField]
        private BehaviorTree m_tree;                                         // the behavior tree

        public GameObject target      ;
        public float pursueRadius = 7;
        public float appraoch_radius = 10.0f;
        public float appr_speed=1;
        public float pursue_spd=2.0f;
        public float m_rotationSpeed=10;
        private string state="-";
        private float cur_speed= 1.0f;
        private HunterBlackboardSimpleUgly m_blackboard;

        private void Start()        {
            //do start us up
            m_blackboard = new HunterBlackboardSimpleUgly();
            m_tree = new BehaviorTree(BuildTree1(), m_blackboard, BlackboardUpdater) { Name = "SimpleTree" };
        }

        void Update() {
            //does not work!
            //Vector3 targetDirection = transform.position -target.transform.position;
            Vector3 targetDirection=target.transform.position-transform.position   ;
            Vector3 desiredVelocity=               targetDirection.normalized*cur_speed;          transform.position= transform.position+ desiredVelocity*Time.deltaTime   ;
              transform.rotation= Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(targetDirection),m_rotationSpeed*Time.deltaTime)      ;
             transform.eulerAngles= new Vector3(0,transform.eulerAngles.y,0) ;
            Debug.Log("TESSSTT!" + transform.eulerAngles);
            m_tree.Update(Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            //UnityEditor.Handles.color = Color.cyan;
            //UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, appraoch_radius); UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, pursueRadius);
            //UnityEditor.Handles.BeginGUI();
            //UnityEditor.Handles.Label(transform.position, $"{state} (speed = {cur_speed})");
            //UnityEditor.Handles.EndGUI();
        }

        private RootNode BuildTree1() {
            return new                     BehaviorTreeBuilder().Name                                     ("HunterBrain")
                .Selector("MainSelector")                    .Sequence("Approach")     .Condition     ("InApproachRangeOnly", InApproachRangeOnly)
            .Do("ToApproach", ToApproach)
                              .RepeatUntilFailure("RepeatUntilFailure").Condition("InApproachRangeOnly",       InApproachRangeOnly).End()
                    .End       ()
                    .Sequence("Pursue")      .Condition          ("InPursueRange", InPursueRange).Do("ToPursue", ToPursue)
              .RepeatUntilFailure             ("RepeatUntilFailure")
             .Condition        ("InPursueRange", InPursueRange).End()               .End()                    .Sequence("Idle")
                        .Do("ToIdle", ToIdle)    .RepeatUntilSuccess("RepeatUntilSuccess")               .Condition("InApproachRange", InApproachRange)
                        .End()        .End()
                .End().Build();
        }

        private void BlackboardUpdater(Blackboard bb) {
            (m_blackboard as HunterBlackboardSimpleUgly).m_distanceToTarget=(target.transform.position - transform.position).magnitude;
        }

        /*
        void CalculateStuff()
        { Vector3 targetDirection= target.transform.position-transform.position;
        }
        */















        private bool InApproachRangeOnly(Blackboard bb)
        {
        return !InPursueRange(bb) && InApproachRange(bb);
        }

        private bool InApproachRange(Blackboard bb)
        { return (bb as UnitBlackboard).m_distanceToTarget < appraoch_radius;
        }



        private bool InPursueRange(Blackboard bb)
        {
            return (bb as UnitBlackboard).m_distanceToTarget < pursueRadius; }




        private NodeState ToApproach(Blackboard bb)
        {
            state = "Approach"; cur_speed = appr_speed; return NodeState.SUCCESS;
        }







        private NodeState ToPursue(Blackboard bb)
        {
            
                     cur_speed = pursue_spd;
          state = "Pursue";
            return NodeState.SUCCESS;
        }



        private NodeState ToIdle(Blackboard bb)
        {
            state = "Idle"; cur_speed = 0.0f;
            return NodeState.SUCCESS;
        }
    }
}
