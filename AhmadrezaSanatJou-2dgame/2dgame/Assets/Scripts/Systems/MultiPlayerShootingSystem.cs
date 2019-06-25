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

public class MultiPlayerShootingSystem : JobComponentSystem
{

    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct MultiplayerShootingJob : IJobForEachWithEntity<Translation, CharacterTag, LoadedShotCount, PhotonViewData>
    {

        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public float DeltaTime;
        public bool Clicked;
        public bool RemoteIsShooting;
        public void Execute(Entity entity, int jobIndex, ref Translation position, [ReadOnly] ref CharacterTag tag,
            ref LoadedShotCount loadedShots, ref PhotonViewData PV)
        {
            if (PV.isMine)
            {
                if (loadedShots.Count > 0 && loadedShots.TimeToNextShoot <= 0 && Clicked)
                {

                    Shoot(tag, CommandBuffer, position, jobIndex);
                    loadedShots.Count--;
                    ClientAvatarsStates.State.LocalIsShooting = true;
                    loadedShots.TimeToNextShoot = CooperGamePlayController.Controller.TimeBetweenShoots;
                }
                else if (loadedShots.TimeToNextShoot > 0)
                {
                    loadedShots.TimeToNextShoot -= DeltaTime;
                }

                if (loadedShots.Count == 0)
                {
                    loadedShots.TimeToNextShoot = CooperGamePlayController.Controller.ReloadingTime;
                    loadedShots.Count = CooperGamePlayController.Controller.ShotCountPerLoad;
                }
            }
            else
            {
                if(RemoteIsShooting)
                {
                    Shoot(tag, CommandBuffer, position, jobIndex);
                    loadedShots.Count--;
                    loadedShots.TimeToNextShoot = CooperGamePlayController.Controller.TimeBetweenShoots;
                    ClientAvatarsStates.State.RemoteIsShooting = false;
                }
            }
            
        }
    }

    static void Shoot(CharacterTag tag, EntityCommandBuffer.Concurrent CommandBuffer, Translation position, int jobIndex)
    {

        if (tag.Value == ChTags.Cooper)
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
        }
        else
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
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        if(ClientAvatarsStates.State == null)
        {
            return new JobHandle();
        }

        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();
        bool clicked = false;


        if (Input.GetKey(KeyCode.Mouse0))
        {
            clicked = true;
        }

        var job = new MultiplayerShootingJob
        {
            CommandBuffer = commandBuffer,
            Clicked = clicked,
            RemoteIsShooting = ClientAvatarsStates.State.RemoteIsShooting,
            DeltaTime = Time.deltaTime,
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}
