using UnityEngine;

namespace SimpleBehaviorTree.Examples
{
    public class UnitAnimation : MonoBehaviour
    {
        //reference to animator
        public Animator anim;

        //when called the animation will play
        public void Attack()
        {
            anim.SetTrigger("ATK");
        }
    }
}

