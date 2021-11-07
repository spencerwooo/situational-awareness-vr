using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using UnityEngine;

public class VisualiseCoords : MonoBehaviour
{
    // Using this method as initialisation so that the script doesn't have to be added to
    // a specific game object, making the hierarchy a bit more tidy
    [RuntimeInitializeOnLoadMethod]

    private static void OnRuntimeMethodLoad()
    {
        var data = CsvReader.Read("InteractionLogs-211020-125559");

        Material userPositionMaterial = Resources.Load<Material>("Materials/UserPositionSphere");
        Material camHitPointMaterial = Resources.Load<Material>("Materials/CamHitPointSphere");
        Material controllerHitPointMaterial = Resources.Load<Material>("Materials/ControllerHitPointSphere");

        for (var i = 0; i < data.Count; i++)
        {
            // CSV dictionary keys include:
            // - frame_no
            // - user_position, user_orientation
            // - camera_hit_obj, camera_hit_point, camera_hit_dist
            // - controller_hit_obj, controller_hit_point, controller_hit_distance

            Debug.Log(data[i]["user_position"]);

            Vector3 userPosition = ParseVector3(data[i]["user_position"].ToString());
            RenderCoordinate(userPosition, userPositionMaterial);

            Vector3 camHitPoint = ParseVector3(data[i]["camera_hit_point"].ToString());
            RenderCoordinate(camHitPoint, camHitPointMaterial);

            Vector3 controllerHitPoint = ParseVector3(data[i]["controller_hit_point"].ToString());
            RenderCoordinate(controllerHitPoint, controllerHitPointMaterial);

            // if (i > 100) break;
        }

        Debug.Log("Loaded CSV.");
    }

    private static void RenderCoordinate(Vector3 coord, Material mat)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = coord;
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Renderer sphereRenderer = sphere.GetComponent<Renderer>();
        sphereRenderer.material = mat;
    }

    private static Vector3 ParseVector3(string coordinates)
    {
        // Coordinates look like (xx, yy, zz), here we parse the coordinate string by removing
        // the leading '(' and trailing ')', and split the string to floats
        string[] coordXYZ = coordinates.TrimStart('(').TrimEnd(')').Split(',');

        float coordX = float.Parse(coordXYZ[0]);
        float coordY = float.Parse(coordXYZ[1]);
        float coordZ = float.Parse(coordXYZ[2]);

        return new Vector3(coordX, coordY, coordZ);
    }
}
