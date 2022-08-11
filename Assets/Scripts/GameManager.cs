using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Vaviables

    [Header("Game Settings")]
    [Tooltip("The Game Manager of the actual sequence")]
    public static GameManager instance;

    [Tooltip("The main Board of the game.")]
    public BoardManager board;

    #endregion

    #region Functions

    /// <summary>
    /// Description:
    /// Standard Unity Function called once when the script is loaded
    /// Input:
    /// none
    /// Return:
    /// void(no return)
    /// </summary>
    private void Awake()
    {
        SetupGameManager();
    }

    /// <summary>
    /// Description:
    /// Setups the Game Manager instance of the current game and makes sure there is exactly one Game Manager
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void SetupGameManager()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity Function called once before the first Update call
    /// Input:
    /// none
    /// Return:
    /// void(no return)
    /// </summary>
    private void Start()
    {
        board.InitGame();
    }

    #endregion // Functions
}
