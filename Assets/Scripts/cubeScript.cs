using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class cubeScript : MonoBehaviour
{

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


    List<GameObject> bubblesort_cubes


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        const int ARRAYSIZE = 8;
        const int TOTAL_LENGTH = 15;
        Vector3 list_position = new Vector3(-37, 3, -19);
        bubblesort_cubes = createCubeList(ARRAYSIZE);

        positionCubeList(bubblesort_cubes, list_position, TOTAL_LENGTH);
    }

    // Update is called once per frame
    void Update()
    {


    }
}

