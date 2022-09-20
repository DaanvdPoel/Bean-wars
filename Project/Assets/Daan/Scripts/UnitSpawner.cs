
using System.Collections.Generic;
using UnityEngine;

namespace SimpleBehaviorTree.Examples
{
    public class UnitSpawner : MonoBehaviour // Daan
    {
        [Header("Unit Settings")]
        [SerializeField] private StatsScriptableObject unitStatsSettings;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Color unitColor;


        [Header("Spawner Settings")]
        [SerializeField] private bool gameRunning = true;
        [Space]
        [SerializeField] private float spawnerHealth = 100;
        public float spawnTimer = 15;
        public float time;
        [Space]
        [SerializeField] private Vector3 spawnOffset;
        [Space]
        [SerializeField] private int unitsPerWave = 10;
        [Space]
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask aiLayer;
        [Space]
        [SerializeField] private Steering.SteeringSettings aiSteeringSettings;
        [SerializeField] private Steering.SteeringSettings playerSteeringSettings;

        [Header("EnemySpawner Settings")]
        [SerializeField] private bool isAISpawner;
        [SerializeField] private List<StatsScriptableObject> enemyUnitPresets;

        [Header("Private")]
        private List<GameObject> unitList = new List<GameObject>();

        private int attackDamage;
        private int attackSpeed;
        private int defense;
        private int movementSpeed;
        private int viewingRange;

        private void Start()
        {
            if (isAISpawner == true && enemyUnitPresets == null)
                Debug.LogWarning("stat preset list is empty");
        }

        public void Update()
        {
            if (spawnerHealth <= 0)
                Death();

            Timer();
        }


        /// <summary>
        /// spawns units
        /// </summary>
        public void SpawnUnit()
        {
            //takes all stats from the ScriptableObject and saves it in private variables
            attackDamage = unitStatsSettings.attackDamage;
            attackSpeed = unitStatsSettings.attackSpeed;
            defense = unitStatsSettings.defense;
            movementSpeed = unitStatsSettings.movementSpeed;
            viewingRange = unitStatsSettings.viewingRange;

            //repeats the unit spawing proces
            for (int i = 0; i < unitsPerWave; i++)
            {
                GameObject unit = Instantiate(unitPrefab, transform.position + spawnOffset, Quaternion.identity); //Instantiates the unit prefab

                UnitBrain unitBrain = unit.GetComponent<UnitBrain>();
                Health unitHealth = unit.GetComponent<Health>();

                if (isAISpawner == true) // if the unit belongs to the AI
                {
                    unitBrain.isPlayerUnit = false;
                    unitBrain.aiLayer = playerLayer;
                    unitBrain.allyLayer = aiLayer;
                    unitHealth.unitSpawner = this;

                    unit.layer = LayerMask.NameToLayer("AIUnit");
                    unit.tag = "Enemy";

                    unit.transform.parent = gameObject.transform;

                    Steering.Steering st = unit.GetComponent<Steering.Steering>();
                    st.m_settings = aiSteeringSettings; //gives steering settings to the unit

                    RandomStats(); //select random preset
                    unitHealth.SetMaxHealth(defense); // set max health based on defense stat
                    //gives all variables to the unit brain script
                    unitBrain.Initialize(movementSpeed, attackSpeed, attackDamage, viewingRange, defense, unitStatsSettings.behaviourState);
                }
                else // if the unit belongs to the player
                {
                    unitBrain.isPlayerUnit = true;
                    unitBrain.aiLayer = aiLayer;
                    unitBrain.allyLayer = playerLayer;
                    unitHealth.unitSpawner = this;

                    unit.layer = LayerMask.NameToLayer("PlayerUnit");
                    unit.tag = "Player";

                    unit.transform.parent = gameObject.transform;

                    Steering.Steering st = unit.GetComponent<Steering.Steering>();
                    st.m_settings = playerSteeringSettings; //gives steering settings to the unit

                    AllStatPointsUsed(); //check if all point are used by the player
                    unitHealth.SetMaxHealth(defense); // set max health based on defense stat
                    //gives all variables to the unit brain script
                    unitBrain.Initialize(movementSpeed, attackSpeed, attackDamage, viewingRange, defense, unitStatsSettings.behaviourState);
                }

                unitList.Add(unit.gameObject); //adds unit to the spawners list
                unit.GetComponent<MeshRenderer>().material.color = unitColor; //chanches the unit color 
            }
        }

        /// <summary>
        /// selects a random stat preset for the enemy units
        /// </summary>
        public void RandomStats()
        {
            if (isAISpawner == true)
            {
                int random = Random.Range(0,enemyUnitPresets.Count - 1);
                unitStatsSettings = enemyUnitPresets[random];
            }
        }

        //===========================================================================================================
        //                                              Made by elmar
        //===========================================================================================================

        /// <summary>
        /// checks if all points are used and if not it wil divide the remaining points over all stats
        /// </summary>
        public void AllStatPointsUsed()
        {
            float totStatsCount = 0;

            totStatsCount += attackDamage;
            totStatsCount += attackSpeed;
            totStatsCount += defense;
            totStatsCount += movementSpeed;
            totStatsCount += viewingRange;

            if (totStatsCount != 150)
            {
                int remainder = Mathf.RoundToInt((150 - totStatsCount) / 5);
                attackDamage += remainder;
                attackSpeed += remainder;
                defense += remainder;
                movementSpeed += remainder;
                viewingRange += remainder;
            }
        }

        //===========================================================================================================
        //                                              Made by elmar ends
        //===========================================================================================================


        /// <summary>
        /// actives the death or victory function in the gamemanager depenting on if it is the players spawner or the AI's spawner
        /// </summary>
        public void Death()
        {
            Debug.Log(gameObject.name + " is dead!");

            if (isAISpawner == true)
            {
                DestoyAllUnits();
                GameManager.instance.Victory();
            }
            else
            {
                DestoyAllUnits();
                GameManager.instance.PlayerLost();
            }
        }

        /// <summary>
        /// destroys all the spawners Units
        /// </summary>
        public void DestoyAllUnits()
        {
            for (int i = 0; i < unitList.Count; i++)
            {
                Destroy(unitList[i]);
            }
        }


        /// <summary>
        /// timer between waves
        /// </summary>
        private void Timer()
        {
            if (gameRunning == true)
            {

                time = time + Time.deltaTime;

                if (time >= spawnTimer)
                {
                    time = 0;
                    SpawnUnit();
                }
            }
        }

        /// <summary>
        /// chanches the spawners health
        /// </summary>
        /// <param name="change"></param>
        public void ChangeHealth(int change)
        {
            spawnerHealth += change;
        }

        /// <summary>
        /// removes the unit from the list
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveUnitFromList(GameObject unit)
        {
            unitList.Remove(unit);
        }

        private void OnDrawGizmos()
        {
            if (isAISpawner == true)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.blue;

            Gizmos.DrawSphere(transform.position + spawnOffset, 0.3f);
        }
    }
}