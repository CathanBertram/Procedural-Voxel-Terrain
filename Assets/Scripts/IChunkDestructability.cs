using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChunkDestructability
{
    public void ReplaceBlock(Vector3 pos, byte newBlock = 0);
}
