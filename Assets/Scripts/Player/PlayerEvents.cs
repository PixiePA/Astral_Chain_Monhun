using UnityEngine;
using System;

public static class PlayerEvents
{

    // Updates if player is alllowed to move
    public static Action<bool> onChangeCanMove;

    public static void ChangeCanMove(bool canMove)
    {
        onChangeCanMove?.Invoke(canMove);
    }

    // Updates if player can turn
    public static Action<bool> onChangeCanTurn;

    public static void ChangeCanTurn(bool canTurn)
    {
        onChangeCanTurn?.Invoke(canTurn);
    }

    // Updates state player is in
    public static Action<string> onChangeState;

    public static void ChangeState(string state)
    {
        onChangeState?.Invoke(state);
    }

    // Resets player attack inputs
    public static Action onResetAttackInputs;

    public static void ResetAttackInputs()
    {
        onResetAttackInputs?.Invoke();
    }

    // When player takes damage
    public static Action<float> onHealthChanged;

    public static void HealthChanged(float health)
    {
        onHealthChanged?.Invoke(health);
    }
}
