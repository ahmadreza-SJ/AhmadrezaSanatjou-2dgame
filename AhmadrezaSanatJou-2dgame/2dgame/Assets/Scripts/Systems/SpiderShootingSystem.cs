using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

public class SpiderShootingSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct SpiderShootingJob : IJobForEachWithEntity<Translation, SpiderTag, LoadedShotCount>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public float DeltaTime;
        public float RandomPicker;

        public void Execute(Entity entity, int jobIndex, ref Translation position, [ReadOnly] ref SpiderTag tag,
           ref LoadedShotCount loadedShots)
        {
            if(loadedShots.TimeToNextShoot > 0)
            {
                loadedShots.TimeToNextShoot -= DeltaTime;
            }
            else
            {
                if(RandomPicker <= SpiderGamePlayController.Controller.ShootingChance)
                {
                    Entity shot = CommandBuffer.CreateEntity(jobIndex);

                    CommandBuffer.AddComponent(jobIndex, shot, new Translation
                    {
                        Value = new float3(position.Value.x, position.Value.y - 0.2f, position.Value.z)
                    });
                    CommandBuffer.AddComponent(jobIndex, shot, new ShotTag { Character = ChTags.Spider });
                    CommandBuffer.AddComponent(jobIndex, shot, new Scale { Value = 0.1f });
                    CommandBuffer.AddComponent(jobIndex, shot, new LocalToWorld { });
                    CommandBuffer.AddSharedComponent(jobIndex, shot, new RenderMesh
                    {
                        mesh = GameDetails.GD.Mesh,
                        material = GameDetails.GD.SpiderShotMaterial,
                    });
                }
                loadedShots.TimeToNextShoot = SpiderGamePlayController.Controller.ShootingDelay;
            }
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

        


        var job = new SpiderShootingJob
        {
            RandomPicker = UnityEngine.Random.Range(0, 100),
            CommandBuffer = commandBuffer,
            DeltaTime = Time.deltaTime,
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}
