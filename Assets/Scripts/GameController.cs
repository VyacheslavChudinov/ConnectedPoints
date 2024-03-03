using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject shapePrefab;
    public GameObject pointPrefab;
    public GameObject linePrefab;
    public int amountOfShapes = 3;

    private List<GameObject> shapes;

    void Start()
    {
        shapes = new List<GameObject>(amountOfShapes);

        for (int k = 0; k < amountOfShapes; k++)
        {
            shapes.Add(Instantiate(shapePrefab));
        }

        for (int i = 0; i < amountOfShapes; i++)
        {
            var shapeInstance = shapes[i].GetComponent<ShapeController>();
            shapeInstance.Lines = new List<GameObject>();

            for (int j = 0; j < shapeInstance.amountOfLines; j++)
            {
                var linePrefabInstance = Instantiate(linePrefab);
                var lineComponent = linePrefabInstance.GetComponent<LineController>();

                if (j > 0 && j < shapeInstance.amountOfLines - 1)
                {
                    lineComponent.endPoint = Instantiate(pointPrefab);
                    lineComponent.startPoint = shapeInstance.Lines[j - 1].GetComponent<LineController>().endPoint;
                    lineComponent.endPoint.transform.position = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
                    lineComponent.endPoint.GetComponent<PointController>().pointMoved += RecalculateLinePositions;
                    lineComponent.endPoint.GetComponent<PointController>().pointMoved += HandleShapes;
                }
                else if (j == shapeInstance.amountOfLines - 1)
                {
                    lineComponent.startPoint = shapeInstance.Lines[j - 1].GetComponent<LineController>().endPoint;
                    lineComponent.endPoint = shapeInstance.Lines[0].GetComponent<LineController>().startPoint;
                }
                else
                {
                    lineComponent.startPoint = Instantiate(pointPrefab);
                    lineComponent.endPoint = Instantiate(pointPrefab);
                    lineComponent.startPoint.transform.position = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
                    lineComponent.endPoint.transform.position = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
                    lineComponent.startPoint.GetComponent<PointController>().pointMoved += RecalculateLinePositions;
                    lineComponent.startPoint.GetComponent<PointController>().pointMoved += HandleShapes;
                    lineComponent.endPoint.GetComponent<PointController>().pointMoved += RecalculateLinePositions;
                    lineComponent.endPoint.GetComponent<PointController>().pointMoved += HandleShapes;
                }

                shapeInstance.Lines.Add(linePrefabInstance);
            }
        }

        RecalculateLinePositions();
        HandleShapes();
    }

    private void RecalculateLinePositions()
    {
        foreach (var shape in shapes)
        {
            var shapeController = shape.GetComponent<ShapeController>();

            foreach (var line in shapeController.Lines)
            {
                var lineRenderer = line.GetComponent<LineRenderer>();
                var lineController = line.GetComponent<LineController>();

                lineRenderer.SetPosition(0, lineController.startPoint.transform.position);
                lineRenderer.SetPosition(1, lineController.endPoint.transform.position);
            }
        }
    }

    private bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {

        Vector2 a = p2 - p1;
        Vector2 b = p3 - p4;
        Vector2 c = p1 - p3;

        float alphaNumerator = b.y * c.x - b.x * c.y;
        float alphaDenominator = a.y * b.x - a.x * b.y;
        float betaNumerator = a.x * c.y - a.y * c.x;
        float betaDenominator = a.y * b.x - a.x * b.y;

        bool doIntersect = true;

        if (alphaDenominator == 0 || betaDenominator == 0)
        {
            doIntersect = false;
        }
        else
        {

            if (alphaDenominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                {
                    doIntersect = false;

                }
            }
            else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
            {
                doIntersect = false;
            }

            if (doIntersect && betaDenominator > 0)
            {
                if (betaNumerator < 0 || betaNumerator > betaDenominator)
                {
                    doIntersect = false;
                }
            }
            else if (betaNumerator > 0 || betaNumerator < betaDenominator)
            {
                doIntersect = false;
            }
        }

        return doIntersect;
    }

    private void HandleShapes()
    {
        ClearLinesStatus();

        for (int i = 0; i < amountOfShapes; i++)
        {
            var firstShape = shapes[i].GetComponent<ShapeController>();
            for (int j = i + 1; j < amountOfShapes; j++)
            {
                var secondShape = shapes[j].GetComponent<ShapeController>();
                ValidateShapes(firstShape, secondShape);
            }
        }
    }

    private void ClearLinesStatus()
    {
        foreach (var shape in shapes)
        {
            var shapeController = shape.GetComponent<ShapeController>();

            foreach (var line in shapeController.Lines)
            {
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.startColor = Color.blue;
                lineRenderer.endColor = Color.blue;
                lineRenderer.startColor = Color.blue;
                lineRenderer.endColor = Color.blue;
            }
        }
    }

    private void ValidateShapes(ShapeController firstShape, ShapeController secondShape)
    {
        for (int i = 0; i < firstShape.Lines.Count; i++)
        {
            for (int j = 0; j < secondShape.Lines.Count; j++)
            {
                MarkIntersectingLines(firstShape.Lines[i].GetComponent<LineRenderer>(), secondShape.Lines[j].GetComponent<LineRenderer>());
            }
        }
    }

    private void MarkIntersectingLines(LineRenderer firstLine, LineRenderer secondLine)
    {
        var result = FasterLineSegmentIntersection(
            firstLine.GetPosition(0),
            firstLine.GetPosition(1),
            secondLine.GetPosition(0),
            secondLine.GetPosition(1));

        if (result)
        {
            firstLine.startColor = Color.red;
            firstLine.endColor = Color.red;
            secondLine.startColor = Color.red;
            secondLine.endColor = Color.red;
        }
    }
}
