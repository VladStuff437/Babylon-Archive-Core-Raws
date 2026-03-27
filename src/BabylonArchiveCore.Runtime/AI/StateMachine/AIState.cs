namespace BabylonArchiveCore.Runtime.AI.StateMachine;

/// <summary>
/// Состояния AI-стейт-машины.
/// </summary>
public enum AIState
{
    Idle,
    Alert,
    Investigate,
    Patrol,
    Chase,
    Attack,
    Flee,
    ReturnToPost,
    Dead
}
