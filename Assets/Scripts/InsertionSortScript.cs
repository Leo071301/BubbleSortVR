using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InsertionSortScript : MonoBehaviour
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
    private List<GameObject> bubblesort_cubes = null; // list of cubes made, positioned, and programmed
    private bool isAnimating = false; // boolean that will check to make sure multiple animations are not going over each other


    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
