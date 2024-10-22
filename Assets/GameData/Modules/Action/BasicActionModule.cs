using DYP;
using DYP.Util;
using UnityEngine;

namespace DYP
{
    [CreateAssetMenuAttribute(fileName = "BasicActionModule", menuName = "ActionModule/Basic")]
    public class BasicActionModule : BaseActionModule
    {


        [SerializeField]
        protected bool m_ResetVelocityY = true;
        public override bool UseGravity { get { return m_ResetVelocityY; } }

        [SerializeField]
        protected bool m_CanUseToSlidingWall = false;
        public override bool CanUseToSlidingWall { get { return m_CanUseToSlidingWall; } }

        [SerializeField]
        protected bool m_CanOnlyBeUsedOnGround = false;
        public override bool CanOnlyBeUsedOnGround { get { return m_CanOnlyBeUsedOnGround; } }

        public override Vector2 GetActionSpeed(float prevTimer, float currTimer)
        {
            // Placeholder implementation, replace with actual logic
            return Vector2.zero;
        }
    }
}