using UnityEngine;

[System.Serializable]
public class SubArray
{
    public int[] m_line;
    public SubArray(int size)
    {
        m_line = new int[size];
    }
}

[ExecuteInEditMode]
public class Graph : MonoBehaviour
{
    public SubArray[] m_matrix;

    private void FillMatrix()
    {
        var vertices = GetComponentInChildren<Transform>().Find("Vertices");
        var verticesCount = vertices.transform.childCount;

        m_matrix = new SubArray[verticesCount];
        for (var i = 0; i < verticesCount; i++)
        {
            m_matrix[i] = new SubArray(verticesCount);
        }

        var lines = GetComponentsInChildren<Line>();
        foreach (var line in lines)
        {
            m_matrix[line.m_startVertex].m_line[line.m_finishVertex] = line.m_weight;
            m_matrix[line.m_finishVertex].m_line[line.m_startVertex] = line.m_weight;
        }
    }

    private void Start()
    {
        FillMatrix();
    }
}

