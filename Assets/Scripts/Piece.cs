using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public enum PieceColor
{
    BLUE,
    GREEN,
    ORANGE,
    PURPLE,
    RED,
    YELLOW,
}

public enum PieceType
{
    NORMAL, 
    H_BONUS, 
    V_BONUS, 
}

public class Piece : MonoBehaviour
{
    public PieceColor color;

    public PieceType type;

    public Sprite[] sprites = new Sprite[3];

    private float tweenDuration = .25f;

    private void Start()
    {
        SetColor();
        SetType();
    }

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

    public void SetType(PieceType _type = PieceType.NORMAL)
    {
        type = _type;
        GetComponent<SpriteRenderer>().sprite = sprites[(int) type]; // Each type is numbered begining from 0
    }

    public Tween GoToTile(Tile target)
    {
        transform.SetParent(target.GetComponent<Transform>());
        target.piece = this;

        return MoveToTile(target);

    }

    public Tween MoveToTile(Tile target)
    {
        return transform.DOMove(target.transform.position, tweenDuration).SetAutoKill(true);
    }

    public bool IsEqualto(Piece piece)
    {
        return piece.color == color;
    }
}
