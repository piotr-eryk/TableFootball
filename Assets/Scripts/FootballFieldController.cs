using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballFieldController : MonoBehaviour
{
    [SerializeField]
    private List<DotController> dotsList;
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private float maxDistanceToClickableDot;
    [SerializeField]
    private GameObject lineRendererPrefab;
    [SerializeField]
    private Transform firstStartingPoint;

    private List<Transform> points = new();
    private DotController previousDot;
    private int pointCount = 0;
    private bool anotherPlayerTurn;
    private Transform currentStartingPoint;

    private void Awake()
    {
        currentStartingPoint = firstStartingPoint;
        lineRenderer.positionCount = 0;

        foreach (var dot in dotsList)
        {
            dot.OnDotClick += TryToDrawLine;
            dot.OnDotClick += DisableWrongColliders;
        }
    }

    private void OnDestroy()
    {
        foreach (var dot in dotsList)
        {
            dot.OnDotClick -= TryToDrawLine;
            dot.OnDotClick -= DisableWrongColliders;
        }
    }

    private void TryToDrawLine(DotController dot)
    {
        if (dot != previousDot && CanDrawLine(previousDot, dot))
        {
            pointCount++;

            if (pointCount < 2)
            {
                lineRenderer = Instantiate(lineRenderer);
                lineRenderer.positionCount = 1;
            }
            else
            {
                lineRenderer.positionCount = pointCount;
                pointCount = 0;

                if (anotherPlayerTurn)
                {
                    pointCount = 0;
                }
            }


            points.Add(dot.transform);
            lineRenderer.SetPosition(lineRenderer.positionCount-1, dot.transform.position);
            previousDot = dot;
        }
    }

    private bool CanDrawLine(DotController currentDot, DotController chosenDot)
    {
        if (points.Count < 2)
        {
            return true;
        }

        List<int> listOfIndexes = new();
        if (chosenDot)
        {
            var nieWiem3 = points.FindAll(x => x.transform.position == chosenDot.transform.position);//moze lepiej by bylo nie odnosic sie to position
            foreach (var x in nieWiem3)
            {
                listOfIndexes.Add(points.IndexOf(chosenDot.transform));//wszystkie punkty sa takie same w liscie
            }
        }

        foreach (var nieWiem2 in listOfIndexes)
        {
            if ((nieWiem2 - 1 >= 0 && lineRenderer.GetPosition(nieWiem2 - 1) == currentDot.transform.position) || (lineRenderer.GetPosition(nieWiem2 + 1) == currentDot.transform.position))
            {
                return false;
            }
        }
        return true;
    }

    public void ResetAllPoints()
    {
        lineRenderer.positionCount = 0;
        points.Clear();
    }

    public void DisableWrongColliders(DotController currentDot)//nazwa
    {
        foreach (var dot in dotsList)
        {
            if ((dot.transform.position - currentDot.transform.position).magnitude > maxDistanceToClickableDot)
            {
                dot.GetComponent<Collider2D>().enabled = false;//bez get component
            }
            else
            {
                dot.GetComponent<Collider2D>().enabled = true;
            }
        }
    }
}
