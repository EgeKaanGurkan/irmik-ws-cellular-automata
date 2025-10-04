using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageInitializer : MonoBehaviour
{
    [SerializeField] public Texture2D texture;
    [SerializeField] public float threshold = 0.5f;
    
        
    public void Initialize(int[,] state)
    {
        Debug.Log("I am in the image initializer");

        int nrows = state.GetLength(0);
        int ncols = state.GetLength(1);

        float ti = 1.0f / (nrows - 1);
        float tj = 1.0f / (ncols - 1);

        for (int i = 0; i < nrows; i++)
        {
            for (int j = 0; j < ncols; j++)
            {
                Color color = texture.GetPixelBilinear(j * tj, i * ti);

                if (color.grayscale > threshold)
                    state[i, j] = 1;
                else
                    state[i, j] = 0;
            }
        }
    }

}
