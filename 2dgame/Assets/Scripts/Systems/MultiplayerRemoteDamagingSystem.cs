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

public class MultiplayerRemoteDamagingSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    

    struct RemoteDamageJob : IJobForEachWithEntity<Translation, ShotTag>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public ChTags RemoteClientChar;
        public Vector3 RemotePos;
        public void Execute(Entity entity, int index, ref Translation position, [ReadOnly] ref ShotTag shotTag)
        {
            if(ClientAvatarsStates.State.RemoteIsDamaged &&
                (position.Value.y < RemotePos.y + CooperGamePlayController.Controller.ColliderRadius + 0.2
                && position.Value.y > RemotePos.y - CooperGamePlayController.Controller.ColliderRadius - 0.2))
            {
                if(RemoteClientChar == ChTags.Cooper)
                {
                    CooperGamePlayController.Controller.Heart--;
                }
                else
                {
                    SpiderGamePlayController.Controller.Heart--;
                }
                CommandBuffer.DestroyEntity(index, entity);
                ClientAvatarsStates.State.RemoteIsDamaged = false;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        JobHandle job = new JobHandle();

        if (ClientAvatarsStates.State == null || ClientAvatarsStates.State.RemoteClientAvatar == null)
        {
            return job;
        }

        ChTags charTag = ChTags.Cooper;

        if(ClientAvatarsStates.State.LocalClientAvatar.name == "CooperAvatar")
        {
            charTag = ChTags.Spider;
        }

        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

        job = new RemoteDamageJob
        {
            RemoteClientChar =charTag ,
            RemotePos = ClientAvatarsStates.State.RemoteClientAvatar.transform.position,
            CommandBuffer = commandBuffer,
        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);
        return job;
    }
}
