using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using System;


public class StackModel : MonoBehaviour
{
    public CellStack stack;
    public ImageInitializer imageInitializer;
    public float threshold = 0.4f;
    public CellularAutomataUIController uiController;

    public int[,] currentState;
    public int[,] nextState;

    public List<int[,]> layerStates; // List to store each layer's 2D state

    public Index2[] offsets;

    public int currentLayer = -1;

    private CsvExporter csvExporter;

    //Setup some possible rules

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        // create model
        currentState = new int[stack.columnCount, stack.rowCount];
        nextState = new int[stack.columnCount, stack.rowCount];

        // define the size of the neighborhood
        offsets = Neighborhood;

        // Initialize the list of 2D arrays to hold states for each layer
        layerStates = new List<int[,]>();

        // initialize model
        Initialize(currentState);
        imageInitializer.Initialize(currentState);

        // Initialize CsvExporter with a path
        CsvExporter exporter = new CsvExporter("Assets/ExportedData/CellPositions.csv");

    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        // bail if stack is full
        if (currentLayer == stack.layerCount - 1)
            return;

        // advance later
        currentLayer++;

        // advance model
        // Step();

        //advance model with My CA rule
        MyCARuleStep();

        // update cells in the stack
        UpdateStack();
    }


    /// <summary>
    /// 
    /// </summary>
    public void ResetModel()
    {
        // reset cell states
        foreach (var layer in stack.layers)
        {
            foreach (var cell in layer.cells)
                cell.state = 0;
        }

        // re-initialize model
        Initialize(currentState);

        // reset layer
        currentLayer = -1;
    }


    /// <summary>
    /// 
    /// </summary>
    public void UpdateStack()
    {
        int[,] currState = currentState;
        Cell[,] currCells = stack.layers[currentLayer].cells;

        int nrows = stack.rowCount;
        int ncols = stack.columnCount;

        // set cell state
        for (int i = 0; i < nrows; i++)
        {
            for (int j = 0; j < ncols; j++)
                currCells[i, j].state = currState[i, j];

        }

        // After updating the layer, copy the currentState into a new 2D array and add it to the list
        int[,] stateCopy = new int[nrows, ncols];
        Array.Copy(currentState, stateCopy, currentState.Length);
        layerStates.Add(stateCopy); // Store the current layer's state

        if (currentLayer == stack.layerCount - 1)
            ExportAllLayersToCsv();


    }


    public void ExportAllLayersToCsv()
    {
        CsvExporter exporter = new CsvExporter("Assets/ExportedData/CellPositions.csv");

        for (int layer = 0; layer < layerStates.Count; layer++)
        {
            int[,] layerState = layerStates[layer];
            exporter.ExportLayerData(layerState, layer, stack.rowCount, stack.columnCount);
        }
    }


    /// <summary>
    /// Initialize the specified state.
    /// </summary>
    /// <param name="state">State.</param>
    public void Initialize(int[,] state)
    {
        int nrows = state.GetLength(0);
        int ncols = state.GetLength(1);

        for (int i = 0; i < nrows; i++)
        {
            for (int j = 0; j < ncols; j++)
            {
                if (UnityEngine.Random.Range(0.0f, 1.0f) > threshold)
                    state[i, j] = 1;
                else
                    state[i, j] = 0;
            }
        }
    }

    /// <summary>
    /// Step this instance.
    /// </summary>

    public void Step()
    {
        Debug.Log("I am in the step method");
        int countX = currentState.GetLength(0);
        int countY = currentState.GetLength(1);

        // calculate next state
        for (int i = 0; i < countX; i++)
        {
            for (int j = 0; j < countY; j++)
            {
                int state = currentState[i, j];

                int sum = GetNeighborSum(i, j);

                if (state == 1) //alive cell rule
                {
                    if (sum < 2 || sum > 5)
                        nextState[i, j] = 0;
                    else if (sum >= 2 && sum <= 3)
                        nextState[i, j] = 1;
                }
                else if (state == 0)//dead cell rule
                {
                    if (sum == 3)
                        nextState[i, j] = 1;
                    else
                        nextState[i, j] = 0;
                }
            }
        }

        // swap state buffers
        var temp = currentState;
        currentState = nextState;
        nextState = temp;
    }

    //Rule combination-Change Rules based on Layers : TODO
    public void MyCARuleStep()
    {
        int countX = currentState.GetLength(0);
        int countY = currentState.GetLength(1);

        // Get the rule for the current layer from UI controller
        CARule currentRule = GetRuleForCurrentLayer();

        for (int i = 0; i < countX; i++)
        {
            for (int j = 0; j < countY; j++)
            {
                int state = currentState[i, j];
                int sum = GetNeighborSum(i, j);

                // Apply the rule based on UI configuration
                if (state == 1) // alive cell rule
                {
                    if (sum < currentRule.survivalMin || sum > currentRule.survivalMax)
                        nextState[i, j] = 0;
                    else if (sum >= currentRule.survivalMin && sum <= currentRule.survivalMax)
                        nextState[i, j] = 1;
                }
                else if (state == 0) // dead cell rule
                {
                    if (sum >= currentRule.birthMin && sum <= currentRule.birthMax)
                        nextState[i, j] = 1;
                    else
                        nextState[i, j] = 0;
                }
            }
        }

        // swap state buffers
        var temp = currentState;
        currentState = nextState;
        nextState = temp;
    }

    private CARule GetRuleForCurrentLayer()
    {
        if (uiController != null)
        {
            return uiController.GetRuleSet().GetRuleForLayer(currentLayer);
        }
        else
        {
            // Fallback to default rules if no UI controller
            if (currentLayer < 20)
                return new CARule(0, 19, 2, 3, 3, 3, "Conway's Life");
            else if (currentLayer < 40)
                return new CARule(20, 39, 2, 4, 4, 6, "Extended Life");
            else
                return new CARule(40, 59, 2, 5, 2, 5, "Permissive Life");
        }
    }

    /// <summary>
    /// Gets the neighbor sum.
    /// </summary>
    /// <returns>The neighbor sum.</returns>
    /// <param name="i0">I0.</param>
    /// <param name="j0">J0.</param>
    public int GetNeighborSum(int i0, int j0)
    {
        var current = currentState;
        int nrows = current.GetLength(0);
        int ncols = current.GetLength(1);
        int sum = 0;

        foreach (Index2 offset in offsets)
        {
            int i1 = Wrap(i0 + offset.I, nrows);
            int j1 = Wrap(j0 + offset.J, ncols);

            if (current[i1, j1] > 0)
                sum++;
        }

        return sum;
    }

    /// <summary>
    /// Wrap the specified i and n.
    /// </summary>
    /// <returns>The wrap.</returns>
    /// <param name="i">The index.</param>
    /// <param name="n">N.</param>
    public int Wrap(int i, int n)
    {
        var remainder = i % n;

        int value = 0;

        if (remainder < 0)
        {
            value = remainder + n;
        }
        else
        {
            value = remainder;
        }
        return value;
    }

    /// <summary>
    /// Index2.
    /// </summary>
    public struct Index2
    {
        public int I; // Row
        public int J; // Column

        /// <summary>
        /// 
        /// </summary>
        public Index2(int i, int j)
        {
            I = i;
            J = j;
        }
    }

    /// <summary>
    /// The moore r1.
    /// </summary>
    public Index2[] Neighborhood =
    {
            new Index2(-1, -1),
            new Index2(-1, 0),
            new Index2(-1, 1),
            new Index2(0, -1),

            new Index2(0, 1),
            new Index2(1, -1),
            new Index2(1, 0),
            new Index2(1, 1)
   };

}

