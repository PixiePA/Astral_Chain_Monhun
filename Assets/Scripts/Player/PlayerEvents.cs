using UnityEngine;
using System;

public static class PlayerEvents
{

    // Updates if player is alllowed to move
    public static Action<bool, Animator> onChangeCanMove;

    public static void ChangeCanMove(bool canMove, Animator animator)
    {
        onChangeCanMove?.Invoke(canMove, animator);
    }

    // Updates if player can turn
    public static Action<bool, Animator> onChangeCanTurn;

    public static void ChangeCanTurn(bool canTurn, Animator animator)
    {
        onChangeCanTurn?.Invoke(canTurn, animator);
    }

    // Updates state player is in
    public static Action<string, Animator> onChangeState;

    public static void ChangeState(string state, Animator animator)
    {
        onChangeState?.Invoke(state, animator);
    }

    // Resets player attack inputs
    public static Action<Animator> onResetAttackInputs;

    public static void ResetAttackInputs(Animator animator)
    {
        onResetAttackInputs?.Invoke(animator);
    }

    // When player takes damage
    public static Action<float> onChangeHealth;

    public static void ChangeHealth(float health)
    {
        onChangeHealth?.Invoke(health);
    }
}
