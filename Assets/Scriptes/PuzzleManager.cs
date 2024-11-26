using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{
    public GameObject blockPrefab; // Préfabriqué du bloc
    public GameObject blockContainerPrefab; // Préfabriqué du container
    public Transform gridParent; // Parent qui contient les blocs (avec GridLayoutGroup)
    public Transform containerParent; // Parent qui contient les containers
    public List<Block> blocks;
    public int gridSize;

    public int numberOfAttempts = 5; // Nombre de tentatives (lignes)
    public int blockLength = 4; // Longueur des blocs dans chaque ligne

    private List<List<BlockContainer>> blockContainers = new List<List<BlockContainer>>();

    public string[] puzzleSolution;
    public string[] CurrentSolution;

    private int currentAttempt = 0;
    private int currentNumberOfBlocksOnContainer = 0;

    void Start()
    {
        GenerateGrid(gridSize);
        GeneratePuzzle(numberOfAttempts, blockLength);
        
        CurrentSolution = new string[blockLength];
    }

    public void CheckPuzzleCompletion()
    {
        UpdateCurrentBlockCount();
        // Logique pour vérifier si le puzzle est résolu
        if (CurrentSolution.SequenceEqual(puzzleSolution))
        {
            Debug.Log("Puzzle résolu !");
            // Reset the puzzle
            currentAttempt = 0;
            currentNumberOfBlocksOnContainer = 0;
            CurrentSolution = new string[blockLength];
            GeneratePuzzle(numberOfAttempts, blockLength);
            GenerateGrid(gridSize);
        }
        else if (currentNumberOfBlocksOnContainer == blockLength)
        {
            Debug.Log("Puzzle non résolu !");
            //Set Color red no exist in the solution and yellow exist in the solution but in the wrong place and green exist in the solution and in the right place

            for (int i = 0; i < blockLength; i++)
            {
                if (puzzleSolution[i] == CurrentSolution[i])
                {
                    blockContainers[currentAttempt][i].ContainBlock.GetComponent<Image>().color = Color.green;

                    foreach (var block in blocks)
                    {
                        if (block.blockValue == int.Parse(CurrentSolution[i]))
                        {
                            block.GetComponent<Image>().color = Color.green;
                        }
                    }

                    blockContainers[currentAttempt + 1][i].ContainBlock = blockContainers[currentAttempt][i].ContainBlock;
                    if (i - 1 == currentNumberOfBlocksOnContainer)
                    {
                        currentNumberOfBlocksOnContainer++;
                    }
                }
                else if (Array.Exists(puzzleSolution, element => element == CurrentSolution[i]))
                {
                    blockContainers[currentAttempt][i].ContainBlock.GetComponent<Image>().color = Color.yellow;
                    foreach (var block in blocks)
                    {
                        if (block.blockValue == int.Parse(CurrentSolution[i]))
                        {
                            block.Unlock();
                            block.GetComponent<Image>().color = Color.yellow;                           

                        }
                    }

                }
                else
                {
                    blockContainers[currentAttempt][i].ContainBlock.GetComponent<Image>().color = Color.red;
                    foreach (var block in blocks)
                    {
                        if (block.blockValue == int.Parse(CurrentSolution[i]))
                        {
                            block.GetComponent<Image>().color = Color.red;
                        }
                    }
                }
            }


            // Reset the puzzle
            currentAttempt++;
            currentNumberOfBlocksOnContainer = 0;
            //Keep correct answers in the next row and the current solution
            for (int i = 0; i < blockLength; i++)
            {
                if (puzzleSolution[i] == CurrentSolution[i])
                {
                    //Set copy of the block in the right place in the blockContainers
                    Block block = Instantiate(blockPrefab, containerParent).GetComponent<Block>();
                    block.transform.SetParent(blockContainers[currentAttempt][i].transform);
                    block.transform.localPosition = Vector3.zero;
                    block.transform.localScale = Vector3.one;
                    //react transform width and height
                    block.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
                    //Color the block green
                    block.GetComponent<Image>().color = Color.green;

                    //Set the Width and Height of the block

                    block.blockValue = int.Parse(CurrentSolution[i]);
                    block.GetComponentInChildren<TMP_Text>().text = block.blockValue.ToString();



                    blockContainers[currentAttempt][i].ContainBlock = block;
                    if (i - 1 == currentNumberOfBlocksOnContainer)
                    {
                        currentNumberOfBlocksOnContainer++;
                    }

                }
                else
                {
                    CurrentSolution[i] = "";
                }

            }

        }
    }


    public void GenerateGrid(int numberOfBlocks)
    {
        // Efface les blocs précédents, s'il y en a
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        blocks.Clear();

        // Met à jour le GridLayoutGroup en fonction de la nouvelle taille
        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();

        // Génère les blocs
        for (int i = 0; i < numberOfBlocks; i++)
        {
            GameObject newBlock = Instantiate(blockPrefab, gridParent);
            Block blockScript = newBlock.GetComponent<Block>();

            // Assigne une valeur au bloc (par exemple, sa position dans la liste)
            blockScript.blockValue = i + 1;
            blocks.Add(blockScript);

            // Assigne la valeur du bloc à son texte
            TMP_Text blockText = newBlock.GetComponentInChildren<TMP_Text>();
            blockText.text = blockScript.blockValue.ToString();

            // Assigne to the button the event OnBlockClicked
            Button blockButton = newBlock.GetComponent<Button>();
            blockButton.onClick.AddListener(() => MoveBlock(blockScript));
        }
    }
    public void GeneratePuzzle(int numberOfAttempts, int blockLength)
    {
        // Génère une solution pour le puzzle
        puzzleSolution = new string[blockLength];
        List<int> allValues = new List<int>();
        // Remplit la liste avec toutes les valeurs possibles with linq
        allValues.AddRange(Enumerable.Range(1, gridSize));
        for (int i = 0; i < blockLength; i++)
        {
            // Génère une valeur aléatoire pour chaque bloc et ne peut pas étre dans le puzzleSolution
            int randomValue = allValues[UnityEngine.Random.Range(0, allValues.Count)];
            allValues.Remove(randomValue);
            puzzleSolution[i] = randomValue.ToString();
        }

        // Efface les containers précédents, s'il y en a
        foreach (Transform child in containerParent)
        {
            Destroy(child.gameObject);
        }
        blockContainers.Clear();

        //Constraint count of the grid layout group
        GridLayoutGroup gridLayout = containerParent.GetComponent<GridLayoutGroup>();
        gridLayout.constraintCount = blockLength;

        // Génère les BlockContainers
        for (int i = 0; i < numberOfAttempts; i++)
        {
            List<BlockContainer> containerRow = new List<BlockContainer>();
            for (int j = 0; j < blockLength; j++)
            {
                GameObject newContainer = Instantiate(blockContainerPrefab, containerParent);
                BlockContainer blockContainerScript = newContainer.GetComponent<BlockContainer>();
                containerRow.Add(blockContainerScript);
            }
            blockContainers.Add(containerRow);
        }
    }

    public void MoveBlock(Block block)
    {
        if (block.isLocked == false)
        {
            //add the block value to the current solution
            UpdateCurrentBlockCount();
            CurrentSolution[currentNumberOfBlocksOnContainer] = block.blockValue.ToString();

            // Check if currentAttempt and currentNumberOfBlocksOnContainer are within bounds
            if (currentAttempt < blockContainers.Count && currentNumberOfBlocksOnContainer < blockContainers[currentAttempt].Count)
            {
                /*                //move to the BlockContainer
                                blockContainers[currentAttempt][currentNumberOfBlocksOnContainer].ContainBlock = block;
                                block.transform.SetParent(blockContainers[currentAttempt][currentNumberOfBlocksOnContainer].transform);
                                block.transform.localPosition = Vector3.zero;
                                block.transform.localScale = Vector3.one;
                                currentNumberOfBlocksOnContainer++;
                                block.isLocked = true;*/
                //Instantiate a new copy of the block container in the grid
                Block blockCopy = Instantiate(blockPrefab, blockContainers[currentAttempt][currentNumberOfBlocksOnContainer].transform).GetComponent<Block>();
                blockContainers[currentAttempt][currentNumberOfBlocksOnContainer].ContainBlock = blockCopy;
                blockCopy.transform.localPosition = Vector3.zero;
                blockCopy.transform.localScale = Vector3.one;
                blockCopy.blockValue = block.blockValue;
                blockCopy.GetComponentInChildren<TMP_Text>().text = blockCopy.blockValue.ToString();
                blockCopy.isLocked = true;
                blockCopy.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);

                //Lock the block
                block.Lock();



            }
            else
            {
                Debug.LogError("Index out of range: currentAttempt or currentNumberOfBlocksOnContainer is out of bounds.");
            }
            CheckPuzzleCompletion();
        }
    }

    private void UpdateCurrentBlockCount()
    {
        currentNumberOfBlocksOnContainer = 0;
        for (int i = 0; i < blockLength; i++)
        {
            if (CurrentSolution[i] != "" && CurrentSolution[i] != null)
            {
                currentNumberOfBlocksOnContainer++;
            }
            else
            {
                break;
            }
        }
    }
}
