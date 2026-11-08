using Content.Shared.Station;

namespace Content.Shared.Station.Components;

/// <summary>
/// Controls spawning on the given station, tracking spawners present on it.
/// </summary>
[RegisterComponent, Access(typeof(SharedStationSpawningSystem))]
public sealed partial class StationSpawningComponent : Component;
