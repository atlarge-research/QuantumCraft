using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum {
  partial class RuntimePlayer {

        // This is needed to find the entity when spawning. Need to look more deeply into what and why
        // it needs to be here
        public AssetRefEntityPrototype CharacterPrototype;
        partial void SerializeUserData(BitStream stream)
        {
          // implementation
          stream.Serialize(ref CharacterPrototype.Id);
        }
  }
}
