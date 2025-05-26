using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Create an Empty Object inside Unity, and attach this script!
// Have fun!

public class BubbleSortScript : MonoBehaviour
{

    [SerializeField] public List<int> number_list;  // List given thru the 'inspector tab', used to create List of GameObjects
    [SerializeField] public int Total_Spacing = 15; // tune the spacing of cubes

    [SerializeField] public Color default_Color;
    [SerializeField] public Material material;      // pass in URP shader - because Spatial SDK provides no support for loading resources

    [SerializeField] public Color Good_Color;       // color if no swap needed
    [SerializeField] public Color Swap_Color;       // swap needed
    [SerializeField] public float Swap_TIME = 2.0f; // swap highlight color time

    [SerializeField] public Color Check_Color;      // emphasize color
    [SerializeField] public float Check_TIME = 1.0f;// emphasize time

    [NonSerialized]
    private List<GameObject> bubblesort_cubes = null;       // Cubes created, positioned, and programmed by this script
    private bool isAnimating = false;                       // dont want to run multiple animations over eachother


    // Coroutine performs the BubbleSort Animation
    public IEnumerator BubbleSortAnimation()
    {
        isAnimating = true;

        //https://www.w3schools.com/dsa/dsa_algo_bubblesort.php
        int n = bubblesort_cubes.Count;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                // Highlight two cubes being compared
                // wait for highlighting to finish
                yield return StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j], Check_Color, Check_TIME));
                yield return StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j + 1], Check_Color, Check_TIME));


                // this translates to [j] > [j + 1]
                if (int.Parse(bubblesort_cubes[j].name) >
                    int.Parse(bubblesort_cubes[j + 1].name))
                {
                    // Highlight two cubes being swapped
                    // dont wait for highlighting to finish
                    StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j], Swap_Color, Swap_TIME));
                    StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j + 1], Swap_Color, Swap_TIME));
                    //wait for swapping to finish
                    yield return StartCoroutine(CubeUtility.swapCubesHorizontally(bubblesort_cubes, j, j + 1, this));


                    // 'number_list' needs to be updated now
                    // C# Fast-Swap
                    (bubblesort_cubes[j], bubblesort_cubes[j + 1]) = (bubblesort_cubes[j + 1], bubblesort_cubes[j]);

                }
                else
                {
                    // Highlight two cubes being swapped
                    // wait for highlighting to finish
                    StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j], Good_Color, Check_TIME));
                    yield return StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j + 1], Good_Color, Check_TIME));
                }
            }
        }

        // TIP: wait some seconds using
        // yield return new WaitForSeconds(float);

        isAnimating = false;
    }

    // Invoked by Spatial SDK Interactable
    public void BubbleSortEvent()
    {
        if (isAnimating) return;        // Prevents multiple 'Events' at once

        // create cubes and configure ONLY via first interaction!
        if (bubblesort_cubes == null)
        {
            bubblesort_cubes = CubeUtility.createCubeList(number_list, material, default_Color);

            // position cubes using the 'Transform' component
            CubeUtility.positionCubeList(bubblesort_cubes, Total_Spacing, this);

        }

        StartCoroutine(BubbleSortAnimation());
    }



    // Update is called once per frame
    void Update()
    {
        // Only when the cubes exist (created on event)
        if (bubblesort_cubes != null)
            CubeUtility.floatCubes(bubblesort_cubes);

        // We Can't test Spatial features inside of unity
        // So the 'D' Key will trigger the animation for testing purposes
        if (Input.GetKeyDown(KeyCode.D))
            BubbleSortEvent();
    }
}
