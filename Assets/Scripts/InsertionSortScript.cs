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
    [SerializeField] public PanelScript textPanel;          // script enables movement of panel

    [SerializeField] public List<int> number_list; // list given through the inspector tab
    [SerializeField] public int Total_Spacing = 15; // spacing between the cubes 

    [SerializeField] public Color default_color; // initialize default color 
    [SerializeField] public Material material; // pass in URP shader since spatial SDK does not give it material when cubes are spontaneously made 
    [SerializeField] public Material material_glow;

    [SerializeField] public Color Good_Color; // this color highlights both if not out of order
    [SerializeField] public Color Swap_Color; // this color highlights when they need to swap
    [SerializeField] public float Swap_TIME = 2.0f; // this determines how long it takes to swap

    [SerializeField] public Color Check_Color; // color that highlights when its checking if it needs to be swapped
    [SerializeField] public float Check_TIME = 1.0f; // time it takes to check if it needs to swap

    [NonSerialized]
    private List<GameObject> insertionsort_cubes = null; // list of cubes made, positioned, and programmed
    private bool isAnimating = false; // boolean that will check to make sure multiple animations are not going over each other


    public IEnumerator InsertionSortAnimation()
    {
        isAnimating = true; // animation has officially started
        yield return StartCoroutine(CubeUtility.AnimateSpawnCubes(insertionsort_cubes, this));
        yield return StartCoroutine(textPanel.SpawnIn());



        yield return StartCoroutine(CubeUtility.AnimateDestroyCubes(insertionsort_cubes, this));
        insertionsort_cubes = null;
        // panel goes away
        yield return StartCoroutine(textPanel.Despawn());


        isAnimating = false;

        /*int n = insertionsort_cubes.Count; // gets amount of cubes that exist 


        for (int i = 1; i < n; i++) {
            int insert_index = i; // gets current index
            int current_value = int.Parse(insertionsort_cubes[insertionsort_cubes.Count - 1].name); // gets last element before its removed
            insertionsort_cubes.RemoveAt(insertionsort_cubes.Count - 1); // removes last element
            for(int j = i - 1; j > -1; j--)
            {
                if (int.Parse(insertionsort_cubes[j].name) > current_value)
                {
                    insert_index = j;

                }
            }
            insertionsort_cubes.Insert(insert_index, insertionsort_cubes[current_value]);
        }
        yield return null;
        isAnimating = false; // no longer animating */

    }

    public void InsertionSortEvent()
    {
        if (isAnimating) return; // prevents multiple events at once



        // create the cubes and position them on the first interaction
        if (insertionsort_cubes == null)
        {
            insertionsort_cubes = CubeUtility.createCubeList(number_list, material, default_color);

            CubeUtility.positionCubeList(insertionsort_cubes, Total_Spacing, this);
        }

        // initialize glow handler here? 

        StartCoroutine(InsertionSortAnimation());
    }

    // Start is called before the first frame update
    void Start()
    {
        liveText.syncLiveText(0);
        Debug.Log("Start method has started.");


    }

    // Update is called once per frame
    void Update()
    {
        // keep it floating throughout the game

        if(insertionsort_cubes != null)
        {
            Debug.Log("About to float cubes");
            CubeUtility.floatCubes(insertionsort_cubes);
            Debug.Log("float cubes created");
        }
        
    }
}
