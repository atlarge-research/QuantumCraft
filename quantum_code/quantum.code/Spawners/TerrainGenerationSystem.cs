using Photon.Deterministic;
using System;

namespace Quantum 
{

    public unsafe class TerrainGenerationSystem : SystemMainThreadFilter<TerrainGenerationSystem.Filter>
    {
        private EntityRef createdBlockEntity;

        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
        }
        public override void OnInit(Frame f)
        {

            Log.Warn("Started initializing terrain");
            var stoneBlockPrototype = f.FindAsset<EntityPrototype>("Resources/DB/Stone Block|EntityPrototype"); // Load from ResourceDB
            var dirtBlockPrototype = f.FindAsset<EntityPrototype>("Resources/DB/Dirt Block|EntityPrototype");
            var grassBlockPrototype = f.FindAsset<EntityPrototype>("Resources/DB/Grass Block|EntityPrototype");

            var height = 2;
            var width = 32;
            var length = 32;

            for (int y = -height; y < 0; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    for (int z = 0; z < width; z++)
                    {

                        if (y == -1) // Top layer (grass)
                        {
                            createdBlockEntity = f.Create(grassBlockPrototype);
                        }
                        else if (y >= -2) // Next layer (dirt)
                        {
                            createdBlockEntity = f.Create(dirtBlockPrototype);
                        }
                        else // Bottom layer (stone) - Not used for now
                        {
                            createdBlockEntity = f.Create(stoneBlockPrototype);
                        }

                        // Positioning the block
                        if (f.Unsafe.TryGetPointer<Transform3D>(createdBlockEntity, out var transform))
                        {
                            transform->Position = new FPVector3(x, y, z);
                        }
                    }
                }
            }
            Console.WriteLine("Terrain generation complete!");


        }

        public override void Update(Frame f, ref Filter filter)
        {
            //ignore
        }
    }
}
