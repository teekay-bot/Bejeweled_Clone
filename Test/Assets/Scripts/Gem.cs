using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    //References
    private GameManager gameManager;
    private FindMatches findMatches;
    private Animator animator;

    [Header("Gem Variables")]
    public int targetX;
    public int targetY;
    public int column, row, previousColumn, previousRow;
    public bool isMatched = false;
    //Swipe Variables
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;
    public float swipeAngle = 0f;
    private float swipeResist = 1f;

    [Header("Bomb Variables")]
    public bool isColumnBomb;
    public bool isRowBomb;
    public GameObject columnBomb;
    public GameObject rowBomb;

    [Header("Prefabs and GameObjects")]
    public GameObject otherGem;

    private void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        gameManager = FindObjectOfType<GameManager>();
        findMatches = FindObjectOfType<FindMatches>();
        animator = GetComponent<Animator>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
    }

    private void Update()
    {
        targetX = column;
        targetY = row;

        if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            //Move toward the target
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.4f);
            if (gameManager.allGems[column, row] != this.gameObject)
            {
                gameManager.allGems[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly set the position
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;  
        }

        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            //Move toward the target
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.4f);
            if (gameManager.allGems[column, row] != this.gameObject)
            {
                gameManager.allGems[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly set the position
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(0.5f);
        if (otherGem != null)
        {
            if (!isMatched && !otherGem.GetComponent<Gem>().isMatched)
            {
                otherGem.GetComponent<Gem>().row = row;
                otherGem.GetComponent<Gem>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                gameManager.currentGem = null;
                gameManager.currentState = GameState.move;
            }
            else
            {
                gameManager.DestroyMatches();             
            }

            //otherGem = null;
        } 
    }

    private void OnMouseDown()
    {
        if (gameManager.currentState == GameState.move)
        {
            animator.enabled = true;
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (gameManager.currentState == GameState.move)
        {
            animator.enabled = false;
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            gameManager.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            MoveGem();     
            gameManager.currentGem = this;
        }
        else
        {
            gameManager.currentState = GameState.move;
        }
    }

    void Swipe(Vector2 direction)
    {
        otherGem = gameManager.allGems[column + (int)direction.x, row + (int)direction.y];
        previousColumn = column;
        previousRow = row;
        otherGem.GetComponent<Gem>().column += -1 * (int)direction.x;
        otherGem.GetComponent<Gem>().row += -1 * (int)direction.y;
        column += (int)direction.x;
        row += (int)direction.y;
        StartCoroutine(CheckMoveCo());
    }

    void MoveGem()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < gameManager.width - 1)
        {
            //Right Swipe
            Swipe(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < gameManager.height - 1)
        {
            //Up Swipe
            Swipe(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            Swipe(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            Swipe(Vector2.down);
        }
        else
        {
            gameManager.currentState = GameState.move;
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject bomb = Instantiate(rowBomb, transform.position, Quaternion.identity);
        bomb.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject bomb = Instantiate(columnBomb, transform.position, Quaternion.identity);
        bomb.transform.parent = this.transform;
    }
}
