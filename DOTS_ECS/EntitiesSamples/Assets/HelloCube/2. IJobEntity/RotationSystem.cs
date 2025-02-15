using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HelloCube.JobEntity
{
    public partial struct RotationSystem : ISystem
    {
        // Waits for entities with IJobEntity components to be created
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.IJobEntity>();
        }


        // The programmer who made this had a preference to use SystemAPI.Time.DeltaTime instead of the unity DeltaTime
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new RotateAndScaleJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                elapsedTime = (float)SystemAPI.Time.ElapsedTime
            };
            job.Schedule();
        }
    }

    [BurstCompile]
    partial struct RotateAndScaleJob : IJobEntity
    {
        public float deltaTime;
        public float elapsedTime;

        // In source generation, a query is created from the parameters of Execute().
        // Here, the query will match all entities having a LocalTransform, PostTransformMatrix, and RotationSpeed component.
        // (In the scene, the root cube has a non-uniform scale, so it is given a PostTransformMatrix component in baking.)
        
        //Looks for components of transform, PostTrandorm and speed in the entity
        //delta time and elapsed time the things the entity doesn't have so they have to be passed down as parameters
        void Execute(ref LocalTransform transform, ref PostTransformMatrix postTransform, in RotationSpeed speed)
        {
            transform = transform.RotateY(speed.RadiansPerSecond * deltaTime);
            postTransform.Value = float4x4.Scale(1, math.sin(elapsedTime), 1);
        }
    }
}
