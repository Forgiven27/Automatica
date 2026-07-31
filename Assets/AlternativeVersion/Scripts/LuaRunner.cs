using MoonSharp.Interpreter;
using LuaCoroutine = MoonSharp.Interpreter.Coroutine;
using UnityEngine;

namespace DullVersion
{
    public class LuaRunner
    {
        private readonly LuaCoroutine _coroutine;
        private double _waitTimer;
        private bool _isDead;

        public bool IsDead => _isDead;

        public LuaRunner(LuaCoroutine coroutine)
        {
            _coroutine = coroutine;
        }

        public void Tick(float dt)
        {
            Debug.Log($"Tick: waitTimer={_waitTimer}, isDead={_isDead}");
            if (_isDead)
                return;

            if (_waitTimer > 0)
            {
                _waitTimer -= dt;
                return;
            }

            DynValue result = _coroutine.Resume();

            if (result.Type == DataType.YieldRequest)
            {
                _waitTimer = result.YieldRequest.ReturnValues[0].Number;
                return;
            }

            if (result.Type == DataType.Void)
            {
                _isDead = true; // coroutine закончилась
            }
        }
    }
}