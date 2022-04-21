using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    //References
    private GameManager gameManager;
    public List<GameObject> currentMatches = new List<GameObject>();

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsRowBomb(Gem gem1, Gem gem2, Gem gem3)
    {
        List<GameObject> currentGems = new List<GameObject>();

        if (gem1.isRowBomb)
        {
            currentMatches.Union(GetRowGem(gem1.row));
        }

        if (gem2.isRowBomb)
        {
            currentMatches.Union(GetRowGem(gem2.row));
        }

        if (gem3.isRowBomb)
        {
            currentMatches.Union(GetRowGem(gem3.row));
        }

        return currentGems;

    }

    private List<GameObject> IsColumnBomb(Gem gem1, Gem gem2, Gem gem3)
    {
        List<GameObject> currentGems = new List<GameObject>();

        if (gem1.isColumnBomb)
        {
            currentMatches.Union(GetColumnGem(gem1.column));
        }

        if (gem2.isColumnBomb)
        {
            currentMatches.Union(GetColumnGem(gem2.column));
        }

        if (gem3.isColumnBomb)
        {
            currentMatches.Union(GetColumnGem(gem3.column));
        }

        return currentGems;

    }

    private void AddToListAndMatch(GameObject gem)
    {
        if (!currentMatches.Contains(gem))
        {
            currentMatches.Add(gem);
        }
        gem.GetComponent<Gem>().isMatched = true;
    }

    private void GetNearbyGems(GameObject gem1, GameObject gem2, GameObject gem3)
    {
        AddToListAndMatch(gem1);
        AddToListAndMatch(gem2);
        AddToListAndMatch(gem3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < gameManager.width; i++)
        {
            for (int j = 0; j < gameManager.height; j++)
            {
                GameObject currentGem = gameManager.allGems[i, j];

                if (currentGem != null)
                {
                    Gem currentGEM = currentGem.GetComponent<Gem>();
                    if (i > 0 && i < gameManager.width - 1)
                    {
                        GameObject leftGem = gameManager.allGems[i - 1, j];                      
                        GameObject rightGem = gameManager.allGems[i + 1, j];
                        
                        if (leftGem != null && rightGem != null)
                        {
                            Gem leftGEM = leftGem.GetComponent<Gem>();
                            Gem rightGEM = rightGem.GetComponent<Gem>();

                            if (leftGem.tag == currentGem.tag && rightGem.tag == currentGem.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftGEM, currentGEM, rightGEM));
                                currentMatches.Union(IsColumnBomb(leftGEM, currentGEM, rightGEM));
                                GetNearbyGems(leftGem, currentGem, rightGem);
                            }
                        }
                    }

                    if (j > 0 && j < gameManager.height - 1)
                    {
                        GameObject upGem = gameManager.allGems[i, j + 1];                      
                        GameObject downGem = gameManager.allGems[i, j - 1];
                        
                        if (upGem != null && downGem != null)
                        {
                            Gem upGEM = upGem.GetComponent<Gem>();
                            Gem downGEM = downGem.GetComponent<Gem>();

                            if (upGem.tag == currentGem.tag && downGem.tag == currentGem.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upGEM, currentGEM, downGEM));
                                currentMatches.Union(IsRowBomb(upGEM, currentGEM, downGEM));
                                GetNearbyGems(upGem, currentGem, downGem);
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnGem(int column)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < gameManager.height; i++)
        {
            if (gameManager.allGems[column, i] != null)
            {
                gems.Add(gameManager.allGems[column, i]);
                gameManager.allGems[column, i].GetComponent<Gem>().isMatched = true;
            }
        }

        return gems;
    }

    List<GameObject> GetRowGem(int row)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < gameManager.width; i++)
        {
            if (gameManager.allGems[i, row] != null)
            {
                gems.Add(gameManager.allGems[i, row]);
                gameManager.allGems[i, row].GetComponent<Gem>().isMatched = true;
            }
        }

        return gems;
    }

    public void CheckBombs()
    {
        //Did the player move something?
        if (gameManager.currentGem != null)
        {
            //Is the gem they moved matched?
            if (gameManager.currentGem.isMatched)
            {
                //Make it unmatched
                gameManager.currentGem.isMatched = false;
                //Decide what kind of bomb to make
                if ((gameManager.currentGem.swipeAngle > -45 && gameManager.currentGem.swipeAngle <= 45) || (gameManager.currentGem.swipeAngle < -135 && gameManager.currentGem.swipeAngle >= 135))
                {
                    //Make a row bomb
                    gameManager.currentGem.MakeRowBomb();
                }
                else
                {
                    //Make a column bomb
                    gameManager.currentGem.MakeColumnBomb();
                }
            }
            //Is the other gem matched?
            else if (gameManager.currentGem.otherGem != null)
            {
                Gem otherGem = gameManager.currentGem.otherGem.GetComponent<Gem>();
                //Is the other Gem matched?
                if (otherGem.isMatched)
                {
                    //Make it unmatched
                    otherGem.isMatched = false;
                    //Decide what kind of bomb to make
                    if ((gameManager.currentGem.swipeAngle > -45 && gameManager.currentGem.swipeAngle <= 45) || (gameManager.currentGem.swipeAngle < -135 && gameManager.currentGem.swipeAngle >= 135))
                    {
                        //Make a row bomb
                        otherGem.MakeRowBomb();
                    }
                    else
                    {
                        //Make a column bomb
                        otherGem.MakeColumnBomb();
                    }
                }
            }
        }
    }
}
