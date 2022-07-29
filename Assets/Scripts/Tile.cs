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

    private void OnMouseDrag()
    {
        if (selected)
        {
            float step = GameManager.instance.board.step;
            Vector3 dragDirection = GetDragDirection();
            Debug.Log(dragDirection);
            if(Mathf.Abs(dragDirection.x) != Mathf.Abs(dragDirection.y))
            {
                Vector3 targetTileCoordinates = (transform.position + step * dragDirection)/step;
                GameObject targetTileObject = GameObject.Find($"[{targetTileCoordinates.x},{targetTileCoordinates.y}]");
                if (targetTileObject != null)
                {
                    Tile targetTile = targetTileObject.GetComponent<Tile>();
                    GameManager.instance.board.Swap(this, targetTile);
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
}
