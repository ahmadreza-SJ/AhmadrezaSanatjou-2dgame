using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


//this system casts the remote client information from it's object and debloy them in the related entity
public class InformationCasterSystem : JobComponentSystem
{
    [BurstCompile]
    struct CooperMovementJob : IJobForEach<Translation, CharacterTag, PhotonViewData>
    {

        public float3 Position;
        public void Execute(ref Translation position, [ReadOnly] ref CharacterTag tag, [ReadOnly] ref PhotonViewData PV)
        {


            if (!PV.isMine)
            {
                position.Value = Position;
            }


        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        if (ClientAvatarsStates.State == null || ClientAvatarsStates.State.RemoteClientAvatar == null)
        {
            return new JobHandle();
        }


        var job = new CooperMovementJob
        {

            Position = ClientAvatarsStates.State.RemoteClientAvatar.transform.position
        }.Schedule(this, inputDependencies);



        return job;
    }
}


