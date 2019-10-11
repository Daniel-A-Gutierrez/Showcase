using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldDrawer : MonoBehaviour
{

    public KeyCode advance;
    public KeyCode reverse;

    /*so i want the algorithms to be able to take a suite of actions, using a master class to do things to the world.
    everything should be recorded in a c# list, and the user should be able to step by step advance the algorithm and rewind it.
    
    each method must be atomic, and invertible. 
    */

    List<System.Action<object[]>> steps;
    List<System.Action<object[]>> inverses;
    List<object[]> arguments;
    List<List<object>> data;
    //references a game object in the list by position they were created at. gives the position in the list data, and then the list within that.
    Dictionary<Tuple<float, float>, Tuple<int,int>> points; 

    int Index;
    void Awake()
    {
        Index = 0;
        steps = new List<Action<object[]>>();
        inverses = new List<Action<object[]>>();
        arguments = new List<object[]>();
        data = new List<List<object>>();
        points = new Dictionary<Tuple<float, float>, Tuple<int, int>>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(advance))
            Advance();
        if(Input.GetKeyDown(reverse))
            Reverse();
    }

    void Advance()
    {
        if (Index < steps.Count)
        {
            steps[Index](arguments[Index]);
            Index++;
        }
    }

    void Reverse()
    {
        if (Index > 0)
        {
            Index--;
            inverses[Index](arguments[Index]);
        }
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
        data[Index].Add(new Tuple<float, float>(x, y)); //also add the position the point was created at so we can find it in the dictionary to delete it.
        s.GetComponent<Renderer>().material.SetColor("_Color", c);
        //cube.GetComponent<Material>().EnableKeyword("_Color");//might need to do this or something
        points[new Tuple<float, float>(x, y)] = new Tuple<int, int>(Index, 0);

    }

    private void _DrawPointInverse(params object[] args)
    {
        float x = (float)args[0];
        float y = (float)args[1];
        Color c = (Color)args[2];
        GameObject g = (GameObject)data[Index][0];
        Destroy(g);
        points.Remove((Tuple<float,float>)data[Index][1]); // this removes the entry in the dictionary by referencing the position it was created at, stored in data on its creation. 
        data[Index].Clear();
        
    }

    public void ColorPoint(float x, float y, Color c)
    {
        steps.Add(_ColorPoint);
        inverses.Add(_ColorPointInverse);
        arguments.Add(new object[] {x,y,c});
        data.Add(new List<object>());
    }

    private void _ColorPoint(params object[] args)
    {
        float x = (float)args[0];
        float y = (float)args[1];
        Color c = (Color)args[2];
        var indeces = points[new Tuple<float,float>(x,y)];
        GameObject g = (GameObject)data[indeces.Item1][indeces.Item2];  
        data[Index].Add(g.GetComponent<Renderer>().material.color);
        g.GetComponent<Renderer>().material.SetColor("_Color", c);
    }

    private void _ColorPointInverse(params object[] args)
    {
        float x = (float)args[0];
        float y = (float)args[1];
        var indeces = points[new Tuple<float,float>(x,y)];
        GameObject g = (GameObject)data[indeces.Item1][indeces.Item2];  
        g.GetComponent<Renderer>().material.SetColor("_Color", (Color)data[Index][0]);
        data[Index].Clear();
    }
}
