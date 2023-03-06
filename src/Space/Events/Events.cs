using Titan.Events;

namespace Space.Events;

internal struct GameStartEvent : IEvent { }
internal struct GameEndedEvent : IEvent { }
internal struct PlayerHitEvent : IEvent { }
