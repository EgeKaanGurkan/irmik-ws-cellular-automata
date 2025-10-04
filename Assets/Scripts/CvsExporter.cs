using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CsvExporter : MonoBehaviour
{

         private string filePath;

        public CsvExporter(string path)
        {
            filePath = path;
        }

    public void ExportLayerData(int[,] state, int layer, int nrows, int ncols)
    {
        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (StreamWriter writer = new StreamWriter(filePath, append: true))
        {
            // Write header for the layer
            writer.WriteLine($"Layer {layer + 1}");  // Correctly label Layer 1, 2, etc.


            // Export each cell in the layer
            for (int i = 0; i < nrows; i++)
            {
                for (int j = 0; j < ncols; j++)
                {
                    writer.Write($"{state[i, j]},");
                }
                writer.WriteLine();
               
            }

            writer.WriteLine();
        }

         // UnityEngine.Debug.Log("Exported cell states to CSV file at: " + filePath);
    }

    

}

