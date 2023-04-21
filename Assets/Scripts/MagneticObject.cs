using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    public enum Polarity
    {
        POSITIVE,
        NEGATIVE,
        BOTH_ATTRACTIVE,
        BOTH_REPULSIVE
    }

    public Polarity polarity;

    public bool AffectPlayer;
    public bool AffectSelf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
