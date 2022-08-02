using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    [Tooltip("The prefab used for Tiles")]
    public GameObject tilePrefab;

    [Tooltip("The prefab used for Pieces")]
    public List<GameObject> piecePrefabs = new List<GameObject>();

    [Tooltip("The grid's width")]
    public int width = 5;

    [Tooltip("The grid's height")]
    public int height = 5;

    [Tooltip("The space between the same corners of two consecutive tiles")]
    public float step = 1.2f;

    [HideInInspector]
    public List<Tile> markedTiles = new List<Tile>();

    // the duration of each tween
    private float tweenDuration = .25f;

    /// <summary>
    /// Description:
    /// Draws the Game Board and setups each piece
    /// Input:
    /// none
    /// Return:
    /// void(no return)
    /// </summary>
    public void InitGame()
    {
        for (int y = 0; y < height; y++) // Raws
        {
            for (int x = 0; x < width; x++) // Columns
            {
                Vector2 tilePosition = new Vector2(x * step, y * step);
                Tile tile = Instantiate<GameObject>(tilePrefab, tilePosition, transform.rotation, transform).GetComponent<Tile>();
                tile.name = $"[{x},{y}]"; // Tile Identification

                //Tile Content
                GameObject tilePiece;
                tilePiece = piecePrefabs[UnityEngine.Random.Range(0, piecePrefabs.Count)];

                tile.CreatePiece(tilePiece);


                // Board Style
                if (x % 2 == y % 2)
                {
                    tile.GetComponent<SpriteRenderer>().color = Color.gray;
                }
            }
        }

        Camera.main.transform.position = new Vector3(width * step / 2, height * step / 2, Camera.main.transform.position.z);
    }

    public async Task Swap(Tile tile1, Tile tile2)
    {
        Piece tile1Piece = tile1.piece;
        Piece tile2Piece = tile2.piece;

        // Exchange pieces
        Sequence sequence = DOTween.Sequence();

        sequence.Join(tile1Piece.transform.DOMove(tile2.transform.position, tweenDuration))
            .Join(tile2Piece.transform.DOMove(tile1.transform.position, tweenDuration));


        tile1Piece.transform.SetParent(tile2.transform);
        tile2Piece.transform.SetParent(tile1.transform);

        tile1.piece = tile2Piece;
        tile2.piece = tile1Piece;

        await sequence.Play().AsyncWaitForCompletion();

        // Check if the move creates match
        CheckForMatchingTiles();

        if (markedTiles.Count <= 0)
        {
            Sequence _sequence = DOTween.Sequence();

            _sequence.Join(tile1Piece.transform.DOMove(tile2Piece.transform.position, tweenDuration))
                .Join(tile2Piece.transform.DOMove(tile1Piece.transform.position, tweenDuration));

            tile1Piece.transform.SetParent(tile1.transform);
            tile2Piece.transform.SetParent(tile2.transform);

            tile1.piece = tile1Piece;
            tile2.piece = tile2Piece;

            await _sequence.Play().AsyncWaitForCompletion();
        }
        else
        {
            // Explode matching tiles
            ClearMatchingTiles();
        }
    }

    public void CheckForMatchingTiles()
    {
        for (int y = 0; y < height; y++) // Raws
        {
            for (int x = 0; x < width; x++) // Columns
            {
                Tile currentTile = GameObject.Find($"[{x},{y}]").GetComponent<Tile>();
                currentTile.GetMatchs();
            }
        }
    }

    public void ClearMatchingTiles()
    {
        if (markedTiles.Count > 0)
        {
            foreach (Tile tile in markedTiles)
            {
                tile.Explode();
            }

            markedTiles = new List<Tile>();
            RefillBoard();
        }
    }

    private async void RefillBoard()
    {
        Sequence refillSequence = DOTween.Sequence();
        for (int y = 0; y < height; y++) // Raws
        {
            for (int x = 0; x < width; x++) // Columns
            {
                Tile currentTile = GameObject.Find($"[{x},{y}]").GetComponent<Tile>();

                if (currentTile.piece == null)
                {
                    if (currentTile.upperTile != null)
                    {
                        Tile cursTile = currentTile;

                        while (cursTile.upperTile != null && cursTile.upperTile.piece == null)
                        {
                            cursTile = cursTile.upperTile;
                        }

                        if (cursTile.upperTile == null)
                        {
                            cursTile.CreatePiece(piecePrefabs[UnityEngine.Random.Range(0, piecePrefabs.Count)]);
                            refillSequence.Join(cursTile.piece.MoveToTile(currentTile));
                            cursTile.piece = null;
                        }
                        else
                        {
                            cursTile = cursTile.upperTile;
                            refillSequence.Join(cursTile.piece.MoveToTile(currentTile));
                            cursTile.piece = null;
                        }
                    }
                    else
                    {
                        currentTile.CreatePiece(piecePrefabs[UnityEngine.Random.Range(0, piecePrefabs.Count)]);
                    }
                }
            }
        }
        await refillSequence.Play().AsyncWaitForCompletion();

        CheckForMatchingTiles();

        if (markedTiles.Count > 0)
        {
            ClearMatchingTiles();
        }
        else if (!MatchIsPossible())
        {
            Debug.Log("Shuffle");
            Shuffle();
        }


    }

    private bool MatchIsPossible()
    {
        for (int y = 0; y < height; y++) // Raws
        {
            for (int x = 0; x < width; x++) // Columns
            {
                Tile currentTile = GameObject.Find($"[{x},{y}]").GetComponent<Tile>();
                if (currentTile.MayHaveHorizontalMatch() || currentTile.MayHaveVerticalMatch())
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Shuffle() // TODO: Start Over
    {
        System.Random rand = new System.Random();

        List<Piece> piecesOnBoard = new List<Piece>();
        List<Tile> tilesOnBoard = new List<Tile>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile currentTile = GameObject.Find($"[{x},{y}]").GetComponent<Tile>();
                piecesOnBoard.Add(currentTile.piece);
                currentTile.piece = null;
                tilesOnBoard.Add(currentTile);
            }
        }


        List<Piece> shuffled = piecesOnBoard.OrderBy(_ => rand.Next()).ToList();
        Debug.Log(piecesOnBoard);
        Debug.Log(shuffled);
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < shuffled.Count; i++)
        {
            sequence.Join(shuffled[i].transform.DOScale(Vector2.zero, tweenDuration));
            //Tile tile = tilesOnBoard[i];
            //Piece piece = shuffled[i];
            //piece.MoveToTile(tile);
        }
    }
}
