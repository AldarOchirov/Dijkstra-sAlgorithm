using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Dijkstra : MonoBehaviour
{
    public Graph m_graph;
    public GameObject m_startVertex;
    public GameObject m_finishVertex;

    private int m_indexStartVertex;
    private int m_indexFinishVertex;

    private GameObject[] m_vertices;
    private Vector3[] m_verticesPositions;
    private int[] m_marks;
    private bool[] m_visited = { false };

    private Stack<int> m_edgeSequence;
    private LineRenderer m_lineRenderer;

    const int INF = 100000;

    void InitGraphInfo()
    {
        var verticesCount = m_graph.m_matrix.Length;
        var vertices = m_graph.GetComponentInChildren<Transform>().Find("Vertices");
        m_vertices = new GameObject[verticesCount];
        var transform = vertices.GetComponentsInChildren<Transform>();
        for (var i = 0; i < verticesCount; i++)
        {
            m_vertices[i] = transform[i + 1].gameObject;
        }

        m_marks = new int[verticesCount];
        m_visited = new bool[verticesCount];

        for (var i = 0; i < verticesCount; i++)
        {
            if (m_vertices[i] != m_startVertex)
            {
                m_marks[i] = INF;
            }
            else
            {
                m_marks[i] = 0;
                m_indexStartVertex = i;
            }

            if (m_vertices[i] == m_finishVertex)
            {
                m_indexFinishVertex = i;
            }
        }
    }

    void TraversingGraph()
    {
        int minIndex, minDistance;
        do
        {
            minIndex = INF;
            minDistance = INF;

            for (var i = 0; i < m_vertices.Length; i++)
            {
                if (!m_visited[i] && m_marks[i] < minDistance)
                {
                    minDistance = m_marks[i];
                    minIndex = i;
                }
            }

            if (minIndex != INF)
            {
                for (var i = 0; i < m_vertices.Length; i++)
                {
                    if (m_graph.m_matrix[minIndex].m_line[i] > 0)
                    {
                        var sumDistance = minDistance + m_graph.m_matrix[minIndex].m_line[i];
                        if (sumDistance < m_marks[i])
                        {
                            m_marks[i] = sumDistance;
                        }
                    }
                }
                m_visited[minIndex] = true;
            }
        } while (minIndex < INF);
    }

    void CreateArrayVerticesPositions()
    {
        m_verticesPositions = new Vector3[m_edgeSequence.Count];
        int i = 0;
        foreach (var edge in m_edgeSequence)
        {
            m_verticesPositions[i] = m_vertices[edge].transform.position;
            i++;
        }
    }

    void CreateLineRenderer()
    {
        m_lineRenderer = m_graph.gameObject.GetComponent<LineRenderer>();
        if (m_lineRenderer == null)
        {
            m_lineRenderer = m_graph.gameObject.AddComponent<LineRenderer>();
        }
        m_lineRenderer.positionCount = m_edgeSequence.Count;
        m_lineRenderer.startColor = Color.green;
        m_lineRenderer.endColor = Color.green;
        m_lineRenderer.widthCurve = AnimationCurve.Linear(0, 0.2f, 1, 0.2f);
        Material whiteDiffuseMat = new Material(Shader.Find("Sprites/Default"));
        m_lineRenderer.material = whiteDiffuseMat;
        m_lineRenderer.SetPositions(m_verticesPositions);
    }

    void RestoreEdgeSequence()
    {
        var index = m_indexFinishVertex;
        var weight = m_marks[m_indexFinishVertex];
        m_edgeSequence = new Stack<int>(m_vertices.Length);
        m_edgeSequence.Push(m_indexFinishVertex);

        while (index != m_indexStartVertex)
        {
            for (var i = 0; i < m_vertices.Length; i++)
            {
                if (m_graph.m_matrix[i].m_line[index] != 0)
                {
                    var diff = weight - m_graph.m_matrix[i].m_line[index];
                    if (diff == m_marks[i])
                    {
                        weight = diff;
                        index = i;
                        m_edgeSequence.Push(i);
                    }
                }
            }
        }
    }

    void Start()
    {
        InitGraphInfo();
        TraversingGraph();
        RestoreEdgeSequence();
        CreateArrayVerticesPositions();
        CreateLineRenderer();
    }

    void OnGUI()
    {
        string strMessage = "Edge sequence: \n";
        foreach (var edge in m_edgeSequence)
        {
            strMessage += "Vertex " + edge.ToString() + "\n";
        }

        GUILayout.Box(strMessage);
    }
}
