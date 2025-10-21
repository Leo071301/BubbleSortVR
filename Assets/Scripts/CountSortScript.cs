using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CountSortScript : MonoBehaviour
{
    // SerializableFields are set in the "Inspector" via unity
    [SerializeField] public LiveTextUtility liveText;       // script enables syncing of text
    [SerializeField] public LiveAudioUtility liveAudio;     // script enables syncing of audio
    [SerializeField] public PanelScript textPanel;          // script enables movement of panel

    [SerializeField] public List<int> number_list;  // list given through the inspector tab
    [SerializeField] public int Total_Spacing = 14; // spacing between the cubes 
    [Header("Transform of the counting list")]
    [SerializeField] public Transform counting_list_pos;   // This is the position where cubes will be moved and sorted

    [SerializeField] public Color default_color;    // initialize default color 
    [SerializeField] public Material material;      // pass in URP shader since spatial SDK does not give it material when cubes are spontaneously made 
    [SerializeField] public Material material_glow;

    [SerializeField] public float Cube_SPEED = 5.5f;

    [NonSerialized]
    private List<GameObject> countsort_cubes = null;// list of cubes made, positioned, and programmed
    private bool isAnimating = false;               // boolean that will check to make sure multiple animations are not going over each other
    private const float text_speed = 0.2f;

    private GlowHandler glowHandler;                // enables glowing effect of cube's of Your Choosing!


    public enum text
    {
        NO_HIGHLIGHT = 0
    }

    void Start()
    {

        //liveText.syncLiveText((int)text.NO_HIGHLIGHT);
    }

    void Update()
    {
        if (countsort_cubes != null) CubeUtility.floatCubes(countsort_cubes);   
    }

    public void CountSortEvent()
    {

        if (isAnimating) return;        // Prevent multiple animations "at once"
        isAnimating = true;


        if (countsort_cubes == null)
        {
            countsort_cubes = CubeUtility.createCubeList(number_list, material, default_color);

            CubeUtility.positionCubeList(countsort_cubes, Total_Spacing, this);
        }

        // initialize glow handler
        glowHandler = new GlowHandler();
        glowHandler.material_normal = material;
        glowHandler.material_glow = material_glow;
        glowHandler.Init(countsort_cubes);  // VERY IMPORTAINT


        StartCoroutine(CountingSortAnimation());
    }

    public IEnumerator CountingSortAnimation()
    {
        // unmsorted_vector_list - The list of vector3's prior to being sorted. These will be used to move the cubes in their correct spot
        // everything else is a C# translation of the count-sort algorithm

        List<Vector3> unsorted_vector_list = new List<Vector3>();

        // copy each position vector, we will need these later
        foreach (var cube in countsort_cubes)       // thank you linq gods
        {
            unsorted_vector_list.Add(cube.transform.position);
        }

        /************************
         *    BEGIN ANIMATION
         ***********************/

        // for max value, im going to simply utilize number_list. Makes things simple
        int max_val = number_list.Max();

        // 2D list, set capaity equal to the highest integer value found in the list
        List<List<GameObject>> counting_list = new List<List<GameObject>>(max_val + 1);
        for (int i = 0; i < counting_list.Capacity; i++)
        {
            counting_list.Add(new List<GameObject>());      // INITIALIZE
        }


        while (countsort_cubes.Count > 0)
        {

            GameObject cube_to_move = countsort_cubes.Last();   // REFERENCE
            int value = int.Parse(cube_to_move.name);

            // pass reference
            counting_list.ElementAt(value).Add(cube_to_move);

            // move cube to the 2D list respectivley
            yield return moveTo2DCountingList(
                cube_to_move,
                value,                                      // row
                counting_list.ElementAt(value).Count - 1);  // column

            // POP
            countsort_cubes.RemoveAt(countsort_cubes.Count - 1);    // remove last element
        }


        int j = 0;
        for (int i = 0; i < counting_list.Count; i++)
        {
            while (counting_list[i].Count > 0)
            {                

                countsort_cubes.Add(counting_list[i].First());  // get back the cube's reference. IN ORDER (sorted)
                Debug.Log("HIT3      j_index is " + j.ToString() + " length of vectorlist: " + unsorted_vector_list.Count.ToString());

                // MOVE CUBE BACK
                yield return CubeUtility.moveCube(
                    countsort_cubes.Last(),
                    unsorted_vector_list[j],
                    Cube_SPEED);
                Debug.LogError("HIT3.5");

                counting_list[i].RemoveAt(0);  // POP
                Debug.LogError("HIT4");

                j++;        // incriment!! Animation only
            }
        }
        Debug.LogError("HIT5");

        // Move back each cube in a Sorted Manner now!!!

        yield return null;

        isAnimating = false;
    }

    public IEnumerator moveTo2DCountingList(GameObject cube, int row, int col)
    {
        const float SPACING = 1.3f;
        Vector3 destination = new Vector3(
            counting_list_pos.position.x + (SPACING * col),
            counting_list_pos.position.y + (SPACING * row),
            counting_list_pos.position.z);

        yield return CubeUtility.moveCube(cube, destination, Cube_SPEED);
    }
}
