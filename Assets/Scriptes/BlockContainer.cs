using UnityEngine;
using System.Collections.Generic;

public class BlockContainer : MonoBehaviour
{
    private Block _ContainBlock;

    public Block ContainBlock { get => _ContainBlock; set => _ContainBlock = value; }
}
