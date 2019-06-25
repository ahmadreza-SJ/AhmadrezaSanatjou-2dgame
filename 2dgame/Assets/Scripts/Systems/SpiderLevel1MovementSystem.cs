using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpiderLevel1MovementSystem : JobComponentSystem
{
    

    struct SpiderLevel1MovementJob : IJobForEach<Translation, SpiderTag, Level1Tag>
    {
       

        public float DeltaTime;
        public float HorizontalMove;


        public void Execute(ref Translation position, [ReadOnly] ref SpiderTag tag, [ReadOnly] ref Level1Tag levelTag)
        {
            if ((position.Value.x > GameDetails.GD.CameraHalfWidth - GameDetails.GD.HorizontalBoundary 
                && SpiderGamePlayController.Controller.Speed > 0)
                || (position.Value.x < -GameDetails.GD.CameraHalfWidth + GameDetails.GD.HorizontalBoundary
                && SpiderGamePlayController.Controller.Speed < 0))
            {
                SpiderGamePlayController.Controller.Speed *= -1;
            }
           
            position = new Translation
            {
                Value = new float3(position.Value.x + HorizontalMove* DeltaTime, position.Value.y, position.Value.z)
            };
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        JobHandle job = new JobHandle();
        if (SpiderGamePlayController.Controller == null)
        {
            return job;
        }

        job = new SpiderLevel1MovementJob
        {
            DeltaTime = Time.deltaTime,
            HorizontalMove = SpiderGamePlayController.Controller.Speed      
        }.Schedule(this, inputDependencies);
        
        return job;
    }
}
