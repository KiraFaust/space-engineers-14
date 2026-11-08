using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Content.Shared.Shuttles.Components;
using Content.Shared.Spawners.Components;
using JetBrains.Annotations;
using Robust.Shared.Configuration;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared.Station.Systems;

namespace Content.Shared.Shuttles.Systems;

public abstract partial class SharedArrivalsSystem : EntitySystem
{
    [Dependency] protected IConfigurationManager Cfg = default!;
    [Dependency] protected IGameTiming Timing = default!;
    [Dependency] protected IRobustRandom Random = default!;

    public bool Enabled { get; protected set; }
    public bool ArrivalsGodmode { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ArrivalsShuttleComponent, ComponentStartup>(OnShuttleStartup);
        Enabled = Cfg.GetCVar(CCVars.ArrivalsShuttles);
        ArrivalsGodmode = Cfg.GetCVar(CCVars.GodmodeArrivals);
        Cfg.OnValueChanged(CCVars.GodmodeArrivals, b => ArrivalsGodmode = b);
    }

    private void OnShuttleStartup(EntityUid uid, ArrivalsShuttleComponent c, ComponentStartup args)
        => EnsureComp<PreventPilotComponent>(uid);

    [PublicAPI]
    public bool TryGetArrivals(out EntityUid uid)
    {
        var q = EntityQueryEnumerator<ArrivalsSourceComponent>();
        while (q.MoveNext(out uid, out _))
            return true;
        return false;
    }

    [PublicAPI]
    public bool IsOnArrivals(Entity<TransformComponent?> entity)
    {
        if (!Resolve(entity, ref entity.Comp)) return false;
        if (!TryGetArrivals(out var arrivals)) return false;
        var grid = Transform(arrivals).GridUid;
        return grid.HasValue && entity.Comp.GridUid == grid;
    }

    public TimeSpan? NextShuttleArrival()
    {
        var q = EntityQueryEnumerator<ArrivalsShuttleComponent>();
        var time = TimeSpan.MaxValue;
        while (q.MoveNext(out _, out var comp))
            if (comp.NextArrivalsTime < time)
                time = comp.NextArrivalsTime;
        var duration = Timing.CurTime;
        return (time < duration) ? null : time - duration;
    }
}
