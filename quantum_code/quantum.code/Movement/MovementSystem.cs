using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* CharacterController;
            public Transform3D* Transform;
            public PitchYaw* PitchYaw;
            public PlayerLink* Link;
        }
        public override void Update(Frame f, ref Filter filter)
        {
            var input = f.GetPlayerInput(filter.Link->Player);

            var inputVector = new FPVector2((FP)input->DirectionX/10, (FP)input->DirectionY/10);

            // math.PI / 2
            //FP temp = FP.FromFloat_UNSAFE(1.57079632679f);

            // Better than math.PI/2, camera can't see inside ground.
            FP temp = FP.FromFloat_UNSAFE(0.9f);

            // input.ValueRW.Pitch = math.clamp(input.ValueRW.Pitch + playerController.inputLook.y, -math.PI / 2,
            // math.PI / 2);
            filter.PitchYaw->Pitch = FPMath.Clamp(filter.PitchYaw->Pitch + input->PitchYaw.Y,-temp, temp);

            // input.ValueRW.Yaw = math.fmod(input.ValueRW.Yaw + playerController.inputLook.x, 2 * math.PI);
            filter.PitchYaw->Yaw = FPMath.ModuloClamped(filter.PitchYaw->Yaw + input->PitchYaw.X, FP.FromFloat_UNSAFE(6.28318530718f));

            filter.Transform->Rotation = FPQuaternion.CreateFromYawPitchRoll(filter.PitchYaw->Yaw, -filter.PitchYaw->Pitch, 0);

            //filter.CharacterController->Move(f, filter.Entity, inputVector.XOY);\
            

            var forwardDirection = filter.Transform->Rotation * FPVector3.Forward;
            forwardDirection.Y = 0; // Project to horizontal plane
            forwardDirection = forwardDirection.Normalized;

            var rightDirection = filter.Transform->Rotation * FPVector3.Right;
            rightDirection.Y = 0;
            rightDirection = rightDirection.Normalized;

            // Map inputVector to absolute directions
            var movementDirection = FPVector3.Zero;
            if (inputVector.Y > 0)  // Forward
                movementDirection += forwardDirection;
            if (inputVector.Y < 0)  // Backward
                movementDirection -= forwardDirection;
            if (inputVector.X > 0)  // Right
                movementDirection += rightDirection;
            if (inputVector.X < 0)  // Left
                movementDirection -= rightDirection;

            // Normalize for consistent movement speed
            movementDirection = movementDirection.Normalized;

            filter.CharacterController->Move(f, filter.Entity, movementDirection);

            if (input->PrimaryAction.WasPressed)
            {
                FPVector3 cameraOffset = FPVector3.Zero;
                cameraOffset.Y = FP.FromFloat_UNSAFE(0.7f);
                Physics3D.Hit3D? RayObj = f.Physics3D.Raycast(filter.Transform->Position+cameraOffset, filter.Transform->Forward, 10);
                EntityRef? hitEntity = RayObj?.Entity;
                if (hitEntity != null)
                {
                    f.Destroy((EntityRef)hitEntity);
                }
            }   
            else if(input->SecondaryAction.WasPressed)
            {
                FPVector3 cameraOffset = FPVector3.Zero;
                cameraOffset.Y = FP.FromFloat_UNSAFE(0.7f);
                Physics3D.Hit3D? RayObj = f.Physics3D.Raycast(filter.Transform->Position + cameraOffset, filter.Transform->Forward, 10);
                EntityRef? hitEntity = RayObj?.Entity;
                if (hitEntity != null)
                {
                    var stoneBlockPrototype = f.FindAsset<EntityPrototype>("Resources/DB/Stone Block|EntityPrototype");
                    var createdBlockEntity = f.Create(stoneBlockPrototype);
                    if (f.Unsafe.TryGetPointer<Transform3D>(createdBlockEntity, out var transform))
                    {
                        
                        // Set position with offset:
                        Log.Info(RayObj?.Normal);
                        FPVector3 tempVec = (FPVector3)RayObj?.Point;
                        tempVec.X = FPMath.Round(tempVec.X);
                        tempVec.Y = FPMath.Round(tempVec.Y);
                        tempVec.Z = FPMath.Round(tempVec.Z);
                        transform->Position = tempVec;
                    }
                }
                else
                {
                    Log.Error("Something went wrong with hit entity");
                }
            }

            if (input->Jump.WasPressed)
            {
                filter.CharacterController->Jump(f);
            }
        }
    }
}
