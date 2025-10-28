using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    GlowHandler glowHandler;

    private const float text_speed = 0.25f;

    public IEnumerator InsertionSortAnimation()
    {
        isAnimating = true; // animation has officially started
        liveText.syncLiveText((int)text.NO_HIGHLIGHT); // tell LiveTextUtility to show text at index 0 (text with no code highlighted)



        yield return StartCoroutine(CubeUtility.AnimateSpawnCubes(insertionsort_cubes, this));
        yield return StartCoroutine(textPanel.SpawnIn());

        int n = insertionsort_cubes.Count;

        liveText.syncLiveText((int)text.N_LEN); // time to highlight n=len(arr)

        for (int i = 1; i < n; i++)
        {
            // remove glow from all cubes, then apply glow to only the unsorted portion, starting from cube i to the end 
            glowHandler.ResetApplyGlowMaterial(insertionsort_cubes, insertionsort_cubes.Skip(i).ToList());

            int key = int.Parse(insertionsort_cubes[i].name);
            int j = i - 1;


            yield return liveText.syncLiveTextWait((int)text.FOR_I, text_speed * 2);
            yield return liveText.syncLiveTextWait((int)text.KEY, text_speed * 2);
            yield return liveText.syncLiveTextWait((int)text.J_EQUALS_I, text_speed * 2);
            // should the while loop be highlighted at this point? 

            // Highlight the two cubes being compared 
            yield return StartCoroutine(CubeUtility.PulseHighlight(insertionsort_cubes[j + 1], insertionsort_cubes[j], Check_Color, Check_TIME));

            while (j >= 0 && int.Parse(insertionsort_cubes[j].name) > key)
            {

                // animate cube shifting here?? and insertion? 
                // add narration of WHILE_J / SHIFT ? INSERT
                insertionsort_cubes[j + 1] = insertionsort_cubes[j]; // would a c# fast swap work here? 
                j = j - 1;
            }

            // insertionsort_cubes is a list of game objects, 
            // insertionsort_cubes[j] gives the cube itself, type is game object 
            // .name is the string that stores the objects name
            // key is an int, so we use ToString() to convert integar into string version 
            // taking the cubes name property (string) and giving it a new string value derived from integar key 
            // IDEA: Place the key into its correct position (j + 1) in the sorted portion
            insertionsort_cubes[j+1].name = key.ToString(); // set j to key (need to update key as string?)

        }

        liveText.syncLiveText((int)text.NO_HIGHLIGHT);
        glowHandler.ResetAllGlow(insertionsort_cubes);


        // cool finished-animation
        foreach (var cube in insertionsort_cubes)
        {
            StartCoroutine(CubeUtility.PulseHighlight(cube, Good_Color, 1.0f));
            yield return new WaitForSeconds(0.2f);
        }

        // and destroy these cubes!!
        yield return StartCoroutine(CubeUtility.AnimateDestroyCubes(insertionsort_cubes, this));
        insertionsort_cubes = null;
        // panel goes away
        yield return StartCoroutine(textPanel.Despawn());

        isAnimating = false;


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

        // initialize glow handler
        glowHandler = new GlowHandler();
        glowHandler.material_normal = material;
        glowHandler.material_glow = material_glow;
        glowHandler.Init(insertionsort_cubes);  // VERY IMPORTAINT 

        StartCoroutine(InsertionSortAnimation());
    }

    // Start is called before the first frame update
    void Start()
    {
        liveText.syncLiveText((int)text.NO_HIGHLIGHT);


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

    public enum audioClips
    {
        Audio1 = 0,
        Audio2,
        Audio3,
        Audio4,
        Audio5 = 5


    }

    public enum text
    {
        NO_HIGHLIGHT = 0,
        N_LEN,
        FOR_I,
        KEY,
        J_EQUALS_I,
        WHILE_J,
        ARR_J,
        J_MINUS,
        ARR_J_KEY = 7

    }



    
}
