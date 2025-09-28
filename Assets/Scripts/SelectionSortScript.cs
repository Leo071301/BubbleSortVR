using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Create an Empty Object inside Unity, and attach this script!
// Have fun!

public class SelectionSortScript : MonoBehaviour
{
    // custom script
    [SerializeField] public LiveTextUtility liveText;       // script enables syncing of text
    [SerializeField] public LiveAudioUtility liveAudio;     // script enables syncing of audio
    [SerializeField] public PanelScript textPanel;          // script enables movement of panel

    [SerializeField] public List<int> number_list;  // List given thru the 'inspector tab', used to create List of GameObjects
    [SerializeField] public int Total_Spacing = 15; // tune the spacing of cubes

    [SerializeField] public Color default_Color;
    [SerializeField] public Material material;      // pass in URP shader - because Spatial SDK provides no support for loading resources
    [SerializeField] public Material material_glow;

    [SerializeField] public Color Good_Color;       // color if no swap needed
    [SerializeField] public Color Caution_Color;       // swap needed
    [SerializeField] public float Caution_TIME = 2.0f; // swap highlight color time

    [SerializeField] public Color Check_Color;      // emphasize color
    [SerializeField] public float Check_TIME = 1.0f;// emphasize time

    [NonSerialized]
    private List<GameObject> selectionsort_cubes = null;       // Cubes created, positioned, and programmed by this script
    private bool    isAnimating = false;                       // dont want to run multiple animations over eachother
    GlowHandler     glowHandler;

    // Coroutine performs the SelectionSort Animation
    public IEnumerator SelectionSortAnimation()
    {
        isAnimating = true;

        yield return StartCoroutine(CubeUtility.AnimateSpawnCubes(selectionsort_cubes, this));
        yield return StartCoroutine(textPanel.SpawnIn());

        //https://www.w3schools.com/dsa/dsa_algo_selectionsort.php
        int n = selectionsort_cubes.Count;

        liveText.syncLiveText((int)text.N_LENARRAY);

        for (int i = 0; i < n - 1; i++)
        {
            glowHandler.ResetApplyGlowMaterial(selectionsort_cubes, selectionsort_cubes.Skip(i).ToList());

            liveText.syncLiveTextWait((int)text.FOR_I,      0.5f);// wait 1/2 second
            liveText.syncLiveTextWait((int)text.MIN_INDEX_I,0.25f);// wait 1/4 second

            int min_index = i;

            for (int j = i + 1; j < n; j++)
            {

                liveText.syncLiveTextWait((int)text.FOR_J,  0.3f);
                liveText.syncLiveTextWait((int)text.IF,     0.3f);

                // Highlight two cubes being compared
                StartCoroutine(CubeUtility.PulseHighlight(selectionsort_cubes[j], Check_Color, Check_TIME));
                yield return StartCoroutine(CubeUtility.PulseHighlight(selectionsort_cubes[min_index], Check_Color, Check_TIME));


                // this translates to [j] < [min_index]
                if (int.Parse(selectionsort_cubes[j].name) <
                    int.Parse(selectionsort_cubes[min_index].name))
                {
                    liveText.syncLiveText((int)text.MIN_INDEX_J);
                    yield return StartCoroutine(CubeUtility.PulseHighlight(selectionsort_cubes[min_index], Caution_Color, Caution_TIME));

                    min_index = j;
                }
                else
                    // this cube is in order *good*
                    yield return StartCoroutine(CubeUtility.PulseHighlight(selectionsort_cubes[min_index], Good_Color, Check_TIME));

            }

            liveText.syncLiveText((int)text.MINVALUE);

            // Highlight cube to be inserted
            // dont wait for highlighting to finish
            yield return StartCoroutine(CubeUtility.PulseHighlight(selectionsort_cubes[min_index], Caution_Color, Caution_TIME));

            // insert cube
            yield return StartCoroutine(CubeUtility.insertCubeAndShift(selectionsort_cubes, min_index, i, this));

            // 'number_list' needs to be updated now, the animation doesent do this - it only moves cubeas around the world
            // C# Insertion
            GameObject temp = selectionsort_cubes[min_index];
            selectionsort_cubes.RemoveAt(min_index);
            selectionsort_cubes.Insert(i, temp);

        }

        liveText.syncLiveText(0);   // back to no text highlighting
        glowHandler.ResetAllGlow(selectionsort_cubes);

        // cool finished-animation
        foreach (var cube in selectionsort_cubes)
        {
            StartCoroutine(CubeUtility.PulseHighlight(cube, Good_Color, 1.0f));
            yield return new WaitForSeconds(0.2f);
        }

        // and destroy these cubes!!
        yield return StartCoroutine(CubeUtility.AnimateDestroyCubes(selectionsort_cubes, this));
        selectionsort_cubes = null;

        yield return StartCoroutine(textPanel.Despawn());

        isAnimating = false;
    }

    // Invoked by Spatial SDK Interactable
    public void SelectionSortEvent()
    {

        if (isAnimating) return;        // Prevents multiple 'Events' at once

        // create cubes and configure ONLY via first interaction!
        if (selectionsort_cubes == null)
        {
            selectionsort_cubes = CubeUtility.createCubeList(number_list, material, default_Color);

            // position cubes using the 'Transform' component
            CubeUtility.positionCubeList(selectionsort_cubes, Total_Spacing, this);
        }

        // initialize glow handler
        glowHandler = new GlowHandler();
        glowHandler.material_normal = material;
        glowHandler.material_glow = material_glow;
        glowHandler.Init(selectionsort_cubes);  // VERY IMPORTAINT 

        StartCoroutine(SelectionSortAnimation());
    }

    void Start()
    {
        // how to know which index is which? Set it up and check in the inspector
        liveText.syncLiveText(0); // 0 is the index is the one that has no highlighting
    }


    // Update is called once per frame
    void Update()
    {
        // Only when the cubes exist (created on event)
        if (selectionsort_cubes != null)
            CubeUtility.floatCubes(selectionsort_cubes);
    }

    public enum text
    {
        NO_HIGHLIGHT = 0,
        N_LENARRAY,
        FOR_I,
        MIN_INDEX_I,
        FOR_J,
        IF,
        MIN_INDEX_J,
        MINVALUE = 7
    }
}