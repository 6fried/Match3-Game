using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile Settings")]

    [Tooltip("Weither if the tile is selected or not")]
    public bool selected;

    [Header("Tile Neighbours")]
    [Tooltip("The upper Tile")]
    public Tile upperTile;
    [Tooltip("The lower Tile")]
    public Tile lowerTile;
    [Tooltip("The left Tile")]
    public Tile leftTile;
    [Tooltip("The right Tile")]
    public Tile rightTile;

    [Header("Tile's content")]
    [Tooltip("The piece actually on the tile")]
    public Piece piece;

    private void Start()
    {
        SaveNeighbourTiles();
    }
    private void SaveNeighbourTiles()
    {
        float step = GameManager.instance.board.step;
        Vector2 p = transform.position/step;

        if (GameObject.Find($"[{p.x},{p.y + 1}]") != null)
        {
            upperTile = GameObject.Find($"[{p.x},{p.y + 1}]").GetComponent<Tile>();
        }

        if (GameObject.Find($"[{p.x},{p.y - 1}]") != null)
        {
            lowerTile = GameObject.Find($"[{p.x},{p.y - 1}]").GetComponent<Tile>();
        }

        if (GameObject.Find($"[{p.x - 1},{p.y}]") != null)
        {
            leftTile = GameObject.Find($"[{p.x - 1},{p.y}]").GetComponent<Tile>();
        }

        if (GameObject.Find($"[{p.x + 1},{p.y}]") != null)
        {
            rightTile = GameObject.Find($"[{p.x + 1},{p.y}]").GetComponent<Tile>();
        }
    }



    /// <summary>
    /// Description:
    /// Instanciate a new Piece on the tile;
    /// Input:
    /// none
    /// Return:
    /// void(no return)
    /// </summary>
    /// <param name="piecePrefab">The object that should be instanciated</param>
    public void CreatePiece(GameObject piecePrefab)
    {
        if (piece == null)
        {
            piece = Instantiate<GameObject>(
                piecePrefab,
                transform.position,
                transform.rotation,
                GetComponent<Transform>()
                ).GetComponent<Piece>();
        }
    }

    private void OnMouseDown()
    {
        selected = true;
    }

    private async void OnMouseDrag()
    {
        if (selected)
        {
            float step = GameManager.instance.board.step;
            Vector3 dragDirection = GetDragDirection();
            if(Mathf.Abs(dragDirection.x) != Mathf.Abs(dragDirection.y))
            {
                Vector3 targetTileCoordinates = (transform.position + step * dragDirection)/step;
                GameObject targetTileObject = GameObject.Find($"[{targetTileCoordinates.x},{targetTileCoordinates.y}]");
                if (targetTileObject != null)
                {
                    Tile targetTile = targetTileObject.GetComponent<Tile>();
                    await GameManager.instance.board.Swap(this, targetTile);
                    StartCoroutine("StopDragProcess");
                }
            }
        }
    }

    private Vector3 GetDragDirection()
    {
        int x, y;
        float xMove = Mathf.Clamp(Input.GetAxis("Mouse X") * 10, -1, 1);
        if (xMove > 0)
        {
            x = Mathf.CeilToInt(xMove);
        }
        else
        {
            x = Mathf.FloorToInt(xMove);
        }

        float yMove = Mathf.Clamp(Input.GetAxis("Mouse Y") * 10, -1, 1);
        if (yMove > 0)
        {
            y = Mathf.CeilToInt(yMove);
        }
        else
        {
            y = Mathf.FloorToInt(yMove);
        }

        return new Vector3(x, y, 0);
    }

    private IEnumerator StopDragProcess()
    {
        yield return new WaitForEndOfFrame();
        selected = false;
    }

    public void GetMatchs()
    {
        BoardManager board = GameManager.instance.board;

        if (HasHorizontalMatch())
        {
            if (!board.markedTiles.Contains(this))
            {
                board.markedTiles.Add(this);
            }

            if (!board.markedTiles.Contains(leftTile))
            {
                board.markedTiles.Add(leftTile);
            }

            if (!board.markedTiles.Contains(rightTile))
            {
                board.markedTiles.Add(rightTile);
            }
        }

        if (HasVerticalMatch())
        {
            if (!board.markedTiles.Contains(this))
            {
                board.markedTiles.Add(this);
            }

            if (!board.markedTiles.Contains(upperTile))
            {
                board.markedTiles.Add(upperTile);
            }

            if (!board.markedTiles.Contains(lowerTile))
            {
                board.markedTiles.Add(lowerTile);
            }
        }

    }

    private bool HasHorizontalMatch()
    {
        if (leftTile != null && rightTile != null)
        {
            return (piece.IsEqualto(leftTile.piece) && piece.IsEqualto(rightTile.piece));
        }

        return false;
    }

    private bool HasVerticalMatch()
    {
        if (upperTile != null && lowerTile != null)
        {
            return (piece.IsEqualto(upperTile.piece) && piece.IsEqualto(lowerTile.piece));
        }

        return false;
    }

    public void Explode()
    {
        GameObject.Destroy(piece.gameObject);
        piece = null;
    }

    public bool HasMatchFor(Piece cmpPiece)
    {
        bool hasHorizontalMatch = false;
        bool hasVerticalMatch = false;

        if (leftTile != null && rightTile != null)
        {
            hasHorizontalMatch = leftTile.piece.IsEqualto(cmpPiece) && rightTile.piece.IsEqualto(cmpPiece);
        }

        if (upperTile != null && lowerTile != null)
        {
            hasVerticalMatch = upperTile.piece.IsEqualto(cmpPiece) && lowerTile.piece.IsEqualto(cmpPiece);
        }

        return (hasHorizontalMatch || hasVerticalMatch);
    }



}
