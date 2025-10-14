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
        NO_HIGHLIGHT = 0,
        FUNCTION_PARTITION,
        PIVOT_ARRAY,
        I_EQUALS_LOW,
        FOR_J,
        IF_ARRAY_J,
        I_PLUS_EQUALS,
        ARRAY_SWAP,
        ARRAY_SWAP_I_PLUS_ONE,
        RETURN_I_PLUS_ONE,
        FUNCTION_QUICKSORT,
        IF_HIGH_IS_NONE,
        HIGH_EQUALS,
        IF_LOW_SMALLER_HIGH,
        PIVOT_INDEX_EQUALS,
        QUICKSORT_ARRAY_LOW,
        QUICKSORT_ARRAY_PIVOT_INDEX = 16
    }

    void Start()
    {
        liveText.syncLiveText((int)text.NO_HIGHLIGHT);
    }

    void Update()
    {
        if (quicksort_cubes != null) CubeUtility.floatCubes(quicksort_cubes);
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
        StartCoroutine(textPanel.SpawnIn());

        float starting_y = quicksort_cubes[0].transform.position.y; // for reference!

        glowHandler.ApplyAllGlow(quicksort_cubes);      // highlight from the start
        yield return StartCoroutine(QuickSort(quicksort_cubes));


        // Move sorted list "back down" to starting y-axis
        foreach (var cube in quicksort_cubes)
        {
            Vector3 destination = new Vector3(cube.transform.position.x,
                                                    starting_y,
                                                    cube.transform.position.z);
            yield return StartCoroutine(CubeUtility.moveCube(cube, destination));   // move!
        }

        glowHandler.ApplyAllGlow(quicksort_cubes);      // highlight from the start

        // cool finished-animation
        foreach (var cube in quicksort_cubes)
        {
            StartCoroutine(CubeUtility.PulseHighlight(cube, Good_Color, 1.0f));
            yield return new WaitForSeconds(0.2f);
        }

        yield return StartCoroutine(CubeUtility.AnimateDestroyCubes(quicksort_cubes, this));
        StartCoroutine(textPanel.Despawn());

        liveText.syncLiveText((int)text.NO_HIGHLIGHT);
        quicksort_cubes = null;     // very importaint
        isAnimating = false;
    }

    // As reference: https://www.w3schools.com/dsa/dsa_algo_quicksort.php
    // Recursive Calls Ahead!
    // Animtion include: syncing text, audio, glow, cube-movements, and the sorting animation
    public IEnumerator QuickSort(List<GameObject> list, int low = 0, int high = -1) // -1 because C# doesent have the "none" type
    {
        liveText.syncLiveTextWait((int)text.FUNCTION_PARTITION,     text_speed / 2);
        liveText.syncLiveTextWait((int)text.IF_HIGH_IS_NONE,        text_speed);
        if (high == -1) 
        {
            high = list.Count - 1;
            liveText.syncLiveTextWait((int)text.HIGH_EQUALS,        text_speed);
        }

        glowHandler.ResetApplyGlowMaterial(quicksort_cubes, list.Skip(low).Take(high-low + 1).ToList());  // glow sub-list

        liveText.syncLiveTextWait((int)text.IF_LOW_SMALLER_HIGH,    text_speed);
        if (low < high)
        {
            liveText.syncLiveTextWait((int)text.PIVOT_INDEX_EQUALS, text_speed);
            yield return StartCoroutine(Partition(list, low, high));
            int pivot_inex = return_value;

            liveText.syncLiveTextWait((int)text.QUICKSORT_ARRAY_LOW,            text_speed);
            yield return StartCoroutine(QuickSort(quicksort_cubes, low, pivot_inex - 1));

            liveText.syncLiveTextWait((int)text.QUICKSORT_ARRAY_PIVOT_INDEX,    text_speed);
            yield return StartCoroutine(QuickSort(quicksort_cubes, pivot_inex + 1, high));

        }

    }

    public IEnumerator Partition(List<GameObject> list, int low, int high)
    {
        liveText.syncLiveTextWait((int)text.FUNCTION_PARTITION,     text_speed);
        // Note:
        // Prior to aniomation, move the sub-list of cubes "up" a layer (Y-axis). 1.3 Units
        foreach (var cube in list.Skip(low).Take(high - low + 1).ToList())
        {
            Vector3 destination = new Vector3(cube.transform.position.x,
                                                    cube.transform.position.y + 1.2f,
                                                    cube.transform.position.z);
            yield return StartCoroutine(CubeUtility.moveCube(cube, destination, 12));   // move!
        }

        glowHandler.ResetApplyGlowMaterial(quicksort_cubes, list.Skip(low).Take(high - low + 1).ToList());  // glow sub-list

        liveText.syncLiveTextWait((int)text.PIVOT_ARRAY,            text_speed);
        int pivot = int.Parse(list[high].name); // to int

        liveText.syncLiveTextWait((int)text.I_EQUALS_LOW,           text_speed);
        int i = low - 1;

        liveText.syncLiveTextWait((int)text.FOR_J,                  text_speed);
        for (int j = low; j < high; j++)
        {
            liveText.syncLiveTextWait((int)text.IF_ARRAY_J,         text_speed);
            yield return StartCoroutine(CubeUtility.PulseHighlight(list[j], list[pivot], Check_Color, Check_TIME));
            if ( int.Parse(list[j].name) <= pivot ) // to int
            {
                liveText.syncLiveTextWait((int)text.I_PLUS_EQUALS,  text_speed);
                i++;

                liveText.syncLiveTextWait((int)text.ARRAY_SWAP,     text_speed);
                // swap animation
                yield return StartCoroutine(CubeUtility.swapCubesVertically(list, i, j, this));
                
                // Fast C# Swap
                (list[i], list[j]) = (list[j], list[i]);
            }
            else
                yield return StartCoroutine(CubeUtility.PulseHighlight(list[j], list[pivot], Good_Color, Check_TIME / 2));

        }

        liveText.syncLiveTextWait((int)text.ARRAY_SWAP_I_PLUS_ONE,  text_speed);
        // swap animation
        yield return StartCoroutine(CubeUtility.swapCubesVertically(list, high, i+1, this));
        liveText.syncLiveTextWait((int)text.RETURN_I_PLUS_ONE,      text_speed);


        // Fast C# Swap
        (list[i+1], list[high]) = (list[high], list[i+1]);
        return_value = i + 1;

        yield return null;
    }

}
