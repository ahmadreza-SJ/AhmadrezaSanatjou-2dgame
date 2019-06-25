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

public class CoinCollidingSystem : JobComponentSystem
{

    private EntityCommandBufferSystem m_Barrier;
    private NativeArray<Translation> CooperPosition;
    private EntityQuery m_Group;
    

    protected override void OnCreate()
    {
        
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_Group = GetEntityQuery(typeof(CooperTag), ComponentType.ReadOnly<Translation>());
    }
    
    struct ShotCollidingJob : IJobForEachWithEntity<Translation, CoinTag>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public float3 CooperPos;
        public float CoinColliderRadius;
        public float CooperColliderRadius;

        public void Execute(Entity entity, int index, ref Translation position, [ReadOnly] ref CoinTag Tag)
        {
            double dist = Math.Sqrt(Math.Pow(CooperPos.x - position.Value.x, 2) + Math.Pow(CooperPos.y - position.Value.y, 2));
            if (dist < CooperColliderRadius + CoinColliderRadius)
            {
                CommandBuffer.DestroyEntity(index, entity);
                GameDetails.GD.Score += GameDetails.GD.EachCoinScoreValue;
            }

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        if(GameDetails.GD == null || CooperGamePlayController.Controller == null)
        {
            return new JobHandle();
        }
        
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();


        CooperPosition = m_Group.ToComponentDataArray<Translation>(Allocator.Persistent);
        var job = new ShotCollidingJob
        {
            CoinColliderRadius = GameDetails.GD.CoinSpriteRadius,
            CooperColliderRadius = CooperGamePlayController.Controller.ColliderRadius,
            CooperPos = CooperPosition[0].Value,
            CommandBuffer = commandBuffer,
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);
        CooperPosition.Dispose();

        return job;
    }
}

