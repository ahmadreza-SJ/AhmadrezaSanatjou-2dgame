using System;
using Unity.Entities;

// this vomponent contains nothing. it's just used to find Spiders by a job's filter
public struct SpiderTag : IComponentData
{
    public int Value;
}
