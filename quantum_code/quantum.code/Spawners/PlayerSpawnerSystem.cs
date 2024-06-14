using Photon.Deterministic;

namespace Quantum
{
    public unsafe class PlayerSpawnerSystem : 
        SystemMainThreadFilter<PlayerSpawnerSystem.Filter>, 
        ISignalOnPlayerDataSet
    {

        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* CharacterController;
            public Transform3D* Transform;
            public PlayerLink* Link;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            return;
        }

        /*
         When creating a player entity from the prefab, it has entityref and a character controller,
        but we need to link it to the local player and assign a position on the map.
         */
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            // Get the data of the player
            var data = f.GetPlayerData(player);

            // Get the prefab of the player controller
            var prototypeEntity = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);

            // Create the entity based on the prototype
            var createdPlayerEntity = f.Create(prototypeEntity);


            // Try to get the playerlink pointer of the created entity, and assign it to the local player
            if (f.Unsafe.TryGetPointer<PlayerLink>(createdPlayerEntity, out var playerLink))
            {
                playerLink->Player = player;
            }


            // Try to get the transform pointer of the created entity, and assign it a position
            if (f.Unsafe.TryGetPointer<Transform3D>(createdPlayerEntity, out var transform))
            {
                
                transform->Position = GetSpawnPosition(player);
            }
        }

        private FPVector3 GetSpawnPosition(int playerNumber)
        {
            // Some kind of spawning system for the player.
            // Can be anything as long as it isn't random between clients
            return new FPVector3(playerNumber+20, 0, 32);
        }
    }
}
