using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRewind : MonoBehaviour
{
    public float rewindSeconds;                      // The amount of seconds ago to rewind to 
    public float intervalTime = 0.1f;                //how often to track the position, the lower the number the more accurate but more expensive on the CPU
    List<Vector3> positions = new List<Vector3>();   // The list of positions recorded
    float maxListSize;                               // The list count before the it pops out the oldest position and pushes in a new position

    private void Start()
    {
        maxListSize = Mathf.RoundToInt( rewindSeconds / intervalTime);
        StartCoroutine("TrackPosition");
    }

    IEnumerator TrackPosition ()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalTime);
            if (positions.Count < maxListSize)
            {
                positions.Add(transform.position);
            }
            else
            {
                positions.Add(transform.position);
                positions.RemoveAt(0);
            }
        }
    }

    public void Rewind ()
    {
        transform.position = positions[0];
        positions.Clear();
    }
}
