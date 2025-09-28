using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 
 *  GlowHandler - is a class which stores a List of Game-objects and the Mesh-Render material attached,
 *                      this is done via hashmap/dictionary to facilitate looking up the correct
 *                      mesh-renderer given any game object. 
 *                      
 *                      Methods are implimented to apply / reset "glowing" material given a subsection 
 *                      of the gameobject-list.
 *                      
 *  * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 */
public class GlowHandler : MonoBehaviour
{
    // I discovered C# has easy getters/setters :) take that Java!
    public Material material_normal {  get; set; }
    public Material material_glow   { get; set; }


    // Declaring Hash-Map
    public Dictionary<GameObject, MeshRenderer> objectMeshRenderers =
        new Dictionary<GameObject, MeshRenderer>();

    // A reference of our cube gameobjects
    public List<GameObject> gameobject_list;

    // total_list of gameobjects, must be 3-D with Mesh-Renderer component
    // VERY EXPENSIVE operations, ONLY CALL in the Start/Event() function.
    public void Init(List<GameObject> total_list)
    {
        for (int i = 0; i < total_list.Count; i++)
        {
            objectMeshRenderers.Add(total_list[i],                                 // key
                                    total_list[i].GetComponent<MeshRenderer>());   // value
        }
    }

    public void ResetApplyGlowMaterial
    (List<GameObject> total_list, List<GameObject> sub_list)
    {
        foreach (var gameobject in total_list)
        {
            // get renderer
            if (objectMeshRenderers.TryGetValue(gameobject, out var renderer))
            {
                renderer.material = material_normal;
            }
            else { Debug.LogError("Renderer1 NOT FOUND! "); }
        }

        //foreach (var gameobject in sub_list)
        for (int i = 0; i < sub_list.Count; i++)
        {
            // get renderer
            if (objectMeshRenderers.TryGetValue(sub_list[i], out var renderer))
            {
                renderer.material = material_glow;
            }
            else { Debug.LogError("Renderer2 NOT FOUND! "); }
        }
    }

    public void ApplyAllGlow(List<GameObject> total_list)
    {
        foreach (var gameobject in total_list)
        {
            // get renderer
            if (objectMeshRenderers.TryGetValue(gameobject, out var renderer))
            {
                renderer.material = material_glow;
            }
            else { Debug.LogError("Renderer NOT FOUND! "); }
        }
    }
    public void ResetAllGlow(List<GameObject> total_list)
    {
        foreach (var gameobject in total_list)
        {
            // get renderer
            if (objectMeshRenderers.TryGetValue(gameobject, out var renderer))
            {
                renderer.material = material_normal;
            }
            else { Debug.LogError("Renderer NOT FOUND! "); }
        }

    }
}
