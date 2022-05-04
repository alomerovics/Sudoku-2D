using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    public int columns = 0;
    public int rows = 0;
    public float squareOffset = 0.0f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0.0f, 0.0f);
    public float squareScale = 1.0f;
    public float square_gap = 0.1f;
    public Color line_highlight_color = Color.red;

    private List<GameObject> gridSquares = new List<GameObject>();
    private int selectedGridData = -1;
    // Start is called before the first frame update
    void Start()
    {
        if (gridSquare.GetComponent<GridSquare>() == null)
        {
            Debug.Log("Bu objede GridSquare sctipti tanýmlý deðil.");
        }
        CreateGrid();
        if (GameSettings.instance.GetContinuePreviousGame())
        {
            SetGridFromFile();
        }
        else
        {
            SetGridNumber(level: GameSettings.instance.GetGameMode());
        }
    }
    void SetGridFromFile()
    {
        string level = GameSettings.instance.GetGameMode();
        selectedGridData = Config.ReadGameBoardLevel();
        var data = Config.ReadGridData();

        SetGridSquareData(data);
        SetGridNotes(Config.GetGridNotes());
    }
    private void SetGridNotes(Dictionary<int, List<int>> notes)
    {
        foreach (var note in notes)
        {
            gridSquares[note.Key].GetComponent<GridSquare>().SetGridNotes(note.Value);

        }
    }
    private void CreateGrid()
    {
        SpawnGridSquares();
        SetSquaresPosition();
    }
    private void SpawnGridSquares()
    {
        int squareIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                gridSquares.Add(Instantiate(gridSquare) as GameObject);
                gridSquares[gridSquares.Count - 1].GetComponent<GridSquare>().SetSquareIndex(squareIndex);
                gridSquares[gridSquares.Count -
                    1].transform.parent = this.transform;
                gridSquares[gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                squareIndex++;
            }
        }
    }
    private void SetSquaresPosition()
    {
        var squareRect = gridSquares[0].GetComponent<RectTransform>();
        Vector2 offset = new Vector2();
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;
        offset.x = squareRect.rect.width * squareRect.transform.localScale.x + squareOffset;
        offset.y = squareRect.rect.height * squareRect.transform.localScale.y + squareOffset;

        int columnNumber = 0;
        int rowNumber = 0;
        foreach (GameObject square in gridSquares)
        {
            if (columnNumber + 1 > columns)
            {
                rowNumber++;
                columnNumber = 0;
                square_gap_number.x = 0;
                row_moved = false;
            }
            var posXOffset = offset.x * columnNumber + (square_gap_number.x * square_gap);
            var posYOffset = offset.y * rowNumber + (square_gap_number.y * square_gap);
            if (columnNumber > 0 && columnNumber % 3 == 0)
            {
                square_gap_number.x++;
                posXOffset += square_gap;
            }
            if (rowNumber > 0 && rowNumber % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                posYOffset += square_gap;
            }
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + posXOffset, startPosition.y - posYOffset);
            columnNumber++;
        }
    }
    private void SetGridNumber(string level)
    {
        selectedGridData = Random.Range(0, SudokuData.Instance.sudokuGame[level].Count);
        var data = SudokuData.Instance.sudokuGame[level][selectedGridData];
        SetGridSquareData(data);
        //foreach (var square in gridSquares)
        //{
        //    square.GetComponent<GridSquare>().SetNumber(Random.Range(0, 10));
        //}
    }
    private void SetGridSquareData(SudokuData.SudokuBoardData data)
    {
        for (int index = 0; index < gridSquares.Count; index++)
        {
            gridSquares[index].GetComponent<GridSquare>().SetNumber(data.unSolvedData[index]);
            gridSquares[index].GetComponent<GridSquare>().SetCorrectNumber(data.solvedData[index]);
            gridSquares[index].GetComponent<GridSquare>().SetHasDefaultValue(data.unSolvedData[index] != 0 && data.unSolvedData[index] == data.solvedData[index]);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnSquareSelected += OnSquareSelected;
        GameEvents.OnUpdateSquareNumber += CheckBoardCompleted;
    }
    private void OnDisable()
    {
        GameEvents.OnSquareSelected -= OnSquareSelected;
        GameEvents.OnUpdateSquareNumber -= CheckBoardCompleted;
        //*****************************************************
        var solved_data = SudokuData.Instance.sudokuGame[GameSettings.instance.GetGameMode()][selectedGridData].solvedData;
        int[] unsolved_data = new int[81];
        Dictionary<string, List<string>> grid_notes = new Dictionary<string, List<string>>();
        for (int i = 0; i < gridSquares.Count; i++)
        {
            var comp = gridSquares[i].GetComponent<GridSquare>();
            unsolved_data[i] = comp.GetSquareNumber();
            string key = "square_note:" + i.ToString();
            grid_notes.Add(key, comp.GetSquareNotes());
        }
        SudokuData.SudokuBoardData current_game_data = new SudokuData.SudokuBoardData(unsolved_data, solved_data);
        if (GameSettings.instance.GetExitAfterWon())// 
        {
            Config.SaveBoardData(current_game_data, 
                level: GameSettings.instance.GetGameMode(), 
                board_index: selectedGridData, 
                error_number: Lives.instance.GetErrorNumbers(), 
                grid_notes);
        }
        else
        {
            Config.DeleteDataFile();
        }
    }
    private void SetSquaresColor(int[] data, Color col)
    {
        foreach (var index in data)
        {
            var comp = gridSquares[index].GetComponent<GridSquare>();
            if (comp.HasWrongValue()==false && comp.IsSelected() == false)
            {
                comp.SetSquareColour(col);
            }
        }
    }
    public void OnSquareSelected(int square_index)
    {
        var horizontal_line = LineIndicator.instance.GetHorizontalLine(square_index);
        var vertical_line = LineIndicator.instance.GetVerticalLine(square_index);
        var square = LineIndicator.instance.GetSquare(square_index);

        SetSquaresColor(LineIndicator.instance.GetAllSquaresIndexes(), Color.white);
        SetSquaresColor(horizontal_line, line_highlight_color);
        SetSquaresColor(vertical_line, line_highlight_color);
        SetSquaresColor(square, line_highlight_color);
    }
    private void CheckBoardCompleted(int number)
    {
        foreach (var square in gridSquares)
        {
            var comp = square.GetComponent<GridSquare>();
            if (comp.IsCorrectNumberSet()==false)
            {
                return;
            }
        }
        GameEvents.OnBoardCompletedMethod();
    }
    public void SolveSudoku()
    {
        foreach (var square in gridSquares)
        {
            var comp = square.GetComponent<GridSquare>();
            comp.SetCorrectNumber();

        }
        CheckBoardCompleted(0);
    }
}
