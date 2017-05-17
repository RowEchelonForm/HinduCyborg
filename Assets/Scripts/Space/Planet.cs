using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public string planetName;

    [TextArea(3, 10)]
    public string description;

    public string level;

    public bool sun;
    
    public bool hasLevel;
}
