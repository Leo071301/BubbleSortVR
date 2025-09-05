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

        StartCoroutine(textPanel.SpawnIn());
        StartCoroutine(CubeUtility.AnimateSpawnCubes(mergesort_cubes, this));
        
        StartCoroutine(MergeSort(mergesort_cubes));
    }

    //https://www.w3schools.com/dsa/dsa_algo_mergesort.php
    // Recursive Calls Ahead! CoRoutine Magic
    // Testing these was actuall HELL 
    public IEnumerator MergeSort(List<GameObject> list)
    {
        //base case
        if (list.Count <= 1) yield break;

        int mid = list.Count / 2;   // integer division

        // C# Linq slicing
        List<GameObject> lefthalf = list.Take(mid).ToList();
        List<GameObject> righthalf = list.Skip(mid).ToList();

        Debug.Log("left half is:  " + lefthalf.Count.ToString());
        Debug.Log("right half is: " + righthalf.Count.ToString());

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
        // okay, im going to be referencing cubes on both left, and right sides,
        // I need to make sure cubes move-above our starting list. Thats where the whole "Destination" vector comes in
        // everything else is a C# translation of the merge sort
        
        List<GameObject> unmerged_list = left.Concat(right).ToList();   // thank you linq gods


        returnList = new List<GameObject>();
        int i = 0;
        int j = 0;

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
                Vector3 destination = unmerged_list[i + j].transform.position;
                destination = new Vector3(destination.x, destination.y + 5, destination.z);   // move up

                yield return StartCoroutine(CubeUtility.PulseHighlight(left[i], Good_Color));  // highlight cuz why not?
                yield return StartCoroutine(CubeUtility.moveCube(left[i], destination));       // move boahhh


                i++;
            }
            else
            {
                returnList.Add(right[j]);

                // prepare to move the cube we just added, up a level (y-axis)
                // unmerged_list has the x and z positions in order, now to move the cubes.
                Vector3 destination = unmerged_list[i + j].transform.position;
                destination = new Vector3 ( destination.x, destination.y + 5, destination.z);   // move up

                yield return StartCoroutine(CubeUtility.PulseHighlight(right[j], Good_Color));  // highlight cuz why not?
                yield return StartCoroutine(CubeUtility.moveCube(right[j], destination));       // move boahhh

                j++;
            }

            returnList.AddRange(left.Skip(i));  // same as python's left[i:]

            // move the remaining cubes - all at once. Left side
            for (; i < left.Count; i++)
            {
                Vector3 destination = unmerged_list[i + j].transform.position;  // TODO test if works
                destination = new Vector3(destination.x, destination.y + 5, destination.z);

                StartCoroutine(CubeUtility.moveCube(left[i], destination));
            }

            returnList.AddRange(right.Skip(j)); // same as python's right[j:]


            yield return new WaitForSeconds(1); // wait

            // move the remaining cubes - all at once. Right side
            for (; j < right.Count; j++)
            {
                Vector3 destination = unmerged_list[i + j].transform.position;  // TODO test if works
                destination = new Vector3(destination.x, destination.y + 5, destination.z);

                StartCoroutine(CubeUtility.moveCube(right[j], destination));
            }

            yield return new WaitForSeconds(1); // wait

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
