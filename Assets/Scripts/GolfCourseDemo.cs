using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GolfCourseDemo : MonoBehaviour
{
    /*
     
     These are the functions that create and set up the cubes. 
     
     */

    public static void addTextToCube(GameObject object_in, String text_in) // takes in game object and text that you are adding to it 
    {
        // here 
        GameObject text_obj = new GameObject(text_in); // creates empty game object named after the text passed into it. object named after content 
        TextMeshPro text = text_obj.AddComponent<TextMeshPro>(); // this is what actually makes it show in the world 
        RectTransform rectTransform = text_obj.GetComponent<RectTransform>(); // ??? 

        text_obj.transform.SetParent(object_in.transform);
        rectTransform.position = new Vector3(-0.25f, 0.05f, 0.52f);
        rectTransform.sizeDelta = new Vector2(1, 1);
    }



    /*
     
     This function returns a list of cube game objects based on the size of the input list using list_in

    mesh - 3d shape and structure. faces, corners, edges, geometry shit 
    mesh filter - cointainer that holds the mesh. This tells Unity what shape the game object should use. Cube mesh for this one 
    mesh renderer - makes shape visible on screen. applies color, material, lighting, and shadows to cube shape. Without this, shit would be invisible 
    material - defines how surface of object looks like. texture, color, like whether its shiny or matte 
    collider - used for physics 
    noise - refers to random, small variations like visual texture or motion jitter 

     
     */

    public List<GameObject> createCubeList(int size_in)
    {
        List<GameObject> cube_array = new List<GameObject>(); // this creates a list that holds many Game Objects named cube_array 

        for(int i = 0; i < size_in; i++) // loops for each cube we want to create, based on size input given 
        {
            int index = i + 1; // starts index at 1 instead of 0 to have natural count, just for label/text clarity

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); // creates primitive cube with default mesh filter, mesh rendered, and collider 
            MeshFilter m_filter = cube.GetComponent<MeshFilter>(); // define this so you can access created mesh filter 
            MeshRenderer m_renderer = cube.GetComponent<MeshRenderer>(); // define this so you can access created mesh renderer to change color, etc 

            // adds a number label to the current cube, using current index as visible text  
            addTextToCube(cube, index.ToString());

            // applies small random rotations to each cube to make them look more natural and less identical
            System.Random noise = new System.Random(); // creates random number generator called noise 
            cube.transform.Rotate(new Vector3( // from cube, grab transform (position, rotate, scale) and rotate it x,y,z degrees in each axis 
                // each axis gets a random number between -3 and 2 
                noise.Next(-3, 3), // x axis 
                noise.Next(-3, 3), // y axis 
                noise.Next(-3, 3))); // z axis 

            cube_array.Add(cube); // take this cube we are currently on and just created --> add it to the list cube_array 
        }

        return cube_array; // return array of cubes 
    }


    /*
     
    Given a list of cubes
    This will position each cube evenly based off of the total length given 

    we pass the list of cubes, the starting positions for all 3 axis (3d location), and total length you want the cubes spread out  
     
     */

    public void positionCubeList(List<GameObject> cube_list, Vector3 start_pos, float total_length)
    {
        // this wil be used to position x axis since this is a horizontal row of cubes, this keeps it simple 
        float half_length = total_length / 2; // this will center the whole layout so cubes are evenly distributed from the middle  
        float spacing = total_length / cube_list.Count; // this is the horizontal distance between each cube 

        for(int i = 0; i < cube_list.Count; i++) // iterates through cube list count (how many cubes)
        {
            // offset each cube 
            cube_list[i].transform.position = new Vector3(
                (start_pos.x + half_length) - (spacing * i), // starts at right edge of row and spaces each cube evenly to the left
                start_pos.y, // keep vertical level the same 
                start_pos.z); // keep distance in scene 
        }
    }


    /*
     
   ********    C# Generators and Unity Coroutines ********** 

    C# Generators - a special type of function that can pause its work and resume later, letting it return values one at a time using the yield keyword 

    Unity Coroutines - Unitys version of a generator, used to run actions over time (like animations or delays) without freezing the rest of the game 
     
     */
}
