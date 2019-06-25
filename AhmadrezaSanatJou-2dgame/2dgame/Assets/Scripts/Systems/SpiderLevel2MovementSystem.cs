using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System;

public class SpiderLevel2MovementSystem : JobComponentSystem
{


    [BurstCompile]
    struct SpiderLevel1MovementJob : IJobForEach<Translation, SpiderTag, Level2Tag, Destination>
    {




        public float DeltaTime;
        public float Speed;
        public float RandomX;
        public float RandomY;


        public void Execute(ref Translation position, [ReadOnly] ref SpiderTag tag, [ReadOnly] ref Level2Tag levelTag, ref Destination destination)
        {
            float verticalMove = destination.Y - position.Value.y;
            float horizontalMove = destination.X - position.Value.x;

            

            if((verticalMove > 0.5 || verticalMove < -0.5) && (horizontalMove > 0.5 || horizontalMove < 0.5))
            {
                //Speed = Speed / (float)Math.Sqrt(Math.Pow(verticalMove, 2) + Math.Pow(horizontalMove, 2));
                position = new Translation
                {
                    Value = new float3(position.Value.x + horizontalMove * DeltaTime * Speed, position.Value.y + verticalMove * DeltaTime * Speed, position.Value.z)
                };
            }
            else
            {
                destination.X = RandomX;
                destination.Y = RandomY;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        JobHandle job = new JobHandle();
        if (SpiderGamePlayController.Controller == null ||GameDetails.GD == null)
        {
            return job;
        }

        GameDetails gd = GameDetails.GD;

        job = new SpiderLevel1MovementJob
        {
            DeltaTime = Time.deltaTime,
            Speed = SpiderGamePlayController.Controller.Speed,
            RandomX = UnityEngine.Random.Range(-(gd.CameraHalfWidth - gd.HorizontalBoundary), (gd.CameraHalfWidth - gd.HorizontalBoundary)),
            RandomY = UnityEngine.Random.Range(gd.VerticalBoundary, (gd.CameraHalfHeight - gd.VerticalBoundary))

        }.Schedule(this, inputDependencies);

        job.Complete();

        return job;
    }
}
