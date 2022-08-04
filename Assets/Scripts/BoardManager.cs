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
    public List<List<Tile>> markedTiles = new List<List<Tile>>();

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

        for (int y = 0; y < height; y++) // Raws
        {
            for (int x = 0; x < width; x++) // Columns
            {
                Tile currentTile = GameObject.Find($"[{x},{y}]").GetComponent<Tile>();
                currentTile.SetRow();
                currentTile.SetColumn();
            }
        }

        Camera.main.transform.position = new Vector3((width-1) * step / 2, (height-1) * step / 2, Camera.main.transform.position.z);
        Camera.main.orthographicSize = width + 2;
        CheckForMatchingTiles();
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
        SearchMatchingTiles();

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

    private void CheckForMatchingTiles()
    {
        SearchMatchingTiles();

        if (markedTiles.Count > 0)
        {
            ClearMatchingTiles();
        }
        else if (!MatchingMoveIsPossible())
        {
            Shuffle();
        }
    }

    public void SearchMatchingTiles()
    {
        for (int y = 0; y < height; y++) // Raws
        {
            for (int x = 0; x < width; x++) // Columns
            {
                Tile currentTile = GameObject.Find($"[{x},{y}]").GetComponent<Tile>();

                List<Tile> currentTileMatchs = currentTile.GetMatchs();
                if (!currentTile.isMarked && currentTileMatchs.Count >= 3)
                {
                    foreach (Tile t in currentTileMatchs)
                    {
                        t.isMarked = true;
                    }
                    markedTiles.Add(currentTileMatchs);
                }
            }
        }
    }

    public void ClearMatchingTiles()
    {
        if (markedTiles.Count > 0)
        {
            foreach (List<Tile> tiles in markedTiles)
            {
                if (tiles.Count > 3)
                {
                    tiles[0].piece.SetType(PieceType.H_BONUS);
                    tiles[0].isMarked = false;
                }
                foreach (Tile tile in tiles)
                {
                    if (tile.isMarked)
                    {
                        tile.Explode();
                        tile.isMarked = false;
                    }
                }
            }

            markedTiles = new List<List<Tile>>();
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
                            refillSequence.Join(cursTile.piece.GoToTile(currentTile));
                            cursTile.piece = null;
                        }
                        else
                        {
                            cursTile = cursTile.upperTile;
                            refillSequence.Join(cursTile.piece.GoToTile(currentTile));
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
    }

    private bool MatchingMoveIsPossible()
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

    private async void Shuffle() // TODO: Start Over
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

        Sequence shuffleSequence = DOTween.Sequence();

        for (int i = 0; i < shuffled.Count; i++)
        {
            shuffleSequence.Join(shuffled[i].GoToTile(tilesOnBoard[i]));
        }

        await shuffleSequence.Play().AsyncWaitForCompletion();

        CheckForMatchingTiles();

    }
}
