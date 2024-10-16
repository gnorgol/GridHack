using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public List<Block> blocks;
    public int gridSize = 4;

    public void CheckPuzzleCompletion()
    {
        // Logique pour vérifier si le puzzle est résolu
    }

    public void MoveBlock(Block block)
    {
        // Logique pour déplacer le bloc s'il est adjacent à une position vide
    }
}
