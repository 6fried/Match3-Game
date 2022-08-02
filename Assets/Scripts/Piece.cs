using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public enum PieceType
{
    BLUE,
    GREEN,
    ORANGE,
    PURPLE,
    RED,
    YELLOW,
}

public class Piece : MonoBehaviour
{
    public PieceType type;

    private float tweenDuration = .25f;

    private void Start()
    {
        SetType();
    }

    private void SetType()
    {
        if (name.Contains("Blue"))
        {
            type = PieceType.BLUE;
        }
        else if (name.Contains("Green"))
        {
            type = PieceType.GREEN;
        }
        else if (name.Contains("Orange"))
        {
            type = PieceType.ORANGE;
        }
        else if (name.Contains("Purple"))
        {
            type = PieceType.PURPLE;
        }
        else if (name.Contains("Red"))
        {
            type = PieceType.RED;
        }
        else if (name.Contains("Yellow"))
        {
            type = PieceType.YELLOW;
        }
    }

    public void GoToTile(Tile target)
    {
        transform.SetParent(target.GetComponent<Transform>());
        target.piece = this;
    }

    public Tween MoveToTile(Tile target)
    {
        GoToTile(target);

        transform.SetParent(target.transform);

        target.piece = this;

        return transform.DOMove(target.transform.position, tweenDuration).SetAutoKill(true);
    }

    public bool IsEqualto(Piece piece)
    {
        return piece.type == type;
    }
}
