using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Photon.Pun;

[RequiresEntityConversion]
public class SpiderAvatarAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        AvatarEntityManager.Manager.SpiderEntity = entity;


        dstManager.AddComponentData(entity, new CooperTag { });
        dstManager.AddComponentData(entity, new CharacterTag { Value = ChTags.Spider });
        dstManager.AddComponentData(entity, new Heart { Value = CooperGamePlayController.Controller.Heart });
        dstManager.AddComponentData(entity, new LoadedShotCount
        {
            Count = CooperGamePlayController.Controller.ShotCountPerLoad,
            TimeToNextShoot = 0
        });
        dstManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = GameDetails.GD.Mesh,
            material = GameDetails.GD.SpiderMaterial
        });
        
        dstManager.AddComponentData(entity, new PhotonViewData { isMine = gameObject.GetComponent<PhotonView>().IsMine });
        // dstManager.AddComponentData(entity, new Modified { Value = false });
    }
}
