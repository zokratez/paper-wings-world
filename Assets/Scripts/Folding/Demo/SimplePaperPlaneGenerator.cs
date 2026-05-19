using UnityEngine;

namespace PaperWings.Demo
{
    /// <summary>
    /// Quick procedural low-poly paper plane for early testing.
    /// Replace this later with real authored 3D models.
    /// </summary>
    public static class SimplePaperPlaneGenerator
    {
        public static GameObject CreateBasicPaperPlane()
        {
            GameObject plane = new GameObject("SimplePaperPlane");

            // Fuselage
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(plane.transform);
            body.transform.localScale = new Vector3(0.08f, 0.04f, 0.6f);
            body.transform.localPosition = new Vector3(0, 0, 0);
            body.GetComponent<Renderer>().material.color = Color.white;

            // Left Wing
            GameObject leftWing = GameObject.CreatePrimitive(PrimitiveType.Quad);
            leftWing.transform.SetParent(plane.transform);
            leftWing.transform.localScale = new Vector3(0.55f, 0.35f, 1);
            leftWing.transform.localPosition = new Vector3(-0.25f, 0.01f, 0.05f);
            leftWing.transform.localRotation = Quaternion.Euler(0, 0, 12);
            leftWing.GetComponent<Renderer>().material.color = Color.white;

            // Right Wing
            GameObject rightWing = GameObject.CreatePrimitive(PrimitiveType.Quad);
            rightWing.transform.SetParent(plane.transform);
            rightWing.transform.localScale = new Vector3(0.55f, 0.35f, 1);
            rightWing.transform.localPosition = new Vector3(0.25f, 0.01f, 0.05f);
            rightWing.transform.localRotation = Quaternion.Euler(0, 0, -12);
            rightWing.GetComponent<Renderer>().material.color = Color.white;

            // Tail
            GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Quad);
            tail.transform.SetParent(plane.transform);
            tail.transform.localScale = new Vector3(0.18f, 0.22f, 1);
            tail.transform.localPosition = new Vector3(0, 0.12f, -0.28f);
            tail.transform.localRotation = Quaternion.Euler(90, 0, 0);
            tail.GetComponent<Renderer>().material.color = Color.white;

            // Add a simple animator slot for future use
            plane.AddComponent<Animator>();

            return plane;
        }
    }
}