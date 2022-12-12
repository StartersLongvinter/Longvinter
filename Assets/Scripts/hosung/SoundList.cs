using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "SoundList/Add SoundList", order = 0)]
public class SoundList : ScriptableObject
{
    public string listName = "";
    public AudioClip[] clips;
}
