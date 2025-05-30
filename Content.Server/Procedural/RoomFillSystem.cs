using Robust.Shared.Map.Components;

namespace Content.Server.Procedural;

public sealed class RoomFillSystem : EntitySystem
{
    [Dependency] private readonly DungeonSystem _dungeon = default!;
    [Dependency] private readonly SharedMapSystem _maps = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RoomFillComponent, MapInitEvent>(OnRoomFillMapInit);
    }

    private void OnRoomFillMapInit(EntityUid uid, RoomFillComponent component, MapInitEvent args)
    {
        var xform = Transform(uid);

        if (xform.GridUid != null)
        {
            var random = new Random();
            var room = _dungeon.GetRoomPrototype(random, component.RoomWhitelist, component.MinSize, component.MaxSize);

            if (room != null)
            {
                var mapGrid = Comp<MapGridComponent>(xform.GridUid.Value);
                _dungeon.SpawnRoom(
                    xform.GridUid.Value,
                    mapGrid,
                    _maps.LocalToTile(xform.GridUid.Value, mapGrid, xform.Coordinates), // WD EDIT
                    room,
                    random,
                    // WD EDIT START
                    null,
                    xform.LocalRotation,
                    component.ClearExisting,
                    component.Rotation);
                    // WD EDIT END
            }
            else
            {
                Log.Error($"Unable to find matching room prototype for {ToPrettyString(uid)}");
            }
        }

        // Final cleanup
        QueueDel(uid);
    }
}
