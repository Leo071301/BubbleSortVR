using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 
 *          Author: Eliseo D.
 *          Date: May 2025
 * 
 * LiveTextUtility - Is a Class which helps modify a given 'TextMeshPro'
 *                  object and achieve a "syncing" of text. Most properties
 *                  of this script are acessable in the inspector.
 *                  
 *                  This script will allow you to store a varying list of text-entries,
 *                  and be able to jump between 'versions' either via preview, or
 *                  via scripting. 
 *      
  * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*/

// Tip: i reccomend a font-size of 0.75 for a nice readable VR expirience
[Serializable]
public class LiveTextUtility : MonoBehaviour
{
    // TextMeshProp object to be passed in
    [SerializeField] public TextMeshPro textMeshPro = null;
    [SerializeField] public Color highlightColor;

    // magical markup highlight trick. 
    [SerializeField] public string highlight_start = "<mark=\"#000000\">";
    [SerializeField] public string highlight_end = "</mark>";

    [Range(0, 6)]               // allow slider in the inspector
    [SerializeField] public int previewIndex = 0;   // which text element we will preview

    [TextArea(5, 20)]           // allow text entry in the inspector
    public List<string> text_list;


    public void syncLiveText(int index)
    {
        // make sure text-object is attached!
        if (textMeshPro == null)
        {
            Debug.Log(" Please Attach a TextMeshPro object to this script! ");
            return;
        }

        // update the text!
        textMeshPro.text = text_list[index];
    }

    private void OnValidate()
    {
        // update html tag in the inspector
        highlight_start =
            "<mark=\"#" + ColorUtility.ToHtmlStringRGBA(highlightColor) + "\">";
        
        syncLiveText(previewIndex);

    }

}
