using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent (typeof(WorldDrawer))]
public class DemonstrateConvexHull : MonoBehaviour
{
    public int cloudSize;
    public int cloudScale;
    WorldDrawer drawer;
    // lets make the bounds of the cloud -1M to +1M in x and y
    // Start is called before the first frame update
    void Start()
    {
        drawer = GetComponent<WorldDrawer>();
        Vector2Int[] cloud = new Vector2Int[cloudSize];
        for(int i = 0 ; i < cloudSize; i++)
        {
            cloud[i] = new Vector2Int(UnityEngine.Random.Range(-cloudScale,cloudScale) , UnityEngine.Random.Range(-cloudScale,cloudScale));
        }

        Array.Sort(cloud, Vector2IntComparer); //ascending or descending?
        List<Vector2Int> remainder = new List<Vector2Int>();
        List<Vector2Int> hull = ComputeHull(cloud, remainder);
        foreach(Vector2Int v in remainder)
            drawer.DrawPoint((float)(v.x)/cloudScale , (float)(v.y)/cloudScale , Color.blue);
        foreach(Vector2Int v in hull)
            drawer.DrawPoint((float)(v.x)/cloudScale , (float)(v.y)/cloudScale , Color.red);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Vector2Int> ComputeHull(Vector2Int[] cloud, List<Vector2Int> remainder)
    {
        List<Vector2Int> hull = new List<Vector2Int>();
        for( int i = 0 ; i < cloudSize ; i++)
        {
            hull.Add(cloud[i]);
            if(hull.Count<3)
                continue;
            while(CalcArea(hull[hull.Count-3] , hull[hull.Count-2] , hull[hull.Count-1]) <= 0)
            {
                hull.RemoveAt(hull.Count-2);
                if(hull.Count<3)
                    break;
            }            
        }
        //upper hull done
        int upperSize = hull.Count;
        for(int i = cloudSize-2; i>=0 ; i--)
        {
            hull.Add(cloud[i]);
            if(hull.Count<2 + upperSize)
                continue;
            while(CalcArea(hull[hull.Count-3] , hull[hull.Count-2] , hull[hull.Count-1]) <= 0)
            {
                if(findIndexOf(hull[hull.Count-2], hull, 0 , upperSize) ==-1) //ruins n log n running time
                    remainder.Add(hull[hull.Count-2]);
                hull.RemoveAt(hull.Count-2);
                if(hull.Count<3 + upperSize)
                    break;
            }     
        }
        if(hull[0] == hull[hull.Count-1])
            hull.RemoveAt(hull.Count-1);
        return hull;
    }

    int findIndexOf( Vector2Int v,List<Vector2Int> l, int start, int finish)
    {
        for(int i = start ; i < finish ; i++)
        {
            if(l[i] == v)
                return i;
        }
        return -1;
    }

    public class V2iComparerHelper : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            Vector2Int a = (Vector2Int)x;
            Vector2Int b = (Vector2Int)y;
            if(a.x == b.x)
                return a.y-b.y;
            return a.x-b.x;
        }
        
    }

    public int Vector2IntComparer(Vector2Int a, Vector2Int b)
    {
        if(a.x == b.x)
            return a.y-b.y;
        return a.x-b.x;
    }


    double CalcArea(Vector2Int a, Vector2Int b, Vector2Int c) //clockwise right turn should return positive x.
    {
        return (b.x-a.x) * (double)(b.y+a.y) + (c.x-b.x) * (double)(c.y+b.y) + (a.x-c.x)*(double)(a.y+c.y);
    }

}
