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

public class CooperShootingSystem : JobComponentSystem
{

    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct CooperShootingJob : IJobForEachWithEntity<Translation, CooperTag, LoadedShotCount, TotalShotCount>
    {

        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public float DeltaTime;
        public bool Clicked;
        public void Execute(Entity entity, int jobIndex, ref Translation position, [ReadOnly] ref CooperTag tag,
            ref LoadedShotCount loadedShots, ref TotalShotCount totalRemainingShots)
        {
            if(totalRemainingShots.Value > 0)
            {
                
                if(loadedShots.Count > 0 && loadedShots.TimeToNextShoot <= 0 && Clicked)
                {
                    Entity shot = CommandBuffer.CreateEntity(jobIndex);
                   
                    CommandBuffer.AddComponent(jobIndex, shot, new Translation
                    {
                        Value = new float3(position.Value.x, position.Value.y + 0.2f, position.Value.z)
                    });
                    CommandBuffer.AddComponent(jobIndex, shot, new ShotTag { Character = ChTags.Cooper });
                    CommandBuffer.AddComponent(jobIndex, shot, new Scale { Value = 0.1f });
                    CommandBuffer.AddComponent(jobIndex, shot, new LocalToWorld { });
                    CommandBuffer.AddSharedComponent(jobIndex, shot, new RenderMesh
                    {
                        mesh = GameDetails.GD.Mesh,
                        material = GameDetails.GD.CooperShotMaterial,
                    });

                    loadedShots.Count--;
                    totalRemainingShots.Value--;
                    GameDetails.GD.CurrentShots = totalRemainingShots.Value;
                    loadedShots.TimeToNextShoot = CooperGamePlayController.Controller.TimeBetweenShoots;
                }
                else if(loadedShots.TimeToNextShoot > 0)
                {
                    loadedShots.TimeToNextShoot -= DeltaTime;
                }

                if(loadedShots.Count == 0)
                {
                    loadedShots.TimeToNextShoot = CooperGamePlayController.Controller.ReloadingTime;
                    loadedShots.Count = CooperGamePlayController.Controller.ShotCountPerLoad;
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();
        bool clicked = false;


        if(Input.GetKey(KeyCode.Mouse0))
        {
            clicked = true;
        }

        var job = new CooperShootingJob
        {
            CommandBuffer = commandBuffer,
            Clicked = clicked,
            DeltaTime = Time.deltaTime,
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}
