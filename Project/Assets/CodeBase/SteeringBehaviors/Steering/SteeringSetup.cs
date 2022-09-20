using UnityEngine;

namespace Steering
{
    public class SteeringSetup : Generic.Singleton<SteeringSetup>
    {
        [Header("Steering Setup")]
        public SteeringStepSettings m_stepSettings = new SteeringStepSettings()
        {
            m_location = SteeringStepSettings.UpdateMethod.Update,
        };
    }
}
