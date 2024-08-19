using System;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * rotateSpeed);
    }
}
