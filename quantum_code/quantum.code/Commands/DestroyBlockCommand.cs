using Photon.Deterministic;
namespace Quantum
{
    public class DestroyBlockCommand : DeterministicCommand
    {
        public FPVector3 BlockPosition;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref BlockPosition);
        }

        public void Execute(Frame f)
        {
        }
    }
}