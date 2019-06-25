using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;

public class ShotMovementSystem : JobComponentSystem
{

    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }


    [BurstCompile]
    struct ShotMovementJob : IJobForEachWithEntity<Translation, ShotTag>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public float DeltaTime;
        public float CooperShotVerticalSpeed;
        public float SpiderShotVerticalSpeed;
        public void Execute(Entity entity, int index, ref Translation position, [ReadOnly] ref ShotTag shotTag)
        {
            if (shotTag.Character == ChTags.Cooper)
            {
                position = new Translation
                {
                    Value = new float3(position.Value.x,
                    position.Value.y + CooperShotVerticalSpeed * DeltaTime,
                    position.Value.z)
                };
                if (position.Value.y > GameDetails.GD.CameraHalfHeight + 5)
                {
                    CommandBuffer.DestroyEntity(index, entity);
                }
            }
            else
            {
                position = new Translation
                {
                    Value = new float3(position.Value.x,
                    position.Value.y - SpiderShotVerticalSpeed * DeltaTime,
                    position.Value.z)
                };
                if (position.Value.y < -(GameDetails.GD.CameraHalfHeight + 5))
                {
                    CommandBuffer.DestroyEntity(index, entity);
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        JobHandle job = new JobHandle();

        if (GameDetails.GD == null)
        {
            return job;
        }


        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

         job = new ShotMovementJob
        {
            CooperShotVerticalSpeed = GameDetails.GD.ShotSpeed,
            SpiderShotVerticalSpeed = GameDetails.GD.ShotSpeed,
            CommandBuffer = commandBuffer,
            DeltaTime = Time.deltaTime,
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}
