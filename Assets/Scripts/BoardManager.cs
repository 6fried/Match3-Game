using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
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
                //do
                //{
                    tilePiece = piecePrefabs[Random.Range(0, piecePrefabs.Count)];
                //}
                //while ((tile.lowerTile != null && tile.lowerTile.HasMatchFor(tilePiece.GetComponent<Piece>())) || (tile.leftTile != null && tile.leftTile.HasMatchFor(tilePiece.GetComponent<Piece>())));

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

        // Check if there are matching tiles
        CheckForMatchingTiles();
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
                            cursTile.CreatePiece(piecePrefabs[Random.Range(0, piecePrefabs.Count)]);
                            refillSequence.Join(cursTile.piece.MoveToTile(currentTile));
                            //await cursTile.piece.MoveToTile(currentTile); // TODO: Tweening
                            cursTile.piece = null;
                        }
                        else
                        {
                            cursTile = cursTile.upperTile;
                            refillSequence.Join(cursTile.piece.MoveToTile(currentTile));
                            //await cursTile.piece.MoveToTile(currentTile); // TODO: Tweening
                            cursTile.piece = null;
                        }
                    }
                    else
                    {
                        currentTile.CreatePiece(piecePrefabs[Random.Range(0, piecePrefabs.Count)]);
                    }
                }
            }
        }
        await refillSequence.Play().AsyncWaitForCompletion();
        CheckForMatchingTiles();
    }

}
