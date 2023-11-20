using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public GameObject[] blockPrefabs; // 方块的预制体数组
    public int rows = 5; // 行数
    public int columns = 5; // 列数
    public List<GameObject> blocks;
    public float moveSpeed = 1f;
    public float moveDistance = 1f;

    private List<string> blockTags; // 用于记录每个位置的方块的tag
    private GameObject _blockToDelete;
    private List<GameObject> _blocksToFall;
    private bool _clicked = false;

    private string lastBlockTag; // 用于追踪上一个方块的tag

    private void Start()
    {
        blockTags = new List<string>();
        _blocksToFall = new List<GameObject>();
        GenerateBlocks();
    }

    void GenerateBlocks()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPosition = new Vector3(col, row, 0); // 根据行列生成位置
                GameObject randomBlockPrefab = GetRandomBlockPrefab();
                GameObject block = Instantiate(randomBlockPrefab, spawnPosition, Quaternion.identity);
                blocks.Add(block);
                blockTags.Add(block.tag);
                lastBlockTag = block.tag;
            }
        }
    }

    GameObject GetRandomBlockPrefab()
    {
        GameObject randomBlockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];

        // 确保下一个方块的tag不同于上一个方块
        while (randomBlockPrefab.tag == lastBlockTag)
        {
            randomBlockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
        }

        return randomBlockPrefab;
    }

    private void Update()
    {

        // 检查玩家的点击
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }


    void FallBlock(GameObject block)
    {
        // 获取下一个位置
        Vector3 nextPosition = block.transform.position + Vector3.down;

        // 移动方块到下一个位置
        block.transform.position = nextPosition;
    }

    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            if(!_clicked)
            {
                _clicked = true;
                _blockToDelete = clickedObject;
            }
            else if(_blockToDelete != clickedObject)
            {
                _clicked = false;
                if(_blockToDelete.tag == clickedObject.tag)
                {
                    AddBlocksToFall(_blockToDelete);
                    AddBlocksToFall(clickedObject);
                    if(_blocksToFall.Contains(_blockToDelete) || _blocksToFall.Contains(clickedObject))
                    {
                        _blocksToFall.Add(_blocksToFall[_blocksToFall.Count - 1]);
                    }

                    if (_blocksToFall.Count != 0) StartFalling();
                    //Debug.Log(_blocksToFall.Count);

                    _blockToDelete.SetActive(false);
                    clickedObject.SetActive(false);


                    //Destroy(clickedObject);
                    //Destroy(_blockToDelete);
                }
            }

            // Log the name of the clicked object
            Debug.Log("Clicked on object: " + clickedObject.name);

        }
        else
        {
            Debug.Log("No object hit by the raycast");
        }
    }

    public void AddBlocksToFall(GameObject block)
    { 
        RaycastHit hit;
        if (Physics.Raycast(block.transform.position, Vector3.up, out hit))
        {
            GameObject aboveBlock = hit.collider.gameObject;
            _blocksToFall.Add(aboveBlock);
            AddBlocksToFall(aboveBlock);// 如果上方有碰撞体，返回上方的方块
        }
        else
        {
            lastBlockTag = block.tag;
            float xPosition = block.transform.position.x;
            float yPosition = block.transform.position.y;
            Vector3 spawnPosition = new Vector3(xPosition, yPosition+1, 0);
            GameObject randomBlockPrefab = GetRandomBlockPrefab();
            GameObject newBlock = Instantiate(randomBlockPrefab, spawnPosition, Quaternion.identity);
            _blocksToFall.Add(newBlock);
        }

    }

    public void StartFalling(float moveDistance = 1f)
    {
        StartCoroutine(MoveObjectsSmoothCoroutine());
    }

    // 协程，逐帧移动对象
    IEnumerator MoveObjectsSmoothCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 initialPosition;

        foreach (GameObject obj in _blocksToFall)
        {
            initialPosition = obj.transform.position;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                obj.transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.down * moveDistance, elapsedTime);
                yield return null;
            }

            elapsedTime = 0f;
        }

        // 移动结束后清空列表
        _blocksToFall.Clear();
    }
}
