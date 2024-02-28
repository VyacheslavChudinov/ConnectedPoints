using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour
{
    private bool isDragging = false;
    public delegate void MoveHandler();
    public MoveHandler pointMoved;

    public void OnMouseUp()
    {
        isDragging = false;
    }

    public void OnMouseDown()
    {
        isDragging = true;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);
            pointMoved();
        }
    }
}
