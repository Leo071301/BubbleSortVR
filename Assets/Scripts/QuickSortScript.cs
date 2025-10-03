using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class QuickSortScript : MonoBehaviour
{
    // SerializableFields are set in the "Inspector" via unity
    [SerializeField] public LiveTextUtility liveText;       // script enables syncing of text
    [SerializeField] public LiveAudioUtility liveAudio;     // script enables syncing of audio
    [SerializeField] public PanelScript textPanel;          // script enables movement of panel

    [SerializeField] public List<int> number_list;  // list given through the inspector tab
    [SerializeField] public int Total_Spacing = 15; // spacing between the cubes 

    [SerializeField] public Color default_color;    // initialize default color 
    [SerializeField] public Material material;      // pass in URP shader since spatial SDK does not give it material when cubes are spontaneously made 
    [SerializeField] public Material material_glow;

    [SerializeField] public Color Good_Color;       // this color highlights both if not out of order
    [SerializeField] public Color Swap_Color;       // this color highlights when they need to swap
    [SerializeField] public float Swap_TIME = 1.5f; // this determines how long it takes to swap

    [SerializeField] public Color Check_Color;      // color that highlights when its checking if it needs to be swapped
    [SerializeField] public float Check_TIME = 1.0f;// time it takes to check if it needs to swap

    [NonSerialized]
    private List<GameObject> quicksort_cubes = null;// list of cubes made, positioned, and programmed
    private bool isAnimating = false;               // boolean that will check to make sure multiple animations are not going over each other
    private const float text_speed = 0.2f;
    private int                 return_value;       // wack work around because Co-Routines can't return values         

    private GlowHandler glowHandler;                // enables glowing effect of cube's of Your Choosing!


    // Enumerations corespond to index's in the quick-sort Live Text Utility
    public enum text
    {
        None,
    }

    void Start()
    {   
        // sync text
    }

    void Update()
    {
        // float cubes
    }

    // Invoked by Spatial SDK Interactable
    public void QuickSortEvent()
    {
        if (isAnimating) return;        // Prevent multiple animations "at once"

        if (quicksort_cubes == null)
        {
            quicksort_cubes = CubeUtility.createCubeList(number_list, material, default_color);

            CubeUtility.positionCubeList(quicksort_cubes, Total_Spacing, this);
        }

        // initialize glow handler
        glowHandler = new GlowHandler();
        glowHandler.material_normal = material;
        glowHandler.material_glow   = material_glow;
        glowHandler.Init(quicksort_cubes);  // VERY IMPORTAINT

        StartCoroutine(QuickSortHelper());
    }

    // Intermediate-step inbetween 
    //
    // Event    ->  Helper  -> QuickSort()
    //
    // This co-routine simply displays animations prior to the "recursive quicksort" crazyness
    public IEnumerator QuickSortHelper()
    {
        isAnimating = true;
        yield return StartCoroutine(CubeUtility.AnimateSpawnCubes(quicksort_cubes, this));
        //StartCoroutine(textPanel.SpawnIn());

        glowHandler.ApplyAllGlow(quicksort_cubes);      // highlight from the start
        yield return StartCoroutine(QuickSort(quicksort_cubes));

        // cool finished-animation
        foreach (var cube in quicksort_cubes)
        {
            StartCoroutine(CubeUtility.PulseHighlight(cube, Good_Color, 1.0f));
            yield return new WaitForSeconds(0.2f);
        }

        yield return StartCoroutine(CubeUtility.AnimateDestroyCubes(quicksort_cubes, this));
        //StartCoroutine(textPanel.Despawn());

        //liveText.syncLiveText((int)text.NO_HIGHLIGHT);
        quicksort_cubes = null;     // very importaint
        isAnimating = false;
    }

    // As reference: https://www.w3schools.com/dsa/dsa_algo_quicksort.php
    // Recursive Calls Ahead!
    // Animtion include: syncing text, audio, glow, cube-movements, and the sorting animation
    public IEnumerator QuickSort(List<GameObject> list, int low = 0, int high = -1) // -1 because C# doesent have the "none" type
    {

        if (high == -1)
            high = list.Count - 1;

        glowHandler.ResetApplyGlowMaterial(quicksort_cubes, list.Skip(low).Take(high-low).ToList());  // glow sub-list


        if (low < high)
        {

            yield return StartCoroutine(Partition(list, low, high));
            int pivot_inex = return_value;

            yield return StartCoroutine(QuickSort(quicksort_cubes, low, pivot_inex - 1));
            yield return StartCoroutine(QuickSort(quicksort_cubes, pivot_inex + 1, high));

        }

    }

    public IEnumerator Partition(List<GameObject> list, int low, int high)
    {
        glowHandler.ResetApplyGlowMaterial(quicksort_cubes, list.Skip(low).Take(high - low + 1).ToList());  // glow sub-list

        int pivot = int.Parse(list[high].name); // to int

        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if ( int.Parse(list[j].name) <= pivot ) // to int
            {
                i++;

                // swap animation
                yield return StartCoroutine(CubeUtility.swapCubesVertically(list, i, j, this));

                // Fast C# Swap
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        // swap animation
        yield return StartCoroutine(CubeUtility.swapCubesVertically(list, high, i+1, this));

        // Fast C# Swap
        (list[i+1], list[high]) = (list[high], list[i+1]);
        return_value = i + 1;

        yield return null;
    }

}
