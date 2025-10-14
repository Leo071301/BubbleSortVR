using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [Header("Drag in the Text-Mesh-Pro to attach!")]
    [SerializeField] public TextMeshPro textMeshPro = null;
    [SerializeField] public Color highlightColor;
    private string previousMark;    // store for cache

    // magical markup highlight trick. 
    [Header("HTML Tags")]
    [SerializeField] public string highlight_start = "<mark=#000000>";
    [SerializeField] public string highlight_end = "</mark>";
    [Space]

    [Header("Preview the Text-Mesh-Pro object! Ignore the errors lol")]
    [Range(0, 25)]               // allow slider in the inspector
    [SerializeField] public int previewIndex = 0;   // which text element we will preview

    [Space]
    [TextArea(5, 20)]           // allow text entry in the inspector
    public List<string> text_list;

    [Header("Create only ONE text entry, and then-> click this check")]
    public bool Auto_Generate_TextList = false;


    // Simply change textMeshPro's text contents
    // I hate using magic numbers, but I cant think of anything better. Or I could make a giant list of Enum's
    public void syncLiveText(int index)
    {
        // make sure text-object is attached!
        if (textMeshPro == null)
        {
            Debug.Log("Please attach a TextMeshPro to this Object!!! ");
            return;
        }

        // update the text!
        textMeshPro.text = text_list[index];
    }

    // Co-routine - Change textMeshPro's text contents
    // with a delay, given ftime as a float
    public IEnumerator syncLiveTextWait(int index, float time_)
    {
        syncLiveText(index);

        yield return new WaitForSeconds(time_);
    }


    private void OnValidate()
    {
        previousMark = highlight_start;
        // update html tag in the inspector
        highlight_start =
            "<mark=#" + ColorUtility.ToHtmlStringRGBA(highlightColor) + ">";

        for (int i = 0; i < text_list.Count; i++)
        {
            text_list[i] = text_list[i].Replace(previousMark, highlight_start);     // nice and nifty handy dandy convinient function
        }

        
        syncLiveText(previewIndex);






        // Auto Generate list of text's
        if (text_list.Count != 1)   return;     // has to be size = 1
        if (!Auto_Generate_TextList) return;    // flag has to be on

        Auto_Generate_TextList = false;


        string TEXT_BASE = text_list[0];

        const int LEN_OF_HTMLTAG = 16;
        const int LEN_OF_WHITETEXT = 2;
        int prev_delim_index = 0;

        // Repeat for however many "\n" delimiters are found
        // Im pissed at how long it took me to debug this smh
        for (int i = 1; i < text_list[0].Count() + 1; i++)
        {
            // append new text_base
            text_list.Add(TEXT_BASE);

            // now add the HTML tags
            text_list[i] = text_list[i].Insert(prev_delim_index, highlight_start);   // first "\n"

            Debug.Log("absolute val: " + (TEXT_BASE.IndexOf("\n", prev_delim_index + 1) - prev_delim_index).ToString());
            if (TEXT_BASE.IndexOf("\n", prev_delim_index + 1) == prev_delim_index + LEN_OF_WHITETEXT)  // Filter Out WHITE SPACE
            {
                // not the best practice
                prev_delim_index = TEXT_BASE.IndexOf("\n", prev_delim_index + 1);   // update delim index
                text_list.RemoveAt(text_list.Count - 1);    // No need for an "unhighlited" codeblock
                i--;        // i hate this
                continue;
            }

            prev_delim_index = TEXT_BASE.IndexOf("\n", prev_delim_index + 1);   // update delim index


            if (prev_delim_index == -1) // ONCE WE'VE REACHED THE END OF THE STRING
            {
                text_list[i] += highlight_end;
                return;
            }

            // now add the HTML tags
            text_list[i] = text_list[i].Insert(prev_delim_index + LEN_OF_HTMLTAG, highlight_end);     // next "\n"

        }

    }

}
