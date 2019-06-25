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

public class ShotCollidingSystem : JobComponentSystem
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
   

    struct ShotCollidingJob : IJobForEachWithEntity<Translation, CharacterTag, Heart, SinglePlayerTag>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public float3 ShotPos;
        public int ShotIndex;
        public ChTags shotTag;

        public void Execute(Entity entity, int index, ref Translation position, [ReadOnly] ref CharacterTag Tag, ref Heart heart,
           [ReadOnly] ref SinglePlayerTag spTag)
        {
            if(Tag.Value == ChTags.Spider && shotTag == ChTags.Cooper)
            {
                double dist = Math.Sqrt(Math.Pow(ShotPos.x - position.Value.x, 2) + Math.Pow(ShotPos.y - position.Value.y, 2));
                if (dist < SpiderGamePlayController.Controller.ColliderRadius)
                {
                    heart.Value--;
                    

                    if (heart.Value == 0)
                    {
                        CommandBuffer.DestroyEntity(index, entity);
                        SpawnCoin(position, CommandBuffer, index);
                        GameDetails.GD.KilledSpidersCount++;
                    }
                    GameDetails.GD.DestroyedShotsIndexes.Add(ShotIndex);
                }
            }
            else if(Tag.Value == ChTags.Cooper && shotTag == ChTags.Spider)
            {
                double dist = Math.Sqrt(Math.Pow(ShotPos.x - position.Value.x, 2) + Math.Pow(ShotPos.y - position.Value.y, 2));
                if (dist < CooperGamePlayController.Controller.ColliderRadius)
                {
                    heart.Value--;
                    GameDetails.GD.CurrentHealth--;
                    GameDetails.GD.DestroyedShotsIndexes.Add(ShotIndex);
                }
            }
            
        }
    }


    static void SpawnCoin(Translation position, EntityCommandBuffer.Concurrent commandBuffer, int jobIndex)
    {
        Entity coin = commandBuffer.CreateEntity(jobIndex);
        commandBuffer.AddComponent(jobIndex, coin, new Translation { Value = position.Value });
        commandBuffer.AddComponent(jobIndex, coin, new CoinTag { });
        commandBuffer.AddComponent(jobIndex, coin, new Scale { Value = GameDetails.GD.CoinSpriteRadius});
        commandBuffer.AddComponent(jobIndex, coin, new LocalToWorld { });
        commandBuffer.AddComponent(jobIndex, coin, new SinglePlayerTag { });
        commandBuffer.AddSharedComponent(jobIndex, coin, new RenderMesh
        {
            mesh = GameDetails.GD.Mesh,
            material = GameDetails.GD.CoinMaterial,
        });
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {

        JobHandle job = new JobHandle();

        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();
        
        
        ShotPositions = m_Group.ToComponentDataArray<Translation>(Allocator.Persistent);
        Tags = m_Group.ToComponentDataArray<ShotTag>(Allocator.Persistent);
        Shots = m_Group.ToEntityArray(Allocator.Persistent);
        for(int i = 0; i < ShotPositions.Length; i++)
        {
            job.Complete();

            job = new ShotCollidingJob
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
