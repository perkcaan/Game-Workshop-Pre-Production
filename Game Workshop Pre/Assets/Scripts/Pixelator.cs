using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixelator : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        this.gameObject.layer = LayerMask.NameToLayer("3D Object");

        GameObject child = new GameObject("PIXEL MASK *DO NOT EDIT*");
        Material material = Resources.Load("Materials/3D Mask") as Material;
        child.AddComponent<MeshFilter>();
        child.AddComponent<MeshRenderer>();

        print(material);

        child.transform.parent = this.transform;
        child.transform.localEulerAngles = Vector3.zero;
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = Vector3.one;
        child.layer = LayerMask.NameToLayer("Pixel Mask");

        child.GetComponent<MeshFilter>().mesh = gameObject.GetComponent<MeshFilter>().mesh;
        child.GetComponent<MeshRenderer>().material = material;
    }
}
