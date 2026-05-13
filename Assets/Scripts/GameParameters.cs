using UnityEngine;
using UnityEngine.InputSystem;

public static class GameParameters
{
    public static float PlayerSpeed = 5f;

    public static float EnemyMinimumSpawnDelay = 1f;
    public static float EnemyMaximumSpawnDelay = 3f;
    public static float EnemyDetectRange = 5f;
    public static float EnemyAttackRange = 2f;
    public static float EnemyMoveSpeed = 3f;

    public static string EnemyAnimationIdleString = "IsIdle";
    public static string EnemyAnimationChasingString = "IsChasing";
    public static string EnemyAnimationAttackingString = "IsAttacking";

}
