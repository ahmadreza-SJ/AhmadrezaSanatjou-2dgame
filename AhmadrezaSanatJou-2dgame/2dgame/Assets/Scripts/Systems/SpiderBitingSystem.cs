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
using Unity.Rendering;

public class SpiderBitingSystem : JobComponentSystem
{
    private EntityCommandBufferSystem m_Barrier;
    private NativeArray<Translation> CooperPosition;
    private EntityQuery m_Group;


    protected override void OnCreate()
    {

        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_Group = GetEntityQuery(typeof(CooperTag), ComponentType.ReadOnly<Translation>());
    }

    [BurstCompile]
    struct ShotCollidingJob : IJobForEachWithEntity<Translation, SpiderTag, Level2Tag>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public float CooperColliderRad;
        public float SpiderColliderRad;

        public float3 CooperPos;

        public void Execute(Entity entity, int index, ref Translation position, [ReadOnly] ref SpiderTag Tag, [ReadOnly] ref Level2Tag lvlTag)
        {
            double dist = Math.Sqrt(Math.Pow(CooperPos.x - position.Value.x, 2) + Math.Pow(CooperPos.y - position.Value.y, 2));
            if (dist < CooperColliderRad + SpiderColliderRad)
            {
                CommandBuffer.DestroyEntity(index, entity);
                GameDetails.GD.CurrentHealth = 0;
            }

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        if(CooperGamePlayController.Controller == null || SpiderGamePlayController.Controller == null)
        {
            return new JobHandle();
        }

        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();
        

        CooperPosition = m_Group.ToComponentDataArray<Translation>(Allocator.Persistent);
        var job = new ShotCollidingJob
        {
            CooperColliderRad = CooperGamePlayController.Controller.ColliderRadius,
            SpiderColliderRad = SpiderGamePlayController.Controller.ColliderRadius,
            CooperPos = CooperPosition[0].Value,
            CommandBuffer = commandBuffer,
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);
        CooperPosition.Dispose();

        return job;
    }
}
