using System.Numerics;
using Space.Invaders;
using Titan.Events;


namespace Space.Events;

internal struct GameStartEvent : IEvent { }
internal struct GameEndedEvent : IEvent { }
internal struct LevelCompletedEvent : IEvent { }
internal struct PlayerHitEvent : IEvent
{
    public Vector2 Position;
    public bool CollisionWithInvader;
}
internal struct PlayerRespawnEvent : IEvent { }

internal struct InvaderDestroyedEvent : IEvent
{
    public InvaderType Type;
    public Vector2 Position;
}
