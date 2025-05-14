using SpatialSys.UnitySDK;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class cubeScript : MonoBehaviour
{

    public void addTextToCube(GameObject object_in, String text_in)
    {
        // create child text object
        GameObject text_obj = new GameObject(text_in);
        Text text = text_obj.AddComponent<Text>();
        RectTransform rectTransform = text_obj.GetComponent<RectTransform>();

        text_obj.transform.SetParent(object_in.transform);
        rectTransform.position = new Vector3(-0.15f, 0.05f, 0.26f);
        rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = new Vector3(
            -rectTransform.localScale.x,
            rectTransform.localScale.x,
            rectTransform.localScale.z);
        text.text = text_in;
        text.fontSize = 5;
        text.color = new Color(77, 65, 66); // greyish red

        // create another lol
        GameObject text_obj2 = new GameObject(text_in);
        Text text2 = text_obj2.AddComponent<Text>();
        RectTransform rectTransform2 = text_obj2.GetComponent<RectTransform>();

        text_obj2.transform.SetParent(object_in.transform);
        rectTransform2.position = new Vector3(0.12f, 0.05f, -0.26f);
        rectTransform2.sizeDelta = new Vector2(0.5f, 0.5f);
        text2.text = text_in;
        text2.fontSize = 5;
        text2.color = new Color(77, 65, 66); // greyish red

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
            cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            // add text
            addTextToCube(cube, index.ToString());

            // add noise
            System.Random noise = new System.Random();
            cube.transform.Rotate(new Vector3(
                noise.Next(-3,3),
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
                new Vector3((start_pos.x - half_length) + (spacing * i),
                start_pos.y,
                start_pos.z);

        }
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        const int ARRAYSIZE = 14;
        const int TOTAL_LENGTH = 14;
        Vector3 list_position = new Vector3(-37,3,-19);
        List<GameObject> cubes = createCubeList(ARRAYSIZE);


        positionCubeList(cubes, list_position, TOTAL_LENGTH);
    }

    // Update is called once per frame
    void Update()
    {

        
    }
}