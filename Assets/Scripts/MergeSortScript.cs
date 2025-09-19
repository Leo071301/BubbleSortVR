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
    [SerializeField] public float Swap_TIME = 2.0f; // this determines how long it takes to swap

    [SerializeField] public Color Check_Color; // color that highlights when its checking if it needs to be swapped
    [SerializeField] public float Check_TIME = 1.0f; // time it takes to check if it needs to swap

    [NonSerialized]
    private List<GameObject> mergesort_cubes = null; // list of cubes made, positioned, and programmed
    private bool isAnimating = false; // boolean that will check to make sure multiple animations are not going over each other

    // This variable stores the return contents of Coroutines, wack workaround but it works lol
    private List<GameObject> returnList     = null;


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

        //debug
        Debug.Log("Cubes are: ");
        foreach (var cube in mergesort_cubes)
        {
            Debug.Log(cube.name + " ");
        }

        yield return StartCoroutine(CubeUtility.AnimateDestroyCubes(mergesort_cubes, this));
        //StartCoroutine(textPanel.Despawn());  // TODO Fix later

        mergesort_cubes = null;     // very importaint
        isAnimating = false;
    }

    //https://www.w3schools.com/dsa/dsa_algo_mergesort.php
    // Recursive Calls Ahead! CoRoutine Magic
    // Testing these was actuall HELL 
    public IEnumerator MergeSort(List<GameObject> list)
    {
        //base case
        if (list.Count <= 1)
        {
            returnList = list;  // return the list itself , since this is the base case
            yield break;        // stop this iterator block
        }

        int mid = list.Count / 2;   // integer division

        // C# Linq slicing
        List<GameObject> lefthalf = list.Take(mid).ToList();
        List<GameObject> righthalf = list.Skip(mid).ToList();

        // highlight left half

        yield return StartCoroutine(MergeSort(lefthalf));
        List<GameObject> sortedLeft = returnList;
        yield return StartCoroutine(MergeSort(righthalf));
        List<GameObject> sortedRight = returnList;

        // perform merging
        yield return StartCoroutine(Merge(sortedLeft, sortedRight));

        // cool finish-highlight effect on returnlist
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

        returnList = new List<GameObject>();        // DONT# TOUCH THIS!! x_X  this script will DIE
        int i = 0;
        int j = 0;

        // begin merge sort

        while (i < left.Count && j < right.Count)
        {
            // same as
            // left[i] < right[j]
            if (int.Parse(left[i].name) <
                int.Parse(right[j].name))
            {
                returnList.Add(left[i]);

                // prepare to move the cube we just added, up a level (y-axis)
                // unmerged_list has the x and z positions in order, now to move the cubes.
                Vector3 destination = unmerged_vector_list[i + j];
                destination = new Vector3(destination.x, destination.y + 1.5f, destination.z);   // position up

                yield return StartCoroutine(CubeUtility.PulseHighlight(left[i], Good_Color));  // highlight cuz why not?
                yield return StartCoroutine(CubeUtility.moveCube(left[i], destination));       // move!!!


                i++;
            }
            else
            {
                returnList.Add(right[j]);

                // prepare to move the cube we just added, up a level (y-axis)
                // unmerged_list has the x and z positions in order, now to move the cubes.
                Vector3 destination = unmerged_vector_list[i + j];
                destination = new Vector3 ( destination.x, destination.y + 1.5f, destination.z);   // position up

                yield return StartCoroutine(CubeUtility.PulseHighlight(right[j], Good_Color));  // highlight cuz why not?
                yield return StartCoroutine(CubeUtility.moveCube(right[j], destination));       // move!!!

                j++;
            }

            returnList.AddRange(left.Skip(i));  // same as python's left[i:]

            // move the remaining cubes - all at once. Left side
            for (; i < left.Count; i++)
            {
                Vector3 destination = unmerged_vector_list[i + j];  // why i+j? Thats what my brain decide on.... nah-jk it's kinda complicated
                destination = new Vector3(destination.x, destination.y + 1.5f, destination.z);

                
                StartCoroutine(CubeUtility.moveCube(left[i], destination));
            }

            returnList.AddRange(right.Skip(j)); // same as python's right[j:]


            yield return new WaitForSeconds(1); // wait

            // move the remaining cubes - all at once. Right side
            for (; j < right.Count; j++)
            {
                Vector3 destination = unmerged_vector_list[i + j];
                destination = new Vector3(destination.x, destination.y + 1.5f, destination.z);

                StartCoroutine(CubeUtility.moveCube(right[j], destination));
            }

            yield return new WaitForSeconds(1.5f); // wait

        }
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
}
