using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.Spawners.Components;
using JetBrains.Annotations;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared.Station.Systems;

/// <summary>
/// Ordered broadcast event fired on any spawner eligible to attempt to spawn a player.
/// </summary>
[PublicAPI]
public sealed class PlayerSpawningEvent : EntityEventArgs
{
    public EntityUid? SpawnResult;
    public readonly ProtoId<JobPrototype>? Job;
    public readonly HumanoidCharacterProfile? HumanoidCharacterProfile;
    public readonly EntityUid? Station;
    public readonly SpawnPointType DesiredSpawnPointType;
    public readonly ICommonSession? Session;

    public PlayerSpawningEvent(
        ProtoId<JobPrototype>? job,
        HumanoidCharacterProfile? humanoidCharacterProfile,
        EntityUid? station,
        SpawnPointType spawnPointType = SpawnPointType.Unset,
        ICommonSession? session = null)
    {
        Job = job;
        HumanoidCharacterProfile = humanoidCharacterProfile;
        Station = station;
        DesiredSpawnPointType = spawnPointType;
        Session = session;
    }
}
