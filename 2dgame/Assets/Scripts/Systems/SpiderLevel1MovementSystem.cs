using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpiderLevel1MovementSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    }

    [BurstCompile]
    struct SpiderLevel1MovementJob : IJobForEach<Translation, SpiderTag, Level1Tag>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public float DeltaTime;
        public float HorizontalMove;


        public void Execute(ref Translation position, [ReadOnly] ref SpiderTag tag, [ReadOnly] ref Level1Tag levelTag)
        {
            if ((position.Value.x > GameDetails.GD.CameraHalfWidth - GameDetails.GD.HorizontalBoundary 
                && SpiderGamePlayController.Controller.HorizontalSpeed > 0)
                || (position.Value.x < -GameDetails.GD.CameraHalfWidth + GameDetails.GD.HorizontalBoundary
                && SpiderGamePlayController.Controller.HorizontalSpeed < 0))
            {
                SpiderGamePlayController.Controller.HorizontalSpeed *= -1;
            }
           
            position = new Translation
            {
                Value = new float3(position.Value.x + HorizontalMove* DeltaTime, position.Value.y, position.Value.z)
            };
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();



        var job = new SpiderLevel1MovementJob
        {
            DeltaTime = Time.deltaTime,
            HorizontalMove = SpiderGamePlayController.Controller.HorizontalSpeed
        }.Schedule(this, inputDependencies);


        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}
