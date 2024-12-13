using UnityEngine;


[System.Serializable]
public class Evidence
{
    
    public string evidenceName;  // Name of the evidence
    public int evidenceID;      // Unique ID for each evidence
    public AudioClip collectSound;  // Sound to play when the evidence is collected
}