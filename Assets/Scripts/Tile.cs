using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    [Header("Tile Settings")]

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

    // Corner Tiles
    private Tile upperLeftTile, upperRightTile, lowerLeftTile, lowerRightTile;


    // Neighbour valid tiles
    private List<Tile> neighbourTiles = new List<Tile>();

    // Weither if the tile is selected or not"
    private static Tile selected;

    private void Start()
    {
        SaveNeighbourTiles();
    }

    private void SaveNeighbourTiles()
    {
        float step = GameManager.instance.board.step;
        Vector2 p = transform.position/step;

        // Top
        if (GameObject.Find($"[{p.x},{p.y + 1}]") != null)
        {
            upperTile = GameObject.Find($"[{p.x},{p.y + 1}]").GetComponent<Tile>();
            neighbourTiles.Add(upperTile);
        }

        // Bottom
        if (GameObject.Find($"[{p.x},{p.y - 1}]") != null)
        {
            lowerTile = GameObject.Find($"[{p.x},{p.y - 1}]").GetComponent<Tile>();
            neighbourTiles.Add(lowerTile);
        }

        // Left
        if (GameObject.Find($"[{p.x - 1},{p.y}]") != null)
        {
            leftTile = GameObject.Find($"[{p.x - 1},{p.y}]").GetComponent<Tile>();
            neighbourTiles.Add(leftTile);
        }

        // Right
        if (GameObject.Find($"[{p.x + 1},{p.y}]") != null)
        {
            rightTile = GameObject.Find($"[{p.x + 1},{p.y}]").GetComponent<Tile>();
            neighbourTiles.Add(rightTile);
        }

        // Upper Left
        if (GameObject.Find($"[{p.x - 1},{p.y + 1}]") != null)
        {
            upperLeftTile = GameObject.Find($"[{p.x - 1},{p.y + 1}]").GetComponent<Tile>();
        }

        // Upper Right
        if (GameObject.Find($"[{p.x + 1},{p.y + 1}]") != null)
        {
            upperRightTile = GameObject.Find($"[{p.x + 1},{p.y + 1}]").GetComponent<Tile>();
        }

        // Lower Left
        if (GameObject.Find($"[{p.x - 1},{p.y - 1}]") != null)
        {
            lowerLeftTile = GameObject.Find($"[{p.x - 1},{p.y - 1}]").GetComponent<Tile>();
        }

        // Lower Right
        if (GameObject.Find($"[{p.x + 1},{p.y - 1}]") != null)
        {
            lowerRightTile = GameObject.Find($"[{p.x + 1},{p.y - 1}]").GetComponent<Tile>();
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
        selected = this;
    }

    private void OnMouseDrag()
    {
        // Piece should follow the cursor        
    }

    private async void OnMouseEnter()
    {
        if(selected != null && selected != this && neighbourTiles.Contains(selected))
        {
            await GameManager.instance.board.Swap(this, selected);
            StartCoroutine("ResetSelection");
        }
    }

    private void OnMouseUp()
    {
        selected = null;
    }
    private IEnumerator ResetSelection()
    {
        yield return new WaitForEndOfFrame();
        selected = null;
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

    public bool HasHorizontalMatch()
    {
        if (leftTile != null && rightTile != null)
        {
            return (piece.IsEqualto(leftTile.piece) && piece.IsEqualto(rightTile.piece));
        }

        return false;
    }

    public bool HasVerticalMatch()
    {
        if (upperTile != null && lowerTile != null)
        {
            return (piece.IsEqualto(upperTile.piece) && piece.IsEqualto(lowerTile.piece));
        }

        return false;
    }

    public void Explode()
    {

        //DOTween.Sequence().Join(piece.transform.DOScale(Vector3.zero, .25f).SetAutoKill(true)).Play();
        Destroy(piece.gameObject);
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

    public bool MayHaveVerticalMatch()
    {
        bool a = false, b = false, c = false, d = false, e = false, f = false, g = false, h = false, i = false, j = false;

        if (upperLeftTile != null && lowerLeftTile != null)
        {
            a = piece.IsEqualto(upperLeftTile.piece) && piece.IsEqualto(lowerLeftTile.piece);
        }
        if (upperRightTile != null && lowerRightTile != null)
        {
            b = piece.IsEqualto(upperRightTile.piece) && piece.IsEqualto(lowerRightTile.piece);
        }
        if (upperTile != null && upperTile.upperTile != null && upperTile.upperTile.upperTile != null)
        {
            c = piece.IsEqualto(upperTile.upperTile.piece) && piece.IsEqualto(upperTile.upperTile.upperTile.piece);
        }
        if (lowerTile != null && lowerTile.lowerTile != null && lowerTile.lowerTile.lowerTile != null)
        {
            d = piece.IsEqualto(lowerTile.lowerTile.piece) && piece.IsEqualto(lowerTile.lowerTile.lowerTile.piece);
        }
        if (leftTile != null)
        {
            if(leftTile.upperTile != null && leftTile.lowerTile != null)
            {
                e = piece.IsEqualto(leftTile.upperTile.piece) && piece.IsEqualto(leftTile.lowerTile.piece);
            }

            if (leftTile.upperTile != null && leftTile.upperTile.upperTile != null)
            {
                f = piece.IsEqualto(leftTile.upperTile.piece) && piece.IsEqualto(leftTile.upperTile.upperTile.piece);
            }

            if (leftTile.lowerTile != null && leftTile.lowerTile.lowerTile != null)
            {
                g = piece.IsEqualto(leftTile.lowerTile.piece) && piece.IsEqualto(leftTile.lowerTile.lowerTile.piece);
            }
        }
        if(rightTile != null)
        {
            if (rightTile.upperTile != null && rightTile.lowerTile != null)
            {
                h = piece.IsEqualto(rightTile.upperTile.piece) && piece.IsEqualto(rightTile.lowerTile.piece);
            }

            if (rightTile.upperTile != null && rightTile.upperTile.upperTile != null)
            {
                i = piece.IsEqualto(rightTile.upperTile.piece) && piece.IsEqualto(rightTile.upperTile.upperTile.piece);
            }

            if (rightTile.lowerTile != null && rightTile.lowerTile.lowerTile != null)
            {
                j = piece.IsEqualto(rightTile.lowerTile.piece) && piece.IsEqualto(rightTile.lowerTile.lowerTile.piece);
            }
        }

        return a || b || c || d || e || f || g || h || i || j;
    }

    public bool MayHaveHorizontalMatch()
    {
        bool a = false, b = false, c = false, d = false, e = false, f = false, g = false, h = false, i = false, j = false;

        if (upperLeftTile != null && upperRightTile != null)
        {
            a = piece.IsEqualto(upperLeftTile.piece) && piece.IsEqualto(upperRightTile.piece);
        }
        if (lowerLeftTile != null && lowerRightTile != null)
        {
            b = piece.IsEqualto(lowerLeftTile.piece) && piece.IsEqualto(lowerRightTile.piece);
        }
        if (leftTile != null && leftTile.leftTile != null && leftTile.leftTile.leftTile != null)
        {
            c = piece.IsEqualto(leftTile.leftTile.piece) && piece.IsEqualto(leftTile.leftTile.leftTile.piece);
        }
        if (rightTile != null && rightTile.rightTile != null && rightTile.rightTile.rightTile != null)
        {
            d = piece.IsEqualto(rightTile.rightTile.piece) && piece.IsEqualto(rightTile.rightTile.rightTile.piece);
        }
        if (upperTile != null)
        {
            if (upperTile.leftTile != null && upperTile.rightTile != null)
            {
                e = piece.IsEqualto(upperTile.leftTile.piece) && piece.IsEqualto(upperTile.rightTile.piece);
            }
            if (upperTile.leftTile != null && upperTile.leftTile.leftTile != null)
            {
                f = piece.IsEqualto(upperTile.leftTile.piece) && piece.IsEqualto(upperTile.leftTile.leftTile.piece);
            }
            if (upperTile.rightTile != null && upperTile.rightTile.rightTile != null)
            {
                g = piece.IsEqualto(upperTile.rightTile.piece) && piece.IsEqualto(upperTile.rightTile.rightTile.piece);
            }
        }
        if (lowerTile != null)
        {
            if (lowerTile.leftTile != null && lowerTile.rightTile != null)
            {
                h = piece.IsEqualto(lowerTile.leftTile.piece) && piece.IsEqualto(lowerTile.rightTile.piece);
            }
            if (lowerTile.leftTile != null && lowerTile.leftTile.leftTile != null)
            {
                i = piece.IsEqualto(lowerTile.leftTile.piece) && piece.IsEqualto(lowerTile.leftTile.leftTile.piece);
            }
            if (lowerTile.rightTile != null && lowerTile.rightTile.rightTile != null)
            {
                j = piece.IsEqualto(lowerTile.rightTile.piece) && piece.IsEqualto(lowerTile.rightTile.rightTile.piece);
            }
        }

        return a || b || c || d || e || f || g || h || i || j;
    }

}
