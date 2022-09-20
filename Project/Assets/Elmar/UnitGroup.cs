using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitGroup : MonoBehaviour
{
    public List<GameObject> unitsInGroup;
    public StatsScriptableObject stats;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// used to set all the units stats
    /// </summary>
    public void Initialize()
    {
        StatsScriptableObject finalStats = new StatsScriptableObject();
        finalStats = stats;
        float totStatsCount = 0;

        totStatsCount += finalStats.attackDamage;
        totStatsCount += finalStats.attackSpeed;
        totStatsCount += finalStats.defense;
        totStatsCount += finalStats.movementSpeed;
        totStatsCount += finalStats.viewingRange;

        if (totStatsCount != 150)
        {
            int remainder = Mathf.RoundToInt((150 - totStatsCount) / 5);
            finalStats.attackDamage += remainder;
            finalStats.attackSpeed += remainder;
            finalStats.defense += remainder;
            finalStats.movementSpeed += remainder;
            finalStats.viewingRange += remainder;
        }

        // throw error if used wrong
        if (unitsInGroup.Count == 0)
            Debug.LogError($"No units in unitsIngroup in {this} \n Add units to list before calling Initialize!!!");

        // set the stats in all units
        foreach (GameObject unit in unitsInGroup)
        {
             SimpleBehaviorTree.Examples.UnitBrain bhv = unit.GetComponent<SimpleBehaviorTree.Examples.UnitBrain>();


        }
    }
}
