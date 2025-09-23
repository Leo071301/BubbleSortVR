using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class MergeSortScript : MonoBehaviour
{
    // custom script
    [SerializeField] public LiveTextUtility liveText;       // script enables syncing of text
    [SerializeField] public LiveAudioUtility liveAudio;     // script enables syncing of audio
    [SerializeField] public PanelScript textPanel;          // script enables movement of panel

    [SerializeField] public List<int> number_list; // list given through the inspector tab
    [SerializeField] public int Total_Spacing = 15; // spacing between the cubes 

    [SerializeField] public Color default_color; // initialize default color 
    [SerializeField] public Material material; // pass in URP shader since spatial SDK does not give it material when cubes are spontaneously made 

    [SerializeField] public Color Good_Color; // this color highlights both if not out of order
    [SerializeField] public Color Swap_Color; // this color highlights when they need to swap
    [SerializeField] public float Swap_TIME = 1.5f; // this determines how long it takes to swap

    [SerializeField] public Color Check_Color; // color that highlights when its checking if it needs to be swapped
    [SerializeField] public float Check_TIME = 1.0f; // time it takes to check if it needs to swap

    [NonSerialized]
    private List<GameObject> mergesort_cubes = null; // list of cubes made, positioned, and programmed
    private bool isAnimating = false; // boolean that will check to make sure multiple animations are not going over each other

    // This variable stores the return contents of Coroutines, wack workaround but it works lol
    private List<GameObject> returnList     = null;
    private const float text_speed = 0.4f;

    // Invoked by Spatial SDK Interactable
    public void MergeSortEvent()
    {

        if (isAnimating) return;        // Prevents multiple 'Animations' at once

        // create cubes and configure ONLY via first interaction!
        if (mergesort_cubes == null)
        {
            mergesort_cubes = CubeUtility.createCubeList(number_list, material, default_color);

            // position cubes using the 'Transform' component
            CubeUtility.positionCubeList(mergesort_cubes, Total_Spacing, this);
        }


        StartCoroutine(MergeSortEventHelper());
    }

    // Intermediate-step inbetween 
    //
    // Event    ->  Helper  -> MergeSort()
    //
    // This co-routine simply displays animations prior to the "recursive mergesort" crazyness

    public IEnumerator MergeSortEventHelper()
    {
        isAnimating = true;
        yield return StartCoroutine(CubeUtility.AnimateSpawnCubes(mergesort_cubes, this));
        //StartCoroutine(textPanel.SpawnIn());  // TODO Fix later

        yield return StartCoroutine(MergeSort(mergesort_cubes));

        // update 
        mergesort_cubes = returnList;

        // cool finished-animation
        foreach (var cube in mergesort_cubes)
        {
            StartCoroutine(CubeUtility.PulseHighlight(cube, Good_Color, 1.0f));
            yield return new WaitForSeconds(0.2f);
        }

        yield return StartCoroutine(CubeUtility.AnimateDestroyCubes(mergesort_cubes, this));
        //StartCoroutine(textPanel.Despawn());  // TODO Fix later


        liveText.syncLiveText((int)text.NO_HIGHLIGHT);
        mergesort_cubes = null;     // very importaint
        isAnimating = false;
    }

    //https://www.w3schools.com/dsa/dsa_algo_mergesort.php
    // Recursive Calls Ahead! CoRoutine Magic
    // Testing these was actuall HELL 
    public IEnumerator MergeSort(List<GameObject> list)
    {
        yield return liveText.syncLiveTextWait((int)text.FUNCTION_MERGESORT, text_speed);

        yield return liveText.syncLiveTextWait((int)text.BASECASE, text_speed);
        yield return new WaitForSeconds(0.5f);
        //base case
        if (list.Count <= 1)
        {
            yield return liveText.syncLiveTextWait((int)text.RETURN_ARR, text_speed * 2);

            returnList = list;  // return the list itself , since this is the base case
            yield break;        // stop this iterator block
        }

        yield return liveText.syncLiveTextWait((int)text.MID, text_speed);

        int mid = list.Count / 2;   // integer division
        yield return liveText.syncLiveTextWait((int)text.LEFTHALF, text_speed);

        // C# Linq slicing
        List<GameObject> lefthalf = list.Take(mid).ToList();
        List<GameObject> righthalf = list.Skip(mid).ToList();

        yield return liveText.syncLiveTextWait((int)text.RIGHTHALF, text_speed);


        // highlight left half

        yield return liveText.syncLiveTextWait((int)text.SORTEDLEFT, text_speed);
        yield return StartCoroutine(MergeSort(lefthalf));
        yield return liveText.syncLiveTextWait((int)text.SORTEDLEFT, text_speed);
        List<GameObject> sortedLeft = returnList;

        yield return liveText.syncLiveTextWait((int)text.SORTEDRIGHT, text_speed);
        yield return StartCoroutine(MergeSort(righthalf));
        yield return liveText.syncLiveTextWait((int)text.SORTEDRIGHT, text_speed);
        List<GameObject> sortedRight = returnList;

        // perform merging
        yield return liveText.syncLiveTextWait((int)text.RETURN_MERGE, text_speed);
        yield return StartCoroutine(Merge(sortedLeft, sortedRight));
        yield return liveText.syncLiveTextWait((int)text.RETURN_MERGE, text_speed / 2);

    }

    public IEnumerator Merge(List<GameObject> left, List<GameObject> right)
    {
        // Note:
        // okay, im going to be referencing cubes on both left, and right sides.
        // I need to make sure cubes move-above the current "level", to do this i create

        // unmerged_vector_list - The list of vector3's prior to being sorted. These will be used to move the cubes in their correct spot
        // everything else is a C# translation of the merge sort


        List<Vector3> unmerged_vector_list = new List<Vector3>();
        
        // copy each position vector, we will need these later
        foreach (var cube in left.Concat(right).ToList())       // thank you linq gods
        {
            unmerged_vector_list.Add(cube.transform.position);
        }

        yield return liveText.syncLiveTextWait((int)text.RESULT_EQUALS, text_speed);
        returnList = new List<GameObject>();        // DONT# TOUCH THIS!! x_X  this script will DIE

        yield return liveText.syncLiveTextWait((int)text.I_AND_J_EQUALS, text_speed);
        int i = 0;
        int j = 0;

        // begin merge sort

        while (i < left.Count && j < right.Count)
        {
            yield return liveText.syncLiveTextWait((int)text.I_AND_J_EQUALS, text_speed / 2);
            yield return liveText.syncLiveTextWait((int)text.IF_LOOP, text_speed / 2);

            yield return StartCoroutine(CubeUtility.PulseHighlight(left[i], right[j], Check_Color));


            // same as
            // left[i] < right[j]
            if (int.Parse(left[i].name) <
                int.Parse(right[j].name))
            {
                yield return liveText.syncLiveTextWait((int)text.RESULT_APPEND_LEFT, text_speed / 2);
                returnList.Add(left[i]);

                // prepare to move the cube we just added, up a level (y-axis)
                // unmerged_list has the x and z positions in order, now to move the cubes.
                Vector3 destination = unmerged_vector_list[i + j];
                destination = new Vector3(destination.x, destination.y + 1.3f, destination.z);   // position up

                StartCoroutine(CubeUtility.PulseHighlight(left[i], Good_Color, Swap_TIME));  // highlight cuz why not?
                yield return StartCoroutine(CubeUtility.moveCube(left[i], destination));       // move!!!

                yield return liveText.syncLiveTextWait((int)text.I_PLUS_EQUALS, text_speed / 2);
                i++;
            }
            else
            {
                yield return liveText.syncLiveTextWait((int)text.RESULT_APPEND_RIGHT, text_speed / 2);
                returnList.Add(right[j]);

                // prepare to move the cube we just added, up a level (y-axis)
                // unmerged_list has the x and z positions in order, now to move the cubes.
                Vector3 destination = unmerged_vector_list[i + j];
                destination = new Vector3 ( destination.x, destination.y + 1.3f, destination.z);   // position up

                StartCoroutine(CubeUtility.PulseHighlight(right[j], Good_Color, Swap_TIME));  // highlight cuz why not?
                yield return StartCoroutine(CubeUtility.moveCube(right[j], destination));       // move!!!

                yield return liveText.syncLiveTextWait((int)text.J_PLUS_EQUALS, text_speed / 2);
                j++;
            }
        }               // f***ing bug found 9/21/25 6:31pm

        yield return liveText.syncLiveTextWait((int)text.RESULT_EXTEND_LEFT, text_speed / 2);

        returnList.AddRange(left.Skip(i));  // same as python's left[i:] 

        // move the remaining cubes - all at once. Left side
        for (; i < left.Count; i++)
        {
            Vector3 destination = unmerged_vector_list[i + j];  // why i+j? Thats what my brain decide on.... nah-jk it's kinda complicated
            destination = new Vector3(destination.x, unmerged_vector_list[i + j - 1].y + 1.3f, destination.z);  // I DONT KNOW HOW BUT THIS WORKS,

            
            StartCoroutine(CubeUtility.moveCube(left[i], destination));
        }
        yield return liveText.syncLiveTextWait((int)text.RESULT_EXTEND_RIGHT, text_speed / 2);

        returnList.AddRange(right.Skip(j)); // same as python's right[j:]



        // move the remaining cubes - all at once. Right side
        for (; j < right.Count; j++)
        {
            Vector3 destination = unmerged_vector_list[i + j];
            destination = new Vector3(destination.x, unmerged_vector_list[i + j - 1].y + 1.3f, destination.z);  // I DONT KNOW HOW BUT THIS WORKS,

            StartCoroutine(CubeUtility.moveCube(right[j], destination));
        }
        yield return liveText.syncLiveTextWait((int)text.RETURN_RESULT, text_speed);

    }


    void Start()
    {
        liveText.syncLiveText(0);
    }

    void Update()
    {
        // Only when the cubes exist(created on event)
        if (mergesort_cubes != null)
            CubeUtility.floatCubes(mergesort_cubes);
    }

    // Enumerations corespond to index's in the merge-sort Live Text Utility
    public enum text
    {
        NO_HIGHLIGHT = 0,
        FUNCTION_MERGESORT,
        BASECASE,
        RETURN_ARR,
        MID,
        LEFTHALF,
        RIGHTHALF,
        SORTEDLEFT,
        SORTEDRIGHT,
        RETURN_MERGE,
        FUNCTION_MERGE,
        RESULT_EQUALS,
        I_AND_J_EQUALS,
        WHILE_LOOP,
        IF_LOOP,
        RESULT_APPEND_LEFT,
        I_PLUS_EQUALS,
        ELSE_LOOP,
        RESULT_APPEND_RIGHT,
        J_PLUS_EQUALS,
        RESULT_EXTEND_LEFT,
        RESULT_EXTEND_RIGHT,
        RETURN_RESULT = 22
    }
}
