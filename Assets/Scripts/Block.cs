using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block {

    public static void AddToMesh(Chunk chunk, int x, int y, int z, int id, List<Vector3> vertices, List<int> triangles, List<Vector2> uv) {
        // top
        if (chunk == null || chunk.IsTransparent(x, y + 1, z))
            AddFace(new Vector3(x, y + 1, z), id, Vector3.up, vertices, triangles, uv);
        // bottom
        if (chunk == null || chunk.IsTransparent(x,y - 1,z))
            AddFace(new Vector3(x, y, z), id, Vector3.down, vertices, triangles, uv);
        // left
        if (chunk == null || chunk.IsTransparent(x - 1, y, z))
            AddFace(new Vector3(x, y, z + 1), id, Vector3.left, vertices, triangles, uv);
        // right
        if (chunk == null || chunk.IsTransparent(x + 1, y, z))
            AddFace(new Vector3(x + 1, y, z + 1), id, Vector3.right, vertices, triangles, uv);
        // front
        if (chunk == null || chunk.IsTransparent(x, y, z + 1))
            AddFace(new Vector3(x, y, z + 1), id, Vector3.forward, vertices, triangles, uv);
        // back
        if (chunk == null || chunk.IsTransparent(x, y, z - 1))
            AddFace(new Vector3(x, y, z), id, Vector3.back, vertices, triangles, uv);
    }

    public static void AddFace(Vector3 corner, int id, Vector3 normal, List<Vector3> vertices, List<int> triangles, List<Vector2> uv) {
        Vector3 right = Vector3.zero;
        Vector3 up = Vector3.zero;
        bool reversed = false;

        if (normal == Vector3.down || normal == Vector3.right || normal == Vector3.forward) {
            reversed = true;
        }

        int index = vertices.Count;

        if (normal == Vector3.up || normal == Vector3.down) {
            up = Vector3.forward;
            right = Vector3.right;
        } else if (normal == Vector3.left || normal == Vector3.right) {
            up = Vector3.up;
            right = Vector3.back;
        } else if (normal == Vector3.forward || normal == Vector3.back) {
            up = Vector3.up;
            right = Vector3.right;
        }

        vertices.Add(corner);
        vertices.Add(corner + up);
        vertices.Add(corner + up + right);
        vertices.Add(corner + right);

        if (reversed) {
            triangles.Add(index + 0);
            triangles.Add(index + 2);
            triangles.Add(index + 1);

            triangles.Add(index + 0);
            triangles.Add(index + 3);
            triangles.Add(index + 2);
        } else {
            triangles.Add(index + 0);
            triangles.Add(index + 1);
            triangles.Add(index + 2);

            triangles.Add(index + 0);
            triangles.Add(index + 2);
            triangles.Add(index + 3);
        }

        Vector2 offset = Vector3.zero;
        float resolution = 0.0625f;

        Vector2 _00 = new Vector2(0,0) * resolution;
        Vector2 _01 = new Vector2(0,1) * resolution;
        Vector2 _11 = new Vector2(1,1) * resolution;
        Vector2 _10 = new Vector2(1,0) * resolution;




        if (normal == Vector3.up) {
            offset = new Vector2(0f, 15f) * resolution;
        } else if (normal == Vector3.down) {
            offset = new Vector2(2f, 15f) * resolution;
        } else {
            offset = new Vector2(3f, 15f) * resolution;
        }


        uv.Add(_00 + offset);
        uv.Add(_01 + offset);
        uv.Add(_11 + offset);
        uv.Add(_10 + offset);
    }
}