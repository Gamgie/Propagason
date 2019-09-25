using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatDetectionGraphDebug : MonoBehaviour
{
    [SerializeField] GameObject debugCube;
    private Material mat;
    private Color initialEmissiveColor;
    private float emissiveValue;

    [Header("Normalized Level Graph")]
    [SerializeField] bool drawNormalizedLevelGraph;
    [SerializeField] int normalizedLevelSample = 1024;
    [SerializeField] Material _normalizedLevelMaterial = null;

    [Header("AVG Graph")]
    [SerializeField] bool drawAVGGraph;
    [SerializeField] int avgSample = 1024;
    [SerializeField] Material _avgMaterial = null;

    // Debug Mesh for avg visualization
    float[] _avgHistory;
    int _avgIndex = 0;
    List<Vector3> _avgVertices;
    Mesh _avgMesh;

    // Debug Mesh for level visualization
    float[] _levelHistory;
    int _levelIndex = 0;
    List<Vector3> _levelVertices;
    Mesh _levelMesh;


    // Start is called before the first frame update
    void Start()
    {
        _avgHistory = new float[avgSample];
        _avgMesh = CreateMesh(avgSample);

        _levelHistory = new float[normalizedLevelSample];
        _levelMesh = CreateMesh(normalizedLevelSample);

        mat = debugCube.GetComponent<Renderer>().material;
        initialEmissiveColor = mat.GetColor("_EmissionColor");
        mat.SetColor("_EmissionColor", initialEmissiveColor * emissiveValue);
    }

    // Update is called once per frame
    void Update()
    {
        if(drawAVGGraph)
        {
            UpdateMeshWithWaveform(_avgMesh, _avgHistory);
            Graphics.DrawMesh(_avgMesh, transform.localToWorldMatrix, _avgMaterial, gameObject.layer);
        }

        if(drawNormalizedLevelGraph)
        {
            UpdateMeshWithWaveform(_levelMesh, _levelHistory);
            Graphics.DrawMesh(_levelMesh, transform.localToWorldMatrix, _normalizedLevelMaterial, gameObject.layer);
        }

        emissiveValue = Mathf.Lerp(emissiveValue, 0, 0.4f);
        mat.SetColor("_EmissionColor", initialEmissiveColor * emissiveValue);
    }

    public void OnBeatDetected()
    {
        emissiveValue = 10.0f;
    }

    public void UpdateGraph(float avgValue, float normalizedLevel)
    {
        if (!gameObject.activeSelf)
            return;

        if(drawAVGGraph)
        {
            _avgHistory[_avgIndex] = avgValue;
            _avgIndex++;
            _avgIndex = _avgIndex % avgSample;
        }

        if(drawNormalizedLevelGraph)
        {
            _levelHistory[_levelIndex] = normalizedLevel;
            _levelIndex++;
            _levelIndex = _levelIndex % normalizedLevelSample;
        }
    }

    Mesh CreateMesh(int verticesCount)
    {
        var indices = new int[2 * (verticesCount - 1)];

        for (var i = 0; i < verticesCount - 1; i++)
        {
            indices[2 * i + 0] = i;
            indices[2 * i + 1] = i + 1;
        }

        List<Vector3> vertices = new List<Vector3>(verticesCount);
        for (var i = 0; i < verticesCount; i++) vertices.Add(Vector3.zero);

        Mesh mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        return mesh;
    }

    void UpdateMeshWithWaveform(Mesh mesh, float[] data)
    {
        var scale = 2.0f / data.Length;

        List<Vector3> vertices = new List<Vector3>(data.Length);
        mesh.GetVertices(vertices);

        for (var i = 0; i < data.Length; i++)
            vertices[i] = new Vector3(scale * i - 1, data[i], 0);

        mesh.SetVertices(vertices);
    }
}
