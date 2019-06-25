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

public class MultiPlayerShotCollidingSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;
    private EntityQuery m_Group;
    NativeArray<Translation> ShotPositions;
    NativeArray<Entity> Shots;
    NativeArray<ShotTag> Tags;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_Group = GetEntityQuery(typeof(ShotTag), ComponentType.ReadOnly<Translation>());
    }

    [BurstCompile]
    struct MPShotCollidingJob : IJobForEachWithEntity<Translation, CharacterTag, Heart, PhotonViewData>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public float3 ShotPos;
        public int ShotIndex;
        public ChTags shotTag;

        public void Execute(Entity entity, int index, ref Translation position, [ReadOnly] ref CharacterTag Tag, ref Heart heart,
          [ReadOnly]  ref PhotonViewData PV)
        {
            if (PV.isMine)
            {
                if (Tag.Value == ChTags.Spider && shotTag == ChTags.Cooper)
                {
                    double dist = Math.Sqrt(Math.Pow(ShotPos.x - position.Value.x, 2) + Math.Pow(ShotPos.y - position.Value.y, 2));
                    if (dist < SpiderGamePlayController.Controller.ColliderRadius)
                    {
                        SpiderGamePlayController.Controller.Heart--;
                        GameDetails.GD.DestroyedShotsIndexes.Add(ShotIndex);

                        ClientAvatarsStates.State.LocalIsDamaged = true;
                    }

                }
                else if (Tag.Value == ChTags.Cooper && shotTag == ChTags.Spider)
                {
                    double dist = Math.Sqrt(Math.Pow(ShotPos.x - position.Value.x, 2) + Math.Pow(ShotPos.y - position.Value.y, 2));
                    if (dist < CooperGamePlayController.Controller.ColliderRadius)
                    {
                        CooperGamePlayController.Controller.Heart--;
                        GameDetails.GD.DestroyedShotsIndexes.Add(ShotIndex);

                        ClientAvatarsStates.State.LocalIsDamaged = true;
                        
                    }

                }
            }
        }
    }


 

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {

        JobHandle job = new JobHandle();

        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();


        ShotPositions = m_Group.ToComponentDataArray<Translation>(Allocator.Persistent);
        Tags = m_Group.ToComponentDataArray<ShotTag>(Allocator.Persistent);
        Shots = m_Group.ToEntityArray(Allocator.Persistent);
        for (int i = 0; i < ShotPositions.Length; i++)
        {
            job.Complete();

            job = new MPShotCollidingJob
            {
                shotTag = Tags[i].Character,
                ShotIndex = Shots[i].Index,
                ShotPos = ShotPositions[i].Value,
                CommandBuffer = commandBuffer,
            }.Schedule(this, inputDependencies);

            m_Barrier.AddJobHandleForProducer(job);
        }

        Shots.Dispose();
        ShotPositions.Dispose();
        Tags.Dispose();
        return job;
    }

}
