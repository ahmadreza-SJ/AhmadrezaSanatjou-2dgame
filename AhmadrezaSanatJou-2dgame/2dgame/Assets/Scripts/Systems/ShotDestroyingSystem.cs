using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ShotDestroyingSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;


    protected override void OnCreate()
    {

        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();


    }

    [BurstCompile]
    struct ShotDestroyingJob : IJobForEachWithEntity<Translation, ShotTag>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, ref Translation position, [ReadOnly] ref ShotTag spiderTag)
        {
            if (GameDetails.GD.DestroyedShotsIndexes.Contains(entity.Index))
            {
                GameDetails.GD.DestroyedShotsIndexes.Remove(entity.Index);
                CommandBuffer.DestroyEntity(index, entity);
                
            }


        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {

        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

        var job = new ShotDestroyingJob
        {
            CommandBuffer = commandBuffer,
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}
