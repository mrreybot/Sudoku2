using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SudokuBoard : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform gridParent;
    private SudokuCell[,] cells = new SudokuCell[9, 9];
    private int[,] sudokuPuzzle = new int[9, 9];
    private int[,] solvedSudoku = new int[9, 9];

    public GameObject playButton;

    void Start()
    {
        GenerateSudokuPuzzle();
        GenerateBoard();

        playButton.SetActive(false);
        Invoke("activeButton", 2);
    }

    void GenerateSudokuPuzzle()
    {
        solvedSudoku = GenerateSolvedSudoku();
        sudokuPuzzle = RemoveNumbersForPuzzle(solvedSudoku, 40);
    }

    int[,] GenerateSolvedSudoku()
    {
        int[,] grid = new int[9, 9];
        FillGrid(grid);
        return grid;
    }

    bool FillGrid(int[,] grid)
    {
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row, col] == 0)
                {
                    numbers.Shuffle();

                    foreach (int num in numbers)
                    {
                        if (IsValidNumber(grid, row, col, num))
                        {
                            grid[row, col] = num;

                            if (FillGrid(grid))
                                return true;

                            grid[row, col] = 0;
                        }
                    }
                    return false;
                }
            }
        }
        return true;
    }

    int[,] RemoveNumbersForPuzzle(int[,] grid, int removeCount)
    {
        int[,] puzzle = (int[,])grid.Clone();
        int removed = 0;

        while (removed < removeCount)
        {
            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);

            if (puzzle[row, col] != 0)
            {
                puzzle[row, col] = 0;
                removed++;
            }
        }
        return puzzle;
    }

    void GenerateBoard()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                GameObject cellObj = Instantiate(cellPrefab, gridParent);
                SudokuCell cell = cellObj.GetComponent<SudokuCell>();

                cells[row, col] = cell;
                cell.row = row;
                cell.col = col;

                if (sudokuPuzzle[row, col] != 0)
                {
                    cell.SetNumber(sudokuPuzzle[row, col], true);
                }
            }
        }
    }

    public void SolveSudoku()
    {
        StartCoroutine(SolveStepByStep());
    }

    private IEnumerator SolveStepByStep()

    {
        yield return new WaitForSeconds(0.1f);
        if (!BacktrackSolve(sudokuPuzzle))
        {
            Debug.Log("No solution exists!");
        }
    }

    private bool BacktrackSolve(int[,] grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row, col] == 0)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (IsValidNumber(grid, row, col, num))
                        {
                            grid[row, col] = num;
                            cells[row, col].SetNumber(num, false);

                            if (BacktrackSolve(grid))
                                return true;

                            grid[row, col] = 0;
                            cells[row, col].SetNumber(0, false);
                        }
                    }
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsValidNumber(int[,] grid, int row, int col, int num)
    {
        for (int i = 0; i < 9; i++)
        {
            if (grid[row, i] == num || grid[i, col] == num)
                return false;
        }
        int startRow = (row / 3) * 3;
        int startCol = (col / 3) * 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[startRow + i, startCol + j] == num)
                    return false;
            }
        }
        return true;
    }

    // GET HINT Fonksiyonu
    public void GetHint()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (sudokuPuzzle[row, col] == 0)
                {
                    emptyCells.Add(new Vector2Int(row, col));
                }
            }
        }

        if (emptyCells.Count == 0)
        {
            Debug.Log("No more hints available!");
            return;
        }

        Vector2Int hintCell = emptyCells[Random.Range(0, emptyCells.Count)];
        int correctNumber = solvedSudoku[hintCell.x, hintCell.y];

        sudokuPuzzle[hintCell.x, hintCell.y] = correctNumber;
        cells[hintCell.x, hintCell.y].SetNumber(correctNumber, true);
    }

    // PLAY AGAIN Fonksiyonu
    public void PlayAgain()
    {
        playButton.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }
    void activeButton()
    {
        playButton.SetActive(true);
    }

}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}


