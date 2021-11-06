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
        for (var i = 0; i < data.Count; i++)
        {
            // CSV dictionary keys include:
            // - frame_no
            // - user_position, user_orientation
            // - camera_hit_obj, camera_hit_point, camera_hit_dist
            // - controller_hit_obj, controller_hit_point, controller_hit_distance

            Debug.Log(data[i]["user_position"]);
            // TODO: bug here
            // Vector3 userPosition = ParseVector3(data[i]["user_position"].ToString());
            // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // sphere.transform.position = userPosition;

            if (i > 10) break;
        }

        Debug.Log("Loaded CSV.");
    }

    private static Vector3 ParseVector3(string coordinates)
    {
        string[] coordXYZ = coordinates.Split(',');
        float coordX = float.Parse(coordXYZ[0]);
        float coordY = float.Parse(coordXYZ[1]);
        float coordZ = float.Parse(coordXYZ[2]);
        return new Vector3(coordX, coordY, coordZ);
    }
}
