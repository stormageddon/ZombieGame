using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
    float destroyHeight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartSink() {
        // get height of terrain at this's position (actually 5 meters under terrain)
        destroyHeight = Terrain.activeTerrain.SampleHeight(this.transform.position) - 5;
        Collider[] colliders = this.transform.GetComponentsInChildren<Collider>();
        foreach(Collider col in colliders)
        {
            Destroy(col);
        }

        InvokeRepeating("SinkIntoGround", 10, 0.05f);
    }

    // Update is called once per frame
    void SinkIntoGround()
    {
        this.transform.Translate(0, -0.001f, 0);
        if (this.transform.position.y < destroyHeight)
        {
            Destroy(this.gameObject);
        }
    }
}
