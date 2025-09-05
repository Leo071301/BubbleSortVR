using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 
 *          Author: Eliseo D.
 *          Date: May 2025
 * 
 * CubeUtility - Is a Static Class which impliments functions
 *              to help set-up and perform animations with 
 *              Unity's 'Cube Primitive' GameObjects
 *      
  * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*/

public class CubeUtility : MonoBehaviour
{
    /* * * * * * * * * * * * * * * * * * * * * * * *
     * 
     *  Functions to Create and Configure Cubes
     *
     * * * * * * * * * * * * * * * * * * * * * * * *
     */

    public static void addTextToCube(GameObject object_in, String text_in)
    {
        // create child text object
        GameObject text_obj = new GameObject(text_in);
        TextMeshPro text = text_obj.AddComponent<TextMeshPro>();
        RectTransform rectTransform = text_obj.GetComponent<RectTransform>();

        text_obj.transform.SetParent(object_in.transform);
        rectTransform.position = new Vector3(-0.25f, 0.05f, 0.52f);
        rectTransform.sizeDelta = new Vector2(1, 1);
        // reflect text ~ so that its not unreadable (like a mirror)
        rectTransform.localScale = new Vector3(
            -rectTransform.localScale.x,
            rectTransform.localScale.x,
            rectTransform.localScale.z);
        text.text = text_in;
        text.fontSize = 10;
        text.color = Color.black;

        // create another text lol
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
     * Apply mesh filters and renders using the material given.
     * 
     * Note: MUST BE Universal Render Pipeline Shader!!!!!!!
     */
    public static List<GameObject> createCubeList(List<int> list_in, Material material, Color color_in)
    {
        List<GameObject> cube_array = new List<GameObject>();

        for (int i = 0; i < list_in.Count; i++)
        {
            int index = i + 1;


            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshFilter m_filter = cube.GetComponent<MeshFilter>();
            MeshRenderer m_renderer = cube.GetComponent<MeshRenderer>();

            // apply shader
            m_renderer.material = material;

            // apply default color
            m_renderer.material.color = color_in;

            // apply cube name
            cube.name = list_in[i].ToString();     // the name will be the number itself

            // add text
            addTextToCube(cube, list_in[i].ToString());

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
     * This will poisition each cube evenly based off of the total length given,
     * its 'Transform' position, and
     * its 'Transform' rotation. 
     * 
     * invokingObject - is the 'this' pointer, the unity object which has the script attached.
     */
    public static void positionCubeList(List<GameObject> cube_list, float total_length, MonoBehaviour invokingObject)
    {
        // simple calculations
        float half_length = total_length / 2;
        float spacing = total_length / cube_list.Count;

        // fetching component info
        // y-axis angle in radiants, will allow us to find the rotation of the X-Y plane. Wierd right?!
        float angle_theta = invokingObject.transform.eulerAngles.y; // degrees
        float angle_theta_r = angle_theta * Mathf.PI / 180.0f;      // radiants


        Vector3 start_pos = invokingObject.transform.position; // fetch it's transform

        for (int i = 0; i < cube_list.Count; i++)
        {
            // set each cube's parent
            // NVM this BREAKS the swap coroutine
            //cube_list[i].transform.parent = invokingObject.transform;


            // rotate each cube
            cube_list[i].transform.Rotate(new Vector3(0, angle_theta * -1, 0));

            // apply scaling to each cube
            cube_list[i].transform.localScale = invokingObject.transform.localScale;

            // yay math
            // image a unit circle. Find where to position each cube - starting from 1 side of the unite circle, and ending at the other
            cube_list[i].transform.position =
                new Vector3((start_pos.x - half_length * Mathf.Cos(angle_theta_r)) + (spacing * Mathf.Cos(angle_theta_r) * i),
                            start_pos.y,
                            (start_pos.z - half_length * Mathf.Sin(angle_theta_r)) + (spacing * Mathf.Sin(angle_theta_r) * i));
        }
    }



    /* * * * * * * * * * * * * * * * * * * * * * * *
     * 
     *  C# Generators and Unity-Coroutines
     *
     * * * * * * * * * * * * * * * * * * * * * * * *
     */

    /*
     * Move cube from it's current postion to the destination vector
     * Move Timing and distance threshold can be readily modified to your liking
     */
    public static IEnumerator moveCube(GameObject cube, Vector3 destination, float speed = 8)
    {
        float move_time = Time.deltaTime * speed; // seconds
        float fTHRESHOLD = 0.1f;

        // keep moving until reaches destination
        while (Vector3.Distance(cube.transform.localPosition, destination) > fTHRESHOLD)
        {
            cube.transform.position = Vector3.Lerp(cube.transform.localPosition, destination, move_time);
            yield return null;  // pause, and come back next frame
        }
    }


    /*
     * This function is a C# Generator which helps simulate Animations https://stackoverflow.com/a/55289109
     * It swaps two cubes based off the given index
     * Uses yield statements to return, and at next frame - pick up where it left off
     * Also invokes co-routine moveCube() where the 'swap_isDone' flag will be set, and the next animation can be played.
     * 
     * "MonoBehaviour invokingClass" passes in the caller 'this' pointer, REQUIRED to use Co-Routines
     */
    public static IEnumerator swapCubesVertically(List<GameObject> cube_list, int first_index, int second_index, MonoBehaviour invokingClass)
    {
        GameObject cube1 = cube_list[first_index];
        GameObject cube2 = cube_list[second_index];

        Vector3 cube1_startPos = cube1.transform.position;
        Vector3 cube2_startPos = cube2.transform.position;

        /******* store position above cube 1 *******/
        Vector3 pos_above =
            new Vector3(cube1.transform.position.x, cube1.transform.position.y + 1.5f, cube1.transform.position.z);
        yield return invokingClass.StartCoroutine(moveCube(cube1, pos_above));


        /******* store position below cube 2 *******/
        Vector3 pos_below =
            new Vector3(cube2.transform.position.x, cube2.transform.position.y - 1.5f, cube2.transform.position.z);
        yield return invokingClass.StartCoroutine(moveCube(cube2, pos_below));

        /******* store position where cube 1 will slide over to *******/
        Vector3 pos1_slideOver =
            new Vector3(cube2_startPos.x, cube1.transform.position.y, cube2_startPos.z);
        yield return invokingClass.StartCoroutine(moveCube(cube1, pos1_slideOver));


        /******* store position where cube 2 will slide over to *******/
        Vector3 pos2_slideOver =
            new Vector3(cube1_startPos.x, cube2.transform.position.y, cube1_startPos.z);
        yield return invokingClass.StartCoroutine(moveCube(cube2, pos2_slideOver));


        /******* put cube1 where cube2 originaly was *******/
        yield return invokingClass.StartCoroutine(moveCube(cube1, cube2_startPos));


        /******* put cube2 where cube1 originaly was *******/
        yield return invokingClass.StartCoroutine(moveCube(cube2, cube1_startPos));
    }

    /*
    * Same as 'swapCubesVertically' 
    * but less steps. Swaps positions directly
    */
    public static IEnumerator swapCubesHorizontally(List<GameObject> cube_list, int first_index, int second_index, MonoBehaviour invokingClass)
    {
        GameObject cube1 = cube_list[first_index];
        GameObject cube2 = cube_list[second_index];

        Vector3 cube1_startPos = cube1.transform.localPosition;
        Vector3 cube2_startPos = cube2.transform.localPosition;


        /******* put cube1 where cube2 originaly was *******/
        invokingClass.StartCoroutine(moveCube(cube1, cube2_startPos));


        /******* put cube2 where cube1 originaly was *******/
        yield return invokingClass.StartCoroutine(moveCube(cube2, cube1_startPos));
    }

    /*
    * This generator will pull down the cube at the "from_index", shift along the cubes prior,
    * And insert this cube into the "to_index", quite visually apealing actually.
    */
    public static IEnumerator insertCubeAndShift(List<GameObject> cube_lists, int from_index, int to_index, MonoBehaviour invokingClass)
    {

        // We going to store  a list of positions inorder to shift each cube. FiLO Order
        Stack<Vector3> shiftPositions = new Stack<Vector3>();

        for (int i = to_index; i <= from_index; i++)    // i found a bug here x_x rip lol
        {
            shiftPositions.Push(cube_lists[i].transform.position);
        }

        // preparing move out the way!
        Vector3 posBelow = shiftPositions.Peek();   // acess the top element in the stack
        posBelow.y -= 3;
        yield return invokingClass.StartCoroutine(moveCube(cube_lists[from_index], posBelow));  // move out the way!


        // shift cubes, 1 by 1        
        for (int k = from_index - 1; k >= to_index; k--)
        {
            yield return invokingClass.StartCoroutine(moveCube(cube_lists[k], shiftPositions.Pop()));    // pop the stack! Each cube will slide along 1 by 1
        }

        // at this point, only 1 position is left in the stack, the "from_index" position, 
        // now we can insert our cube

        // preparing to slide!
        posBelow = shiftPositions.Peek();
        posBelow.y -= 3;
        yield return invokingClass.StartCoroutine(moveCube(cube_lists[from_index], posBelow));              // slide!
        yield return invokingClass.StartCoroutine(moveCube(cube_lists[from_index], shiftPositions.Pop()));   // and insert!

        // NOTE: NOTE: NOTE: This is an animation only, "cube_lists" is still unchanged.
    }

    /*
    * Animates each cube sequentially, placing cubes at y -50 and 
    * bringing it back to its original y position
    */
    public static IEnumerator AnimateSpawnCubes(List<GameObject> cube_list, MonoBehaviour invokingClass)
    {
        foreach (var cube in cube_list)
        {
            Vector3 destination = cube.transform.position;

            //place above the world
            cube.transform.position = new Vector3 (destination.x, destination.y + 100, destination.z);

            //and move back! viola!
            invokingClass.StartCoroutine(moveCube(cube, destination));
        }
        yield return new WaitForSeconds(1);         // optional
    }

    /*
    * Animates each cube sequentially, placing cubes 10 units below its y-level and 
    * destroying cubes
    */
    public static IEnumerator AnimateDestroyCubes(List<GameObject> cube_list, MonoBehaviour invokingClass)
    {
        foreach (var cube in cube_list)
        {
            Vector3 destination = cube.transform.position;

            destination = new Vector3(destination.x, destination.y - 15, destination.z);

            // BEGONE!!!
            invokingClass.StartCoroutine(moveCube(cube, destination));
        }

        yield return new WaitForSeconds(1);     // wait before destroying objects

        foreach (var cube in cube_list)
        {
            Destroy(cube);  // adios
        }

        // cant set to null here, i would only be overriding the reference
        // i would need a pointer for that, meh ill do it in the sortingscript
    }



    /*
     * This Generator will change the color of 'obj' instantly to color_in ( a pulse )
     * And gradually turn its back to its original color
     * 'highlight_time' is 1.5 Seconds by default, You can pass in a longer time and the HighLight animation will last longer
     * https://www.youtube.com/watch?v=pbU2tInJTOQ
     */
    public static IEnumerator PulseHighlight(GameObject obj, Color color_in, float highlight_time = 1.5f)
    {
        float timeElapsed = 0f;

        // the target color will be it's current 
        Color current_color = color_in;

        MeshRenderer m_renderer = obj.GetComponent<MeshRenderer>();
        Color target_color = m_renderer.material.color;

        // keep changing colors untill target color is reached
        while (timeElapsed < highlight_time && m_renderer != null)
        {
            timeElapsed += Time.deltaTime * 1.5f / highlight_time;

            //m_renderer.material.color = Color.Lerp(tartet_color, current_color, Mathf.PingPong(timeElapsed , 1));
            m_renderer.material.color = Color.Lerp(current_color, target_color, timeElapsed);
            yield return null;
        }

    }

    /* * * * * * * * * * * * * * * * * * * * * * * *
     * 
     *  Good-To-Have Behaviour
     *
     * * * * * * * * * * * * * * * * * * * * * * * *
     */

    // Uses Sinusoidal functions to impliment floating behaviour
    public static void floatCubes(List<GameObject> cube_list)
    {
        // Scale factor of 3000 for Unity
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


}

// BIG Credit to Dev Log for this wonderfull Design Aspect regarding Co-Routines
// https://onewheelstudio.com/blog/2022/8/16/chaining-unity-coroutines-knowing-when-a-coroutine-finishes
