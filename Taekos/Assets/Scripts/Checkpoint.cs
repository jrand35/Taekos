using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {
    public int checkpointIndex;
    private SpriteRenderer renderer;
    private Color normal;
    private Color got;

    void Start()
    {
        normal = new Color(0.25f, 0.25f, 0.25f, 1f);
        got = new Color(1f, 1f, 1f, 1f);
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = normal;
    }

    public int getCheckpointIndex(){
        return checkpointIndex;
    }

    public void getCheckpoint()
    {
        renderer.color = got;
    }
}
