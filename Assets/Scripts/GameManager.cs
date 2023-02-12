using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static event Action<GameObject> OnInstantiatedCopy;

	public int NumberOfPrimitives { get; set; } = 1;
	public int Lives { get; set; } = 3;

    [SerializeField]
    private GameObject _cursor;
    [SerializeField]
    private GameObject _startGameCanvas;
    [SerializeField]
    private GameObject _percentCorrectCanvas;
    [SerializeField]
    private GameObject _hudCanvas;
    [SerializeField]
    private TextMeshProUGUI _percentCorrectTMP;
    [SerializeField]
    private TextMeshProUGUI _totalScoreTMP;
    [SerializeField]
    private GameObject _copyBlankPrefab;
    [SerializeField]
    private float _timerLength = 5f;
    [SerializeField]
    private Vector2 _originalPosition;
    [SerializeField]
    private Vector2 _copyPosition;
    [SerializeField]
    private float _minimumPercent = 70f;
    [SerializeField]
    private int _calculationGridWidth = 100;
    [SerializeField]
    private TextMeshProUGUI _livesText;
    [SerializeField]
    private TextMeshProUGUI _timerText;
    [SerializeField]
    private Image _timerImage;
    
    private float _timer;
    private bool _isDrawing = false;
    private RandomImageCreator _randomImageCreator;
    private GameObject _originalInstance;
    private GameObject _copyInstance;
    private float _totalScore = 0f; 

    private void Start()
    {
        _randomImageCreator = FindObjectOfType<RandomImageCreator>();

        _totalScoreTMP.text = "Copy the original images as closely as you can";

        InitializeRound();
    }

    private void InitializeRound()
    {
        NumberOfPrimitives = 1;
        Lives = 3;
        _livesText.text = "Attempts Remaining: " + Lives.ToString();
        _totalScore = 0f;
        _timer = _timerLength;
        _timerImage.fillAmount = 0f;
        _timerText.text = Mathf.RoundToInt(_timerLength).ToString();
        _cursor.SetActive(false);
        _isDrawing = false;
        _startGameCanvas.SetActive(true);
        _hudCanvas.SetActive(false);
    }

    // Start button enables cursor and set GameState to Draw. 
    public void StartGame()
    {
        // Set _isDrawing to true. 
        _isDrawing = true;

        // Create random image. 
        _originalInstance = _randomImageCreator.CreateRandomImage(NumberOfPrimitives);

        // Set random image. 
        _originalInstance.transform.position = _originalPosition;

        // Set cursor to active. 
        _cursor.SetActive(true);

        // Set blank copy canvas. 
        _copyInstance = Instantiate(_copyBlankPrefab, _copyPosition, Quaternion.identity);
        OnInstantiatedCopy.Invoke(_copyInstance);

        // Deactivate Canvases. 
        _startGameCanvas.SetActive(false);
        _percentCorrectCanvas.SetActive(false);
        _hudCanvas.SetActive(true);

        Debug.Log("End of StartGame method");
    }

    private void Update()
    {
        if(_isDrawing)
        {
            _timer -= Time.deltaTime;

            // Set timer HUD. 
            int countdown = Mathf.CeilToInt(_timer);
            float percentDone = 1f - (_timer / _timerLength);
            _timerText.text = countdown.ToString();
            _timerImage.fillAmount = percentDone;

            if (_timer <= 0f)
            {
                EndRound();
            }
        }
    }

    private void EndRound()
    {
        // Reset Timer. 
        _timer = _timerLength;
        _timerImage.fillAmount = 0f;
        _timerText.text = Mathf.RoundToInt(_timerLength).ToString();

        // Disable cursor. 
        _cursor.SetActive(false);

        // Set _isDrawing back to false so update doesn't run. 
        _isDrawing = false;

        // Increase number of primitives for next round. 
        NumberOfPrimitives++;

        // Calculate percent correct. 
        // How to get reference to completed copy here? 
        float percentCorrect = CalculatePercentCorrect();

        // Destroy original and copy instances. 
        Destroy(_originalInstance);
        Destroy(_copyInstance);
        _originalInstance = null;
        _copyInstance = null;

        // If not correct enough, lose a "life". 
        if (percentCorrect < _minimumPercent)
        {
            Lives--;
            _livesText.text = "Attempts Remaining: " + Lives.ToString();

            // If out of lives, Game Over. 
            if (Lives <= 0)
            {
                GameOver(percentCorrect);
                return;
            }

            // Setup and activate percent correct canvas. 
            _percentCorrectTMP.text = percentCorrect.ToString() + "%" + 
                "\n" + "Copied Correctly" +
                "\n" + "Not Good Enough!" + 
                "\n" + Lives.ToString() + " Attempts Remaining";
            _percentCorrectCanvas.SetActive(true);
            _hudCanvas.SetActive(false);
        }
        // If correct enough, add percent to total score. 
        else
        {
            _totalScore += percentCorrect;

            // Setup and activate percent correct canvas. 
            _percentCorrectTMP.text = percentCorrect.ToString() + "%" + 
                "\n" + "Copied Correctly" +
                "\n" + "Great Job!"+ 
                "\n" + "Total Score: " + _totalScore;
            _percentCorrectCanvas.SetActive(true);
            _hudCanvas.SetActive(false);
        }
    }

    private void GameOver(float percentCorrect)
    {
        // Set score on canvas. 
        _totalScoreTMP.text = percentCorrect.ToString() + "%" + 
                "\n" + "Copied Correctly" + 
                "\n" + "You're clearly broken." +
                "\n" + "In the trash with you!" + 
                "\n" + "Total Score: " + _totalScore;

        // _totalScoreTMP.text = "Total Score" + "\n" + _totalScore.ToString();

        // Activate Start Game Canvas. 
        _startGameCanvas.gameObject.SetActive(true);
        _hudCanvas.SetActive(false);

        // Initialize Round
        InitializeRound();
    }

    private float CalculatePercentCorrect()
    {
        int numberOfSimilarTiles = 0;

        for (int i = -Mathf.FloorToInt(_calculationGridWidth / 2); i < Mathf.CeilToInt(_calculationGridWidth / 2); i++)
        {
            for (int j = -Mathf.FloorToInt(_calculationGridWidth / 2); j < Mathf.CeilToInt(_calculationGridWidth / 2); j++)
            {
                float x = (float)i / (float)_calculationGridWidth;
                float y = (float)j / (float)_calculationGridWidth;

                Vector3 originalPosition = _originalInstance.transform.TransformPoint(new Vector3(x, y, 0f));
                Vector3 copyPosition = _copyInstance.transform.TransformPoint(new Vector3(x, y, 0f));

                //Debug.Log("i = " + i + ", j = " + j);
               // Debug.Log("(x,y) = " + "(" + x + "," + y + ")");
                Debug.Log("Original: " + originalPosition + ", Copy: " + copyPosition);

               // Debug.Log("Original Tile [" + i + ", " + j + "] has ink on it: " + Physics2D.Raycast(originalPosition - (Vector3.down * 0.001f), Vector3.up, 0.002f));
               // Debug.Log("Copy Tile [" + i + ", " + j + "] has ink on it: " + Physics2D.Raycast(originalPosition - (Vector3.down * 0.001f), Vector3.up, 0.002f));
                Debug.Log("Tiles similar: " + (Physics2D.Raycast(originalPosition - (Vector3.down * 0.001f), Vector3.up, 0.002f)
                    == Physics2D.Raycast(copyPosition - (Vector3.down * 0.001f), Vector3.up, 0.002f)));

                if (Physics2D.Raycast(originalPosition - (Vector3.down * 0.001f), Vector3.up, 0.002f)
                    == Physics2D.Raycast(copyPosition - (Vector3.down * 0.001f), Vector3.up, 0.002f))
                {
                    numberOfSimilarTiles++;
                }
            }
        }

        return 100f * (float)numberOfSimilarTiles / ((float)_calculationGridWidth * (float)_calculationGridWidth);
    }
}