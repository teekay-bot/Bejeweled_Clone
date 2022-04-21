using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    wait,
    move
}

public class GameManager : MonoBehaviour
{
    //References
    public GameState currentState = GameState.move;
    private FindMatches findMatches;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    public Button noiseRed;

    [Header("Board Variables")]
    public int width;
    public int height;
    [SerializeField] private int offset;

    [Header("Score Variables")]
    [SerializeField] private int baseGemValue = 20;
    [SerializeField] private int streakValue = 1;

    [Header("Prefabs and GameObjects")]
    [SerializeField] private BackgroundTile[,] allTiles;
    [SerializeField] private GameObject[] gems;
    [SerializeField] private GameObject tilePrefabs;
    public GameObject[,] allGems;
    [SerializeField] private GameObject destroyEffect;
    public Gem currentGem;



    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allGems = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempTilePos = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefabs, tempTilePos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "Tile " + i + "," + j;
                int gemToUse = Random.Range(0, gems.Length);

                int maxIteration = 0;
                while (MatchesAt(i, j, gems[gemToUse]) && maxIteration < 100)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    maxIteration++;
                }
                maxIteration = 0;

                Vector2 tempGemPos = new Vector2(i, j + offset);
                GameObject gem = Instantiate(gems[gemToUse], tempGemPos, Quaternion.identity);
                gem.GetComponent<Gem>().row = j;
                gem.GetComponent<Gem>().column = i;
                gem.transform.parent = this.transform;
                gem.name = "Gem " + i + "," + j;
                allGems[i, j] = gem; 
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject gem)
    {
        if (column > 1 && row > 1)
        {
            if (allGems[column - 1, row].tag == gem.tag && allGems[column - 2, row].tag == gem.tag)
            {
                return true;
            }

            if (allGems[column, row - 1].tag == gem.tag && allGems[column, row - 2].tag == gem.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allGems[column, row - 1].tag == gem.tag && allGems[column, row - 2].tag == gem.tag)
                {
                    return true;
                }
            }

            if (column > 1)
            {
                if (allGems[column - 1, row].tag == gem.tag && allGems[column - 2, row].tag == gem.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DestroyMatchedAt(int column, int row)
    {
        if (allGems[column, row].GetComponent<Gem>().isMatched)
        {
            //How many elements are in the matched gems list from FindMatches?
            if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();   
            }

            //Does the sound manager exist?
            if (soundManager != null)
            {
                soundManager.PlayDestroyNoise();   
            }
            GameObject particle = Instantiate(destroyEffect, allGems[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.3f);
            Destroy(allGems[column, row]);
            scoreManager.IncreaseScore(baseGemValue * streakValue);
            allGems[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    DestroyMatchedAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allGems[i, j].GetComponent<Gem>().row -= nullCount;
                    allGems[i, j] = null;
                }
            }

            nullCount = 0;
        }
        yield return new WaitForSeconds(0.3f);

        StartCoroutine(RefillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    int gemToUse = Random.Range(0, gems.Length);
                    int maxIteration = 0;

                    while (MatchesAt(i,j, gems[gemToUse]) && maxIteration < 100)
                    {
                        maxIteration++;
                        gemToUse = Random.Range(0, gems.Length);
                    }

                    maxIteration = 0;
                    GameObject gem = Instantiate(gems[gemToUse], tempPos, Quaternion.identity);
                    allGems[i, j] = gem;
                    gem.GetComponent<Gem>().row = j;
                    gem.GetComponent<Gem>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    if (allGems[i, j].GetComponent<Gem>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator RefillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);

        while (MatchesOnBoard())
        {
            streakValue++;
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentGem = null;
        yield return new WaitForSeconds(0.5f);

        //if (IsDeadLocked())
        //{
        //    Debug.Log("Dead Locked");
        //}
        currentState = GameState.move;
        streakValue = 1;
    }

    private void SwitchGems(int column, int row, Vector2 direction)
    {
        //Take the second gem and save it in a holder
        GameObject holder = allGems[column + (int)direction.x, row + (int)direction.y];
        //Switching the first gem to be the second position
        allGems[column + (int)direction.x, row + (int)direction.y] = allGems[column, row];
        //Set the fisrt gem to be the second gem
        allGems[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i,j] != null)
                {
                    //Make sure that one and two to the right are in the board
                    if (i < width - 2)
                    {
                        //Check if the gem to the right and two to the right exist
                        if (allGems[i + 1, j] != null && allGems[i + 2, j] != null)
                        {
                            if (allGems[i + 1, j].tag == allGems[i, j].tag && allGems[i + 2, j].tag == allGems[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    //Make sure that above are in the board
                    if (j < height - 2)
                    {
                        //Check if the gem above exist
                        if (allGems[i, j + 1] != null && allGems[i, j + 2] != null)
                        {
                            if (allGems[i, j + 1].tag == allGems[i, j].tag && allGems[i, j + 2].tag == allGems[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }     
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchGems(column, row, direction);
        if (CheckForMatches())
        {
            SwitchGems(column, row, direction);
            return true;
        }
        else
        {
            SwitchGems(column, row, direction);
            return false;
        }
    }

    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    if (i < width - 2)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    if (i < height - 2)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}
