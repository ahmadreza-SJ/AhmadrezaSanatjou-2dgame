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
 

    //mesh and material referances to be rendered
    [SerializeField] private Mesh Mesh;
    [SerializeField] private Material CooperMaterial;
    [SerializeField] private Material SpiderMaterial;

    [SerializeField] private int Level;


    // count of spiders can be entered depend on the level
    public int SpiderCount;
    


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
            typeof(CooperTag)
            
            );

        //Set Cooper's Mesh and Sprite to be rendered
        eManager.SetSharedComponentData(CooperEntity, new RenderMesh
        {
            mesh = Mesh,
            material = CooperMaterial
        });



        //set Cooper's position
        float3 position = transform.TransformPoint(new float3(0, -GameDetails.GD.CameraHalfHeight + 1, 0));

        eManager.SetComponentData(CooperEntity, new Translation
        {
            Value = position
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
        

       for (int i = 0; i < SpiderCount; i++)
       {
            //creates the entity for each spider and declares the type of components wich it contains 
            Entity SpiderEntity = eManager.CreateEntity(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(SpiderTag)
            
            );

            //specifies the spider level by attaching the level tag component to it
            
            switch(Level)
            {
                case 1: eManager.AddComponent(SpiderEntity, typeof(Level1Tag));break;
                default: Debug.LogError("Invalid Level");break;
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
                mesh = Mesh,
                material = SpiderMaterial
            });

            //set spider's position
            float3 position = transform.TransformPoint(new float3(posX, posY, 0));
            filledPositions.Add(filledPositionIndex);

            eManager.SetComponentData(SpiderEntity, new Translation
            {
                Value = position
            });
        }
    }

    
    
}
