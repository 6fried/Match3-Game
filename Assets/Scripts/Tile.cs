using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Variables

    #region Public

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

    [Header("Tile Infos")]
    [Tooltip("Weither if the tile is marked to be clear or not")]
    public bool isMarked;
    [Tooltip("Weither if the tile have already been checked or not")]
    public bool isChecked;
    [Tooltip("Weither if the tile is exploding or not")]
    public bool isExploding;

    #endregion // Public

    #region Private

    // Corner Tiles
    private Tile upperLeftTile, upperRightTile, lowerLeftTile, lowerRightTile;

    //Raycast elements
    Ray ray;
    RaycastHit hit;

    // Neighbour valid tiles
    private readonly List<Tile> neighbourTiles = new List<Tile>();
    private readonly List<Tile> row = new List<Tile>();
    private readonly List<Tile> column = new List<Tile>();

    // Weither if the tile is selected or not"
    private static Tile selected;

    #endregion // Private

    #endregion // Vabiables

    #region Functions

    /// <summary>
    /// Description:
    /// Standard Unity called Once before the first frame update
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void Start()
    {
        SaveNeighbourTiles();
        SetRow();
        SetColumn();
    }

    /// <summary>
    /// Description:
    /// Saves all neighbour tiles
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
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
    /// Saves all tiles on the same row
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void SetRow()
    {
        BoardManager board = GameManager.instance.board;
        float step = board.step;
        Vector2 p = transform.position / step;

        for (float x = 0; x < board.width; x++)
        {
            if (x != p.x)
            {
                row.Add(GameObject.Find($"[{x},{p.y}]").GetComponent<Tile>());
            }
        }
    }

    /// <summary>
    /// Description:
    /// Saves all tiles on the same column
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void SetColumn()
    {
        BoardManager board = GameManager.instance.board;
        float step = board.step;
        Vector2 p = transform.position / step;

        for (float y = 0; y < board.height; y++)
        {
            if (y != p.y)
            {
                column.Add(GameObject.Find($"[{p.x},{y}]").GetComponent<Tile>());
            }
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

    /// <summary>
    /// Description:
    /// Standard Unity Function called when the user has pressed the mouse button while over the GUIElement or Collider
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void OnMouseDown()
    {
        selected = this;
    }

    /// <summary>
    /// Description:
    /// Standard Unity Function called when the user has clicked on a GUIElement or Collider and is still holding down the mouse
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void OnMouseDrag()
    {
        if(selected == this)
        {
            int boardMask = 1 << 8;
            ray = Camera.main.ScreenPointToRay(-Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f, boardMask))
            {
                Debug.Log("Drag");

                transform.position = hit.point;
            }
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity Function called when the mouse entered the GUIElement or the Collider
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private async void OnMouseEnter()
    {
        if(selected != null && selected != this && neighbourTiles.Contains(selected))
        {
            await GameManager.instance.board.Swap(this, selected);
            StartCoroutine("ResetSelection");
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity Function called when the user has released the mouse button
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void OnMouseUp()
    {
        selected = null;
    }

    /// <summary>
    /// Description
    /// Sets the Selected tile to null after one frame
    /// Input: none
    /// Return:
    /// IEnumerator
    /// </summary>
    /// <returns>Allows this to function like a coroutine</returns>
    private IEnumerator ResetSelection()
    {
        yield return new WaitForEndOfFrame();
        selected = null;
    }

    /// <summary>
    /// Description:
    /// Returns all tiles that matches with this
    /// Input: 
    /// none
    /// Return:
    /// List<Tile>
    /// </summary>
    /// <returns>All matching tiles</returns>
    public List<Tile> GetMatchs()
    {
        List<Tile> matchingTiles = new List<Tile>();
        List<Tile> horizontalMatchingTiles = GetHorizontalMatchs();
        List<Tile> verticalMatchingTiles = GetVerticalMatchs();

        if (horizontalMatchingTiles.Count >= 3)
        {
            matchingTiles.AddRange(horizontalMatchingTiles);
        }

        if (verticalMatchingTiles.Count >= 3)
        {
            if (matchingTiles.Contains(this))
            {
                verticalMatchingTiles.Remove(this);
            }
            matchingTiles.AddRange(verticalMatchingTiles);
        }

        return matchingTiles;
    }

    /// <summary>
    /// Description:
    /// Returns all tiles that horizontally matches with this
    /// Input: 
    /// none
    /// Return:
    /// List<Tile>
    /// </summary>
    /// <returns>All Horizontal matching tiles</returns>
    public List<Tile> GetHorizontalMatchs()
    {
        List<Tile> matchs = new List<Tile> { this };

        isChecked = true;

        if (leftTile != null && !leftTile.isChecked && leftTile.piece.IsEqualto(piece))
        {
            matchs.AddRange(leftTile.GetHorizontalMatchs());
        }

        if (rightTile != null && !rightTile.isChecked && rightTile.piece.IsEqualto(piece))
        {
            matchs.AddRange(rightTile.GetHorizontalMatchs());
        }

        isChecked = false;

        return matchs;
    }

    /// <summary>
    /// Description:
    /// Returns all tiles that vertically matches with this
    /// Input: 
    /// none
    /// Return:
    /// List<Tile>
    /// </summary>
    /// <returns>All Vertical matching tiles</returns>
    public List<Tile> GetVerticalMatchs()
    {
        List<Tile> matchs = new List<Tile> { this };

        isChecked = true;

        if(upperTile != null && !upperTile.isChecked && upperTile.piece.IsEqualto(piece))
        {
            matchs.AddRange(upperTile.GetVerticalMatchs());
        }

        if (lowerTile != null && !lowerTile.isChecked && lowerTile.piece.IsEqualto(piece))
        {
            matchs.AddRange(lowerTile.GetVerticalMatchs());
        }

        isChecked = false;

        return matchs;
    }

    /// <summary>
    /// Description:
    /// Destroy the piece on the tile
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void Explode()
    {
        isExploding = true;
        switch (piece.type)
        {
            case PieceType.NORMAL:
                break;

            case PieceType.H_BONUS:
                foreach (Tile t in row)
                {
                    if (!t.isExploding && t.piece != null)
                    {
                        if (t.isMarked)
                        {
                            t.isMarked = false;
                        }
                        t.Explode();
                    }
                }
                break;

            case PieceType.V_BONUS:
                foreach (Tile t in column)
                {
                    if (!t.isExploding && t.piece != null)
                    {
                        if (t.isMarked)
                        {
                            t.isMarked = false;
                        }
                        t.Explode();
                    }
                }
                break;

            default:
                break;
        }
        if (piece != null) {
            Destroy(piece.gameObject);
            piece = null;
        }
        isExploding = false;
    }

    /// <summary>
    /// Description:
    /// Checks if this tile can have vertical match
    /// Input:
    /// none
    /// Return:
    /// bool
    /// </summary>
    /// <returns> True if it can have match, else unless</returns>
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

    /// <summary>
    /// Description:
    /// Checks if this tile can have horizontal match
    /// Input:
    /// none
    /// Return:
    /// bool
    /// </summary>
    /// <returns> True if it can have match, else unless</returns>
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

    #endregion // Functions
}
