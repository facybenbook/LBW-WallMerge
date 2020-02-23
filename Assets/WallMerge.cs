﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMerge : MonoBehaviour
{
    private Vector3 closestCorner;
    private Vector3 nextCorner;
    private Vector3 previousCorner;
    private Vector3 chosenCorner;

    public float lerp;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1))
            {
                if(hit.transform.GetComponentInChildren<RaySearch>() != null)
                {
                    RaySearch search = hit.transform.GetComponentInChildren<RaySearch>();
                    List<Vector3> cornerPoints = new List<Vector3>();
                    foreach (MeshPoint mp in search.cornerPoints)
                    {
                        cornerPoints.Add(mp.position);
                    }

                    closestCorner = GetClosestPoint(cornerPoints.ToArray(), hit.point);
                    int index = search.cornerPoints.FindIndex(x => x.position == closestCorner);

                    nextCorner = (index < search.cornerPoints.Count - 1) ? search.cornerPoints[index + 1].position : search.cornerPoints[0].position;
                    previousCorner = (index > 0) ? search.cornerPoints[index - 1].position : search.cornerPoints[search.cornerPoints.Count - 1].position;

                    print("<color=yellow><b>NEXT CORNER: </b></color>" + (hit.point - nextCorner).normalized);
                    print("<color=blue><b>PREVIOUS CORNER: </b></color>" + (hit.point - previousCorner).normalized);

                    chosenCorner =
                        (Mathf.Min((hit.point - nextCorner).magnitude, (hit.point - previousCorner).magnitude) == (hit.point - previousCorner).magnitude ? previousCorner : nextCorner);

                    float distance = Vector3.Distance(closestCorner, chosenCorner);
                    float playerDis = Vector3.Distance(chosenCorner, hit.point);

                    lerp = Mathf.Abs(distance - playerDis) / ((distance + playerDis) / 2);

                    FindObjectOfType<MoveTest>().transform.position = Vector3.Lerp(closestCorner, chosenCorner, lerp);
                }
            }
        }
    }
    Vector3 GetClosestPoint(Vector3[] points, Vector3 currentPoint)
    {
        Vector3 pMin = Vector3.zero;
        float minDist = Mathf.Infinity;

        foreach (Vector3 p in points)
        {
            float dist = Vector3.Distance(p, currentPoint);
            if (dist < minDist)
            {
                pMin = p;
                minDist = dist;
            }
        }
        return pMin;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward);
        Gizmos.DrawSphere(closestCorner, .2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(previousCorner, .2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(nextCorner, .2f);

    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}