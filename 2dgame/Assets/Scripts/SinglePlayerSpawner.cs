using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;

public class SinglePlayerSpawner : MonoBehaviour
{


   



    void Start()
    {


        // get the world's entity manager and creates and initialize the entities of Cooper and spiders 
        var entityManager = World.Active.EntityManager;
        SpawnCooper(entityManager);
        SpawnSpiders(entityManager);

    }


    /// <summary>
    /// this function creates the entity of Cooper and intialize its Mesh and Material and spawn it
    /// in the the bottom-center of the scene
    /// </summary>
    /// <param name="eManager"></param>
    void SpawnCooper(EntityManager eManager)
    {

        //creates the entity for Cooper and declares the type of components wich this entity contains 
        Entity CooperEntity = eManager.CreateEntity(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(CooperTag),
            typeof(TotalShotCount),
            typeof(LoadedShotCount),
            typeof(CharacterTag),
            typeof(Heart),
            typeof(SinglePlayerTag)
            );

        //Set Cooper's Mesh and Sprite to be rendered
        eManager.SetSharedComponentData(CooperEntity, new RenderMesh
        {
            mesh = GameDetails.GD.Mesh,
            material = GameDetails.GD.CooperMaterial
        });

        eManager.SetComponentData(CooperEntity, new TotalShotCount
        {
            Value = CooperGamePlayController.Controller.TotalShotsAtStart,
        });

        eManager.SetComponentData(CooperEntity, new LoadedShotCount
        {
            Count = CooperGamePlayController.Controller.ShotCountPerLoad,
            TimeToNextShoot = 0,
        });

        //set Cooper's position
        float3 position = transform.TransformPoint(new float3(0, -GameDetails.GD.CameraHalfHeight + 1, 0));

        eManager.SetComponentData(CooperEntity, new Translation
        {
            Value = position
        });

        eManager.SetComponentData(CooperEntity, new CharacterTag
        {
            Value = ChTags.Cooper
        });

        eManager.SetComponentData(CooperEntity, new Heart
        {
            Value = CooperGamePlayController.Controller.Heart
        });

    }

    /// <summary>
    /// this function creates the entity for Spiders and intialize their Mesh and Material and spawn them
    /// in random positions in the upper half of the scene, the positions are discrete
    /// </summary>
    /// <param name="eManager"></param>
    void SpawnSpiders(EntityManager eManager)
    {
        //this set holdes the positions wich previous spiders have been spawned in so we can check it to avoid spawning two spiders in a single point
        //the format of save position in this set is like: for point (x, y) the saved value is x * 100 + y, so each point is going to have 
        //a unique saved value
        HashSet<int> filledPositions = new HashSet<int>();


        for (int i = 0; i < GameDetails.GD.SpiderCount; i++)
        {
            //creates the entity for each spider and declares the type of components wich it contains 
            Entity SpiderEntity = eManager.CreateEntity(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(SpiderTag),
            typeof(LoadedShotCount),
            typeof(CharacterTag),
            typeof(Heart),
            typeof(SinglePlayerTag)

            );

            //specifies the spider level by attaching the level tag component to it

            switch (GameDetails.GD.Level)
            {
                case 1: eManager.AddComponent(SpiderEntity, typeof(Level1Tag)); break;
                case 2:
                    {
                        eManager.AddComponent(SpiderEntity, typeof(Level2Tag));
                        eManager.AddComponent(SpiderEntity, typeof(Destination));
                        
                        GameDetails gd = GameDetails.GD;

                        eManager.SetComponentData(SpiderEntity, new Destination
                        {
                            X = UnityEngine.Random.Range(-(gd.CameraHalfWidth - gd.HorizontalBoundary), (gd.CameraHalfWidth - gd.HorizontalBoundary)),
                            Y = UnityEngine.Random.Range(gd.VerticalBoundary, (gd.CameraHalfHeight - gd.VerticalBoundary))

                        });
                        

                        break;
                    }
                default: Debug.LogError("Invalid Level"); break;
            }


            int posX;
            int posY;
            int filledPositionIndex;

            //pick new random positions while the picked position is saved in the set of previousely picked positions
            do
            {
                posX = (int)Math.Round(UnityEngine.Random.Range(-GameDetails.GD.CameraHalfWidth + 2, GameDetails.GD.CameraHalfWidth - 1));
                posY = (int)Math.Round(UnityEngine.Random.Range(1, GameDetails.GD.CameraHalfHeight - 1));
                filledPositionIndex = posX * 1000 + posY;

            }
            while (filledPositions.Contains(filledPositionIndex));

            //Set spider's Mesh and Sprite to be rendered
            eManager.SetSharedComponentData(SpiderEntity, new RenderMesh
            {
                mesh = GameDetails.GD.Mesh,
                material = GameDetails.GD.SpiderMaterial
            });

            //set spider's position
            float3 position = transform.TransformPoint(new float3(posX, posY, 0));
            filledPositions.Add(filledPositionIndex);

            eManager.SetComponentData(SpiderEntity, new Translation
            {
                Value = position
            });

            eManager.SetComponentData(SpiderEntity, new LoadedShotCount
            {
                Count = 1000,
                TimeToNextShoot = UnityEngine.Random.Range(0, SpiderGamePlayController.Controller.ShootingDelay)
                
            });

            eManager.SetComponentData(SpiderEntity, new CharacterTag
            {
                Value = ChTags.Spider

            });

            eManager.SetComponentData(SpiderEntity, new Heart
            {
                Value = SpiderGamePlayController.Controller.Heart

            });

        }
    }



}
