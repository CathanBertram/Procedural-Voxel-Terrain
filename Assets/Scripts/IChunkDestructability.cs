using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChunkDestructability
{
    public void PlaceBlock(Vector3 pos, byte newBlock);
    public void BreakBlock(Vector3 pos, byte newBlock = 0);

}
