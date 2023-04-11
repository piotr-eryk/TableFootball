using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FootballFieldController : MonoBehaviour
{
    [SerializeField]
    private List<DotController> dotsList;
    [SerializeField]
    private LineRenderer firstPlayerLine;
    [SerializeField]
    private LineRenderer secondPlayerLine;

    [SerializeField]
    private float maxDistanceToClickableDot;
    [SerializeField]
    private DotController firstStartingPoint;

    private List<Transform> points = new();
    private int pointCount = 0;
    private bool anotherPlayerTurn = true;
    private DotController currentStartingPoint;
    private LineRenderer currentPlayerLine;

    private ObjectPool<LineRenderer> firstPlayerLinePool;
    private ObjectPool<LineRenderer> secondPlayerLinePool;

    private void Awake()
    {
        currentPlayerLine = firstPlayerLine;
        currentStartingPoint = firstStartingPoint;
        firstPlayerLine.positionCount = 0;

        foreach (var dot in dotsList)
        {
            dot.OnDotClick += TryToDrawLine;
            dot.OnDotClick += ChangeCollidersEnabled;
        }
    }

    private void Start()
    {
        CreateFirstLinePoint();

        firstPlayerLinePool = new ObjectPool<LineRenderer>(createFunc: () => Instantiate(firstPlayerLine),
actionOnGet: (obj) => obj.gameObject.SetActive(true), actionOnRelease: (obj) => obj.gameObject.SetActive(false),
actionOnDestroy: (obj) => Destroy(obj), collectionCheck: false, defaultCapacity: 20, maxSize: 50);

        secondPlayerLinePool = new ObjectPool<LineRenderer>(createFunc: () => Instantiate(secondPlayerLine),
actionOnGet: (obj) => obj.gameObject.SetActive(true), actionOnRelease: (obj) => obj.gameObject.SetActive(false),
actionOnDestroy: (obj) => Destroy(obj), collectionCheck: false, defaultCapacity: 20, maxSize: 50);
    }

    private void OnDestroy()
    {
        foreach (var dot in dotsList)
        {
            dot.OnDotClick -= TryToDrawLine;
            dot.OnDotClick -= ChangeCollidersEnabled;
        }
    }

    private void OnGetObject()
    {
        gameObject.SetActive(true);
    }

    private void TryToDrawLine(DotController dot)
    {
        if (dot != currentStartingPoint && CanDrawLine(currentStartingPoint, dot))
        {
            if (anotherPlayerTurn)
            {
                currentPlayerLine = currentPlayerLine == firstPlayerLine ? secondPlayerLine : firstPlayerLine;
            }


            pointCount++;

            if (pointCount < 2)
            {
                firstPlayerLine = firstPlayerLinePool.Get();
                firstPlayerLine.positionCount = 1;
            }
            else
            {
                firstPlayerLine.positionCount = pointCount;
                pointCount = 0;

                if (anotherPlayerTurn)
                {
                    pointCount = 0;
                }
            }


            points.Add(dot.transform);
            firstPlayerLine.SetPosition(firstPlayerLine.positionCount-1, dot.transform.position);

            currentStartingPoint = dot;
            Debug.Log(currentPlayerLine.name + " " + anotherPlayerTurn);
            CreateFirstLinePoint();//jakos struktura mi tu nie pasuje
        }
    }

    private void CreateFirstLinePoint()
    {
        pointCount++;

        if (pointCount < 2)
        {
            firstPlayerLine = Instantiate(firstPlayerLine);
            firstPlayerLine.positionCount = 1;
        }
        else
        {
            firstPlayerLine.positionCount = pointCount;
            pointCount = 0;

            if (anotherPlayerTurn)
            {
                pointCount = 0;
            }
        }
        points.Add(currentStartingPoint.transform);
        firstPlayerLine.SetPosition(firstPlayerLine.positionCount - 1, currentStartingPoint.transform.position);
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
            var nieWiem3 = points.FindAll(x => x.transform.position == chosenDot.transform.position);//TODO: change transform position
            foreach (var x in nieWiem3)
            {
                listOfIndexes.Add(points.IndexOf(chosenDot.transform));
            }
        }

        //foreach (var x in listOfIndexes)
        //{
        //    if ((x - 1 >= 0 && lineRenderer.GetPosition(x - 1) == currentDot.transform.position) || (lineRenderer.GetPosition(x + 1) == currentDot.transform.position))
        //    {
        //        return false;
        //    }
        //}
        return true;
    }

    public void ResetAllPoints()
    {
        firstPlayerLine.positionCount = 0;
        points.Clear();
    }

    public void ChangeCollidersEnabled(DotController currentDot)
    {
        foreach (var dot in dotsList)
        {
            if ((dot.transform.position - currentDot.transform.position).magnitude > maxDistanceToClickableDot)
            {
                dot.GetComponent<Collider2D>().enabled = false;//TODO: change get component
            }
            else
            {
                dot.GetComponent<Collider2D>().enabled = true;
            }
        }
    }
}
