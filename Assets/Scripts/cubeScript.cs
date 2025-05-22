using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


public class cubeScript : MonoBehaviour
{
    /*
     *  Member Variables
     */

    [SerializeField] public Vector3 bubble_sort_pos;        // Position
    [SerializeField] public List<int> bubble_sort_numbers;  // List given thru the 'inspector tab', used to create List of GameObjects
    [SerializeField] public GameObject Interactable_bubble;// pass in Interactable used to triggle the animation, will disable the gameobject during...and re-enable it once done

    [NonSerialized]
    private List<GameObject> bubblesort_cubes = null;       // Cubes created, positioned, and programmed by this script



    /* * * * * * * * * * * * * * * * * * * * * * * *
     * 
     *  Functions to Create and Configure Cubes
     *
     * * * * * * * * * * * * * * * * * * * * * * * *
     */
    public void addTextToCube(GameObject object_in, String text_in)
    {
        // create child text object
        GameObject text_obj = new GameObject(text_in);
        TextMeshPro text = text_obj.AddComponent<TextMeshPro>();
        RectTransform rectTransform = text_obj.GetComponent<RectTransform>();

        text_obj.transform.SetParent(object_in.transform);
        rectTransform.position = new Vector3(-0.25f, 0.05f, 0.52f);
        rectTransform.sizeDelta = new Vector2(1, 1);
        rectTransform.localScale = new Vector3(
            -rectTransform.localScale.x,
            rectTransform.localScale.x,
            rectTransform.localScale.z);
        text.text = text_in;
        text.fontSize = 10;
        text.color = Color.black;

        // create another lol
        GameObject text_obj2 = new GameObject(text_in);
        TextMeshPro text2 = text_obj2.AddComponent<TextMeshPro>();
        RectTransform rectTransform2 = text_obj2.GetComponent<RectTransform>();

        text_obj2.transform.SetParent(object_in.transform);
        rectTransform2.position = new Vector3(0.25f, 0.05f, -0.52f);
        rectTransform2.sizeDelta = new Vector2(1, 1);
        text2.text = text_in;
        text2.fontSize = 10;
        text2.color = Color.black;

    }

    /*
     * Return a C# List of <GameObject>'s with size_in to determine list size
     * Apply mesh filters and renders accordingly, can be readily modified.
     */
    public List<GameObject> createCubeList(int size_in)
    {
        List<GameObject> cube_array = new List<GameObject>();

        for (int i = 0; i < size_in; i++)
        {
            int index = i + 1;


            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshFilter m_filter = cube.GetComponent<MeshFilter>();
            MeshRenderer m_renderer = cube.GetComponent<MeshRenderer>();

            // decorate cube
            cube.name = "Cube " + index;
            m_renderer.material.color = Color.white;

            // add text
            addTextToCube(cube, index.ToString());

            // add noise
            System.Random noise = new System.Random();
            cube.transform.Rotate(new Vector3(
                noise.Next(-3, 3),
                noise.Next(-3, 3),
                noise.Next(-3, 3)));

            cube_array.Add(cube);
        }

        return cube_array;
    }

    /*
     * Given a list of cubes
     * This will poisition each cube evenly based off of the total length given.
     */
    public void positionCubeList(List<GameObject> cube_list, Vector3 start_pos, float total_length)
    {
        // I will focus on positioning on the x-axis
        float half_length = total_length / 2;
        float spacing = total_length / cube_list.Count;

        for (int i = 0; i < cube_list.Count; i++)
        {

            // offset each cube
            cube_list[i].transform.position =
                new Vector3((start_pos.x + half_length) - (spacing * i),
                start_pos.y,
                start_pos.z);

        }
    }



    /* * * * * * * * * * * * * * * * * * * * * * * *
     * 
     *  C# Generators and Unity-Coroutines
     *
     * * * * * * * * * * * * * * * * * * * * * * * *
     */


    // Not the best practice but meh
    bool swap_isDone = false;


    /*
     * C# Generator
     * Move cube from it's current postion to the destination vector
     * Move Timing and distance threshold can be readily modified to your liking
     */
    public IEnumerator moveCubeTest(GameObject cube, Vector3 destination)
    {
        // define variables
        float move_time = Time.deltaTime * 5; // seconds
        float fTHRESHOLD = 0.1f;
        swap_isDone = false;

        while (Vector3.Distance(cube.transform.localPosition, destination) > fTHRESHOLD)
        {
            cube.transform.position = Vector3.Lerp(cube.transform.localPosition, destination, move_time);
            yield return null;  // dont return anything
        }

        swap_isDone = true;
        yield return null;

        // mabye used later
        //cube.transform.position = Vector3.SmoothDamp(cube.transform.position, destination, ref velocity, move_time);

    }


    /*
     * This function is a C# Generator which helps simulate Animations https://stackoverflow.com/a/55289109
     * It swaps two cubes based off the given index
     * Uses yield statements to return, and at next frame - pick up where it left off
     * Also invokes co-routine moveCube() where the 'swap_isDone' flag will be set, and the next animation can be played.
     */
    public IEnumerator swapCubesTest(List<GameObject> cube_list, int first_index, int second_index)
    {
        GameObject cube1 = cube_list[first_index];
        GameObject cube2 = cube_list[second_index];

        Vector3 cube1_startPos = cube1.transform.position;
        Vector3 cube2_startPos = cube2.transform.position;

        /******* store position above cube 1 *******/
        Vector3 pos_above =
            new Vector3(cube1.transform.position.x, cube1.transform.position.y + 1.5f, cube1.transform.position.z);
        StartCoroutine(moveCubeTest(cube1, pos_above));
        while (!swap_isDone)
        {
            yield return null;
        }


        /******* store position below cube 2 *******/
        Vector3 pos_below =
            new Vector3(cube2.transform.position.x, cube2.transform.position.y - 1.5f, cube2.transform.position.z);
        StartCoroutine(moveCubeTest(cube2, pos_below));
        while (!swap_isDone)
        {
            yield return null;
        }

        /******* store position where cube 1 will slide over to *******/
        Vector3 pos1_slideOver =
            new Vector3(cube2_startPos.x, cube1.transform.position.y, cube2_startPos.z);
        StartCoroutine(moveCubeTest(cube1, pos1_slideOver));
        while (!swap_isDone)
        {
            yield return null;
        }


        /******* store position where cube 2 will slide over to *******/
        Vector3 pos2_slideOver =
            new Vector3(cube1_startPos.x, cube2.transform.position.y, cube1_startPos.z);
        StartCoroutine(moveCubeTest(cube2, pos2_slideOver));
        while (!swap_isDone)
        {
            yield return null;
        }


        /******* put cube1 where cube2 originaly was *******/
        StartCoroutine(moveCubeTest(cube1, cube2_startPos));
        while (!swap_isDone)
        {
            yield return null;
        }
        /******* put cube2 where cube1 originaly was *******/
        StartCoroutine(moveCubeTest(cube2, cube1_startPos));
        while (!swap_isDone)
        {
            yield return null;
        }

    }

    /*
     * This Generator will change the color of 'obj' instantly to color_in ( a pulse )
     * And gradually turn its back to its original color
     * https://www.youtube.com/watch?v=pbU2tInJTOQ
     */
    public IEnumerator PulseHighlight(GameObject obj, Color color_in)
    {
        float timeElapsed = 0f;

        // the target color will be it's current 
        Color current_color = color_in;

        MeshRenderer m_renderer = obj.GetComponent<MeshRenderer>();
        Color target_color = m_renderer.material.color;

        // keep changing colors untill target color is reached
        while (timeElapsed < 1.5f && m_renderer != null)
        {
            timeElapsed += Time.deltaTime;

            //m_renderer.material.color = Color.Lerp(tartet_color, current_color, Mathf.PingPong(timeElapsed , 1));
            m_renderer.material.color = Color.Lerp(current_color, target_color, timeElapsed);
            yield return null;
        }

    }

    public IEnumerator bubbleSort()
    {
        Interactable_bubble.SetActive(false);

        StartCoroutine(swapCubesTest(bubblesort_cubes, 2, 5));
        StartCoroutine(PulseHighlight(bubblesort_cubes[2], Color.cyan));
        StartCoroutine(PulseHighlight(bubblesort_cubes[5], Color.cyan));
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        Interactable_bubble.SetActive(true);

    }

    /* * * * * * * * * * * * * * * * * * * * * * * *
     * 
     *  Misc. Functions
     *
     * * * * * * * * * * * * * * * * * * * * * * * *
     */

    // Uses Sinusoidal functions to impliment floating behaviour
    public void floatCubes(List<GameObject> cube_list)
    {
        // Scale factor of 3000 for unity
        // Scale factor of 300 for Spatial VR
        const int SCALE = 300;

        for (int i = 0; i < cube_list.Count; i++)
        {
            cube_list[i].transform.position = new Vector3(
                cube_list[i].transform.position.x,
                cube_list[i].transform.position.y + Mathf.Sin(Time.time + i) / SCALE,
                cube_list[i].transform.position.z + Mathf.Sin(Time.time + i) / SCALE * 2);
        }
    }


    // Invoked by Spatial SDK Interactable
    public void BubbleSortEvent()
    {
        const int ARRAYSIZE = 8;
        const int TOTAL_LENGTH = 15;
        bubblesort_cubes = createCubeList(ARRAYSIZE);

        positionCubeList(bubblesort_cubes, bubble_sort_pos, TOTAL_LENGTH);

        StartCoroutine(bubbleSort());
    }



    // Update is called once per frame
    void Update()
    {
        // Only when the cubes exist (created on event)
        if (bubblesort_cubes != null)
            floatCubes(bubblesort_cubes);

    }
}
