using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;



public class MultiPlayerMovementSystem : JobComponentSystem
{
    [BurstCompile]
    struct MultiplayerMovementJob : IJobForEach<Translation, CharacterTag, PhotonViewData>
    {

        public float DeltaTime;
        public float HorizontalMove;
        public float VerticalMove;
        public void Execute(ref Translation position, [ReadOnly] ref CharacterTag tag, [ReadOnly] ref PhotonViewData PV)
        {
            
                if ((position.Value.x > GameDetails.GD.CameraHalfWidth - GameDetails.GD.HorizontalBoundary && HorizontalMove > 0)
                || (position.Value.x < -GameDetails.GD.CameraHalfWidth + GameDetails.GD.HorizontalBoundary && HorizontalMove < 0))
                {
                    HorizontalMove = 0;
                }

            if (tag.Value == ChTags.Cooper)
            {
                if ((position.Value.y > -GameDetails.GD.VerticalBoundary && VerticalMove > 0)
                    || (position.Value.y < -GameDetails.GD.CameraHalfHeight + GameDetails.GD.VerticalBoundary && VerticalMove < 0))
                {
                    VerticalMove = 0;
                }

            }
            else
            {
                if ((position.Value.y > GameDetails.GD.CameraHalfHeight - GameDetails.GD.VerticalBoundary && VerticalMove > 0)
                    || (position.Value.y <  GameDetails.GD.VerticalBoundary && VerticalMove < 0))
                {
                    VerticalMove = 0;
                }
            }

            if(PV.isMine)
            {

                ClientAvatarsStates.State.Position = position.Value;
                position = new Translation
                {
                    Value = new float3(position.Value.x + HorizontalMove * DeltaTime, position.Value.y
                + VerticalMove * DeltaTime, position.Value.z)
                };
            }
            

        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {

        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(KeyCode.W))
        {
            vertical = (int)MoveDirectionsVertical.Up * CooperGamePlayController.Controller.VerticalSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vertical = (int)MoveDirectionsVertical.Down * CooperGamePlayController.Controller.VerticalSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontal = (int)MoveDirectionsHorizontal.Left * CooperGamePlayController.Controller.HorizontalSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontal = (int)MoveDirectionsHorizontal.Right * CooperGamePlayController.Controller.HorizontalSpeed;
        }

        var job = new MultiplayerMovementJob
        {

            DeltaTime = Time.deltaTime,
            HorizontalMove = horizontal,
            VerticalMove = vertical,
        }.Schedule(this, inputDependencies);



        return job;
    }
}
