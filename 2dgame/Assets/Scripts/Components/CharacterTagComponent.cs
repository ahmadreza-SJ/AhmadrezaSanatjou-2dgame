using System;
using Unity.Entities;

public enum ChTags
{
    Cooper,
    Spider
}

public struct CharacterTag: IComponentData
{
    public ChTags Value;
}
