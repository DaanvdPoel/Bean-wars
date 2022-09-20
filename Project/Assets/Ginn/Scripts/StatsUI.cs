using UnityEngine;
using TMPro;


    public class StatsUI : MonoBehaviour
    {
        //reference to scriptable object for stats
        public StatsScriptableObject stats;

        //reference to text components for stats
        public TextMeshProUGUI movementSpeedText;
        public TextMeshProUGUI attackSpeedText;
        public TextMeshProUGUI attackDamageText;
        public TextMeshProUGUI viewingRangeText;
        public TextMeshProUGUI defenseText;
        public TextMeshProUGUI remainingPointsText;

        //menu's for both stats and behaviours
        public GameObject behaviourMenu;
        public GameObject statMenu;


        // Start is called before the first frame update
        void Start()
        {
            // sets basic stats and the stat menu active
            stats.movementSpeed = 10;
            stats.attackDamage = 10;
            stats.attackSpeed = 10;
            stats.viewingRange = 10;
            stats.defense = 20;
            stats.remainingPoints = 90;

            statMenu.SetActive(true);
            behaviourMenu.SetActive(false);
        }

        private void FixedUpdate()
        {
            //shows and changes the text/values when needed

            movementSpeedText.text = "Movement: " + stats.movementSpeed.ToString();
            attackSpeedText.text = "Attack speed:" + stats.attackSpeed.ToString();
            attackDamageText.text = "Attack damage: " + stats.attackDamage.ToString();
            viewingRangeText.text = "Viewing range: " + stats.viewingRange.ToString();
            defenseText.text = "Defense:" + stats.defense.ToString();

            remainingPointsText.text = "Remaining points: " + stats.remainingPoints.ToString();
        }

        //disables and enables both menu's when needed

        public void BehaviourMenuEnable()
        {
            statMenu.SetActive(false);
            behaviourMenu.SetActive(true);
        }

        public void StatMenuEnable()
        {
            behaviourMenu.SetActive(false);
            statMenu.SetActive(true);
        }

        //for each function below this comment the value of a stat will increase based on the + button for the stat
        public void IncreaseMovementSpeed()
        {
            if (stats.remainingPoints > 0)
            {
                stats.movementSpeed = stats.movementSpeed + 10;
                stats.remainingPoints = stats.remainingPoints - 10;

                if (stats.movementSpeed > 100)
                {
                    stats.movementSpeed = 100;
                    stats.remainingPoints = stats.remainingPoints + 10;
                }
            }
        }
        public void IncreaseValueAttackSpeed()
        {
            if (stats.remainingPoints > 0)
            {
                stats.attackSpeed = stats.attackSpeed + 10;
                stats.remainingPoints = stats.remainingPoints - 10;

                if (stats.attackSpeed > 100)
                {
                    stats.attackSpeed = 100;
                    stats.remainingPoints = stats.remainingPoints + 10;
                }
            }
        }
        public void IncreaseValueAttackDamage()
        {
            if (stats.remainingPoints > 0)
            {
                stats.attackDamage = stats.attackDamage + 10;
                stats.remainingPoints = stats.remainingPoints - 10;

                if (stats.attackDamage > 100)
                {
                    stats.attackDamage = 100;
                    stats.remainingPoints = stats.remainingPoints + 10;
                }
            }
        }
        public void IncreaseValueViewingRange()
        {
            if (stats.remainingPoints > 0)
            {
                stats.viewingRange = stats.viewingRange + 10;
                stats.remainingPoints = stats.remainingPoints - 10;

                if (stats.viewingRange > 100)
                {
                    stats.viewingRange = 100;
                    stats.remainingPoints = stats.remainingPoints + 10;
                }
            }
        }
        public void IncreaseValueDefense()
        {
            if (stats.remainingPoints > 0)
            {
                stats.defense = stats.defense + 10;
                stats.remainingPoints = stats.remainingPoints - 10;

                if (stats.defense > 100)
                {
                    stats.defense = 100;
                    stats.remainingPoints = stats.remainingPoints + 10;
                }
            }
        }

        //for each function below this comment the value of a stat will decrease based on the - button for the stat

    public void DecreaseMovementSpeed()
        {
            if (stats.remainingPoints <= 100)
            {
                stats.movementSpeed = stats.movementSpeed - 10;
                stats.remainingPoints = stats.remainingPoints + 10;
                if (stats.movementSpeed < 10)
                {
                    stats.movementSpeed = 10;
                    stats.remainingPoints = stats.remainingPoints - 10;
                }
            }
        }
        public void DecreaseValueAttackSpeed()
        {
            if (stats.remainingPoints <= 100)
            {
                stats.attackSpeed = stats.attackSpeed - 10;
                stats.remainingPoints = stats.remainingPoints + 10;
                if (stats.attackSpeed < 10)
                {
                    stats.attackSpeed = 10;
                    stats.remainingPoints = stats.remainingPoints - 10;
                }
            }
        }
        public void DecreaseValueAttackDamage()
        {
            if (stats.remainingPoints <= 100)
            {
                stats.attackDamage = stats.attackDamage - 10;
                stats.remainingPoints = stats.remainingPoints + 10;
                if (stats.attackDamage < 10)
                {
                    stats.attackDamage = 10;
                    stats.remainingPoints = stats.remainingPoints - 10;
                }
            }
        }
        public void DecreaseValueViewingRange()
        {
            if (stats.remainingPoints <= 100)
            {
                stats.viewingRange = stats.viewingRange - 10;
                stats.remainingPoints = stats.remainingPoints + 10;
                if (stats.viewingRange < 10)
                {
                    stats.viewingRange = 10;
                    stats.remainingPoints = stats.remainingPoints - 10;
                }
            }
        }
        public void DecreaseValueDefense()
        {
            if (stats.remainingPoints <= 100)
            {
                stats.defense = stats.defense - 10;
                stats.remainingPoints = stats.remainingPoints + 10;
                if (stats.defense < 20)
                {
                    stats.defense = 20;
                    stats.remainingPoints = stats.remainingPoints - 10;
                }
            }
        }
        
        // for each function below this comment changes the beahaviour state for the corresponding button
        public void AggressiveBehaviour()
        {
            stats.behaviourState = StatsScriptableObject.BehaviourList.Aggressive;
        }

        public void DefensiveBehaviour()
        {
            stats.behaviourState = StatsScriptableObject.BehaviourList.Defensive;
        }

        public void LoyalBehaviour()
        {
            stats.behaviourState = StatsScriptableObject.BehaviourList.Loyal;
        }

        public void WandererBehaviour()
        {
            stats.behaviourState = StatsScriptableObject.BehaviourList.Wanderer;
        }

        public void GuardPathABehaviour()
        {
            stats.behaviourState = StatsScriptableObject.BehaviourList.GuardPathA;
        }

        public void GuardPathBBehaviour()
        {
            stats.behaviourState = StatsScriptableObject.BehaviourList.GuardPathB;
        }

        public void GuardPathCBehaviour()
        {
            stats.behaviourState = StatsScriptableObject.BehaviourList.GuardPathC;
        }
    }

