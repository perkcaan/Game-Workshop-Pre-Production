using UnityEngine;

public class SaveableObject
{

    public string loadID;
    public GameObject gameObjectReference;

    public SaveableObject(GameObject go)
    {
        gameObjectReference = go;

        //Generate a (hopefully) unique id for the object using its x/y coordinates at start
        string grapheme = go.name.Substring(0);
        string xCoordString = go.transform.position.x.ToString();
        string yCoordString = go.transform.position.y.ToString();
        loadID = grapheme + xCoordString + yCoordString;
    }
}
