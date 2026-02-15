using UnityEngine;

namespace StateMachine
{
    public interface IPlayerState
    {
        public void Enter(PlayerStateMachine playerStateMachine,StateContext context);
        public void Update();
        public void Exit();
    }
}