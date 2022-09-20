using UnityEngine;

public class UnitStats : MonoBehaviour
    {
    //reference to variables and scriptable object
        public StatsScriptableObject stats;
        public float speed;
        public float attackSpeed;
        public float attackDamage;
        public float viewingRange;
        public float health;

        public float timer;

        private void Start()
        {
            //stats changed accordingly when unit spawns in   
            speed = speed * stats.movementSpeed;
            attackSpeed = attackSpeed * stats.attackSpeed;
            attackDamage = attackDamage * stats.attackDamage;
            viewingRange = viewingRange * stats.viewingRange;
            health = health * stats.defense;
        }
    }
