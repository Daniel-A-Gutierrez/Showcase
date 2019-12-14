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

    //stores a sequence of methods to call
    List<System.Action<object[]>> steps;
    //stores the inverse methods corresponding to each step
    List<System.Action<object[]>> inverses;
    //stores the arguments to each step and its inverse
    List<object[]> arguments;
    //stores any data created by a step which must be deleted or accessed later on.
    List<List<object>> data;

    //position -> step Index , sublist Index.
    Dictionary<Tuple<float, float>, Tuple<int,int>> points; 

    int Index;

    /*  Must Conform to these. data stores lists of objects, to be colored those objects need to be indexed by points by creation position.

        var indeces = points[new Tuple<float,float>(x,y)];
        GameObject g = (GameObject)data[indeces.Item1][indeces.Item2];  
     */




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
    //stores game object into data[index][ count]  
    //[count +1] : starting position of game object.
    //stores into points [x,y] : (Index,data[Index].count (prior to insertion)) 
    private void _DrawPoint(params object[] args)
    {   
        float x = (float)args[0];
        float y = (float)args[1];
        Color c = (Color)args[2];
         //Create a game object as a primitive
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.position = new Vector3(x,y);
        s.transform.localScale = new Vector3(.1f,.1f,.1f);

        //record where in data[Index] our game object is stored
        int insertPoint = data[Index].Count;
        data[Index].Add(s);
        //also add the position the point was created at so we can find it in the dictionary to delete it.
        data[Index].Add(new Tuple<float, float>(x, y));
        
        //set the color
        s.GetComponent<Renderer>().material.SetColor("_Color", c);
        //cube.GetComponent<Material>().EnableKeyword("_Color");//might need to do this or something

        //record in points what Index the point is at, as well as where in data[Index]
        points[new Tuple<float, float>(x, y)] = new Tuple<int, int>(Index, insertPoint);

    }
    //note : points[x,y] = (Index, insertion Point of game object)
    //removes game object from data[points[x,y].item1][points[x,y].item2]
    //removes starting position from data[points[x,y].item1][points[x,y].item2+1]
    //removes x,y tuple key and value from points
    private void _DrawPointInverse(params object[] args)
    {
        float x = (float)args[0];
        float y = (float)args[1];
        Color c = (Color)args[2];
        //use the x and y stored in the argument to find the index and insertion point in data using points.
        var IndexandInsertionPoint = points[new Tuple<float,float>(x,y)];
        if(IndexandInsertionPoint.Item1 != Index)
            print ( "What the f***");
        //destroy the game object and its starting position tuple.
        GameObject g = (GameObject)data[IndexandInsertionPoint.Item1][IndexandInsertionPoint.Item2];
        Destroy(g);
        data[IndexandInsertionPoint.Item1].RemoveAt(IndexandInsertionPoint.Item2+1);
        data[IndexandInsertionPoint.Item1].RemoveAt(IndexandInsertionPoint.Item2);
        //use the starting position recorded in the arguments to remove it from points dictionary. 
        points.Remove(new Tuple<float,float>(x,y)); 
        //data[Index].Clear(); cant clear it since now im assuming i can draw multiple points at one value of index, inside data[index]
        //realistically this should be equivalent to just deleting the last game object in data, but if it isnt thats a problem.
    }

    //draw points

    public void DrawPoints(Vector2[] vects, Color c)
    {
        steps.Add(_DrawPoints);
        inverses.Add(_DrawPointsInverse);
        arguments.Add( new object[]{vects,c});
        data.Add(new List<object>());
    }

    //this is gonna do draw point many times.
    private void _DrawPoints(params object[] args)
    {   
        Vector2[] vects = (Vector2[])args[0];
        Color c = (Color)args[1];
        foreach(Vector2 v in vects)
            _DrawPoint(v.x,v.y,c);
    }

    private void _DrawPointsInverse(params object[] args)
    {
        Vector2[] vects = (Vector2[])args[0];
        Color c = (Color)args[1];
        for(int i = 0 ; i < vects.Length; i++)
            _DrawPointInverse(v.x,v.y,c);        
    }




    //draw line

    //draw curve

    //draw triangle

    //hide point

    //hide line

    //hide curve

    //hide triangle

    //color line

    //color curve

    //color triangle




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
