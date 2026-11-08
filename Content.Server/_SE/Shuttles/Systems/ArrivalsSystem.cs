using Content.Server.GameTicking;
using Content.Server.Spawners.EntitySystems;
using Content.Server.Station.Systems;
using Content.Shared.GameTicking;
using Content.Shared.Movement.Components;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Spawners.Components;
using Content.Shared.Station.Systems;
using Robust.Shared.Map;

namespace Content.Server.Shuttles.Systems;

public sealed partial class ArrivalsSystem : SharedArrivalsSystem
{
    [Dependency] private GameTicker _ticker = default!;
    [Dependency] private StationSpawningSystem _stationSpawning = default!;

    [SubscribeLocalEvent(
        before: new[] { typeof(SpawnPointSystem) },
        after:  new[] { typeof(ContainerSpawnPointSystem) })]
    public void HandlePlayerSpawning(PlayerSpawningEvent ev)
    {
        if (ev.SpawnResult != null) return;
        if (!Enabled || _ticker.RunLevel != GameRunLevel.InRound) return;
        if (!HasComp<StationArrivalsComponent>(ev.Station)) return;
        if (!TryGetArrivals(out var arrivals)) return;
        if (!TryComp(arrivals, out TransformComponent? xform)) return;

        var mapId = xform.MapID;
        var possible = new List<EntityCoordinates>();
        var q = EntityQueryEnumerator<SpawnPointComponent, TransformComponent>();
        while (q.MoveNext(out _, out var sp, out var spXform))
        {
            if (sp.SpawnType != SpawnPointType.LateJoin || spXform.MapID != mapId)
                continue;
            possible.Add(spXform.Coordinates);
        }
        if (possible.Count == 0) return;

        var spawnLoc = Random.Pick(possible);
        ev.SpawnResult = _stationSpawning.SpawnPlayerMob(
            spawnLoc, ev.Job, ev.HumanoidCharacterProfile, ev.Station,
            session: ev.Session);   // ← без session банк не спишет

        EnsureComp<PendingClockInComponent>(ev.SpawnResult.Value);
        EnsureComp<AutoOrientComponent>(ev.SpawnResult.Value);
        if (ArrivalsGodmode)
            EnsureComp<GodmodeComponent>(ev.SpawnResult.Value);
    }
}
