using System.Diagnostics;
using CyberBlood.Scenes.Entities;
using Godot;

namespace CyberBlood.addons.playable_spatial;

[Tool]
public class PlayableSpatial : Spatial {
    [Export] public NodePath SpawnPointPath { get; set; }
    public Position3D SpawnPoint { get; private set; }

    public override void _Ready() {
        Debug.Assert(!SpawnPointPath.IsEmpty());

        SpawnPoint = GetNode<Position3D>(SpawnPointPath);

        var player = GetNode<Player>("Player");
        player.SetSpawn(SpawnPoint);
    }
}
