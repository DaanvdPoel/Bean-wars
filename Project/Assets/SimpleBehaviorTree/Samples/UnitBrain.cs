using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteeringBehaviour { Flocking, Wandering, Fleeing, FollowingPath, Approach, Attacking, AttackingBase }

namespace SimpleBehaviorTree.Examples
{
    /// <summary>
    /// The blackboard class that passes on information to all nodes in the behavior tree during the update.
    /// </summary>
    class UnitBlackboard : Blackboard
    {
        public float m_distanceToTarget = 0.0f;
    }

    /// <summary>
    /// Hunter brain class using behavior tree. Also implements debug feedback (dumping tree and reporting status updates)
    /// </summary>
    public class UnitBrain : MonoBehaviour
    {
        public enum BuildOption { AgressiveTree, DefensiveTree, LoyalTree, WanderTree, GuardPath }

        [SerializeField]
        private BehaviorTree tree;                    // the behavior tree

        [Header("Target")]
        public GameObject target;                     // our target object
        public float attackRadius = 2f;               // range for unit to hit from

        public List<GameObject> listOfEnemiesInRange = new List<GameObject>(); // list of all the enemies in range
        public List<GameObject> listOfAlliesInRange = new List<GameObject>(); // list of all the enemies in range

        [Header("Config")]
        public BuildOption buildOption = BuildOption.AgressiveTree; // which behavior tree do we use?
        public bool isPlayerUnit;                                   // true if unit is blue(player unit)
        public LayerMask aiLayer;                                   // layer for the AI units
        public LayerMask allyLayer;                                 // layer for the player units

        [Header("Unit stats")]
        public StatsScriptableObject.BehaviourList behaviourState;  // the assigned behaviour by the player
        public int movementSpeed;              // movement speed for the unit
        public int attackSpeed;                // attack speed for the unit
        public int attackDamage;               // attack damage for the unit
        public int viewingRange;               // viewing range for the unit
        public int defense;                    // defense/hp for the unit
        [Space]
        public int baseAttackSpeed;            // base attack speed for units (for balancing)

        [Header("Steering Settings")]
        public float approachSpeed = 1.0f;            // the approach speed in m/s  
        public float pursueSpeed = 2.0f;              // the pursue speed in m/s
        public float patrolSpeed = 1.0f;              // the pursue speed in m/s
        public float rotationSpeed = 10.0f;           // rotaton speed in degrees/s
        public bool doNotMove = true;                 // set to true to prevent the NPC from moving (debug option)
        public float maxPersueTime = 2.0f;


        [Header("Feedback")]
        private float activeSpeed = 2.0f;             // the active speed in m/s
        private string state = "-";                   // string that provides feedback on the current state
        public SteeringBehaviour currentSteeringState;// current steering behaviour thats being used
        public float attackTimer = 2f;                // attack timer 

        [Header("Private")]
        private UnitBlackboard blackboard;            // the blackboard used to pass info to the behavior tree during updates
        private float chaseTimer;                     // timer for chasing when in defensive or guard path
        private bool updateLists = true;              // variable used for updating enemy and allies in range lists
        private Steering.Steering steering;           // steering script attached to this unit
        private UnitAnimation anim;                   // animator attached to this unit (sword animation)
        private AudioSource audioSource;              // audio source for this unit
        //------------------------------------------------------------------------------------------
        // Unity overrides
        //------------------------------------------------------------------------------------------
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Call to initalize the stats of a unite
        /// </summary>
        /// <param name="_movementSpeed"> The movements speed of the unit</param>
        /// <param name="_attackSpeed"> The attack speed of a unit</param>
        /// <param name="_attackDamage"> The attack damage of a unit</param>
        /// <param name="_viewingRange"> The viewingRange of a unit</param>
        /// <param name="_defense"> The defense of a unit</param>
        /// <param name="_behaviourState"> The behaviourState of a unit</param>
        public void Initialize(int _movementSpeed, int _attackSpeed, int _attackDamage, int _viewingRange, int _defense, StatsScriptableObject.BehaviourList _behaviourState)
        {
            // assign stats
            movementSpeed = _movementSpeed;
            attackSpeed = _attackSpeed;
            attackDamage = _attackDamage;
            viewingRange = _viewingRange;
            defense = _defense;
            behaviourState = _behaviourState;

            // sellect correct tree to build
            switch (behaviourState)
            {
                case StatsScriptableObject.BehaviourList.Aggressive:
                    buildOption = BuildOption.AgressiveTree;
                    break;
                case StatsScriptableObject.BehaviourList.Defensive:
                    buildOption = BuildOption.DefensiveTree;
                    break;
                case StatsScriptableObject.BehaviourList.Loyal:
                    buildOption = BuildOption.LoyalTree;
                    break;
                case StatsScriptableObject.BehaviourList.Wanderer:
                    buildOption = BuildOption.WanderTree;
                    break;
                case StatsScriptableObject.BehaviourList.GuardPathA:
                    buildOption = BuildOption.GuardPath;
                    break;
                case StatsScriptableObject.BehaviourList.GuardPathB:
                    buildOption = BuildOption.GuardPath;
                    break;
                case StatsScriptableObject.BehaviourList.GuardPathC:
                    buildOption = BuildOption.GuardPath;

                    break;
                default:
                    break;
            }

            // get refrences
            steering = GetComponent<Steering.Steering>();
            anim = GetComponent<UnitAnimation>();

            // init blackboard
            blackboard = new UnitBlackboard();

            // prepare behavior tree
            switch (buildOption)
            {
                case BuildOption.AgressiveTree: tree = new BehaviorTree(AgressiveTree(), blackboard, BlackboardUpdater) { Name = "AgressiveTree" }; break;
                case BuildOption.DefensiveTree: tree = new BehaviorTree(DefensiveTree(), blackboard, BlackboardUpdater) { Name = "DefensiveTree" }; break;
                case BuildOption.LoyalTree: tree = new BehaviorTree(LoyalTree(), blackboard, BlackboardUpdater) { Name = "LoyalTree" }; break;
                case BuildOption.WanderTree: tree = new BehaviorTree(WanderTree(), blackboard, BlackboardUpdater) { Name = "WanderTree" }; break;
                case BuildOption.GuardPath: tree = new BehaviorTree(GuardPathTree(), blackboard, BlackboardUpdater) { Name = "GuardPathTree" }; break;
            }
        }

        void Update()
        {
            Collider[] enemyHits = Physics.OverlapSphere(transform.position, viewingRange / 2, aiLayer, QueryTriggerInteraction.Ignore);
            Collider[] allyHits = Physics.OverlapSphere(transform.position, viewingRange / 2, allyLayer, QueryTriggerInteraction.Ignore);

            listOfAlliesInRange.Clear();
            listOfEnemiesInRange.Clear();
            // put all enemies in range in a list
            if (updateLists)
                foreach (var item in enemyHits) listOfEnemiesInRange.Add(item.transform.gameObject);

            // put all allies in range in a list
            foreach (var item in allyHits) listOfAlliesInRange.Add(item.transform.gameObject);

            // timer for attacking
            if (currentSteeringState == SteeringBehaviour.Attacking || currentSteeringState == SteeringBehaviour.AttackingBase)
                attackTimer -= attackSpeed * baseAttackSpeed * Time.deltaTime;

            // true when supposed to attack
            if (attackTimer <= 0 && currentSteeringState == SteeringBehaviour.Attacking)
            {
                Health otherHealth = null;
                try { otherHealth = target.GetComponent<Health>(); }
                catch
                {
                    if (listOfEnemiesInRange.Count > 0)
                    {
                        target = listOfEnemiesInRange[0];
                        otherHealth = target.GetComponent<Health>();
                    }
                    else
                    {
                        attackTimer = attackSpeed / baseAttackSpeed;
                    }
                }

                AudioManager.instance.PlaySound(audioSource, Random.Range(1, 4));

                //check if attack killed enemy
                float health = otherHealth.health;
                otherHealth.ChangeHealth(-attackDamage);
                attackTimer = attackSpeed / baseAttackSpeed;
                if (listOfEnemiesInRange.Count > 1 && attackDamage >= health)
                {
                    listOfEnemiesInRange.RemoveAt(0);
                    target = listOfEnemiesInRange[0];
                    ToAttackEnemy(blackboard);
                }
                else
                {
                    if (listOfEnemiesInRange.Count > 0)
                    {
                        listOfEnemiesInRange.RemoveAt(0);
                    }
                    target = gameObject;
                }
                anim.Attack();
            }

            // do not move if requested
            if (!doNotMove)
            {
                // calculate target direction and desired velocity
                Vector3 targetDirection = target.transform.position - transform.position;
                Vector3 desiredVelocity = targetDirection.normalized * activeSpeed;


                // update position and rotation
                transform.position = transform.position + desiredVelocity * Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotationSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
            if (currentSteeringState == SteeringBehaviour.AttackingBase)
            {
                try
                {
                    if (Vector3.Distance(transform.position, target.transform.position) < attackRadius)
                    {
                        if (attackTimer <= 0)
                        {
                            UnitSpawner otherBase = target.GetComponent<UnitSpawner>();
                            otherBase.ChangeHealth(-attackDamage);
                            attackTimer = attackSpeed / baseAttackSpeed;
                        }
                    }
                }
                catch (System.Exception)
                {
                    Debug.Log("Something is wrong in AttackingBaseUpdate");
                    throw;
                }
            }

            // what to do when approaching
            if (currentSteeringState == SteeringBehaviour.Approach)
            {
                UnitBrain otherBrain = null;
                try
                {
                    otherBrain = target.GetComponent<UnitBrain>();
                }
                catch
                {
                    if (listOfEnemiesInRange.Count > 0)
                    {
                        target = listOfEnemiesInRange[0];
                        otherBrain = target.GetComponent<UnitBrain>();
                    }
                }

                if (otherBrain != null)
                {
                    SteeringBehaviour otherState = otherBrain.currentSteeringState;

                    if (otherState == SteeringBehaviour.Fleeing)
                    {
                        chaseTimer += Time.deltaTime;
                    }
                }
            }
            else
            {
                chaseTimer = 0;
            }

            // update the behavior tree
            tree.Update(Time.deltaTime);

            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        /// <summary>
        /// gizmos for debugging
        /// </summary>
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, viewingRange / 2);
            UnityEditor.Handles.BeginGUI();
            UnityEditor.Handles.Label(transform.position, $"{state} (speed = {activeSpeed})");
            UnityEditor.Handles.EndGUI();
#endif
        }

        //------------------------------------------------------------------------------------------
        // Methods that create a behavior tree
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Build a Agressive  behavior tree using the BehaviorTreeBuilder.
        /// </summary>
        /// <returns>A agressive Brain behavior tree.</returns>
        private RootNode AgressiveTree()
        {
            return new BehaviorTreeBuilder()
                .Name("UnitBrain")
                // agressive tree start
                .Selector("Agressive")
                    // runs away when defence is below 20
                    .Sequence("Flee")
                        .Condition("DefenceLow", DefenceLow)

                        .Do("ToFlee", ToFlee)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("DefenceLow", DefenceLow)
                        .End()

                    .End()
                    // attack oponent base
                    .Sequence("AttackBase")
                        .Condition("EnemyBaseInRange", EnemyBaseInRange)

                        .Do("ToAttackBase", ToAttackBase)
                        .RepeatUntilFailure()
                            .Condition("NoFleeingConditions", NoFleeingConditions)
                        .End()

                    .End()
                    // flock to target is just heading to oponent base
                    .Sequence("FlockToTarget")
                        .Condition("NoEnemyInVision", NoEnemyInVision)

                        .Do("ToFlock", ToFlock)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("NoEnemyInVisionAndDefenceNotLow", NoEnemyInVisionAndDefenceNotLow)
                        .End()

                    .End()
                    // chase down enemy untill either in attack range or out vision
                    .Sequence("ApproachEnemy")
                        .Condition("InVisionRangeOnly", InVisionRangeOnly)

                        .Do("ToApproach", ToApproach)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InVisionRangeOnlyAndDefenceNotLow", InVisionRangeOnlyAndDefenceNotLow)
                        .End()

                    .End()
                    // attack the enemy if it is in the attack radius
                    .Sequence("AttackEnemy")
                        .Condition("EnemyInAttackRange", EnemyInAttackRange)

                        .Do("ToAttackEnemy", ToAttackEnemy)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("EnemyInAttackRangeAndDefenceNotLow", EnemyInAttackRangeAndDefenceNotLow)
                        .End()

                    .End()
                .End()
                .Build();
        }

        /// <summary>
        /// Build a defensive behavior tree using the BehaviorTreeBuilder.
        /// </summary>
        /// <returns>A defensive Brain behavior tree.</returns>
        private RootNode DefensiveTree()
        {
            return new BehaviorTreeBuilder()
                .Name("UnitBrain")
                // agressive tree start
                .Selector("Defensive")
                    // runs away when defence is below 20
                    .Sequence("Flee")
                        .Condition("FleeingConditions", FleeingConditions)

                        .Do("ToFlee", ToFlee)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("FleeingConditions", FleeingConditions)
                        .End()

                    .End()
                    // attack oponent base
                    .Sequence("AttackBase")
                        .Condition("EnemyBaseInRange", EnemyBaseInRange)

                        .Do("ToAttackBase", ToAttackBase)
                        .RepeatUntilFailure()
                            .Condition("NoFleeingConditions", NoFleeingConditions)
                        .End()

                    .End()
                    // flock to target is just heading to oponent base
                    .Sequence("FlockToTarget")
                        .Condition("NoEnemyInVision", NoEnemyInVision)

                        .Do("ToFlock", ToFlock)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("StopFlocking", StopFlocking)
                        .End()

                    .End()
                    // chase down enemy untill either in attack range or out vision
                    .Sequence("ApproachEnemy")
                        .Condition("InVisionRangeOnly", InVisionRangeOnly)

                        .Do("ToApproach", ToApproach)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InVisionRangeOnlyAndFleeConditions", InVisionRangeOnlyAndNoFleeConditions)
                        .End()

                    .End()
                    // attack the enemy if it is in the attack radius
                    .Sequence("AttackEnemy")
                        .Condition("EnemyInAttackRange", EnemyInAttackRange)

                        .Do("ToAttackEnemy", ToAttackEnemy)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("EnemyInAttackRangeAndFleeConditions", EnemyInAttackRangeAndNoFleeConditions)
                        .End()

                    .End()
                .End()
                .Build();
        }

        /// <summary>
        /// the loyal behaviour tree
        /// </summary>
        /// <returns></returns>
        private RootNode LoyalTree()
        {
            return new BehaviorTreeBuilder()
                .Name("UnitBrain")
                // agressive tree start
                .Selector("Loyal")
                    // runs away when defence is below 20
                    .Sequence("Flee")
                        .Condition("FleeingConditions", FleeingConditions)

                        .Do("ToFlee", ToFlee)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("FleeingConditions", FleeingConditions)
                        .End()

                    .End()
                    // attack oponent base
                    .Sequence("AttackBase")
                        .Condition("EnemyBaseInRange", EnemyBaseInRange)

                        .Do("ToAttackBase", ToAttackBase)
                        .RepeatUntilFailure()
                            .Condition("NoFleeingConditions", NoFleeingConditions)
                        .End()

                    .End()

                    // chase down enemy untill either in attack range or out vision
                    .Sequence("ApproachAlly")
                        .Condition("AllyInVisionRangeOnly", AllyInVisionRangeOnly)

                        .Do("ToApproachAlly", ToApproachAlly)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("AllyInVisionRangeOnly", AllyInVisionRangeOnly)
                        .End()

                    .End()

                    // attack the enemy if it is in the attack radius
                    .Sequence("AttackEnemy")
                        .Condition("EnemyInAttackRange", EnemyInAttackRange)

                        .Do("ToAttackEnemy", ToAttackEnemy)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("EnemyInAttackRangeAndFleeConditions", EnemyInAttackRangeAndNoFleeConditions)
                        .End()

                    .End()

                    // Idle
                    .Sequence("Idle")
                        .Condition("NooneInVision", NooneInVision)
                        .Do("ToIdle", ToIdle)

                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("NooneInVision", NooneInVision)

                        .End()
                    .End()
                .End()
                .Build();
        }

        /// <summary>
        /// Build a wander behavior tree using the BehaviorTreeBuilder.
        /// </summary>
        /// <returns>A Wander Brain behavior tree.</returns>
        private RootNode WanderTree()
        {
            return new BehaviorTreeBuilder()
                .Name("UnitBrain")
                // agressive tree start
                .Selector("Wander")
                    // runs away when defence is below 20
                    .Sequence("Flee")
                        .Condition("FleeingConditions", FleeingConditions)

                        .Do("ToFlee", ToFlee)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("FleeingConditions", FleeingConditions)
                        .End()

                    .End()
                    // attack oponent base
                    .Sequence("AttackBase")
                        .Condition("EnemyBaseInRange", EnemyBaseInRange)

                        .Do("ToAttackBase", ToAttackBase)
                        .RepeatUntilFailure()
                            .Condition("NoFleeingConditions", NoFleeingConditions)
                        .End()

                    .End()
                    // Wander to target is just heading to oponent base
                    .Sequence("WanderToTarget")
                        .Condition("NoEnemyInVision", NoEnemyInVision)

                        .Do("ToWander", ToWander)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("NoEnemyInVisionAndDefenceNotLow", NoEnemyInVisionAndDefenceNotLow)
                        .End()

                    .End()
                    // chase down enemy untill either in attack range or out vision
                    .Sequence("ApproachEnemy")
                        .Condition("InVisionRangeOnly", InVisionRangeOnly)

                        .Do("ToApproach", ToApproach)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InVisionRangeOnlyAndDefenceNotLow", InVisionRangeOnlyAndDefenceNotLow)
                        .End()

                    .End()
                    // attack the enemy if it is in the attack radius
                    .Sequence("AttackEnemy")
                        .Condition("EnemyInAttackRange", EnemyInAttackRange)

                        .Do("ToAttackEnemy", ToAttackEnemy)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("EnemyInAttackRangeAndDefenceNotLow", EnemyInAttackRangeAndDefenceNotLow)
                        .End()

                    .End()
                .End()
                .Build();
        }

        /// <summary>
        /// Build a defensive behavior tree using the BehaviorTreeBuilder.
        /// </summary>
        /// <returns>A defensive Brain behavior tree.</returns>
        private RootNode GuardPathTree()
        {
            return new BehaviorTreeBuilder()
                .Name("UnitBrain")
                // agressive tree start
                .Selector("GuardPath")
                    // runs away when defence is below 20
                    .Sequence("Flee")
                        .Condition("FleeingConditions", FleeingConditions)

                        .Do("ToFlee", ToFlee)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("FleeingConditions", FleeingConditions)
                        .End()

                    .End()

                    // Follow Path
                    .Sequence("FollowPath")
                        .Condition("NoEnemyInVision", NoEnemyInVision)

                        .Do("ToFollowPath", ToFollowPath)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("NoEnemyInVision", NoEnemyInVision)
                        .End()

                    .End()
                    // chase down enemy untill either in attack range or out vision
                    .Sequence("ApproachEnemy")
                        .Condition("InVisionRangeOnly", InVisionRangeOnly)

                        .Do("ToApproach", ToApproach)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InVisionRangeOnlyAndFleeConditions", InVisionRangeOnlyAndNoFleeConditions)
                        .End()

                    .End()
                    // attack the enemy if it is in the attack radius
                    .Sequence("AttackEnemy")
                        .Condition("EnemyInAttackRange", EnemyInAttackRange)

                        .Do("ToAttackEnemy", ToAttackEnemy)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("EnemyInAttackRangeAndFleeConditions", EnemyInAttackRangeAndNoFleeConditions)
                        .End()

                    .End()
                .End()
                .Build();
        }

        /// <summary>
        /// Method that updates out blackboard before each update.
        /// </summary>
        /// <param name="bb">The blackboard to update.</param>
        /// 
        private void BlackboardUpdater(Blackboard bb)
        {
            // update distance to target
            if (target != null)
            {
                (blackboard as UnitBlackboard).m_distanceToTarget = (target.transform.position - transform.position).magnitude;
            }
            else
            {
                (blackboard as UnitBlackboard).m_distanceToTarget = 999f;
            }
        }

        //------------------------------------------------------------------------------------------
        // All methods used by the behavior tree
        //------------------------------------------------------------------------------------------

        #region Action methods linked to the behavior tree

        /// <summary>
        /// true if either Defence is below 20 or Outnumberd 1/3
        /// </summary>
        private bool DefenceLowOrOutnumberd(Blackboard bb)
        {
            return DefenceLow(bb) || Outnumberd(bb);
        }


        /// <summary>
        /// true if units defence is below 20
        /// </summary>
        private bool DefenceLow(Blackboard bb)
        {
            if (defense < 20)
                return true;
            else
                return false;
        }

        /// <summary>
        /// true if no ally nor enemy is in vision
        /// </summary>
        private bool NooneInVision(Blackboard bb)
        {
            return NoEnemyInVision(bb) && !AllyInVision(bb);
        }

        /// <summary>
        /// true if there is no enemies in vision, just allies
        /// </summary>
        private bool AllyInVisionRangeOnly(Blackboard bb)
        {
            return AllyInVision(bb) && NoEnemyInVision(bb);
        }

        /// <summary>
        /// function to swap to ApproachAlly
        /// </summary>
        private NodeState ToApproachAlly(Blackboard bb)
        {
            state = "ApproachAlly";
            currentSteeringState = SteeringBehaviour.Approach;

            activeSpeed = approachSpeed;
            if (listOfAlliesInRange.Count != 0)
            {
                // look for a teammate to target that is not loyal behaviour
                for (int i = 0; i < listOfAlliesInRange.Count; i++)
                {
                    target = listOfAlliesInRange[i];
                    if (target.GetComponent<UnitBrain>().behaviourState != StatsScriptableObject.BehaviourList.Loyal)
                    {
                        break;
                    }
                    else if (i == listOfAlliesInRange.Count - 1)
                    {
                        target = gameObject;
                    }
                }

                // select steering behaviours that need to be used in this state
                List<Steering.IBehavior> behaviours = new List<Steering.IBehavior>
                {
                    new Steering.AvoidWall(),
                    new Steering.Flock(gameObject),
                    new Steering.Pursue(target, this)
                };

                steering.SetBehaviors(behaviours, "ApproachingALly");
            }
            else
                Debug.LogError($"List of allies in {this} is empty when trying to acces it in ToApproachAlly()");

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// true is there is no enemies in vision and not in range of the other base and there is no fleeing conditions met
        /// </summary>
        private bool StopFlocking(Blackboard bb)
        {
            return NoEnemyInVision(bb) && NoFleeingConditions(bb) && EnemyBaseNotInRange(bb);
        }

        /// <summary>
        /// true if units defence is above or equal to 20
        /// </summary>
        private bool DefenceNotLow(Blackboard bb)
        {
            if (defense >= 20)
                return true;
            else
                return false;
        }

        /// <summary>
        /// true if unit is outnumberd by enemies 1 to 3
        /// </summary>
        private bool Outnumberd(Blackboard bb)
        {
            if (listOfAlliesInRange.Count * 3 <= listOfEnemiesInRange.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// trur if unit is not outnumberd
        /// </summary>
        private bool NotOutnumberd(Blackboard bb)
        {
            if (listOfAlliesInRange.Count * 3 <= listOfEnemiesInRange.Count)
                return false;
            else
                return true;
        }

        /// <summary>
        /// true if the enemy base is in range
        /// </summary>
        private bool EnemyBaseInRange(Blackboard bb)
        {
            if (isPlayerUnit)
            {
                if (Vector3.Distance(transform.position, RefrenceManager.instance.enemyBase.position) < attackRadius)
                    return true;
                else
                    return false;
            }
            else
            {
                if (Vector3.Distance(transform.position, RefrenceManager.instance.playerBase.position) < attackRadius)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// true if enemy base is not in range
        /// </summary>
        private bool EnemyBaseNotInRange(Blackboard bb)
        {
            if (isPlayerUnit)
            {
                if (Vector3.Distance(transform.position, RefrenceManager.instance.enemyBase.position) < attackRadius)
                    return false;
                else
                    return true;
            }
            else
            {
                if (Vector3.Distance(transform.position, RefrenceManager.instance.playerBase.position) < attackRadius)
                    return false;
                else
                    return true;
            }
        }

        #region combined checks
        /// <summary>
        /// Check wether the target is further away than the pursue range but still within the approach range.
        /// </summary>
        private bool InVisionRangeOnly(Blackboard bb) { return EnemyInVision(bb) && NoEnemyInAttackRange(bb); }
        private bool NoEnemyInVisionAndDefenceNotLow(Blackboard bb) => NoEnemyInVision(bb) && DefenceNotLow(bb);
        private bool InVisionRangeOnlyAndDefenceNotLow(Blackboard bb) => InVisionRangeOnly(bb) && DefenceNotLow(bb);
        private bool EnemyInAttackRangeAndDefenceNotLow(Blackboard bb) => EnemyInAttackRange(bb) && DefenceNotLow(bb);
        private bool NoEnemyInVisionAndFleeConditions(Blackboard bb) => NoEnemyInVision(bb) && FleeingConditions(bb);
        private bool NoEnemyInVisionAndNoFleeConditions(Blackboard bb) => NoEnemyInVision(bb) && NoFleeingConditions(bb);
        private bool InVisionRangeOnlyAndFleeConditions(Blackboard bb) => InVisionRangeOnly(bb) && FleeingConditions(bb);
        private bool InVisionRangeOnlyAndNoFleeConditions(Blackboard bb) => InVisionRangeOnly(bb) && NoFleeingConditions(bb);
        private bool EnemyInAttackRangeAndFleeConditions(Blackboard bb) => EnemyInAttackRange(bb) && FleeingConditions(bb);
        private bool EnemyInAttackRangeAndNoFleeConditions(Blackboard bb) => EnemyInAttackRange(bb) && NoFleeingConditions(bb);
        private bool FleeingConditions(Blackboard bb) { return DefenceLow(bb) || Outnumberd(bb); }
        private bool NoFleeingConditions(Blackboard bb) { return DefenceNotLow(bb) || NotOutnumberd(bb); }
        #endregion

        /// <summary>
        /// true if there is any enemies in range
        /// </summary>
        private bool NoEnemyInVision(Blackboard bb)
        {
            return listOfEnemiesInRange.Count == 0;
        }

        /// <summary>
        /// true if there is any enemies in range
        /// </summary>
        private bool EnemyInVision(Blackboard bb)
        {
            bool inRange = false;
            if (listOfEnemiesInRange.Count != 0)
                inRange = true;
            return inRange;
        }

        /// <summary>
        /// true if there is any allies in range
        /// </summary>
        private bool AllyInVision(Blackboard bb)
        {
            bool inRange = false;
            foreach (GameObject GO in listOfAlliesInRange)
            {
                if (GO.GetComponent<UnitBrain>().behaviourState != StatsScriptableObject.BehaviourList.Loyal)
                {
                    inRange = true;
                }
            }

            return inRange;
        }

        /// <summary>
        /// true if an enemy is in attack range
        /// </summary>
        private bool EnemyInAttackRange(Blackboard bb)
        {
            foreach (GameObject item in listOfEnemiesInRange)
                if (Vector3.Distance(item.transform.position, transform.position) < attackRadius)
                    return true;
            return false;
        }

        private bool NoEnemyInAttackRange(Blackboard bb)
        {
            foreach (GameObject item in listOfEnemiesInRange)
                if (Vector3.Distance(item.transform.position, transform.position) < attackRadius)
                    return false;
            return true;
        }

        /// <summary>
        /// Switch to approach modus.
        /// </summary>
        private NodeState ToApproach(Blackboard bb)
        {
            AudioManager.instance.PlaySoundEffect(4); //plays ready for fight sound effect

            state = "Approach";
            currentSteeringState = SteeringBehaviour.Approach;

            activeSpeed = approachSpeed;
            if (listOfEnemiesInRange.Count != 0)
            {
                target = listOfEnemiesInRange[0];

                if (target.GetComponent<UnitBrain>().currentSteeringState == SteeringBehaviour.Fleeing)
                {
                    StartCoroutine(ChaseTimer(maxPersueTime));
                }

                // all steering behaviours to use in this state
                List<Steering.IBehavior> behaviours = new List<Steering.IBehavior>
                {
                    new Steering.AvoidWall(),
                    new Steering.Pursue(target, this)
                };

                steering.SetBehaviors(behaviours, "Approaching");
            }
            else
                Debug.LogError($"List of enemies in {this} is empty when trying to acces it in ToApproach()");

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// timer to check if a unit has been chasing for to long
        /// </summary>
        private IEnumerator ChaseTimer(float time)
        {
            SteeringBehaviour tempState = currentSteeringState;
            GameObject temp = target;
            yield return new WaitForSeconds(time);
            if (target == temp && currentSteeringState == tempState)
            {
                updateLists = false;
                listOfEnemiesInRange.Clear();
            }
            yield return new WaitForSeconds(time / 2);
            {
                updateLists = true;
            }
        }

        /// <summary>
        /// switch to attacking other base behaviour
        /// </summary>
        private NodeState ToAttackBase(Blackboard bb)
        {
            state = "AttackingBase";
            currentSteeringState = SteeringBehaviour.AttackingBase;

            if (isPlayerUnit)
                target = RefrenceManager.instance.enemyBase.gameObject;
            else
                target = RefrenceManager.instance.playerBase.gameObject;

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// Swith to flee behaviour
        /// </summary>
        private NodeState ToFlee(Blackboard bb)
        {
            state = "Flee";
            currentSteeringState = SteeringBehaviour.Fleeing;

            if (isPlayerUnit)
                target = RefrenceManager.instance.playerBase.gameObject;
            else
                target = RefrenceManager.instance.enemyBase.gameObject;

            // list of steering behaviours to use
            List<Steering.IBehavior> behaviours = new List<Steering.IBehavior>
                {
                    new Steering.AvoidWall(),
                    new Steering.Flee(target)
                };

            steering.SetBehaviors(behaviours, "Fleeing");

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// Switch to the follow path behaviour
        /// </summary>
        private NodeState ToFollowPath(Blackboard bb)
        {
            state = "FollowPath";

            List<Steering.IBehavior> behaviours = new List<Steering.IBehavior>();
            behaviours.Add(new Steering.Flock(gameObject));
            behaviours.Add(new Steering.AvoidWall());

            // select the correct guard path
            switch (behaviourState)
            {
                case StatsScriptableObject.BehaviourList.GuardPathA:
                    if (isPlayerUnit)
                        behaviours.Add(new Steering.FollowPath(RefrenceManager.instance.blueGuardpathA));
                    else
                        behaviours.Add(new Steering.FollowPath(RefrenceManager.instance.redGuardpathA));
                    break;
                case StatsScriptableObject.BehaviourList.GuardPathB:
                    if (isPlayerUnit)
                        behaviours.Add(new Steering.FollowPath(RefrenceManager.instance.blueGuardpathB));
                    else
                        behaviours.Add(new Steering.FollowPath(RefrenceManager.instance.redGuardpathB));
                    break;
                case StatsScriptableObject.BehaviourList.GuardPathC:
                    if (isPlayerUnit)
                        behaviours.Add(new Steering.FollowPath(RefrenceManager.instance.blueGuardpathC));
                    else
                        behaviours.Add(new Steering.FollowPath(RefrenceManager.instance.redGuardpathC));
                    break;
                default:
                    Debug.LogError($"You tried to set follow path without having behaviourState in guardpath state in ToFollowPath() \n State = {behaviourState}");
                    break;
            }

            steering.SetBehaviors(behaviours, "FOllowPath");

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// switch to the flocking behaviour
        /// </summary>
        private NodeState ToFlock(Blackboard bb)
        {
            state = "Flock";
            currentSteeringState = SteeringBehaviour.Flocking;

            if (isPlayerUnit)
                target = RefrenceManager.instance.enemyBase.gameObject;
            else
                target = RefrenceManager.instance.playerBase.gameObject;

            // list of steering behaviours to use
            List<Steering.IBehavior> behaviours = new List<Steering.IBehavior>
                {
                    new Steering.AvoidWall(),
                    new Steering.Flock(gameObject),
                    new Steering.Seek(target)
                };

            steering.SetBehaviors(behaviours, "Flocking");

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// switch to wander state
        /// </summary>
        private NodeState ToWander(Blackboard bb)
        {
            state = "Wander";
            currentSteeringState = SteeringBehaviour.Wandering;

            if (isPlayerUnit)
                target = RefrenceManager.instance.enemyBase.gameObject;
            else
                target = RefrenceManager.instance.playerBase.gameObject;

            // list of steering behaviours to use
            List<Steering.IBehavior> behaviours = new List<Steering.IBehavior>
                {
                    new Steering.AvoidWall(),
                    new Steering.Flock(gameObject),
                    new Steering.Seek(target),
                    new Steering.Wander(transform)
                };

            steering.SetBehaviors(behaviours, "Wander");

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// switch to attacking state
        /// </summary>
        private NodeState ToAttackEnemy(Blackboard bb)
        {
            if (listOfEnemiesInRange.Count != 0)
                target = listOfEnemiesInRange[0];
            else
                Debug.LogError($"List of enemies in {this} is empty when trying to acces it in ToAttackEnemy()");

            state = "AttackEnemy";
            currentSteeringState = SteeringBehaviour.Attacking;

            // list of steering behaviours to use
            List<Steering.IBehavior> behaviours = new List<Steering.IBehavior>
                {
                    new Steering.AvoidWall(),
                    new Steering.Pursue(target, this)
                };

            steering.SetBehaviors(behaviours, "Attacking");

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// Switch to idle modus.
        /// </summary>
        private NodeState ToIdle(Blackboard bb)
        {
            state = "Idle";

            // list of steering behaviours to use
            List<Steering.IBehavior> behaviours = new List<Steering.IBehavior>
                {
                    new Steering.AvoidWall(),
                    new Steering.Flock(gameObject)
                };

            steering.SetBehaviors(behaviours, "Idle");

            return NodeState.SUCCESS;
        }

        #endregion
    }
}