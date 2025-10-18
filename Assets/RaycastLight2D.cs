using UnityEngine;
using System.Collections.Generic;
using UnityEngine.U2D;

// tutorial: https://www.youtube.com/watch?v=YHEMrq0sPFg
public class RaycastLight2D : MonoBehaviour
{
    public int count;
    public float length;
    public float angle;
    public SpriteShapeController Light;

    public List<Vector3> collisionPoints;

    private void Update()
    {
        Vector3 center = transform.position;
        Vector3 point = transform.GetChild(0).transform.position;
        Vector3 ray = (point - center).normalized;

        for (int i = 0; i < count; i++)
        {

            Quaternion rotation = Quaternion.AngleAxis(angle/ count, Vector3.back);
            ray = rotation * ray;

            RaycastHit2D hitInfo = Physics2D.Raycast(center, ray, length);

            if(hitInfo)
            {
                collisionPoints.Add(hitInfo.point);
            }
            else
            {
                collisionPoints.Add(center + ray * length);
            }

           
        }

        Light.spline.SetPosition(0,center);
        Light.spline.SetHeight(0, 0);

        if (0 >= Light.spline.GetPointCount())
        {
            Light.spline.InsertPointAt(0, center);
            Light.spline.SetHeight(0, 0);
        }
        else
        {
            Light.spline.SetPosition(0, center);
            Light.spline.SetHeight(0, 0);
        }

        for (int i = 0; i < collisionPoints.Count; i++)
        {
            try
            {
                if (i >= Light.spline.GetPointCount())
                {
                    Light.spline.InsertPointAt(i + 1, collisionPoints[i]);
                    Light.spline.SetHeight(i + 1, 0);
                }
                else
                {
                    Light.spline.SetPosition(i + 1, collisionPoints[i]);
                    Light.spline.SetHeight(i + 1, 0);
                }
            }
            catch (System.Exception e) 
            {
                Light.spline.Clear();
                collisionPoints.Clear();
                Update();
                return;
            }
            

                
        }

        collisionPoints.Clear();

        
    }



}