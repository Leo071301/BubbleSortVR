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

        StartCoroutine(MergeSort(mergesort_cubes));
        
        // - - After Sorting is Complete - -
        List<GameObject> sorted_list = returnList;

    }

    //https://www.w3schools.com/dsa/dsa_algo_mergesort.php
    // Recursive Calls Ahead! CoRoutine Magic
    public IEnumerator MergeSort(List<GameObject> list)
    {
        //base case
        if (list.Count <= 0) yield break;

        int mid = list.Count / 2;   // integer division

        // C# Linq slicing
        List<GameObject> lefthalf = list.Take(mid).ToList();
        List<GameObject> righthalf = list.Skip(mid).ToList();

        yield return StartCoroutine(MergeSort(lefthalf));
        List<GameObject> sortedLeft = returnList;
        yield return StartCoroutine(MergeSort(righthalf));
        List<GameObject> sortedRight = returnList;

        // perform merging
        yield return StartCoroutine(Merge(sortedLeft, sortedRight));

        //optional
        List<GameObject> sortedList = returnList;
    }

    public IEnumerator Merge(List<GameObject> left, List<GameObject> right)
    {
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
                i++;
            }
            else
            {
                returnList.Add(right[j]);
                j++;
            }


        }
        yield return null;

    }
    /*while i < len(left) and j<len(right):
        if left[i] < right[j]:
            result.append(left[i])
            i += 1
        else:
            result.append(right[j])
            j += 1

    result.extend(left[i:])
    result.extend(right[j:])

    return result

        yield return null;
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        liveText.syncLiveText(0);

    }

    // Update is called once per frame
    void Update()
    {
        // Only when the cubes exist(created on event)
        if (mergesort_cubes != null)
            CubeUtility.floatCubes(mergesort_cubes);
    }
}
