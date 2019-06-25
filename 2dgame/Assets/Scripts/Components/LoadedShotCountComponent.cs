using System;
using Unity.Entities;

public struct LoadedShotCount : IComponentData
{
    public int Count;
    public float TimeToNextShoot;
}
