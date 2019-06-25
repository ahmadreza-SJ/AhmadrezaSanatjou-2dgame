using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CoinMovementSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    
    struct CoinMovementJob : IJobForEachWithEntity<Translation, CoinTag>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;


        public float DeltaTime;
        public float VerticalSpeed;
        public void Execute(Entity entity, int index, ref Translation position, [ReadOnly] ref CoinTag tag)
        {
            if (position.Value.y < -GameDetails.GD.CameraHalfHeight - 5)
            {
                CommandBuffer.DestroyEntity(index, entity);
            }
            position = new Translation
            {
                Value = new float3(position.Value.x, position.Value.y
                - VerticalSpeed * DeltaTime, position.Value.z)
            };

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        JobHandle job = new JobHandle();

        if(GameDetails.GD == null)
        {
            return new JobHandle();
        }
        
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();
        job = new CoinMovementJob
        {
            CommandBuffer = commandBuffer,
            DeltaTime = Time.deltaTime,
            VerticalSpeed = GameDetails.GD.CoinSpeed
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}


