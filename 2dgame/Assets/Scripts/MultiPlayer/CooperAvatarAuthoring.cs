using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Photon.Pun;

[RequiresEntityConversion]
public class CooperAvatarAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
       
        dstManager.AddComponentData(entity, new CooperTag { });
        dstManager.AddComponentData(entity, new CharacterTag { Value = ChTags.Cooper });
        dstManager.AddComponentData(entity, new Heart { Value = CooperGamePlayController.Controller.Heart });
        dstManager.AddComponentData(entity, new LoadedShotCount
        {
            Count = CooperGamePlayController.Controller.ShotCountPerLoad,
            TimeToNextShoot = 0
        });
        dstManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = GameDetails.GD.Mesh,
            material = GameDetails.GD.CooperMaterial
        });
        
        dstManager.AddComponentData(entity, new PhotonViewData { isMine = gameObject.GetComponent<PhotonView>().IsMine });
       // dstManager.AddComponentData(entity, new Modified { Value = false });
    }
}
