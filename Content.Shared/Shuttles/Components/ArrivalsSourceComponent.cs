using Content.Shared.Shuttles.Systems;

namespace Content.Shared.Shuttles.Components;

/// <summary>
/// Added to a designated arrivals station for players to spawn at, if enabled.
/// </summary>
[RegisterComponent, Access(typeof(SharedArrivalsSystem))]
public sealed partial class ArrivalsSourceComponent : Component;
