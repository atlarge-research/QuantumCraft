using Photon.Deterministic;
namespace Quantum
{
    public class SpawnBlockCommand : DeterministicCommand
    {
        public FPVector3 BlockPosition;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref BlockPosition);
        }

        public void Execute(Frame f)
        {
            var stoneBlockPrototype = f.FindAsset<EntityPrototype>("Resources/DB/Stone Block|EntityPrototype");
            var createdBlockEntity = f.Create(stoneBlockPrototype);
            unsafe
            {
                if (f.Unsafe.TryGetPointer<Transform3D>(createdBlockEntity, out var transform))
                {
                    transform->Position = BlockPosition;
                }
            }
        }
    }
}