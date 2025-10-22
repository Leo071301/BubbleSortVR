using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Create an Empty Object inside Unity, and attach this script!
// Have fun!

public class BubbleSortScript : MonoBehaviour
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
    [SerializeField] public Color Swap_Color;       // swap needed
    [SerializeField] public float Swap_TIME = 2.0f; // swap highlight color time

    [SerializeField] public Color Check_Color;      // emphasize color
    [SerializeField] public float Check_TIME = 1.0f;// emphasize time

    [NonSerialized]
    private List<GameObject> bubblesort_cubes = null;       // Cubes created, positioned, and programmed by this script
    private bool isAnimating = false;                       // dont want to run multiple animations over eachother
    GlowHandler glowHandler;                                // enables glowing effect of cube's of Your Choosing!

    private const float text_speed = 0.25f;

    public enum text
    {
        NO_HIGHLIGHT = 0,
        N_LENARRAY,
        FOR_I,
        FOR_J,
        IF,
        SWAP = 5
    }

    void Start()
    {
        liveText.syncLiveTextWait((int)text.NO_HIGHLIGHT, text_speed);
    }


    // Update is called once per frame
    void Update()
    {
        // Only when the cubes exist (created on event)
        if (bubblesort_cubes != null)
            CubeUtility.floatCubes(bubblesort_cubes);
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

        // initialize glow handler
        glowHandler = new GlowHandler();
        glowHandler.material_normal = material;
        glowHandler.material_glow = material_glow;
        glowHandler.Init(bubblesort_cubes);  // VERY IMPORTAINT 

        StartCoroutine(BubbleSortAnimation());
    }

    // Coroutine performs the BubbleSort Animation
    public IEnumerator BubbleSortAnimation()
    {
        isAnimating = true;

        liveText.syncLiveText((int)text.NO_HIGHLIGHT);
        yield return StartCoroutine(CubeUtility.AnimateSpawnCubes(bubblesort_cubes, this));
        yield return StartCoroutine(textPanel.SpawnIn());

        yield return liveAudio.playNextAudio(0);

        //https://www.w3schools.com/dsa/dsa_algo_bubblesort.php
        int n = bubblesort_cubes.Count;

        yield return liveText.syncLiveTextWait((int)text.N_LENARRAY, text_speed * 2);

        for (int i = 0; i < n - 1; i++)
        {
            yield return liveText.syncLiveTextWait((int)text.FOR_I, text_speed);
            
            for (int j = 0; j < n - i - 1; j++)
            {
                // highlight the cube's to be sorted! Will leave sorted cube's un-highlighted
                glowHandler.ResetApplyGlowMaterial(bubblesort_cubes, bubblesort_cubes.Take(n-i).ToList());


                yield return liveText.syncLiveTextWait((int)text.FOR_J, text_speed);
                yield return liveText.syncLiveTextWait((int)text.IF,    text_speed);

                // Highlight two cubes being compared
                yield return StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j], bubblesort_cubes[j + 1], Check_Color, Check_TIME));

                // this translates to [j] > [j + 1]
                if (int.Parse(bubblesort_cubes[j].name) >
                    int.Parse(bubblesort_cubes[j + 1].name))
                {
                    yield return liveText.syncLiveTextWait((int)text.SWAP, text_speed);

                    // Highlight two cubes being swapped-dont wait for animation to finish
                    StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j], bubblesort_cubes[j + 1], Swap_Color, Swap_TIME));
                    // swapping animation
                    yield return StartCoroutine(CubeUtility.swapCubesHorizontally(bubblesort_cubes, j, j + 1, this));


                    // 'number_list' needs to be updated now
                    // C# Fast-Swap
                    (bubblesort_cubes[j], bubblesort_cubes[j + 1]) = (bubblesort_cubes[j + 1], bubblesort_cubes[j]);

                }
                else
                {

                    // Highlight two cubes being swapped
                    yield return StartCoroutine(CubeUtility.PulseHighlight(bubblesort_cubes[j], bubblesort_cubes[j + 1], Good_Color, Check_TIME));
                }
            }
        }

        liveText.syncLiveText((int)text.NO_HIGHLIGHT);
        glowHandler.ResetAllGlow(bubblesort_cubes);

        // cool finished-animation
        foreach (var cube in bubblesort_cubes)
        {
            StartCoroutine(CubeUtility.PulseHighlight(cube, Good_Color, 1.0f));
            yield return new WaitForSeconds(0.2f);
        }

        // and destroy these cubes!!
        yield return StartCoroutine(CubeUtility.AnimateDestroyCubes(bubblesort_cubes, this));
        bubblesort_cubes = null;
        // panel goes away
        yield return StartCoroutine(textPanel.Despawn());

        isAnimating = false;

    }
}