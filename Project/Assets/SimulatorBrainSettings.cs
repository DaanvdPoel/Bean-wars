using UnityEngine;

namespace Steering
{
    [CreateAssetMenu(fileName = "SimulatorBrainSettings", menuName = "Steering/ SimulatorBrainSettings", order = 3)]
    public class SimulatorBrainSettings : ScriptableObject
    {
        [Header("Settings")]
        public float size = 0.5f; // the start size of the cube
        public float FOV = 10;      //field of view
        public float attackRange = 0.5f; // the range where in a cube can attack
        public float foodQuality = 0.5f; // the amount of growth 1 food cube gives

        public LayerMask FOVLayer; //the layer where the food and other cubes are on

    }
}

