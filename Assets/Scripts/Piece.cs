using UnityEngine;
using DG.Tweening;

/// <summary>
/// Description:
/// Possible Piece Color
/// </summary>
public enum PieceColor
{
    BLUE,
    GREEN,
    ORANGE,
    PURPLE,
    RED,
    YELLOW,
}

/// <summary>
/// Description:
/// Possible Piece Type
/// </summary>
public enum PieceType
{
    NORMAL, 
    H_BONUS, 
    V_BONUS, 
}

public class Piece : MonoBehaviour
{
    #region Variables

    [Header("Pieces Details")]
    [Tooltip("The color of the piece")]
    public PieceColor color;
    [Tooltip("The type of the piece")]
    public PieceType type;
    [Tooltip("Piece Images")]
    public Sprite[] sprites = new Sprite[3];

    // Duration of a tween
    private readonly float tweenDuration = .25f;

    #endregion // Variables

    #region Functions
    /// <summary>
    /// Description:
    /// Standard Unity Function called Once before the first frame update
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void Start()
    {
        SetColor();
        SetType();
    }

    /// <summary>
    /// Description:
    /// Sets the color of the piece according to its prefab's name
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void SetColor()
    {
        if (name.Contains("Blue"))
        {
            color = PieceColor.BLUE;
        }
        else if (name.Contains("Green"))
        {
            color = PieceColor.GREEN;
        }
        else if (name.Contains("Orange"))
        {
            color = PieceColor.ORANGE;
        }
        else if (name.Contains("Purple"))
        {
            color = PieceColor.PURPLE;
        }
        else if (name.Contains("Red"))
        {
            color = PieceColor.RED;
        }
        else if (name.Contains("Yellow"))
        {
            color = PieceColor.YELLOW;
        }
    }

    /// <summary>
    /// Description:
    /// Changes the type of the piece
    /// Input:
    /// PieceType _type
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="_type">The type to assign to the piece</param>
    public void SetType(PieceType _type = PieceType.NORMAL)
    {
        type = _type;
        GetComponent<SpriteRenderer>().sprite = sprites[(int) type]; // Each type is numbered begining from 0
    }

    /// <summary>
    /// Description:
    /// Returns a tween for moving to the target tile and sets its piece to this
    /// Input:
    /// Tile target
    /// Return:
    /// Tween
    /// </summary>
    /// <param name="target">The tile to move to</param>
    /// <returns>The movement tween</returns>
    public Tween GoToTile(Tile target)
    {
        transform.SetParent(target.GetComponent<Transform>());
        target.piece = this;

        return MoveToTile(target);

    }

    /// <summary>
    /// Description:
    /// Returns a tween for moving to the target tile
    /// Input:
    /// Tile target
    /// Return:
    /// Tween
    /// </summary>
    /// <param name="target">The tile to move to</param>
    /// <returns>The movement tween</returns>
    public Tween MoveToTile(Tile target)
    {
        return transform.DOMove(target.transform.position, tweenDuration).SetAutoKill(true);
    }

    /// <summary>
    /// Description:
    /// Checks if the piece's coolor is equal to an other's one
    /// Input:
    /// Piece piece
    /// Return
    /// bool
    /// </summary>
    /// <param name="piece">The piece to compare to</param>
    /// <returns>Wheither if they are same color or not</returns>
    public bool IsEqualto(Piece piece)
    {
        return piece.color == color;
    }

    #endregion // Functions
}
