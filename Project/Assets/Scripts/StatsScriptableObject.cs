using UnityEngine;

//creates scriptable object in unity editor

//[CreateAssetMenu(fileName = "StatsUnit", menuName = "ScriptableObjects/UnitStats", order = 1)]
public class StatsScriptableObject : ScriptableObject
{
    //all variables which can be changed for the units
    public enum BehaviourList { Aggressive, Defensive, Loyal, Wanderer, GuardPathA, GuardPathB, GuardPathC }

    public BehaviourList behaviourState;
    public int movementSpeed;
    public int attackSpeed;
    public int attackDamage;
    public int viewingRange;
    public int defense;
    public int remainingPoints;
}

