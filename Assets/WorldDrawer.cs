using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldDrawer : MonoBehaviour
{
    
    /*so i want the algorithms to be able to take a suite of actions, using a master class to do things to the world.
    everything should be recorded in a c# list, and the user should be able to step by step advance the algorithm and rewind it.
    
    each method must be atomic, and invertible. 
    */

    List<System.Action<object[]>> steps;
    List<System.Action<object[]>> inverses;
    List<object[]> arguments;
    List<List<object>> data;
    int Index;
    void Awake()
    {
        Index = 0;
        steps = new List<Action<object[]>>();
        inverses = new List<Action<object[]>>();
        arguments = new List<object[]>();
        data = new List<List<object>>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
            Advance();
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            Reverse();
    }

    void Advance()
    {
        steps[Index](arguments[Index]);
        Index ++;
    }

    void Reverse()
    {
        Index--;
        inverses[Index](arguments[Index]);
    }

    public void DrawPoint(float x, float y, Color c)
    {
        steps.Add(_DrawPoint);
        inverses.Add(_DrawPointInverse);
        arguments.Add( new object[]{x,y,c});
        data.Add(new List<object>());
    }

    private void _DrawPoint(params object[] args)
    {   
        float x = (float)args[0];
        float y = (float)args[1];
        Color c = (Color)args[2];
         //Create a new cube primitive to set the color on
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.position = new Vector3(x,y);
        s.transform.localScale = new Vector3(.1f,.1f,.1f);
        data[Index].Add(s);
        //Get the Renderer component from the new cube
        /*var cubeRenderer =*/s.GetComponent<Renderer>().material.SetColor("_Color", c);
        //cube.GetComponent<Material>().EnableKeyword("_Color");//might need to do this or something
        //Call SetColor using the shader property name "_Color" and setting the color to red
        //cubeRenderer.material.SetColor("_Color", c);


    }

    private void _DrawPointInverse(params object[] args)
    {
        float x = (float)args[0];
        float y = (float)args[1];
        Color c = (Color)args[2];
        GameObject g = (GameObject)data[Index][0];
        Destroy(g);
        data[Index].Clear();
    }
}
